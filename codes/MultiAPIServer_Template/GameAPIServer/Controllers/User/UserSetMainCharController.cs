using APIServer.DTO;
using APIServer.DTO.User;
using APIServer.Servicies.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using ZLogger;

namespace APIServer.Controllers.User;

[ApiController]
[Route("[controller]")]
public class UserSetMainChar : ControllerBase
{
    readonly ILogger<UserSetMainChar> _logger;
    readonly IUserService _userService;

    public UserSetMainChar(ILogger<UserSetMainChar> logger, IUserService userService)
    {
        _logger = logger;
        _userService = userService;
    }

    /// <summary>
    /// 메인 캐릭터 설정 API
    /// 유저의 메인 캐릭터를 설정합니다.
    /// </summary>
    [HttpPost]
    public async Task<UserSetMainCharResponse> SetUserMainChar([FromHeader] HeaderDTO header, UserSetMainCharRequest request)
    {
        UserSetMainCharResponse response = new();

        response.Result = await _userService.SetUserMainChar(header.Uid, request.CharKey);

        _logger.ZLogInformation($"[UserSetMainChar] Uid : {header.Uid}, CharKey : {request.CharKey}");
        return response;
    }
}
