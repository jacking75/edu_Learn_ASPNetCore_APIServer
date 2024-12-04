using System;
using System.Threading.Tasks;

using APIServer.ModelReqRes;
using APIServer.Services;
using APIServer.Services.MasterData;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using static LogManager;

namespace APIServer.Controllers;

[ApiController]
[Route("[controller]")]
public class CreateAccount : ControllerBase
{
    private readonly ILogger<CreateAccount> _logger;

    private readonly IAccountDb _accountDb;

    private readonly IGameDb _gameDb;

    private readonly MasterDataManager _masterDataMgr;


    public CreateAccount(ILogger<CreateAccount> logger, IAccountDb accountDb, IGameDb gameDb, MasterDataManager masterDataMgr)
    {
        _logger = logger;
        _accountDb = accountDb;
        _gameDb = gameDb;
        _masterDataMgr = masterDataMgr;
    }


    [HttpPost]
    public async Task<CreateAccountResponse> Post(CreateAccountRequest request)
    {
        var response = new CreateAccountResponse();

        LoggingForInformation(_logger, EventType.CreateAccount, "Request CreateAccount", new { Email = request.Email, Password = request.Password });

        // 게임 플레이 데이터 생성
        var createdUserId = await CreateUserPlayData(1, 0);
        if (createdUserId == 0)
        {
            response.Result = ErrorCode.FailedCreateUserPlayData;
            return response;
        }


        // 기본 아이템 지급
        if (await GiveDefaultUserItem(createdUserId) == false)
        {
            await Rollback(createdUserId);

            response.Result = ErrorCode.FailedAddUserItem;
            return response;
        }


        // 출석부 생성
        if (await CreateUserAttendanceBook(createdUserId) == false)
        {
            await Rollback(createdUserId);
            response.Result = ErrorCode.FailedCreateUserAttendanceBook;
            return response;
        }


        // 계정 생성
        if (await CreateUserAccount(createdUserId, request.Email, request.Password) == false)
        {
            await Rollback(createdUserId);
            response.Result = ErrorCode.FailedCreateAccount;
            return response;
        }

        LoggingForInformation(_logger, EventType.CreateAccount, "Successed CreateAccount");

        return response;
    }


    private async Task<Int64> CreateUserPlayData(Int16 level, Int32 exp)
    {
        var createdUserId = await _gameDb.CreateUserPlayDataAndGetId(level, exp);
        if (createdUserId == 0)
        {
            LoggingForError(_logger, EventType.CreateAccount, message: "Failed GameDb.CreateUserPlayData()", new { Level = level, Exp = exp });
        }

        return createdUserId;
    }


    private async Task<bool> CreateUserAttendanceBook(Int64 userId)
    {
        if (await _gameDb.CreateUserAttendanceBook(userId) == false)
        {
            LoggingForError(_logger, EventType.CreateAccount, message: "Failed GameDb.CreateUserAttendanceBook()", new { UserId = userId });
            return false;
        }

        return true;
    }


    private async Task<bool> CreateUserAccount(Int64 userId, string email, string password)
    {
        if (await _accountDb.CreateAccount(email, password, userId) == false)
        {
            LoggingForError(_logger, EventType.CreateAccount, message: "Failed AccountDb.CreateAccount()", new { UserId = userId, Email = email, Password = password });
            return false;
        }
        return true;
    }


    private async Task<bool> GiveDefaultUserItem(Int64 userId)
    {
        foreach (var tuple in _masterDataMgr.DefaultGiveItemList)
        {
            if (await _gameDb.AddUserItem(userId, tuple.Item1, tuple.Item2) == false)
            {
                LoggingForError(_logger, EventType.CreateAccount, message: "Failed GameDb.AddUserItem()", new { UserId = userId, ItemCode = tuple.Item1.item_code, ItemCount = tuple.Item2 });
                return false;
            }
        }

        return true;
    }


    private async Task Rollback(Int64 userId)
    {
        LoggingForInformation(_logger, EventType.CreateAccount, message: "Rollback Start", new { UserId = userId });

        if (await _gameDb.RemoveAllUserData(userId) != ErrorCode.None)
        {
            LoggingForError(_logger, EventType.CreateAccount, message: "Failed GameDb.RemoveAllUserData()", new { UserId = userId });
        }
        LoggingForInformation(_logger, EventType.CreateAccount, message: "Successed GameDb.RemoveAllUserData()", new { UserId = userId });

        LoggingForInformation(_logger, EventType.CreateAccount, message: "Rollback Done");
    }



}