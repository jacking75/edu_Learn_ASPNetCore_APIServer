using System.Text.Json.Serialization;

namespace WaitingQueue.Server.Models;


public class QueueStatus
{
    [JsonPropertyName("status")]
    public string Status { get; set; } = "not_in_queue"; // "not_in_queue", "waiting", "active"

    [JsonPropertyName("canAccess")]
    public bool CanAccess { get; set; } = false;

    [JsonPropertyName("position")]
    public long? Position { get; set; }

    [JsonPropertyName("totalInQueue")]
    public long? TotalInQueue { get; set; }

    [JsonPropertyName("activeUsers")]
    public long? ActiveUsers { get; set; }

    [JsonPropertyName("estimatedWaitTime")]
    public WaitTime? EstimatedWaitTime { get; set; }
}
