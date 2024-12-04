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
public class EnterStage : ControllerBase
{
    private readonly ILogger<EnterStage> _logger;

    private readonly IGameDb _gameDb;

    readonly IMemoryDb _memoryDb;

    private readonly MasterDataManager _masterDataMgr;


    public EnterStage(ILogger<EnterStage> logger, IGameDb gameDb, IMemoryDb memoryDb, MasterDataManager masterDataMgr)
    {
        _logger = logger;
        _gameDb = gameDb;
        _memoryDb = memoryDb;
        _masterDataMgr = masterDataMgr;
    }


    [HttpPost]
    public async Task<EnterStageResponse> Post(EnterStageRequest request)
    {
        var response = new EnterStageResponse();
        var userInfo = (CertifiedUser)HttpContext.Items[nameof(CertifiedUser)]!;
        var email = userInfo.Email;
        var userId = userInfo.UserId;
        var stageCode = request.StageCode;


        // 입장 가능한가?
        if (await IsPossibleEnter(userId, stageCode) == false)
        {
            response.Result = ErrorCode.ImpossibleEnterStage;
            return response;
        }


        // 입장
        if (await DoEnterStage(email, stageCode) == false)
        {
            response.Result = ErrorCode.FailedRedisRegist;
            return response;
        }


        // 해당 스테이지에서 등장 가능한 아이템, 적군 리스트 담기
        response.AppearanceItem = _masterDataMgr.GetStageFarmingItemList(stageCode);
        response.AppearanceEnemy = _masterDataMgr.GetStageEnemyList(stageCode);


        return response;
    }


    private async Task<bool> IsPossibleEnter(Int64 userId, Int32 stageCode)
    {
        // 처음 스테이지라면 이전 스테이지 완료 여부 확인 스킵
        if (IsFirstStage(stageCode) == true)
        {
            return true;
        }

        // 이전 스테이지 완료 여부 확인.
        return await IsCompletedPrevStage(userId, stageCode);
    }


    private bool IsFirstStage(Int32 stageCode) => stageCode == (Int32)MasterDataCode.StageCode.스테이지_1;


    private async Task<bool> IsCompletedPrevStage(Int64 userId, Int32 stageCode)
    {
        var (error, complete) = await _gameDb.IsCompleteStage(userId, stageCode - 1);
        if (error != ErrorCode.None)
        {
        }

        return complete;
    }


    private async Task<bool> DoEnterStage(string email, Int32 stageCode)
    {
        var battleInfo = CreateBattleInfo(stageCode);
        var error = await _memoryDb.RegistUserBattleInfo(email, battleInfo);
        if (error != ErrorCode.None)
        {
            return false;
        }
        return true;
    }


    private UserBattleInfo CreateBattleInfo(Int32 stageCode)
    {
        var info = new UserBattleInfo()
        {
            StageCode = stageCode,
            ItemFarmingCounterRef = _masterDataMgr.ItemFarmingCounter[stageCode],
            EnemySlayCounterRef = _masterDataMgr.EnemySlayCounter[stageCode],
            CompleteRewardExp = _masterDataMgr.StageCompleteRewardExp[stageCode],
        };

        return info;
    }



}
