using System.Net;
using System.Net.Sockets;
using System.Text;
using Prometheus;



Counter TcpRequestsTotal = Metrics.CreateCounter("tcp_requests_total", "Total number of TCP requests handled.");


// 1) Prometheus 전용 HTTP 서버 시작 (/metrics 노출)
// 포트 1234에서 metrics 엔드포인트 제공
var metricServer = new KestrelMetricServer(port: 1234);
metricServer.Start();

// 2) TCP 서버 시작
var listener = new TcpListener(IPAddress.Any, 9000);
listener.Start();
Console.WriteLine("TCP Server listening on port 9000. Prometheus on :1234/metrics");

while (true)
{
    var client = await listener.AcceptTcpClientAsync();
    _ = HandleClient(client);
}


async Task HandleClient(TcpClient client)
{
    using var stream = client.GetStream();
    var buffer = new byte[1024];
    int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);

    string received = Encoding.UTF8.GetString(buffer, 0, bytesRead);
    Console.WriteLine($"Received: {received}");

    // 3) 요청 카운터 증가
    TcpRequestsTotal.Inc();

    // 에코 응답
    byte[] response = Encoding.UTF8.GetBytes($"Echo: {received}");
    await stream.WriteAsync(response, 0, response.Length);
}