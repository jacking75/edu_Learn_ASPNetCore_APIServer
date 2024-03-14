using ApiServer.Model;
using ApiServer.Services;
using Microsoft.AspNetCore.Mvc;
using ServerCommon;
using ZLogger;

namespace ApiServer.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CatchController : ControllerBase
    {
        private readonly IGameDb _gameDb;
        private readonly IRedisDb _redisDb;
        private readonly IDataStorage _dataStorage;
        private readonly ILogger<CatchController> _logger;
        private readonly IRankingManager _rankingManager;

        public CatchController(ILogger<CatchController> logger, IGameDb gameDb, IRedisDb redisDb, IDataStorage dataStorage, IRankingManager ranking)
        {
            _logger = logger;
            _gameDb = gameDb;
            _redisDb = redisDb;
            _dataStorage = dataStorage;
            _rankingManager = ranking;
        }

        // 수습 기간 프로젝트임으로 기능이 간단하게 구현되었습니다.
        [HttpPost]
        public async Task<CatchResponse> CatchPost(CatchRequest request)
        {
            var response = new CatchResponse();

            // 랜덤으로 몬스터 잡을 확률 - 테스트 중에는 100%.
            var (errorCode, monster) = GetRandomMonsterAsync(request);
            if (errorCode != ErrorCode.None)
            {
                response.Result = errorCode;
                _logger.ZLogError($"{nameof(CatchPost)} ErrorCode : {response.Result}");
                return response;
            }

            // 현재 날짜 - 시간 정보를 원하는 경우 DateTime.Now를 사용할 것. 
            // 잡은 정보 저장
            var rand = new Random();
            var randomCombatPoint = rand.Next(100, 1501);
            response.Date = DateTime.Today;
            (errorCode, var catchId) = await _gameDb.SetCatchAsync(request.ID, request.MonsterID, response.Date, randomCombatPoint);
            if (errorCode != ErrorCode.None)
            {
                response.Result = errorCode;
                if (errorCode != ErrorCode.CatchFail)
                {
                    _logger.ZLogError($"{nameof(CatchPost)} ErrorCode : {response.Result}");
                }

                return response;
            }

            // 기획데이터에 있지만, 테스르틀 위해서 랜덤하게 강화 캔디를 준다.
            var randomUpgradeCandy = rand.Next(100, 1001);
            errorCode = await RaiseUpgradeCandyAsync(request, randomUpgradeCandy, catchId);
            if (errorCode != ErrorCode.None)
            {
                response.Result = errorCode;
                _logger.ZLogError($"{nameof(CatchPost)} ErrorCode : {response.Result}");
                return response;
            }

            // 기획 데이터를 읽어왔지만 사용하지 않고, 랜덤하게 StarCount 주는 방식으로 수정
            var randomStarCount = rand.Next(100, 1001);
            errorCode = await RaiseStarCountAsync(request, randomStarCount, catchId, randomUpgradeCandy);
            if (errorCode != ErrorCode.None)
            {
                response.Result = errorCode;
                _logger.ZLogError($"{nameof(CatchPost)} ErrorCode : {response.Result}");
                return response;
            }

            response.CatchID = catchId;
            response.StarCount = randomStarCount;
            response.UpgradeCandy = randomUpgradeCandy;
            response.MonsterID = request.MonsterID;
            response.CombatPoint = randomCombatPoint;

            _logger.ZLogError($"Catch Success : {request.ID} {response.CatchID} {response.MonsterID} {response.CombatPoint}");
            return response;
        }

        private Tuple<ErrorCode, Monster> GetRandomMonsterAsync(CatchRequest request)
        {
            var rand = new Random();
            var randValue = rand.Next(1, 101); // 랜덤 확률. 1~100

            // 테스트 중이기 때문에 확률 100%
            if (randValue < 0)
            {
                return new Tuple<ErrorCode, Monster>(ErrorCode.CatchFail, null);
            }

            var monster = _dataStorage.GetMonsterInfo(request.MonsterID);
            if (monster == null)
            {
                return new Tuple<ErrorCode, Monster>(ErrorCode.DataStorageReadMonsterFail, null); ;
            }

            return new Tuple<ErrorCode, Monster>(ErrorCode.None, monster);
        }

        private async Task<ErrorCode> RaiseUpgradeCandyAsync(CatchRequest request, Int32 upgradeCandy, Int64 rollbackCatchId)
        {
            var errorCode = await _gameDb.UpdateUpgradeCostAsync(request.ID, upgradeCandy);
            if (errorCode != ErrorCode.None)
            {
                var insideErrorCode = await _gameDb.RollbackSetCatchAsync(rollbackCatchId);
                if (insideErrorCode != ErrorCode.None)
                {
                    _logger.ZLogError($"{nameof(CatchPost)} ErrorCode : {insideErrorCode}");
                }

                return errorCode;
            }
            return ErrorCode.None;
        }         
        
        private async Task<ErrorCode> RaiseStarCountAsync(CatchRequest request, Int32 starCount, Int64 rollbackCatchId, Int32 rollbackUpgradeCandy)
        {
            var errorCode = await _rankingManager.UpdateStarCount(request.ID, starCount, _gameDb, _redisDb);
            if (errorCode != ErrorCode.None)
            {
                var insideErrorCode = await _gameDb.RollbackSetCatchAsync(rollbackCatchId);
                if (insideErrorCode != ErrorCode.None)
                {
                    _logger.ZLogError($"{nameof(CatchPost)} ErrorCode : {insideErrorCode}");
                }

                insideErrorCode = await _gameDb.UpdateUpgradeCostAsync(request.ID, -rollbackUpgradeCandy);
                if (insideErrorCode != ErrorCode.None)
                {
                    _logger.ZLogError($"{nameof(CatchPost)} ErrorCode : {insideErrorCode}");
                }

                return errorCode;
            }
            return ErrorCode.None;
        }
    }
}