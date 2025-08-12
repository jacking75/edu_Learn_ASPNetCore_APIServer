using System.Text.Json.Serialization;

namespace WaitingQueue.Server.Models;


public class WaitTime
{
    [JsonPropertyName("seconds")]
    public int Seconds { get; set; }

    [JsonPropertyName("minutes")]
    public int Minutes { get; set; }

    [JsonPropertyName("formatted")]
    public string Formatted { get; set; } = string.Empty;
}