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
public class ReceiveInAppProduct : ControllerBase
{
    private readonly ILogger<ReceiveInAppProduct> _logger;

    private readonly IGameDb _gameDb;

    private readonly MasterDataManager _masterDataMgr;

    public ReceiveInAppProduct(ILogger<ReceiveInAppProduct> logger, IGameDb gameDb, MasterDataManager masterDataMgr)
    {
        _logger = logger;
        _gameDb = gameDb;
        _masterDataMgr = masterDataMgr;
    }

    [HttpPost]
    public async Task<ReceiveInAppProductResponse> Post(ReceiveInAppProductRequest request)
    {
        var response = new ReceiveInAppProductResponse();
        var authUserInfo = (CertifiedUser)HttpContext.Items[nameof(CertifiedUser)]!;
        var userId = authUserInfo.UserId;

        var pid = request.PID;
        var receipt = request.Receipt;


        // 이미 지급받은 상품인지 확인
        var error = await VerifyReceipt(receipt);
        if (error != ErrorCode.None)
        {
            response.Result = error;
            return response;
        }


        // 영수증 정보 저장
        if (await RecordReceipt(userId, pid, receipt) == false)
        {
            response.Result = ErrorCode.FailedAddInAppProductReceipt;
            return response;
        }


        // 인앱 상품 아이템 지급
        var (success, sendedIdList) = await SendInAppProductItemList(userId, pid);
        if (success == false)
        {
            await Rollback(userId, receipt, sendedIdList);

            response.Result = ErrorCode.FailedAddUserMail;
            return response;
        }



        return response;
    }


    private async Task<ErrorCode> VerifyReceipt(string receipt)
    {
        // DB 에러와 영수증 중복을 구분한다.

        var (error, isPaid) = await _gameDb.IsPaidInAppReceipt(receipt);

        if (error != ErrorCode.None)
        {
            return error;
        }

        if (isPaid == true)
        {
            return ErrorCode.AlreadyReceivedRecept;
        }

        return ErrorCode.None;
    }


    private async Task<bool> RecordReceipt(Int64 userId, Int32 pid, string receipt)
    {
        if (await _gameDb.AddPaidInAppHistory(userId, pid, receipt) == false)
        {
            return false;
        }

        return true;
    }


    private async Task<(bool, List<Int64>)> SendInAppProductItemList(Int64 userId, Int32 pid)
    {
        var productItemInfoList = _masterDataMgr.GetInAppProductItemList(pid);
        var sendedIdList = new List<Int64>(productItemInfoList.Count);

        foreach (var productItemInfo in productItemInfoList)
        {
            var itemInfo = productItemInfo.Item1;
            var itemCount = productItemInfo.Item2;

            var sendedId = await SendMail(userId, pid, itemInfo.item_code, itemCount);
            if (sendedId == 0)
            {
                return (false, sendedIdList);
            }

            sendedIdList.Add(sendedId);
        }

        return (true, sendedIdList);
    }


    private async Task<Int64> SendMail(Int64 userId, Int32 pid, Int32 itemCode, Int32 count)
    {
        return await _gameDb.AddUserMailAndGetId(userId, CreateUserMail(pid, itemCode, count));
    }


    public UserMail CreateUserMail(Int32 pid, Int32 itemCode, Int32 count)
    {
        return new UserMail()
        {
            mail_type = (Int16)MasterDataCode.MailCode.유료상품,
            mail_title = $"{pid} 인앱 상품 아이템입니다.",
            item_code = itemCode,
            item_count = count,
            expire_date = DateTime.MaxValue.Date
        };
    }


    private async Task Rollback(Int64 userId, string receipt, List<Int64> sendedIdList)
    {
        await _gameDb.RemoveUserMailList(userId, sendedIdList);
        await _gameDb.RemovePaidInAppHistory(receipt);
    }



}