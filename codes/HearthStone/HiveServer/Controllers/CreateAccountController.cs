using Microsoft.AspNetCore.Mvc;
using HiveServer.Models.DTO;
using HiveServer.Services.Interface;

namespace HiveServer.Controllers;

[ApiController]
[Route("Auth/CreateAccount")]
public class CreateAccountController
{
    readonly ILogger<CreateAccountController> _logger;
    readonly IAuthService _authService;

    public CreateAccountController(ILogger<CreateAccountController> logger, IAuthService authService)
    {
        _logger = logger;
        _authService = authService;
    }

    [HttpPost]
    public async Task<CreateAccountResponse> CreateAccount(CreateAccountRequest request) 
    {
        CreateAccountResponse res = new();

        res.Result = await _authService.CreateAccount(request.EmailID, request.Password, request.NickName);

        return res;
    }

}
