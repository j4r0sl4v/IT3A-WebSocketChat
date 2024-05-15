using System;
using System.Net;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace IT3B_Chat.Server
{
     class Program
    {
        private static async Task HandleClient(WebSocket socket)
        {
            byte[] buffer = new byte[1024];
            WebSocketReceiveResult result = await socket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            string username = Encoding.UTF8.GetString(buffer, 0, result.Count);

            Console.WriteLine($"Client connected: {username}");

            while (socket.State == WebSocketState.Open)
            {
                result = await socket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                string message = $"{username}: {Encoding.UTF8.GetString(buffer, 0, result.Count)}";

                Console.WriteLine($"Received message: {message}");

                // Přeposlat zprávu všem klientům
                await SendToAllClients(message);
            }

            Console.WriteLine($"Client disconnected: {username}");
        }

        private static async Task SendToAllClients(string message)
        {
            foreach (var client in IT3B_Chat.Client)
            {
                await client.SendAsync(Encoding.UTF8.GetBytes(message), WebSocketMessageType.Text, true, CancellationToken.None);
            }
        }

        static async Task Main(string[] args)
        {
            HttpListener listener = new HttpListener();
            listener.Prefixes.Add("http://localhost:8080/");
            listener.Start();

            Console.WriteLine("Server started. Listening on http://localhost:8080/");

            while (true)
            {
                HttpListenerContext context = await listener.GetContextAsync();
                if (context.Request.IsWebSocketRequest)
                {
                    HttpListenerWebSocketContext webSocketContext = await context.AcceptWebSocketAsync(null);
                    WebSocket socket = webSocketContext.WebSocket;
                    _ = Task.Run(() => HandleClient(socket));
                }
                else
                {
                    context.Response.StatusCode = 400;
                    context.Response.Close();
                }
            }
        }
    }
}
