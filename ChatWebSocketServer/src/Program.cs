using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

var clients = new List<WebSocket>();

app.UseWebSockets();
app.UseStaticFiles();

app.MapGet("/", () => "WebSocket-Chat-Server läuft. Verbinde dich per WebSocket.");

app.Map("/ws", async context =>
{
    if (context.WebSockets.IsWebSocketRequest)
    {
        var socket = await context.WebSockets.AcceptWebSocketAsync();
        clients.Add(socket);
        Console.WriteLine("Neuer Client verbunden");

        var buffer = new byte[1024 * 4];
        while (socket.State == WebSocketState.Open)
        {
            var result = await socket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            if (result.MessageType == WebSocketMessageType.Close)
            {
                await socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Schließen", CancellationToken.None);
                clients.Remove(socket);
                Console.WriteLine("Client getrennt");
            }
            else
            {
                var msg = Encoding.UTF8.GetString(buffer, 0, result.Count);
                Console.WriteLine("Empfangen: " + msg);

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