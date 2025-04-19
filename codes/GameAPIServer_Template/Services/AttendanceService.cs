using GameAPIServer.Repository.Interfaces;
using GameAPIServer.Servicies.Interfaces;
using GameAPIServer.Models.DAO;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ZLogger;
using GameAPIServer.Models;

namespace GameAPIServer.Servicies;

public class AttendanceService : IAttendanceService
{
    readonly ILogger<AttendanceService> _logger;
    readonly IGameDb _gameDb;
    readonly IMasterDb _masterDb;
    readonly IItemService _itemService;

    public AttendanceService(ILogger<AttendanceService> logger, IGameDb gameDb, IMasterDb masterDb, IItemService itemService)
    {
        _logger = logger;
        _gameDb = gameDb;
        _masterDb = masterDb;
        _itemService = itemService;
    }

    public async Task<(ErrorCode, GdbAttendanceInfo)> GetAttendanceInfo(int uid)
    {
        try
        {
            return (ErrorCode.None, await _gameDb.GetAttendanceById(uid));
        }
        catch (Exception e)
        {
            _logger.ZLogError(e,
                $"[Attendance.GetAttendance] ErrorCode: {ErrorCode.AttendanceInfoFailException}, Uid: {uid}");
            return (ErrorCode.AttendanceInfoFailException, null);
        }
    }

    public async Task<(ErrorCode, List<ReceivedReward>)> CheckAttendanceAndReceiveRewards(int uid)
    {
        try
        {
            List<ReceivedReward> totalRewards = [];

            //출석 체크
            var rowCount = await _gameDb.CheckAttendanceById(uid);
            if (rowCount != 1)
            {
                return (ErrorCode.AttendanceCheckFailAlreadyChecked, null);
            }

            var attendanceInfo = await _gameDb.GetAttendanceById(uid);
            var attendanceCnt = attendanceInfo.attendance_cnt;

            //출석 보상 수령
            var reward = _masterDb._attendanceRewardList.Find(reward => reward.day_seq == attendanceCnt);
            
            // 가챠 보상일 경우
            if(reward.reward_type == "gacha")
            {
                for (int i = 0; i < reward.reward_qty; i++)
                {
                    var (errorCode, rewards) = await _itemService.ReceiveOneGacha(uid, reward.reward_key);
                    if (errorCode != ErrorCode.None)
                    {
                        return (errorCode, null);
                    }
                    totalRewards.Add(new ReceivedReward(reward.reward_key, rewards));
                }
               
                return (ErrorCode.None, totalRewards);
            }
            // 일반 보상일 경우
            else
            {
                await _itemService.ReceiveReward(uid, reward);
                totalRewards.Add(new ReceivedReward(reward.reward_key, [reward]));
                return (ErrorCode.None, totalRewards);
            }
        }
        catch (Exception e)
        {
            _logger.ZLogError(e,
                $"[Attendance.CheckAttendance] ErrorCode: {ErrorCode.AttendanceCheckFailException}, Uid: {uid}");
            return (ErrorCode.AttendanceCheckFailException, null);
        }
    }
}


