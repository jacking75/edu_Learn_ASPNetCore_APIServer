using ApiServer.Model;
using ApiServer.Services;
using Microsoft.AspNetCore.Mvc;
using ServerCommon;
using ZLogger;

namespace ApiServer.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DailyCheckController : ControllerBase
    {
        private readonly IGameDb _gameDb;
        private readonly IRedisDb _redisDb;
        private readonly IDataStorage _dataStorage;
        private readonly IRankingManager _rankingManager;
        private readonly ILogger<DailyCheckController> _logger;
        
        public DailyCheckController(ILogger<DailyCheckController> logger, IGameDb gameDb, IRedisDb redisDb, IDataStorage dataStorage, IRankingManager ranking)
        {
            _logger = logger;
            _gameDb = gameDb;
            _redisDb = redisDb;
            _dataStorage = dataStorage;
            _rankingManager = ranking;
        }
        
        // 보상은 간단히 하기 위해서 요일별로 excel에 지정되어있는 기획데이터의 StarPoint만 주고 있음.
        [HttpPost]
        public async Task<DailyCheckResponse> DailyCheckPost(DailyCheckRequest request)
        {
            var response = new DailyCheckResponse();
            
            // DB를 조회헤서 유저의 출석 체크 성공 여부를 알려줍니다.
            var (errorCode, prevDate) = await _gameDb.TryDailyCheckAsync(request.ID);
            if (errorCode != ErrorCode.None)
            {
                response.Result = errorCode;
                _logger.ZLogError($"{nameof(DailyCheckPost)} ErrorCode : {response.Result}");
                return response;
            }
            
            // 보상 주기 - 월요일부터 일요일까지 요일 별로 보상이 있음.
            // 일요일 0, 월요일 1, 화요일 2, ...
            var dailyInfo = _dataStorage.GetDailyInfo((Int32) DateTime.Today.DayOfWeek + 1);
            if (dailyInfo is null)
            {
                response.Result = ErrorCode.DailyCheckFailNoStoredData;
                _logger.ZLogError($"{nameof(DailyCheckPost)} ErrorCode : {response.Result}");
                return response;
            }
            
            errorCode = await UpdateStarCountAsync(request, dailyInfo.StarCount, prevDate);
            if (errorCode != ErrorCode.None)
            {
                response.Result = errorCode;
                _logger.ZLogError($"{nameof(DailyCheckPost)} ErrorCode : {response.Result}");
                return response;
            }
            
            response.StarCount = dailyInfo.StarCount;
            
            return response;
        }

        private async Task<ErrorCode> UpdateStarCountAsync(DailyCheckRequest request, Int32 starCount, DateTime rollbackPrevDate)
        {
            var errorCode = await _rankingManager.UpdateStarCount(request.ID, starCount, _gameDb, _redisDb);
            if (errorCode != ErrorCode.None)
            {
                // Rollback 시도
                var innerErrorCode = await _gameDb.RollbackDailyCheckAsync(request.ID, rollbackPrevDate);
                if (innerErrorCode != ErrorCode.None)
                {
                    _logger.ZLogError($"{nameof(DailyCheckPost)} ErrorCode : {innerErrorCode}");
                }

                return errorCode;
            }
            return ErrorCode.None;
        }
    }
}