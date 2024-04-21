using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using ZLogger;
using basic2_06.Repository;


namespace basic2_04.Controllers;

[Route("api/[controller]")]
[ApiController]
public class LoginController : ControllerBase
{
    private readonly IGameDB _gameDB;
    private readonly IMemoryDB _memoryDB;
    private readonly ILogger<LoginController> _logger;

    public LoginController(ILogger<LoginController> logger, IGameDB gameDB, IMemoryDB memoryDB)
    {
        _gameDB = gameDB;
        _memoryDB = memoryDB;
        _logger = logger;
    }

    [HttpPost]
    public async Task<PkLoginResponse> Post(PkLoginRequest request)
    {
        _logger.ZLogInformation($"[Request Login] ID:{request.ID}, PW:{request.PW}");

        var response = new PkLoginResponse();

        // ID, PW 검증
        (ErrorCode errorCode, long uid) = await _gameDB.AuthCheck(request.ID, request.PW);
        if (errorCode != ErrorCode.None)
        {
            response.Result = errorCode;
            return response;
        }


        string authToken = CreateAuthToken();
        errorCode = await _memoryDB.RegistUserAsync(request.ID, authToken, uid);
        if (errorCode != ErrorCode.None)
        {
            response.Result = errorCode;
            return response;
        }

        response.AuthToken = authToken;
        return response;
    }



    private const String AllowableCharacters = "abcdefghijklmnopqrstuvwxyz0123456789";

    public String CreateAuthToken()
    {
        var bytes = new Byte[25];
        using (var random = RandomNumberGenerator.Create())
        {
            random.GetBytes(bytes);
        }

        return new String(bytes.Select(x => AllowableCharacters[x % AllowableCharacters.Length]).ToArray());
    }
}


public class PkLoginRequest
{
    public string ID { get; set; }
    public string PW { get; set; }
}

public class PkLoginResponse
{
    public ErrorCode Result { get; set; }
    public string AuthToken { get; set; }
}
