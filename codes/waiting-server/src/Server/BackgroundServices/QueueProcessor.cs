using Microsoft.AspNetCore.SignalR;
using WaitingQueue.Server.Hubs;
using WaitingQueue.Server.Services;



namespace WaitingQueue.Server.BackgroundServices;

public class QueueProcessor : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<QueueProcessor> _logger;

    public QueueProcessor(IServiceProvider serviceProvider, ILogger<QueueProcessor> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Queue Processor is starting.");

        while (!stoppingToken.IsCancellationRequested)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var queueService = scope.ServiceProvider.GetRequiredService<IQueueService>();
                var tokenService = scope.ServiceProvider.GetRequiredService<ITokenService>();
                var hubContext = scope.ServiceProvider.GetRequiredService<IHubContext<QueueHub>>();

                var processedUsers = await queueService.ProcessQueueAsync();

                if (processedUsers.Any())
                {
                    _logger.LogInformation($"Processing {processedUsers.Count} users from queue");

                    foreach (var userId in processedUsers)
                    {
                        var accessToken = tokenService.GenerateAccessToken(userId);
                        await hubContext.Clients.Group($"user-{userId}").SendAsync("queue-ready", new { status = "active", accessToken }, stoppingToken);
                    }

                    // 남은 사용자들의 대기 순번 업데이트
                    await queueService.UpdateQueuePositionsAsync();
                }
            }

            await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
        }

        _logger.LogInformation("Queue Processor is stopping.");
    }
}