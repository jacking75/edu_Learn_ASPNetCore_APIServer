using ApiServer.Model;
using ApiServer.Services;
using Microsoft.AspNetCore.Mvc;
using ServerCommon;
using ZLogger;

namespace ApiServer.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class RecvMailController : ControllerBase
    {
        private readonly IGameDb _gameDb;
        private readonly IRedisDb _redisDb;
        private readonly IRankingManager _rankingManager;
        private readonly ILogger<RecvMailController> _logger;

        public RecvMailController(ILogger<RecvMailController> logger, IGameDb gameDb, IRedisDb redisDb, IRankingManager ranking)
        {
            _logger = logger;
            _gameDb = gameDb;
            _redisDb = redisDb;
            _rankingManager = ranking;
        }

        [HttpPost]
        public async Task<RecvMailResponse> RecvMailPost(RecvMailRequest request)
        {
            var response = new RecvMailResponse();

            var (errorCode, starCount, date) = await _gameDb.RecvMailAsync(request.ID, request.MailID);
            
            if (errorCode != ErrorCode.None)
            {
                response.Result = errorCode;
                _logger.ZLogError($"{nameof(RecvMailPost)} ErrorCode : {response.Result}");
                return response;
            }

            errorCode = await UpdateStarCountAsync(request, starCount, date);
            if(errorCode != ErrorCode.None)
            {                
                response.Result = errorCode;
                _logger.ZLogError($"{nameof(RecvMailPost)} ErrorCode : {response.Result}");
                return response;
            }

            response.StarCount = starCount;

            return response;
        }

        private async Task<ErrorCode> UpdateStarCountAsync(RecvMailRequest request, Int32 starCount, DateTime rollbackDate)
        {
            var errorCode = await _rankingManager.UpdateStarCount(request.ID, starCount, _gameDb, _redisDb);
            if (errorCode != ErrorCode.None)
            {
                // Rollback
                var innerErrorCode = await _gameDb.RollbackRecvMailAsync(request.ID, starCount, rollbackDate);
                if (innerErrorCode != ErrorCode.None)
                {
                    _logger.ZLogError($"{nameof(RecvMailPost)} ErrorCode : {innerErrorCode}");
                }

                return errorCode;
            }
            return ErrorCode.None;
        }
    }
}