//TODO 샘플용 코드. 사용하지 않으면 삭제해도 됨.
using APIServer.Model.DAO;
using APIServer.Model.DTO;
using APIServer.Repository;
using APIServer.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using ZLogger;
using static LogManager;

namespace APIServer.Controllers;

[ApiController]
[Route("[controller]")]
public class CreateCharacter : ControllerBase
{
    private readonly IGameDb _gameDb;
    private readonly IMemoryDb _memoryDb;
    private readonly ILogger<CreateCharacter> _logger;

    public CreateCharacter(ILogger<CreateCharacter> logger, IGameDb gameDb, IMemoryDb memoryDb)
    {
        _logger = logger;
        _gameDb = gameDb;
        _memoryDb = memoryDb;
    }

    [HttpPost]
    public async Task<CreateCharacterRes> Post(CreateCharacterReq request)
    {
        RdbAuthUserData userInfo = (RdbAuthUserData)HttpContext.Items[nameof(RdbAuthUserData)]!;

        CreateCharacterRes response = new();

        (ErrorCode errorCode, long characterId) = await CreateDB(userInfo.AccountId, request.NickName);
        if (errorCode != ErrorCode.None)
        {
            response.Result = errorCode;
            return response;
        }

        _logger.ZLogInformationWithPayload(EventIdDic[EventType.CreateCharacter], new { request.Email, request.NickName, CharacterId = characterId },
            $"CreateCharacter Success");
        return response;
    }

    public async Task<(ErrorCode, long)> CreateDB(long accountId, string nickName)
    {
        long characterId = 0;
        ErrorCode errorCode = ErrorCode.None;

        try
        {
            (errorCode, characterId) = await _gameDb.InsertCharacter(accountId, nickName);

            errorCode = await _gameDb.InsertCharacterItem(characterId);
        }
        catch (Exception e)
        {
            DeleteCharacter(nickName, characterId);
            _logger.ZLogError(e, $"[CreateCharacter] ErrorCode : {errorCode}, characterId : {characterId}");
        }

        return (ErrorCode.None, characterId);
    }

    public async void DeleteCharacter(string nickName, long characterId)
    {
        _ = await _gameDb.DeleteCharacter(nickName);

        if (characterId != 0)
        {
            _ = await _gameDb.DeleteCharacterItem(characterId);
        }
    }
}
