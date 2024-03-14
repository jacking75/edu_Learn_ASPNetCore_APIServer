using ApiServer.Model;
using ApiServer.Services;
using Microsoft.AspNetCore.Mvc;
using ServerCommon;
using ZLogger;

namespace ApiServer.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UpgradeController : ControllerBase
    {
        private readonly IGameDb _gameDb;
        private readonly IRedisDb _redisDb;
        private readonly IDataStorage _dataStorage;
        private readonly IRankingManager _rankingManager;
        private readonly ILogger<UpgradeController> _logger;
        
        public UpgradeController(ILogger<UpgradeController> logger, IGameDb gameDb, IRedisDb redisDb, IDataStorage dataStorage, IRankingManager ranking)
        {
            _logger = logger;
            _gameDb = gameDb;
            _redisDb = redisDb;
            _dataStorage = dataStorage;
            _rankingManager = ranking;
        }

        // 수습 기간 프로젝트 임으로 실제로 몬스터 강화하지는 않고 유저 경험치를 주는 방식으로 진행.
        [HttpPost]
        public async Task<UpgradeResponse> UpgradePost(UpgradeRequest request)
        {
            var response = new UpgradeResponse();
            
            // 유저의 정보를 가져옵니다.
            var (errorCode, userGameInfo) = await _gameDb.GetUserGameInfoAsync(request.ID);
            if(errorCode != 0)
            {
                response.Result = errorCode;
                _logger.ZLogError($"{nameof(UpgradePost)} ErrorCode : {response.Result}");
                return response;
            }

            // 몬스터에 대한 기획 데이터를 가져온다.
            var monsterUpgrade = _dataStorage.GetMonsterUpgrade(request.MonsterID);
            if (monsterUpgrade == null)
            {
                // 잘못된 몬스터 ID
                response.Result = ErrorCode.UpgradePostFailNoMonsterId;
                _logger.ZLogError($"{nameof(UpgradePost)} ErrorCode : {response.Result}");
                return response;
            }

            // 유저가 'UpgradeCandy'에 대한 값을 지불 가능한지 확인한다.
            var totalUpgradeCost = monsterUpgrade.UpdateCost * request.UpgradeSize;
            if(userGameInfo.UpgradeCandy < totalUpgradeCost)
            {
                response.Result = ErrorCode.UpgradePostFailNoUpgradeCost;
                _logger.ZLogError($"{nameof(UpgradePost)} ErrorCode : {response.Result}");
                return response;
            }

            // 유저가 '별의모래'에 대한 값을 지불 가능한지 확인한다.
            var totalStarCount = monsterUpgrade.StarCost * request.UpgradeSize;
            if(userGameInfo.StarPoint < totalStarCount)
            {
                response.Result = ErrorCode.UpgradePostFailNoStarPoint;
                _logger.ZLogError($"{nameof(UpgradePost)} ErrorCode : {response.Result}");
                return response;
            }

            // 실제로 db에 UpgradeCost를 지불합니다.
            errorCode = await _gameDb.UpdateUpgradeCostAsync(request.ID, -totalUpgradeCost);
            if (errorCode != ErrorCode.None)
            {
                response.Result = errorCode;
                _logger.ZLogError($"{nameof(UpgradePost)} ErrorCode : {response.Result}");
                return response;
            }

            // 실제로 db에 StarCount를 지불합니다.
            errorCode = await UpdateStarCountAsync(request, -totalStarCount, totalUpgradeCost);
            if (errorCode != ErrorCode.None)
            {
                // 업데이트 실패
                response.Result = errorCode;
                _logger.ZLogError($"{nameof(UpgradePost)} ErrorCode : {response.Result}");
                return response;
            }

            // 몬스터 cp 증가하기!
            errorCode = await UpdateCatchCombatPointAsync(request, monsterUpgrade.Exp, totalUpgradeCost, totalStarCount);
            if(errorCode != ErrorCode.None)
            {
                // 업데이트 실패
                response.Result = errorCode;
                _logger.ZLogError($"{nameof(UpgradePost)} ErrorCode : {response.Result}");
                return response;
            }            
            
            return response;
        }

        private async Task<ErrorCode> UpdateStarCountAsync(UpgradeRequest request, Int32 minusStarCount, Int32 totalUpgradeCost)
        {
            // 실제로 db에 StarCount를 지불합니다.
            var errorCode = await _rankingManager.UpdateStarCount(request.ID, minusStarCount, _gameDb, _redisDb);
            if (errorCode != ErrorCode.None)
            {
                // Rollback
                var innerErrorCode = await _gameDb.UpdateUpgradeCostAsync(request.ID, totalUpgradeCost);
                if (innerErrorCode != ErrorCode.None)
                {
                    _logger.ZLogError($"{nameof(UpgradePost)} ErrorCode : {innerErrorCode}");
                }

                return errorCode;
            }
            return ErrorCode.None;
        }

        private async Task<ErrorCode> UpdateCatchCombatPointAsync(UpgradeRequest request, Int32 combatPoint, Int32 totalUpgradeCost, Int32 totalStarCount)
        {
            // 몬스터 cp 증가하기!
            var errorCode = await _gameDb.UpdateCatchCombatPointAsync(request.CatchID, combatPoint);
            if (errorCode != ErrorCode.None)
            {
                // Rollback
                var innerErrorCode = await _gameDb.UpdateUpgradeCostAsync(request.ID, totalUpgradeCost);
                if (innerErrorCode != ErrorCode.None)
                {
                    _logger.ZLogError($"{nameof(UpgradePost)} ErrorCode : {innerErrorCode}");
                }

                innerErrorCode = await _rankingManager.UpdateStarCount(request.ID, totalStarCount, _gameDb, _redisDb);
                if (innerErrorCode != ErrorCode.None)
                {
                    _logger.ZLogError($"{nameof(UpgradePost)} ErrorCode : {innerErrorCode}");
                }

                return errorCode;
            }
            return ErrorCode.None;
        }
    }
}