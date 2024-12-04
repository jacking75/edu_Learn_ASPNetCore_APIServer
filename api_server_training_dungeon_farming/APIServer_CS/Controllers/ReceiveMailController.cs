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
public class ReceiveMail : ControllerBase
{
    private readonly ILogger<ReceiveMail> _logger;

    private readonly IGameDb _gameDb;

    private readonly MasterDataManager _masterDataMgr;

    public ReceiveMail(ILogger<ReceiveMail> logger, IGameDb gameDb, MasterDataManager masterDataMgr)
    {
        _logger = logger;
        _gameDb = gameDb;
        _masterDataMgr = masterDataMgr;
    }

    [HttpPost]
    public async Task<ReceiveMailResponse> Post(ReceiveMailRequest request)
    {
        var response = new ReceiveMailResponse();
        var authUser = (CertifiedUser)HttpContext.Items[nameof(CertifiedUser)]!;
        var userId = authUser.UserId;


        // 메일 정보 읽기
        var mailInfo = await LoadMailInfo(userId, request.MailId);
        if (mailInfo is null)
        {
            response.Result = ErrorCode.NotExistUserMail;
            return response;
        }


        // 메일을 수신 상태로 변경
        if (await UpdateMailStatusAsReceive(mailInfo.mail_id) == false)
        {
            response.Result = ErrorCode.FailedUpdateUserMail;
            return response;
        }


        // 아이템 타입에 따른 지급 시작
        var (success, addedInfoList) = await AddUserItemInMail(userId, mailInfo);
        if (success == false)
        {
            await Rollback(userId, mailInfo, addedInfoList);

            response.Result = ErrorCode.FailedAddUserItem;
            return response;
        }




        response.ReceivedItemList = GetAddedIdList(addedInfoList);
        response.ReceivedItemCount = response.ReceivedItemList.Count;
        return response;
    }


    private async Task<UserMail> LoadMailInfo(Int64 userId, Int64 mailId)
    {
        var (error, loadedData) = await _gameDb.GetUserMail(mailId);
        if (error != ErrorCode.None)
        {
        }

        if (loadedData is null)
        {
        }

        return loadedData;
    }


    private async Task<bool> UpdateMailStatusAsReceive(Int64 mailId)
    {
        if (await _gameDb.UpdateMailStatusAsReceive(mailId) != ErrorCode.None)
        {
            return false;
        }

        return true;
    }


    private async Task<(bool, List<(Int64, Int32)>)> AddUserItemInMail(Int64 userId, UserMail mail)
    {
        var itemInfo = _masterDataMgr.GetItemInfo(mail.item_code);

        if (MasterDataCode.IsPossibleOverlap(itemInfo.item_type_code) == true)
        {
            // 곂칠 수 있는 아이템인 경우
            var addedId = await AddUserItemForOverlapType(userId, itemInfo, mail.item_count);
            if (addedId == 0)
            {
                return (false, null);
            }

            return (true, new List<(Int64, Int32)> { (addedId, itemInfo.item_code) });
        }

        // 곂칠 수 없는 아이템인 경우
        return await AddUserItemForNonOverlapType(userId, itemInfo, mail.item_count);
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


    private async Task<(bool, List<(Int64, Int32)>)> AddUserItemForNonOverlapType(Int64 userId, ItemInfo itemInfo, Int32 count)
    {
        var addedInfoList = new List<(Int64, Int32)>(count);

        for (var i = 0; i < count; i++)
        {
            var addedId = await _gameDb.AddUserItemAndGetId(userId, itemInfo, 1);
            if (addedId == 0)
            {
                return (false, addedInfoList);
            }
            addedInfoList.Add((addedId, itemInfo.item_code));
        }

        return (true, addedInfoList);
    }


    private async Task Rollback(Int64 userId, UserMail mailInfo, List<(Int64, Int32)> addedInfoList)
    {
        // 추가한 아이템 정보 롤백
        await RollbackAddedItemList(userId, mailInfo, addedInfoList);

        // 메일 미수신 상태로 변경
        await _gameDb.UpdateMailStatusAsNotReceive(mailInfo.mail_id);
    }


    private async Task RollbackAddedItemList(Int64 userId, UserMail mailInfo, List<(Int64, Int32)> addedInfoList)
    {
        foreach (var addedInfo in addedInfoList)
        {
            var itemCode = addedInfo.Item2;
            var itemInfo = _masterDataMgr.GetItemInfo(itemCode);
            var inventoryItemId = addedInfo.Item1;

            // 개수만 증가시킨 아이템은 개수만 차감한다.
            if (MasterDataCode.IsPossibleOverlap(itemInfo.item_type_code) == true)
            {
                await _gameDb.UpdateInventoryItemCountAndGetId(userId, itemCode, -mailInfo.item_count);
            }
            else
            {
                await _gameDb.RemoveUserInventoryItem(inventoryItemId);
            }
        }
    }


    private List<Int64> GetAddedIdList(List<(Int64, Int32)> addedList)
    {
        var resultList = new List<Int64>(addedList.Count);

        foreach (var item in addedList)
        {
            resultList.Add(item.Item1);
        }
        return resultList;
    }



}
