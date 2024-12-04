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
public class FarmingItem : ControllerBase
{
    readonly ILogger<FarmingItem> _logger;

    readonly IMemoryDb _memoryDb;

    readonly MasterDataManager _masterDataMgr;


    public FarmingItem(ILogger<FarmingItem> logger, IMemoryDb memoryDb, MasterDataManager masterDataMgr)
    {
        _logger = logger;
        _memoryDb = memoryDb;
        _masterDataMgr = masterDataMgr;
    }


    [HttpPost]
    public async Task<FarmingItemInDungeonResponse> Post(FarmingItemInDungeonRequest request)
    {
        var response = new FarmingItemInDungeonResponse();
        var userInfo = (CertifiedUser)HttpContext.Items[nameof(CertifiedUser)]!;

        var email = userInfo.Email;
        var itemCode = request.ItemCode;
        var stageCode = request.StageCode;


        // 전투 정보 읽기
        var battleInfo = await LoadUserBattleInfo(email);
        if (battleInfo is null)
        {
            response.Result = ErrorCode.NotExistsUserBattleInfo;
            return response;
        }


        // 요청 데이터와 전투 정보 매칭
        if (battleInfo.Validate(stageCode) == false)
        {
            response.Result = ErrorCode.InvalidBattleInfo;
            return response;
        }


        // 해당 스테이지에서 등장할 수 있는 아이템인가?
        if (IsAppearanceItem(stageCode, itemCode) == false)
        {
            response.Result = ErrorCode.ImpossibleAppearanceItem;
            return response;
        }


        // 파밍 아이템 추가
        if (await AddFarmingItem(email, stageCode, itemCode, battleInfo) == false)
        {
            response.Result = ErrorCode.FailedAddFarmingItem;
            return response;
        }



        return response;
    }


    private async Task<UserBattleInfo> LoadUserBattleInfo(string email)
    {
        var (exist, loadedData) = await _memoryDb.GetUserBattleInfo(email);
        if (exist == false)
        {
            return null;
        }

        return loadedData;

    }


    private bool IsAppearanceItem(Int32 stageCode, Int32 itemCode)
        => _masterDataMgr.IsExistStageFarmingItem(stageCode, itemCode);


    private async Task<bool> AddFarmingItem(string email, Int32 stageCode, Int32 itemCode, UserBattleInfo info)
    {
        if (info.AddFarmingItem(itemCode) == false)
        {
            return false;
        }

        var error = await _memoryDb.UpdateUserBattleInfo(email, info);
        if (error != ErrorCode.None)
        {
            return false;
        }

        return true;
    }



}
