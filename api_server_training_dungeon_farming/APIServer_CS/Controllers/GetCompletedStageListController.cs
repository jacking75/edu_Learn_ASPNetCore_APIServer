using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using APIServer.ModelDB;
using APIServer.ModelReqRes;
using APIServer.Services;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace APIServer.Controllers;

[ApiController]
[Route("[controller]")]
public class GetCompleteStageList : ControllerBase
{
    readonly ILogger<GetCompleteStageList> _logger;

    readonly IGameDb _gameDb;

    public GetCompleteStageList(ILogger<GetCompleteStageList> logger, IGameDb gameDb)
    {
        _logger = logger;
        _gameDb = gameDb;
    }

    [HttpPost]
    public async Task<GetCompleteStageListResponse> Post(GetCompleteStageListRequest request)
    {
        var response = new GetCompleteStageListResponse();
        var userInfo = (CertifiedUser)HttpContext.Items[nameof(CertifiedUser)]!;

        response.CompletedStage = await LoadClearInfo(userInfo.UserId);

        return response;
    }


    private async Task<List<Int32>> LoadClearInfo(Int64 userId)
    {
        var (error, loadedData) = await _gameDb.GetCompletedStageList(userId);
        if (error != ErrorCode.None)
        {
        }

        return loadedData;
    }



}
