using System.Text.Json.Serialization;

namespace WaitingQueue.Server.Models;


public class QueueInfo
{
    [JsonPropertyName("queueLength")]
    public long QueueLength { get; set; }

    [JsonPropertyName("activeUsers")]
    public long ActiveUsers { get; set; }

    [JsonPropertyName("maxConcurrentUsers")]
    public int MaxConcurrentUsers { get; set; }

    [JsonPropertyName("availableSlots")]
    public long AvailableSlots { get; set; }

    [JsonPropertyName("nextInQueue")]
    public List<QueueMemberInfo> NextInQueue { get; set; } = new();
}
