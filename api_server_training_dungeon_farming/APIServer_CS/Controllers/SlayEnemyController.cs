using System;
using System.Threading.Tasks;

using APIServer.ModelDB;
using APIServer.ModelReqRes;
using APIServer.Services;
using APIServer.Services.MasterData;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace APIServer.Controllers;

[ApiController]
[Route("[controller]")]
public class SlayEnemy : ControllerBase
{
    readonly ILogger<SlayEnemy> _logger;

    readonly IMemoryDb _memoryDb;

    readonly MasterDataManager _masterDataMgr;

    public SlayEnemy(ILogger<SlayEnemy> logger, IMemoryDb memoryDb, MasterDataManager masterDataMgr)
    {
        _logger = logger;
        _memoryDb = memoryDb;
        _masterDataMgr = masterDataMgr;
    }


    [HttpPost]
    public async Task<SlayEnemyResponse> Post(SlayEnemyRequest request)
    {
        var response = new SlayEnemyResponse();
        var userInfo = (CertifiedUser)HttpContext.Items[nameof(CertifiedUser)]!;

        var email = userInfo.Email;
        var enemyCode = request.EnemyCode;
        var stageCode = request.StageCode;


        // 전투 정보 읽기
        var battleInfo = await LoadUserBattleInfo(email);
        if (battleInfo is null)
        {
            response.Result = ErrorCode.NotExistsUserBattleInfo;
            return response;
        }


        // 유효성 체크
        if (battleInfo.Validate(stageCode, enemyCode) == false)
        {
            response.Result = ErrorCode.InvalidBattleInfo;
            return response;
        }


        // 전투 정보 갱신
        if (await UpdateSlayEnemyCount(email, enemyCode, battleInfo) == false)
        {
            response.Result = ErrorCode.FailedRedisUpdate;
            return response;
        }




        return response;
    }


    private async Task<UserBattleInfo> LoadUserBattleInfo(string email)
    {
        var (exist, loadedData) = await _memoryDb.GetUserBattleInfo(email);
        if (exist == false)
        {
        }

        return loadedData;
    }


    private async Task<bool> UpdateSlayEnemyCount(string email, Int32 enemyCode, UserBattleInfo info)
    {
        info.IncrementSlainEnemyCount(enemyCode);

        var error = await _memoryDb.UpdateUserBattleInfo(email, info);
        if (error != ErrorCode.None)
        {
            return false;
        }

        return true;
    }



}
