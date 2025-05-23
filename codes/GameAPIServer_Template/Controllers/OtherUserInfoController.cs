﻿using GameAPIServer.Servicies.Interfaces;
using GameAPIServer.Models.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using ZLogger;

namespace GameAPIServer.Controllers;

[ApiController]
[Route("[controller]")]
public class OtherUserInfoController : ControllerBase
{
    readonly ILogger<OtherUserInfoController> _logger;
    readonly IUserService _userService;

    public OtherUserInfoController(ILogger<OtherUserInfoController> logger, IUserService userService)
    {
        _logger = logger;
        _userService = userService;
    }

    /// <summary>
    /// 다른 유저 조회 API
    /// 다른 유저의 정보(uid, 닉네임, 점수 정보, 대표 캐릭터 정보, 등수)를 조회합니다.
    /// </summary>
    [HttpPost]
    public async Task<OtherUserInfoResponse> GetOtherUserInfo(OtherUserInfoRequest request)
    {
        OtherUserInfoResponse response = new();

        (response.Result, response.UserInfo) = await _userService.GetOtherUserInfo(request.Uid);

        _logger.ZLogInformation($"[OtherUserInfo] Uid : {request.Uid}");
        return response;
    }
}
