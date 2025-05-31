using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net.WebSockets;
using System.Threading;
using System.Text;
using System.Linq;

namespace Chat
{
    public class ChatHandler
    {
        private readonly List<WebSocket> _clients = new List<WebSocket>();

        public async Task AcceptConnection(WebSocket socket)
        {
            _clients.Add(socket);
            Console.WriteLine("New client connected");

            var buffer = new byte[1024 * 4];
            while (socket.State == WebSocketState.Open)
            {
                var result = await socket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                if (result.MessageType == WebSocketMessageType.Close)
                {
                    await socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", CancellationToken.None);
                    _clients.Remove(socket);
                    Console.WriteLine("Client disconnected");
                }
                else
                {
                    var msg = Encoding.UTF8.GetString(buffer, 0, result.Count);
                    Console.WriteLine("Received: " + msg);
                    await BroadcastMessage(msg);
                }
            }
        }

        private async Task BroadcastMessage(string message)
        {
            var sendBuffer = Encoding.UTF8.GetBytes(message);
            var tasks = _clients.Select(async client =>
            {
                if (client.State == WebSocketState.Open)
                {
                    await client.SendAsync(sendBuffer, WebSocketMessageType.Text, true, CancellationToken.None);
                }
            });
            await Task.WhenAll(tasks);
        }
    }
}