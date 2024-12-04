using System;
using System.Collections.Generic;

using Microsoft.Extensions.Options;

namespace APIServer.Services;

public class ChannelUserManager
{
    private readonly object _lock = new object();
    private readonly ChannelConfig _config;
    private Int32 _lastRandomEnterChannelNumber;

    private Dictionary<Int32, List<Int64>> _usersByChannel;


    public ChannelUserManager(IOptions<ChannelConfig> config)
    {
        _config = config.Value;
        _lastRandomEnterChannelNumber = _config.ChannelStartNumber;
        _usersByChannel = new(_config.ChannelMaxCount);
        for (var i = _config.ChannelStartNumber; i < _config.ChannelMaxCount; i++)
        {
            _usersByChannel.Add(i, new List<Int64>(_config.ChannelUserMaxCount));
        }
    }


    public Int32 RandomEnter(Int64 userId)
    {
        bool isFind = false;

        lock (_lock)
        {
            for (var i = _lastRandomEnterChannelNumber; i < _config.ChannelMaxCount; i++)
            {
                if (_usersByChannel[i].Count != _config.ChannelUserMaxCount)
                {
                    isFind = true;
                    _lastRandomEnterChannelNumber = i;
                    break;
                }
            }

            if (_lastRandomEnterChannelNumber != _config.ChannelStartNumber && isFind == false)
            {
                for (var i = _config.ChannelStartNumber; i < _lastRandomEnterChannelNumber; i++)
                {
                    if (_usersByChannel[i].Count != _config.ChannelUserMaxCount)
                    {
                        isFind = true;
                        _lastRandomEnterChannelNumber = i;
                        break;
                    }
                }
            }

            if (isFind == true)
            {
                _usersByChannel[_lastRandomEnterChannelNumber].Add(userId);
                return _lastRandomEnterChannelNumber;
            }
        }

        return 0;
    }


    public ErrorCode Enter(Int32 channelNumber, Int64 userId)
    {
        List<Int64> users;
        if (_usersByChannel.TryGetValue(channelNumber, out users) == false)
        {
            return ErrorCode.InvalidChannelNumber;
        }
        if (users.Count == _config.ChannelUserMaxCount)
        {
            return ErrorCode.ChannelIsFull;
        }

        lock (_lock)
        {
            if (users.Contains(userId) == true)
            {
                return ErrorCode.AlreadyChannelUser;
            }
            users.Add(userId);
        }
        return ErrorCode.None;
    }


    public ErrorCode Leave(Int32 channelNumber, Int64 userId)
    {
        List<Int64> users;
        if (_usersByChannel.TryGetValue(channelNumber, out users) == false)
        {
            return ErrorCode.InvalidChannelNumber;
        }

        lock (_lock)
        {
            if (users.Remove(userId) == false)
            {
                return ErrorCode.InvalidChannelUserId;
            }
        }
        return ErrorCode.None;
    }



}
