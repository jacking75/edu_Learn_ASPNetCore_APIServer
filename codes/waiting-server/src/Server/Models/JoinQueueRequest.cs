using System.Text.Json.Serialization;

namespace WaitingQueue.Server.Models;

public class JoinQueueRequest
{
    [JsonPropertyName("userId")]
    public string UserId { get; set; } = string.Empty;

    [JsonPropertyName("email")]
    public string? Email { get; set; }

    [JsonPropertyName("metadata")]
    public object? Metadata { get; set; }
}
