using System;
using System.Threading.Tasks;

using APIServer.ModelDB;
using APIServer.ModelReqRes;
using APIServer.Services;
using APIServer.Services.MasterData;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using static LogManager;

namespace APIServer.Controllers;

[ApiController]
[Route("[controller]")]
public class EnhanceItem : ControllerBase
{
    private readonly ILogger<EnhanceItem> _logger;
    private readonly IGameDb _gameDb;
    private readonly MasterDataManager _masterDataMgr;
    private readonly Random _randomBox = new(Guid.NewGuid().GetHashCode());

    public EnhanceItem(ILogger<EnhanceItem> logger, IGameDb gameDb, MasterDataManager masterDataMgr)
    {
        _logger = logger;
        _gameDb = gameDb;
        _masterDataMgr = masterDataMgr;
        _randomBox = new Random();
    }

    [HttpPost]
    public async Task<EnhanceItemResponse> Post(EnhanceItemRequest request)
    {
        var response = new EnhanceItemResponse();
        var userInfo = (CertifiedUser)HttpContext.Items[nameof(CertifiedUser)]!;


        LoggingForInformation(_logger, EventType.EnhanceItem, "Received request", new { UserId = userInfo.UserId, InventoryItemId = request.InventoryItemId });


        // 강화 대상 아이템 정보 읽어오기
        var item = await LoadItemInfo(request.InventoryItemId);
        if (item is null)
        {
            response.Result = ErrorCode.NotExistUserItem;
            return response;
        }


        // 강화 가능 여부 확인
        if (IsPossibleEnhance(item) == false)
        {
            response.Result = ErrorCode.ImposibbleEnhanceItem;
            return response;
        }


        // 강화 시작
        var (error, isSuccess) = await Enhance(item);
        if (error != ErrorCode.None)
        {
            response.Result = error;
            return response;
        }


        response.IsSuccess = isSuccess;
        return response;
    }


    private async Task<UserInventoryItem> LoadItemInfo(Int64 inventoryItemId)
    {
        var (error, loadedData) = await _gameDb.GetUserInventoryItem(inventoryItemId);
        if (error != ErrorCode.None)
        {
            LoggingForError(_logger, EventType.EnhanceItem, "Invalid InventoryItemId", new { inventoryItemId = inventoryItemId });
            return null;
        }

        return loadedData;
    }


    public bool IsPossibleEnhance(UserInventoryItem itemInfo)
    {
        var enhanceMaxCount = _masterDataMgr.GetMaxEnhanceCount(itemInfo.item_code);

        if (enhanceMaxCount == 0
            || itemInfo.enhance_stage >= enhanceMaxCount)
        {
            LoggingForError(_logger, EventType.EnhanceItem, "It is over enhance count", new { inventoryItemId = itemInfo.inventory_item_id, EnhanceStage = itemInfo.enhance_stage });
            return false;
        }

        return true;
    }


    private async Task<(ErrorCode, bool)> Enhance(UserInventoryItem item)
    {
        var isSuccess = Gacha();

        LoggingForInformation(_logger, EventType.EnhanceItem, "Try Enhance");

        if (isSuccess == true)
        {
            return (await EnhanceSuccess(item), true);
        }

        return (await RemoveItem(item.inventory_item_id), false);
    }


    private async Task<ErrorCode> EnhanceSuccess(UserInventoryItem item)
    {
        var enhanceStage = GetNextEnhanceStage(item.enhance_stage);
        var itemType = _masterDataMgr.GetItemType(item.item_code);

        switch (itemType)
        {
            case (Int32)MasterDataCode.ItemTypeCode.무기:
                var attackPower = GetEnhanceIncreaseValue(item.item_attack_power);
                LoggingForInformation(_logger, EventType.EnhanceItem, "Successed Enhance",
                    new
                    {
                        ItemType = itemType,
                        PrevAttackPower = item.item_attack_power,
                        UpdateAttackPower = attackPower,
                        PrevEnhanceStage = item.enhance_stage,
                        UpdateEnhanceStage = enhanceStage
                    });

                return await _gameDb.UpdateEnhanceItemAttackPower(item.inventory_item_id, enhanceStage, attackPower);


            case (Int32)MasterDataCode.ItemTypeCode.방어구:
                var defensivePower = GetEnhanceIncreaseValue(item.item_defensive_power);
                LoggingForInformation(_logger, EventType.EnhanceItem, "Successed Enhance",
                    new
                    {
                        ItemType = itemType,
                        PrevDefensivePower = item.item_defensive_power,
                        UpdateDefensivePower = defensivePower,
                        PrevEnhanceStage = item.enhance_stage,
                        UpdateEnhanceStage = enhanceStage
                    });

                return await _gameDb.UpdateEnhanceItemDefensivePower(item.inventory_item_id, enhanceStage, defensivePower);

            default:
                return ErrorCode.InvalidItemType;
        }
    }


    private Int32 GetNextEnhanceStage(Int32 originStage) => originStage + 1;


    private Int64 GetEnhanceIncreaseValue(Int64 originValue) => originValue + GetIncreaseValue(originValue);


    private readonly double EnhanceSuccessIncreasePercentage = 10.0f;
    private Int32 GetIncreaseValue(double originValue)
    {
        // 기존값의 10% 값을 더한 후 소수 첫번째 자리에서 반올림한다.
        var increaseValue = (double)originValue / EnhanceSuccessIncreasePercentage;
        increaseValue = Math.Round(increaseValue, 1);

        LoggingForInformation(_logger, EventType.EnhanceItem, "the rate of increase", new { OriginValue = originValue, Increase = (Int32)Math.Ceiling(increaseValue) });

        return (Int32)Math.Ceiling(increaseValue);
    }


    private readonly Int32 RandomMinValue = 1;
    private readonly Int32 RandomMaxValue = 100;
    private readonly Int16 EnhanceSuccessProbability = 30;
    private bool Gacha()
    {
        var randomNumber = _randomBox.Next(RandomMinValue, RandomMaxValue);

        if (randomNumber <= EnhanceSuccessProbability)
        {
            return true;
        }

        return false;
    }


    private async Task<ErrorCode> RemoveItem(Int64 inventoryItemId)
    {
        LoggingForInformation(_logger, EventType.EnhanceItem, "Failed Enhance");

        var error = await _gameDb.RemoveUserInventoryItem(inventoryItemId);
        if (error != ErrorCode.None)
        {
            LoggingForError(_logger, EventType.EnhanceItem, "Failed GameDb.RemoveUserInventoryItem()", new { inventoryItemId = inventoryItemId });
        }

        LoggingForInformation(_logger, EventType.EnhanceItem, "Successed Remove InventoryItem", new { inventoryItemId = inventoryItemId });

        return error;
    }



}