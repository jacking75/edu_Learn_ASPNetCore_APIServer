using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;

using APIServer.ModelDB;

using CloudStructures;
using CloudStructures.Structures;

using Microsoft.Extensions.Logging;

using SqlKata.Execution;

using StackExchange.Redis;

using static LogManager;

namespace APIServer.Services;

public class RedisDb : IMemoryDb
{
    private readonly Int32 ChannelMessageMaxCount = 50;
    private readonly Int32 ChannelMinNum = 1;
    private readonly Int32 ChannelMaxNum = 100;
    private readonly ILogger<RedisDb> _logger = GetLogger<RedisDb>();
    private RedisConnection _connection;
    private string _gameNotice;


    public void Init(string redisAddress)
    {
        var config = new RedisConfig(null, redisAddress);
        _connection = new RedisConnection(config);

        TryConnectCheck();
    }


    private async Task RemoveAllChannel()
    {
        for (var i = ChannelMinNum; i <= ChannelMaxNum; i++)
        {
            var key = MemoryDbKeyMaker.GetChannelKey() + i.ToString();

            var db = _connection.GetConnection().GetDatabase();
            await db.KeyDeleteAsync(key);
        }
    }


    public async ValueTask DisposeAsync()
    {
        await RemoveAllChannel();

        Close();
    }


    public string GetGameNotice() => _gameNotice;


    public async Task<bool> LoadGameNotice(string key)
    {
        try
        {
            var handle = new RedisString<string>(_connection, key, null);
            var loadedData = await handle.GetAsync();
            if (loadedData.HasValue == false)
            {
                return false;
            }

            _gameNotice = loadedData.Value;

            return true;
        }
        catch (Exception ex)
        {
            return false;
        }
    }


    public async Task<ErrorCode> CheckCertifiedUser(string email, string authToken, Int32 appVersion, Int32 masterDataVersion)
    {
        try
        {
            var (isExists, storedCertifiedUser) = await GetCertifiedUser(email);

            if (isExists == false)
            {
                return ErrorCode.NotExsitCertifiedUser;
            }

            if (storedCertifiedUser.Validate(email, authToken, appVersion, masterDataVersion) == false)
            {
                return ErrorCode.NotCertifiedUser;
            }

            return ErrorCode.None;
        }
        catch (Exception ex)
        {
            return ErrorCode.RedisException;
        }
    }


    #region Set Services

    public async Task<ErrorCode> RegistCertifiedUserInfo(string id, CertifiedUser value)
    {
        var key = MemoryDbKeyMaker.MakeCertifiedUserKey(id);

        try
        {
            var handle = new RedisString<CertifiedUser>(_connection, key, null);
            await handle.SetAsync(value, LoginTimeSpan());

            return ErrorCode.None;
        }
        catch (Exception ex)
        {
            return ErrorCode.RedisException;
        }
    }


    public async Task<ErrorCode> UpdateCertifiedUserInfo(string id, CertifiedUser value)
    {
        var key = MemoryDbKeyMaker.MakeCertifiedUserKey(id);

        try
        {
            var handle = new RedisString<CertifiedUser>(_connection, key, null);
            await handle.SetAsync(value, LoginTimeSpan());

            return ErrorCode.None;
        }
        catch (Exception ex)
        {
            return ErrorCode.RedisException;
        }
    }


    public async Task<ErrorCode> RegistUserBattleInfo(string id, UserBattleInfo value)
    {
        var key = MemoryDbKeyMaker.MakeBattleInfoKey(id);

        try
        {
            var handle = new RedisString<UserBattleInfo>(_connection, key, null);
            if (await handle.SetAsync(value, BattleTimeSpan()) == false)
            {
                return ErrorCode.FailedRedisRegist;
            }

            return ErrorCode.None;
        }
        catch (Exception ex)
        {
            return ErrorCode.RedisException;
        }
    }


    public async Task<ErrorCode> UpdateUserBattleInfo(string id, UserBattleInfo value)
    {
        var key = MemoryDbKeyMaker.MakeBattleInfoKey(id);

        try
        {
            var handle = new RedisString<UserBattleInfo>(_connection, key, null);
            await handle.SetAsync(value, BattleTimeSpan());

            return ErrorCode.None;
        }
        catch (Exception ex)
        {
            return ErrorCode.RedisException;
        }
    }


    public async Task<bool> TryLockUserRequest(string id)
    {
        var key = MemoryDbKeyMaker.MakeUserRequestLockKey(id);

        try
        {
            var handle = new RedisString<CertifiedUser>(_connection, key, null);

            // 이미 있는 Key라면 false
            if (await handle.SetAsync(new CertifiedUser { }, NxKeyTimeSpan(), When.NotExists) == false)
            {
                return false;
            }

            return true;
        }
        catch (Exception ex)
        {
            return false;
        }
    }


    public async Task<bool> AddChannelChatInfo(Int32 channelNumber, ChatInfo value)
    {
        var key = MemoryDbKeyMaker.MakeChannelKey(channelNumber);

        try
        {
            var db = _connection.GetConnection().GetDatabase();
            var result = await db.StreamAddAsync(
                key: key,
                streamField: "",
                streamValue: JsonSerializer.Serialize(value),
                maxLength: ChannelMessageMaxCount
            );

            if (result.HasValue == false)
            {
                return false;
            }

            return true;
        }
        catch (Exception ex)
        {
            return false;
        }
    }

    #endregion



    #region Get Services


    public async Task<(bool, CertifiedUser)> GetCertifiedUser(string id)
    {
        var key = MemoryDbKeyMaker.MakeCertifiedUserKey(id);

        try
        {
            var handle = new RedisString<CertifiedUser>(_connection, key, null);
            var loadedData = await handle.GetAsync();
            if (loadedData.HasValue == false)
            {
                return (false, null);
            }

            return (true, loadedData.Value);
        }
        catch (Exception ex)
        {
            return (false, null);
        }
    }


    public async Task<(bool, UserBattleInfo)> GetUserBattleInfo(string id)
    {
        var key = MemoryDbKeyMaker.MakeBattleInfoKey(id);

        try
        {
            var handle = new RedisString<UserBattleInfo>(_connection, key, null);
            var loadedData = await handle.GetAsync();
            if (loadedData.HasValue == false)
            {
                return (false, null);
            }

            return (true, loadedData.Value);
        }
        catch (Exception ex)
        {
            return (false, null);
        }
    }



    public async Task<(bool, List<ChatInfo>)> GetChannelChatInfoList(Int32 channelNumber, Int32 count, string messageId)
    {
        var key = MemoryDbKeyMaker.MakeChannelKey(channelNumber);

        try
        {
            var db = _connection.GetConnection().GetDatabase();

            StreamEntry[] loadedData;
            if (string.IsNullOrEmpty(messageId) == true)
            {
                // MessageId가 없다면, 현재 존재하는 채널 메시지 중 Id 기준 내림 차순으로 ChannelLatestMessageGetCount 만큼 읽어온다.
                loadedData = await db.StreamRangeAsync(key, "-", "+", count, Order.Descending);
            }
            else
            {
                // MessageId가 있다면, 해당 Id 다음 메시지들을 ChannelLatestMessageGetCount 만큼 읽어온다.
                loadedData = await db.StreamReadAsync(key, position: messageId, count);
            }

            // 메시지가 존재하지 않는다.
            if (loadedData is null)
            {
                return (true, null);
            }

            // 읽어온 스트림 데이터들에서 메시지 정보를 출력한다.
            var messageList = DeserializeStreamDatas(loadedData);

            return (true, messageList);
        }
        catch (Exception ex)
        {
            return (false, null);
        }
    }


    private List<ChatInfo> DeserializeStreamDatas(StreamEntry[] streams)
    {
        var messageList = new List<ChatInfo>(streams.Length);

        foreach (var streamEntry in streams)
        {
            // StreamRangeAsync()와 StreamReadAsync()의 반환 자료형은 StreamEntry[]다.
            // streamEntry는 Id와 Values 나뉘어지는데, Id는 해당 스트림의 고유값이며, Values는 Feild와 value로 구분된다.
            // 여기서 Feild는 스트림 데이터를 구분하는 구분자이며(해당 서버에서는 사용하지 않는다.)
            // Value에 우리의 ChatInfo 클래스가 직렬화되어있는 스트링 데이터가 담겨있다.
            var rawData = streamEntry.Values[0].Value.ToString();
            var message = JsonSerializer.Deserialize<ChatInfo>(rawData);
            message.MessageId = streamEntry.Id;

            messageList.Add(message);
        }

        return messageList;
    }

    #endregion

    #region Del Services

    public async Task<bool> UnlockUserRequest(string id)
    {
        var key = MemoryDbKeyMaker.MakeUserRequestLockKey(id);

        try
        {
            var handle = new RedisString<CertifiedUser>(_connection, key, null);
            return await handle.DeleteAsync();
        }
        catch (Exception ex)
        {
            return false;
        }
    }


    public async Task<bool> RemoveUserBattleInfo(string id)
    {
        var key = MemoryDbKeyMaker.MakeBattleInfoKey(id);

        try
        {
            var handle = new RedisString<UserBattleInfo>(_connection, key, null);
            return await handle.DeleteAsync();
        }
        catch (Exception ex)
        {
            return false;
        }
    }

    #endregion



    private TimeSpan NxKeyTimeSpan()
    {
        return TimeSpan.FromSeconds(MemoryDbKeyExpireTime.UserRequestLockSecond);
    }


    private TimeSpan LoginTimeSpan()
    {
        return TimeSpan.FromMinutes(MemoryDbKeyExpireTime.LoginMin);
    }


    private TimeSpan BattleTimeSpan()
    {
        return TimeSpan.FromMinutes(MemoryDbKeyExpireTime.BattleMin);
    }


    private void TryConnectCheck()
    {
        _connection.GetConnection();
    }


    private void Close()
    {
        _connection.GetConnection().Close();
    }



}