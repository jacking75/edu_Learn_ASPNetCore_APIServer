using WaitingQueue.Server.Models;


namespace WaitingQueue.Server.Services;

public interface IQueueService
{
    Task<QueueAddResult> AddToQueueAsync(string userId, UserData userData);
    Task<QueueStatus> GetQueueStatusAsync(string userId);
    Task<List<string>> ProcessQueueAsync();
    Task<bool> RemoveFromQueueAsync(string userId);
    Task<QueueInfo> GetQueueInfoAsync();
    Task ClearQueueAsync();
    Task UpdateQueuePositionsAsync();
}