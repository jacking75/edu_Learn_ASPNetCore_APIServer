using Microsoft.AspNetCore.SignalR;





namespace WaitingQueue.Server.Hubs;

public class QueueHub : Hub
{
    // 사용자가 자신의 고유 room(그룹)에 참여
    public async Task JoinQueue(string userId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, $"user-{userId}");
    }

    // 클라이언트에게 대기열 업데이트 정보 전송
    public async Task SendQueueUpdate(string userId, object data)
    {
        await Clients.Group($"user-{userId}").SendAsync("queue-update", data);
    }

    // 대기열 처리가 완료된 사용자에게 알림 전송
    public async Task SendQueueReady(string userId, object data)
    {
        await Clients.Group($"user-{userId}").SendAsync("queue-ready", data);
    }
}
