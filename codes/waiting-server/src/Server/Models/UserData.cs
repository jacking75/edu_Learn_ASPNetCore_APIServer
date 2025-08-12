namespace WaitingQueue.Server.Models;


public class UserData
{
    public string UserId { get; set; } = string.Empty;
    public string QueueToken { get; set; } = string.Empty;
    public long JoinedAt { get; set; }
    public string? Email { get; set; }
    public object? Metadata { get; set; }
}