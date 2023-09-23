using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using ZLogger;

[ApiController]
[Route("[controller]")]
public class LoginController : ControllerBase
{
    readonly ILogger Logger;
    private IMemoryCache MemoryCache;
    
    public LoginController(ILogger<LoginController> logger, IMemoryCache memoryCache)
    {
        Logger = logger;
        MemoryCache = memoryCache;
    }
    
    [HttpPost]
    public async Task<PkLoginResponse> Post(PkLoginRequest request)
    {
        Logger.ZLogInformation($"[Request Login] ID:{request.ID}, PW:{request.PW}");
        
        var response = new PkLoginResponse();
        response.Result = ErrorCode.None;                
        response.Authtoken = "test";
        return response;
    }
}


public class PkLoginRequest
{
    public string ID { get; set; }
    public string PW { get;set; }
}

public class PkLoginResponse
{
    public ErrorCode Result { get; set; }
    public string Authtoken { get; set; }
}

class DBUserInfo
{
    public UInt64 UID { get; set; }
    public string PW { get; set; }
    public string NickName { get; set; }
    public string Salt { get; set; }
}
