using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using APIServer.ModelDB;
using APIServer.ModelReqRes;
using APIServer.Services;
using APIServer.Services.MasterData;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace APIServer.Controllers;


[ApiController]
[Route("[controller]")]
public class CompleteStage : ControllerBase
{
    private readonly ILogger<CompleteStage> _logger;

    private readonly IGameDb _gameDb;

    private readonly IMemoryDb _memoryDb;

    private readonly MasterDataManager _masterDataMgr;


    public CompleteStage(ILogger<CompleteStage> logger, IGameDb gameDb, IMemoryDb memoryDb, MasterDataManager masterDataMgr)
    {
        _logger = logger;
        _gameDb = gameDb;
        _memoryDb = memoryDb;
        _masterDataMgr = masterDataMgr;
    }

    private CertifiedUser LoadCertifiedUserInfo() => (CertifiedUser)HttpContext.Items[nameof(CertifiedUser)]!;


    [HttpPost]
    public async Task<CompleteStageResponse> Post(CompleteStageRequest request)
    {
        var response = new CompleteStageResponse();
        var userInfo = LoadCertifiedUserInfo();

        var userId = userInfo.UserId;
        var email = userInfo.Email;
        var stageCode = request.StageCode;


        // 유저의 전투 정보 읽기
        var battleInfo = await LoadBattleInfo(email);
        if (battleInfo is null)
        {
            response.Result = ErrorCode.NotExistsUserBattleInfo;
            return response;
        }


        // 요청 데이터의 스테이지 코드가 현재 전투 중인 스테이지인지 확인
        if (battleInfo.Validate(stageCode) == false)
        {
            response.Result = ErrorCode.InvalidBattleInfo;
            return response;
        }


        // 모든 적을 처리했는지 확인
        if (battleInfo.IsAllSlainEnemies() == false)
        {
            response.Result = ErrorCode.NotCompletedStage;
            return response;
        }


        // 스테이지 완료 이력 갱신
        var (recordSuccess, compltedDate) = await RecordHistory(userId, stageCode);
        if (recordSuccess == false)
        {
            response.Result = ErrorCode.FailedAddDompletedStageHistory;
            return response;
        }


        // 획득한 경험치 추가
        if (await UpdateUserExp(userId, battleInfo.CompleteRewardExp) == false)
        {
            await Rollback(userId, stageCode, compltedDate);

            response.Result = ErrorCode.FailedUpdateUserExp;
            return response;
        }


        // 파밍한 아이템 추가
        var (success, addedInfos) = await AddFarmingItemFromUser(userId, battleInfo);
        if (success == false)
        {
            await Rollback(userId, stageCode, compltedDate, battleInfo.CompleteRewardExp, addedInfos);

            response.Result = ErrorCode.FailedAddUserItem;
            return response;
        }


        // 전투 정보 삭제
        await RemoveBattleInfo(email);




        return response;
    }


    private async Task<UserBattleInfo> LoadBattleInfo(string email)
    {
        var (exist, loadedData) = await _memoryDb.GetUserBattleInfo(email);
        if (exist == false)
        {
        }

        return loadedData;
    }


    private async Task<(bool, DateTime)> RecordHistory(Int64 userId, Int32 stageCode)
    {
        bool success;
        var (_, completedDate) = await _gameDb.GetStageCompleteDate(userId, stageCode);

        // 해당 스테이지를 완료했었다면 완료 날짜만 업데이트.
        if (completedDate != DateTime.MinValue)
        {
            success = await UpdateHistoryDate(userId, stageCode);
            return (success, completedDate);
        }

        // 그게 아니면 신규 추가
        success = await AddHistory(userId, stageCode);
        return (success, DateTime.MinValue);
    }


    private async Task<bool> UpdateHistoryDate(Int64 userId, Int32 stageCode)
    {
        if (await _gameDb.UpdateCompletedStageHistory(userId, stageCode, DateTime.Now) != ErrorCode.None)
        {
            return false;
        }
        return true;
    }


    private async Task<bool> AddHistory(Int64 userId, Int32 stageCode)
    {
        if (await _gameDb.AddCompleteStageHistory(userId, stageCode) == false)
        {
            return false;
        }
        return true;
    }


    private async Task<bool> UpdateUserExp(Int64 userId, Int32 amount)
    {
        var error = await _gameDb.UpdateUserExp(userId, amount);
        if (error != ErrorCode.None)
        {
            return false;
        }

        return true;
    }


    private async Task<(bool, List<(Int64, Int32)>)> AddFarmingItemFromUser(Int64 userId, UserBattleInfo info)
    {
        var addedInfos = new List<(Int64, Int32)>(info.FarmedItems.Count);

        foreach (var farmedItem in info.FarmedItems)
        {
            Int64 addedId;
            var itemInfo = _masterDataMgr.GetItemInfo(farmedItem.Key);
            for (var i = 0; i < farmedItem.Value; i++)
            {
                if (MasterDataCode.IsPossibleOverlap(itemInfo.item_type_code) == true)
                {
                    addedId = await AddUserItemForOverlapType(userId, itemInfo, 1);
                }
                else
                {
                    addedId = await _gameDb.AddUserItemAndGetId(userId, itemInfo, 1);
                }

                // 실패
                if (addedId == 0)
                {
                    return (false, addedInfos);
                }

                addedInfos.Add((addedId, itemInfo.item_code));
            }
        }

        return (true, addedInfos);
    }


    private async Task<Int64> AddUserItemForOverlapType(Int64 userId, ItemInfo itemInfo, Int32 count)
    {
        var existCount = await _gameDb.GetInventoryItemCount(userId, itemInfo.item_code);
        if (existCount > 0)
        {
            // 이미 존재하는 경우 개수만 증가시킨다.
            return await UpdateJustItemCount(userId, itemInfo.item_code, existCount + count);
        }

        return await _gameDb.AddUserItemAndGetId(userId, itemInfo, count);
    }


    private async Task<Int64> UpdateJustItemCount(Int64 userId, Int32 itemCode, Int32 count)
    {
        var updatedId = await _gameDb.UpdateInventoryItemCountAndGetId(userId, itemCode, count);
        if (updatedId == 0)
        {
        }

        return updatedId;
    }


    private async Task<bool> RemoveBattleInfo(string authToken)
    {
        if (await _memoryDb.RemoveUserBattleInfo(authToken) == false)
        {
            return false;
        }

        return true;
    }


    private async Task Rollback(Int64 userId, Int32 completedStageCode, DateTime completedDate, Int32 exp = 0, List<(Int64, Int32)> addedInfoList = null)
    {
        // 1. 추가한 경험치 차감
        await _gameDb.UpdateUserExp(userId, -exp);

        // 2. 추가한 아이템 삭제
        await RollbackAddedItemList(userId, addedInfoList);

        // 3. 추가한 완료 이력 삭제
        if (completedDate != DateTime.MinValue)
        {
            // 이전에 완료 이력이 있다면, 날짜만 이전 날짜로 갱신
            await _gameDb.UpdateCompletedStageHistory(userId, completedStageCode, completedDate);
        }
        else
        {
            // 신규 추가된 이력이라면 이력 자체를 삭제한다.
            await _gameDb.RemoveCompletedStageHistory(userId, completedStageCode);
        }
    }


    private async Task RollbackAddedItemList(Int64 userId, List<(Int64, Int32)> addedInfos)
    {
        foreach (var addedInfo in addedInfos)
        {
            var itemCode = addedInfo.Item2;
            var itemInfo = _masterDataMgr.GetItemInfo(itemCode);
            var inventoryItemId = addedInfo.Item1;

            if (MasterDataCode.IsPossibleOverlap(itemInfo.item_type_code) == true)
            {
                // 개수만 증가시킨 아이템은 개수만 차감한다.
                await _gameDb.UpdateInventoryItemCountAndGetId(userId, itemCode, -1);
            }
            else
            {
                // 신규 추가된 아이템이라면 아이템 자체를 인벤토리에서 삭제한다.
                await _gameDb.RemoveUserInventoryItem(inventoryItemId);
            }
        }
    }



}
