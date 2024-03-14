using ApiServer.Model;
using ApiServer.Services;
using Microsoft.AspNetCore.Mvc;
using ServerCommon;
using ZLogger;

namespace ApiServer.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class EvolveController : ControllerBase
    {
        private readonly IGameDb _gameDb;
        private readonly IDataStorage _dataStorage;
        private readonly ILogger<EvolveController> _logger;

        public EvolveController(ILogger<EvolveController> logger, IGameDb gameDb, IDataStorage dataStorage)
        {
            _logger = logger;
            _gameDb = gameDb;
            _dataStorage = dataStorage;
        }

        [HttpPost]
        public async Task<EvolveResponse> EvolvePost(EvolveRequest request)
        {
            var response = new EvolveResponse();

            // 비용이 충분한지 알아야함.
            var result = _dataStorage.GetMonsterEvolve(request.MonsterID);
            if (result is null)
            {
                // 잘못된 몬스터 ID
                response.Result = ErrorCode.EvolvePostFailNoMonsterId;
                _logger.ZLogError($"{nameof(EvolvePost)} ErrorCode : {response.Result}");
                return response;
            }
            
            // 비용 삭감 시도.
            var errorCode = await _gameDb.UpdateUpgradeCostAsync(request.ID, -result.CandyCount);
            if (errorCode != ErrorCode.None)
            {
                response.Result = errorCode;
                _logger.ZLogError($"{nameof(EvolvePost)} ErrorCode : {response.Result}");
                return response;
            }
            
            // 기존의 Catch의 MonsterID를 update 시켜야함.
            errorCode = await EvolveCatchMonsterAsync(request, result.EvolveMonsterID, result.CandyCount);
            if(errorCode != ErrorCode.None)
            {                
                response.Result = errorCode;
                _logger.ZLogError($"{nameof(EvolvePost)} ErrorCode : {response.Result}");
                return response;
            }

            response.EvolveMonsterID = result.EvolveMonsterID;
            _logger.ZLogError($"Evolve Success : {request.CatchID} {request.MonsterID} {response.EvolveMonsterID} {result.CandyCount}");
            return response;
        }

        private async Task<ErrorCode> EvolveCatchMonsterAsync(EvolveRequest request, Int64 evolveID, Int32 rollbackCandyCount)
        {
            // 기존의 Catch의 MonsterID를 update 시켜야함.
            var errorCode = await _gameDb.EvolveCatchMonsterAsync(request.CatchID, evolveID);
            if (errorCode != ErrorCode.None)
            {
                // Rollback
                var innerErrorCode = await _gameDb.UpdateUpgradeCostAsync(request.ID, rollbackCandyCount);
                if (innerErrorCode != ErrorCode.None)
                {
                    _logger.ZLogError($"{nameof(EvolvePost)} ErrorCode : {innerErrorCode}");
                }

                return errorCode;
            }
            return ErrorCode.None;
        }
    }
}