using System.Net.WebSockets;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

var clients = new List<WebSocket>();

app.UseWebSockets();

app.Map("/", async context =>
{
    if (context.WebSockets.IsWebSocketRequest)
    {
        var socket = await context.WebSockets.AcceptWebSocketAsync();
        clients.Add(socket);
        Console.WriteLine("New client connected");

        var buffer = new byte[1024 * 4];
        while (socket.State == WebSocketState.Open)
        {
            var result = await socket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            if (result.MessageType == WebSocketMessageType.Close)
            {
                await socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", CancellationToken.None);
                clients.Remove(socket);
                Console.WriteLine("Client disconnected");
            }
            else
            {
                var msg = Encoding.UTF8.GetString(buffer, 0, result.Count);
                Console.WriteLine("Received: " + msg);

                // Рассылаем сообщение всем клиентам
                var sendBuffer = Encoding.UTF8.GetBytes(msg);
                var tasks = clients.Select(async c =>
                {
                    if (c.State == WebSocketState.Open)
                    {
                        await c.SendAsync(sendBuffer, WebSocketMessageType.Text, true, CancellationToken.None);
                    }
                });
                await Task.WhenAll(tasks);
            }
        }
    }
    else
    {
        context.Response.StatusCode = 400;
    }
});

app.Run();
