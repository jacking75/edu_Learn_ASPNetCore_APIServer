using System.Text.Json.Serialization;

namespace WaitingQueue.Server.Models;

public class QueueMemberInfo
{
    [JsonPropertyName("userId")]
    public string UserId { get; set; } = string.Empty;

    [JsonPropertyName("position")]
    public int Position { get; set; }

    [JsonPropertyName("joinedAt")]
    public DateTime JoinedAt { get; set; }

    [JsonPropertyName("estimatedWaitTime")]
    public WaitTime EstimatedWaitTime { get; set; } = new();
}
