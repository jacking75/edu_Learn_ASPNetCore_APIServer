using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using StackExchange.Redis;
using WaitingQueue.Server.Hubs;
using WaitingQueue.Server.Models;

namespace WaitingQueue.Server.Services;

public class QueueService : IQueueService
{
    private readonly IDatabase _redis;
    private readonly IConfiguration _config;
    private readonly IHubContext<QueueHub> _hubContext;
    private const string QueueKey = "waiting:queue";
    private const string ActiveUsersKey = "active:users";
    private const string UserDataPrefix = "user:data:";

    public QueueService(IConnectionMultiplexer redis, IConfiguration config, IHubContext<QueueHub> hubContext)
    {
        _redis = redis.GetDatabase();
        _config = config;
        _hubContext = hubContext;
    }

    public async Task<QueueAddResult> AddToQueueAsync(string userId, UserData userData)
    {
        var timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        var queueToken = Guid.NewGuid().ToString();

        // 사용자 데이터 저장 (Hash)
        var userKey = $"{UserDataPrefix}{userId}";
        await _redis.HashSetAsync(userKey, new HashEntry[]
        {
            new("userId", userId),
            new("queueToken", queueToken),
            new("joinedAt", timestamp.ToString()),
            new("email", userData.Email ?? ""),
            new("metadata", JsonConvert.SerializeObject(userData.Metadata ?? new object()))
        });
        await _redis.KeyExpireAsync(userKey, TimeSpan.FromMinutes(_config.GetValue<int>("Queue:TimeoutMinutes")));

        // 대기열에 추가 (Sorted Set)
        await _redis.SortedSetAddAsync(QueueKey, userId, timestamp);

        var position = await GetQueuePositionAsync(userId);
        return new QueueAddResult { /* ... */ };
    }

    public async Task<List<string>> ProcessQueueAsync()
    {
        var activeUsers = await _redis.SetLengthAsync(ActiveUsersKey);
        var availableSlots = _config.GetValue<int>("Queue:MaxConcurrentUsers") - activeUsers;

        if (availableSlots <= 0) return new List<string>();

        // 대기열에서 사용자 가져오기
        var nextUsers = await _redis.SortedSetRangeByRankAsync(QueueKey, 0, availableSlots - 1);
        var processedUsers = new List<string>();

        foreach (var userId in nextUsers)
        {
            await _redis.SortedSetRemoveAsync(QueueKey, userId);
            await _redis.SetAddAsync(ActiveUsersKey, userId);
            processedUsers.Add(userId.ToString());
        }
        return processedUsers;
    }


    /// <summary>
    /// 특정 사용자의 현재 대기열 상태를 가져옵니다.
    /// </summary>
    /// <param name="userId">사용자 ID</param>
    /// <returns>사용자의 대기열 상태</returns>
    public async Task<QueueStatus> GetQueueStatusAsync(string userId)
    {
        // 1. 대기열(Sorted Set)에서 사용자의 순위(rank)를 확인합니다.
        var rank = await _redis.SortedSetRankAsync(QueueKey, userId);

        // 2. 사용자가 대기열에 없는 경우
        if (rank is null)
        {
            // 활성 사용자 목록(Set)에 있는지 확인합니다.
            if (await _redis.SetContainsAsync(ActiveUsersKey, userId))
            {
                return new QueueStatus { Status = "active", CanAccess = true };
            }
            // 어디에도 없으면 "not_in_queue" 상태를 반환합니다.
            return new QueueStatus { Status = "not_in_queue", CanAccess = false };
        }

        // 3. 사용자가 대기열에 있는 경우, 관련 정보를 계산하여 반환합니다.
        var position = rank.Value + 1;
        var totalInQueue = await _redis.SortedSetLengthAsync(QueueKey);
        var activeUsers = await _redis.SetLengthAsync(ActiveUsersKey);

        return new QueueStatus
        {
            Status = "waiting",
            Position = position,
            TotalInQueue = totalInQueue,
            ActiveUsers = activeUsers,
            EstimatedWaitTime = CalculateWaitTime(position),
            CanAccess = false
        };
    }


    /// <summary>
    /// 대기열 또는 활성 사용자 목록에서 사용자를 제거합니다.
    /// </summary>
    /// <param name="userId">제거할 사용자 ID</param>
    /// <returns>제거 성공 여부</returns>
    public async Task<bool> RemoveFromQueueAsync(string userId)
    {
        // 1. 대기열(Sorted Set)에서 사용자를 제거합니다.
        var removedFromQueue = await _redis.SortedSetRemoveAsync(QueueKey, userId);

        // 2. 활성 사용자 목록(Set)에서도 제거를 시도합니다.
        var removedFromActive = await _redis.SetRemoveAsync(ActiveUsersKey, userId);

        // 3. 사용자 관련 데이터(Hash)를 삭제합니다.
        await _redis.KeyDeleteAsync($"{UserDataPrefix}{userId}");

        // 대기열이나 활성 목록 중 한 곳에서라도 제거되었다면 true를 반환합니다.
        return removedFromQueue || removedFromActive;
    }


    /// <summary>
    /// 전체 대기열의 현재 정보를 가져옵니다. (관리자용)
    /// </summary>
    /// <returns>전체 대기열 정보</returns>
    public async Task<QueueInfo> GetQueueInfoAsync()
    {
        // 1. 현재 대기 인원과 활성 사용자 수를 가져옵니다.
        var queueLength = await _redis.SortedSetLengthAsync(QueueKey);
        var activeUsers = await _redis.SetLengthAsync(ActiveUsersKey);
        var maxConcurrent = _config.GetValue<int>("Queue:MaxConcurrentUsers");

        // 2. 대기열의 맨 앞 10명의 정보를 가져옵니다.
        var queueMembers = await _redis.SortedSetRangeByRankWithScoresAsync(QueueKey, 0, 9);

        // 3. 조회된 사용자 정보를 QueueMemberInfo 모델로 변환합니다.
        var nextInQueue = queueMembers.Select((member, index) => new QueueMemberInfo
        {
            UserId = member.Element.ToString(),
            Position = index + 1,
            JoinedAt = DateTimeOffset.FromUnixTimeMilliseconds((long)member.Score).UtcDateTime,
            EstimatedWaitTime = CalculateWaitTime(index + 1)
        }).ToList();

        return new QueueInfo
        {
            QueueLength = queueLength,
            ActiveUsers = activeUsers,
            MaxConcurrentUsers = maxConcurrent,
            AvailableSlots = Math.Max(0, maxConcurrent - activeUsers),
            NextInQueue = nextInQueue
        };
    }

    /// <summary>
    /// 모든 대기열 및 활성 사용자 정보를 초기화합니다.
    /// </summary>
    public async Task ClearQueueAsync()
    {
        // 1. 대기열과 활성 사용자 Set의 키를 삭제합니다.
        await _redis.KeyDeleteAsync(new RedisKey[] { QueueKey, ActiveUsersKey });

        // 2. Redis 서버에서 "user:data:" 패턴을 가진 모든 키를 찾아 삭제합니다.
        // 주의: KEYS 명령어는 운영 환경에서 성능 문제를 일으킬 수 있으므로,
        // SCAN 명령어를 사용하거나 다른 방식으로 관리하는 것이 좋습니다.
        // 여기서는 Node.js 코드와의 일관성을 위해 KEYS를 사용합니다.
        var server = _redis.Multiplexer.GetServer(_redis.Multiplexer.GetEndPoints().First());
        var userKeys = server.Keys(pattern: $"{UserDataPrefix}*").ToArray();
        if (userKeys.Length > 0)
        {
            await _redis.KeyDeleteAsync(userKeys);
        }
    }

    /// <summary>
    /// 대기열에 남아있는 모든 사용자에게 변경된 순번을 실시간으로 알립니다.
    /// </summary>
    public async Task UpdateQueuePositionsAsync()
    {
        // 1. 현재 대기열 정보를 가져옵니다.
        var queueInfo = await GetQueueInfoAsync();

        // 2. 대기열에 있는 각 사용자에게 SignalR을 통해 업데이트된 정보를 전송합니다.
        foreach (var user in queueInfo.NextInQueue)
        {
            await _hubContext.Clients.Group($"user-{user.UserId}").SendAsync("queue-update", new
            {
                position = user.Position,
                estimatedWaitTime = user.EstimatedWaitTime,
                timestamp = DateTime.UtcNow
            });
        }
    }




    private async Task<long?> GetQueuePositionAsync(string userId)
    {
        var rank = await _redis.SortedSetRankAsync(QueueKey, userId);
        return rank.HasValue ? rank + 1 : null;
    }

    private WaitTime CalculateWaitTime(long? position)
    {
        if (position is null || position.Value <= 0) return new WaitTime();

        var maxConcurrent = _config.GetValue<double>("Queue:MaxConcurrentUsers");
        var serviceTime = _config.GetValue<int>("Queue:EstimatedServiceTimeSeconds");

        // 자신의 차례가 몇 번째 배치에 속하는지 계산합니다.
        var batchPosition = Math.Ceiling(position.Value / maxConcurrent);
        var estimatedSeconds = (int)(batchPosition * serviceTime);

        return new WaitTime
        {
            Seconds = estimatedSeconds,
            Minutes = (int)Math.Ceiling(estimatedSeconds / 60.0),
            Formatted = FormatWaitTime(estimatedSeconds)
        };
    }

    private string FormatWaitTime(int seconds)
    {
        var timeSpan = TimeSpan.FromSeconds(seconds);
        if (timeSpan.TotalMinutes < 1) return $"{timeSpan.Seconds} seconds";
        if (timeSpan.TotalHours < 1) return $"{Math.Ceiling(timeSpan.TotalMinutes)} minutes";

        return $"{timeSpan.Hours} hours {timeSpan.Minutes} minutes";
    }

}