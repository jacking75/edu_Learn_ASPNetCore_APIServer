using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.Diagnostics.Tracing;
using System.Net;
using System.Net.Sockets;
using System.Text;


int pid = Process.GetCurrentProcess().Id;
Console.WriteLine($"Current Process ID: {pid}");

var meter = new Meter("SocketServer.Metrics", "1.0");
var connectionCounter = meter.CreateCounter<int>("socket_connections_total");
var messageCounter = meter.CreateCounter<int>("socket_messages_received_total"); 
var messageLengthHistogram = meter.CreateHistogram<int>("socket_message_length");

var server = new TcpListener(IPAddress.Any, 32452);
server.Start();
Console.WriteLine("Socket server running on port 32452...");

while (true)
{
    var client = await server.AcceptTcpClientAsync();
    connectionCounter.Add(1);
    Console.WriteLine("Client connected");

    _ = Task.Run(async () =>
    {
        using var stream = client.GetStream();
        var buffer = new byte[1024];

        while (true)
        {
            int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
            if (bytesRead <= 0) break;

            messageCounter.Add(1);
            messageLengthHistogram.Record(bytesRead);

            var received = Encoding.UTF8.GetString(buffer, 0, bytesRead);
            Console.WriteLine($"Received: {received}");

            var response = Encoding.UTF8.GetBytes($"Echo: {received}");
            await stream.WriteAsync(response, 0, response.Length);
        }
    });
}



