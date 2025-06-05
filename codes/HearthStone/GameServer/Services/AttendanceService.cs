using GameServer.Models;
using GameServer.Services.Interface;
using GameServer.Repository.Interface;
using ZLogger;
using System.Transactions;
using System.Data;
using GameServer.Models.DTO;
using System.Reflection;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using GameServer.Repository;

namespace GameServer.Services;

public class AttendanceService : IAttendanceService
{
    readonly ILogger<AttendanceService> _logger;
    readonly IGameDb _gameDb;
    readonly IMasterDb _masterDb;

    public AttendanceService(ILogger<AttendanceService> logger, IGameDb gameDb, IMasterDb masterDb)
    {
        _logger = logger;
        _gameDb = gameDb;
        _masterDb = masterDb;
    }

    public async Task<(ErrorCode, List<AttendanceInfo>)> GetAttendanceInfoList(Int64 accountUid)
    {
        try
        {
            return (ErrorCode.None, await _gameDb.GetAttendanceInfoList(accountUid));
        }
        catch (Exception e)
        {
            _logger.ZLogError(e,
                        $"[Attendance.GetAttendance] ErrorCode: {ErrorCode.AttendanceInfoFailException}, accountUid: {accountUid}");

            return (ErrorCode.AttendanceInfoFailException, null);
        }
    }
    public async Task<(ErrorCode, ReceivedReward?)> CheckAttendanceAndReceiveRewards(Int64 accountUid, int eventKey)
    {
        IDbTransaction? transaction = null;
        try
        {
            // 출석 정보 조회
            var attendanceInfo = await _gameDb.GetAttendanceInfo(accountUid, eventKey);
            if (attendanceInfo == null)
            {
                return (ErrorCode.AttendanceInfoFailException, null);
            }

            // 이미 오늘 출석했는지 체크 (CheckAttendance가 1 반환 시 성공)
            transaction = _gameDb.GetDbConnection().BeginTransaction();
            int rowCount = await _gameDb.CheckAttendance(accountUid, eventKey, transaction);
            if (rowCount < 1)
            {
                transaction.Rollback();
                return (ErrorCode.AttendanceCheckFailAlreadyChecked, null);
            }

            // 출석 보상 정보 조회
            int nextDaySeq = attendanceInfo.attendance_no + 1;
            var rewardInfo = _masterDb._attendanceRewardList
                .FirstOrDefault(r => r.event_id == eventKey && r.day_seq == nextDaySeq);

            if (rewardInfo == null)
            {
                transaction.Commit();
                return (ErrorCode.None, new ReceivedReward
                {
                    CurrencyList = new List<AssetInfo>(),
                    ItemList = new List<ItemInfo>()
                });
            }

            // 보상 상세 정보 조회
            var rewardDetailList = _masterDb._rewardInfoList
                .Where(r => r.reward_key == rewardInfo.reward_key)
                .ToList();

            var receivedReward = new ReceivedReward
            {
                CurrencyList = new List<AssetInfo>(),
                ItemList = new List<ItemInfo>()
            };

            // 보상 지급
            foreach (var reward in rewardDetailList)
            {
                if (reward.reward_class == "currency")
                {
                    var currency = new AssetInfo
                    {
                        asset_name = reward.reward_type,
                        asset_amount = reward.reward_value
                    };
                    int result = await _gameDb.AddAssetInfo(accountUid, currency.asset_name, currency.asset_amount, transaction);
                    if (result < 1)
                    {
                        transaction.Rollback();
                        return (ErrorCode.AttendanceCheckFailUpdateMoney, null);
                    }
                    receivedReward.CurrencyList.Add(currency);
                }
                else if (reward.reward_class == "item")
                {
                    var itemId = int.Parse(reward.reward_type);

                    var item = new ItemInfo
                    {
                        item_id = itemId,
                        item_cnt = (int)reward.reward_value
                    };
                    int result = await _gameDb.AddItemInfo(accountUid, item.item_id, item.item_cnt, transaction);
                    if (result < 1)
                    {
                        transaction.Rollback();
                        return (ErrorCode.AttendanceCheckFailUpdateItem, null);
                    }
                    receivedReward.ItemList.Add(item);
                }
            }

            transaction.Commit();
            return (ErrorCode.None, receivedReward);
        }
        catch (Exception e)
        {
            transaction?.Rollback();
            _logger.ZLogError(e, $"[Attendance.CheckAttendanceAndReceiveRewards] Error: {e.Message}, accountUid: {accountUid}, Key: {eventKey}");
            return (ErrorCode.AttendanceCheckFailException, null);
        }
    }
}

