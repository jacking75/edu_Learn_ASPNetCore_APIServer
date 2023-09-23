using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("[controller]")]
public class CreateAccount : Controller
{
    [HttpPost]
    public async Task<PkCreateAccountResponse> Post(PkCreateAccountRequest request)
    {
        var response = new PkCreateAccountResponse {Result = ErrorCode.None};        
        return response;
    }

    
}


public class PkCreateAccountRequest
{
    public string ID { get; set; }
    public string PW { get; set; }
    public string NickName { get; set; }
}

public class PkCreateAccountResponse
{
    public ErrorCode Result { get; set; }
}
