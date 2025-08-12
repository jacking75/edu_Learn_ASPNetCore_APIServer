using System.Text.Json.Serialization;

namespace WaitingQueue.Server.Models;

public class QueueAddResult
{
    [JsonPropertyName("userId")]
    public string UserId { get; set; } = string.Empty;

    [JsonPropertyName("queueToken")]
    public string QueueToken { get; set; } = string.Empty;

    [JsonPropertyName("position")]
    public long? Position { get; set; }

    [JsonPropertyName("estimatedWaitTime")]
    public WaitTime EstimatedWaitTime { get; set; } = new();
}