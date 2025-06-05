using HiveServer.Services.Interface;
using HiveServer.Repository.Interface;
using ZLogger;
using StackExchange.Redis;

namespace HiveServer.Services;

public class AuthService:IAuthService
{
    readonly ILogger<AuthService> _logger;
    readonly IHiveDb _hiveDb;
    readonly IMemoryDb _memoryDb;

    public AuthService(ILogger<AuthService> logger, IHiveDb hiveDb, IMemoryDb memoryDb)
    {
        _logger = logger;
        _hiveDb = hiveDb;
        _memoryDb = memoryDb;
    }

    public async Task<ErrorCode> CreateAccount(string emailId, string password, string nickname)
    {
        if(await _hiveDb.DuplicateNickname(nickname) > 0)
        {
            return ErrorCode.DuplicateNickname;
        }

        return await _hiveDb.CreateAccount(emailId, password,nickname);
    }

    public async Task<(ErrorCode, Int64, string)> Login(string emailId, string password) 
    {
        (var result, var accountUid) = await _hiveDb.VerifyUser(emailId, password);
        if(result != ErrorCode.None)
        {
            return (result, 0, "");
        }
        
        var token = Security.GenerateAuthToken();
        result = await _memoryDb.RegistUserAsync(accountUid, token);
        if (result != ErrorCode.None)
        {
            return (result, accountUid, "");
        }

        return (result, accountUid, token);
    }
    public async Task<ErrorCode> VerifyToken(Int64 accountUid, string hiveToken) 
    {
        return await _memoryDb.CheckUserAuthAsync(accountUid, hiveToken);
    }

}

