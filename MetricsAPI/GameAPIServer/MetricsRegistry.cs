using System.Diagnostics.Metrics;

namespace GameAPIServer;

public static class MetricsRegistry
{
    public static readonly Meter Meter = new("ApiServer.Metrics", "1.0");

    public static readonly Counter<int> RequestCounter =
        Meter.CreateCounter<int>("api_requests_total");

    public static readonly Histogram<double> ResponseTimeHistogram =
        Meter.CreateHistogram<double>("api_response_time_ms");
}