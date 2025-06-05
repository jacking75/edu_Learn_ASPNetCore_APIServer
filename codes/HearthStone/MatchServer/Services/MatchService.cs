using MatchServer.Repository;
using MatchServer.Services.Interface;
using MatchServer.Models;
using System.Collections.Concurrent;
using ZLogger;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace MatchServer.Services;

public class MatchService : IMatchService
{
    readonly ConcurrentQueue<Int64> _reqQueue;
    readonly ILogger<MatchService> _logger;
    private readonly IServiceProvider _serviceProvider;
    private Dictionary<Int64, DateTime> waitingUserList; // 'new' 키워드 제거

    public MatchService(IServiceProvider serviceProvider, ILogger<MatchService> logger)
    {
        _reqQueue = new ConcurrentQueue<Int64>();
        _logger = logger;
        _serviceProvider = serviceProvider;
        waitingUserList = new Dictionary<Int64, DateTime>();

        Task.Run(() => Run());
    }

    public void Dispose()
    {
        _reqQueue.Clear();
    }

    public ErrorCode AddUser(Int64 accountUid)
    {
        _reqQueue.Enqueue(accountUid);
        _logger.ZLogDebug($"AddUser {accountUid}");
        return ErrorCode.None;
    }

    async Task<bool> RegistWaitingInfo(MatchInfo matchInfo)
    {
        try
        {
            var memoryDb = _serviceProvider.GetRequiredService<IMemoryDb>();
            await memoryDb.RegistMatchWaitingInfo(matchInfo);
            await memoryDb.RegistUserMatchWaiting(matchInfo);
            return true;
        }
        catch (Exception e)
        {
            _logger.ZLogError($"RegistWaitingInfo 실패: {e.Message}");
            return false;
        }
    }

    async Task<bool> MatchPVP()
    {
        try
        {
            if (_reqQueue.TryDequeue(out Int64 user_1) && _reqQueue.TryDequeue(out Int64 user_2))
            {
                // 대기 목록에서 사용자 제거
                waitingUserList.Remove(user_1);
                waitingUserList.Remove(user_2);

                var matchInfo = new MatchInfo()
                {
                    MatchGUID = Guid.NewGuid(),
                    MatchType = 0,
                    UserList = new List<Int64>() { user_1, user_2 }
                };

                if (await RegistWaitingInfo(matchInfo) == false)
                {
                    _reqQueue.Enqueue(user_1);
                    _reqQueue.Enqueue(user_2);
                }

                return true;
            }

            return false;
        }
        catch (Exception ex)
        {
            _logger.ZLogError($"MatchPVP Error : {ex}");
            return false;
        }
    }

    async Task<bool> MatchPVE()
    {
        try
        {
            if (!_reqQueue.TryPeek(out Int64 accountUid))
            {
                return false;
            }

            if (!waitingUserList.ContainsKey(accountUid))
            {
                waitingUserList[accountUid] = DateTime.Now;
                return false; // 아직 5초가 지나지 않았으므로 매칭하지 않음
            }

            var waitingTime = DateTime.Now - waitingUserList[accountUid];
            if (waitingTime.TotalSeconds <= 5)
            {
                return false; // 아직 5초가 지나지 않았으므로 매칭하지 않음
            }

            if (!_reqQueue.TryDequeue(out accountUid))
            {
                return false;
            }

            waitingUserList.Remove(accountUid);

            _logger.ZLogDebug($"MatchPVE 매칭 진행: {accountUid} (대기 시간: {waitingTime.TotalSeconds:F1}초)");

            var matchInfo = new MatchInfo()
            {
                MatchGUID = Guid.NewGuid(),
                MatchType = 1,
                UserList = new List<Int64>() { accountUid }
            };

            if (await RegistWaitingInfo(matchInfo) == false)
            {
                _reqQueue.Enqueue(accountUid);
                return false;
            }

            return true;
        }
        catch (Exception ex)
        {
            _logger.ZLogError($"MatchPVE Error : {ex}");
            return false;
        }
    }

    async Task Run()
    {
        while (true)
        {
            try
            {
                if (_reqQueue.Count >= 2)
                {
                    if (await MatchPVP() == false)
                    {
                        Thread.Sleep(1);
                    }
                }
                //else if (_reqQueue.Count == 1)
                //{
                //    if (await MatchPVE() == false)
                //    {
                //        Thread.Sleep(100); // 대기 시간을 약간 늘림
                //    }
                //}
                else
                {
                    Thread.Sleep(100); // 큐가 비어있을 때 CPU 점유율 낮추기
                }
            }
            catch (Exception ex)
            {
                _logger.ZLogError($"MatchService Error : {ex}");
                Thread.Sleep(1000);
            }
        }
    }
}
