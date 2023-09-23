using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;


[ApiController]
[Route("[controller]")]
public class LoginController : ControllerBase
{
    [HttpPost]
    public async Task<PkLoginResponse> Post(PkLoginRequest request)
    {
        Console.WriteLine($"[Request Login] ID:{request.ID}, PW:{request.PW}");
        var response = new PkLoginResponse();
        
        return response;
    }
}


public class PkLoginRequest
{
    public string ID { get; set; }
    public string PW { get; set; }
}

public class PkLoginResponse
{
    public int Result { get; set; }
}
