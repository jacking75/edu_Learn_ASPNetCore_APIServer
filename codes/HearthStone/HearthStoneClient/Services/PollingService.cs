namespace HearthStoneClient.Services;

public class PollingService
{
    public int pollingInterval = 1000;
    private const int Min_Interval = 1000;
    private const int Max_Interval = 10000;
    public async Task StartPolling(Func<Task<bool>> pollAction, CancellationToken ct)
    {
        var hasChanged = await pollAction();

        if (hasChanged)
        {
            pollingInterval = Math.Max(Min_Interval, pollingInterval / 2);
        }
        else
        {
            pollingInterval = Math.Min(Max_Interval, pollingInterval * 2);
        }

        await Task.Delay(pollingInterval, ct);
    }
}
