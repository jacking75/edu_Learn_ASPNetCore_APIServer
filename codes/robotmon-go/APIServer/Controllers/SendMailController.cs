using ApiServer.Model;
using ApiServer.Services;
using Microsoft.AspNetCore.Mvc;
using ServerCommon;
using ZLogger;

namespace ApiServer.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SendMailController : ControllerBase
    {
        private readonly IGameDb _gameDb;
        private readonly IRedisDb _redisDb;
        private readonly IRankingManager _rankingManager;
        private readonly ILogger<SendMailController> _logger;

        public SendMailController(ILogger<SendMailController> logger, IGameDb gameDb, IRedisDb redisDb, IRankingManager ranking)
        {
            _logger = logger;
            _gameDb = gameDb;
            _redisDb = redisDb;
            _rankingManager = ranking;
        }

        [HttpPost]
        public async Task<SendMailResponse> SendMailPost(SendMailRequest request)
        {
            var response = new SendMailResponse();

            var result = await _gameDb.SendMailAsync(request.sendID, request.StarCount);
            var errorCode = result.Item1;
            var lastInsertId = result.Item2;
            if (errorCode != ErrorCode.None)
            {
                response.Result = errorCode;
                _logger.ZLogError($"{nameof(SendMailPost)} ErrorCode : {response.Result}");
                return response;
            }

            // 원래 유저의 정보에서 StarCount를 차감합니다.
            errorCode = await UpdateStarCountAsync(request, lastInsertId);
            if (errorCode != ErrorCode.None)
            {                
                response.Result = errorCode;
                _logger.ZLogError($"{nameof(SendMailPost)} ErrorCode : {response.Result}");
                return response;
            }
            return response;
        }

        private async Task<ErrorCode> UpdateStarCountAsync(SendMailRequest request, Int64 lastInsertId)
        {
            // 원래 유저의 정보에서 StarCount를 차감합니다.
            var errorCode = await _rankingManager.UpdateStarCount(request.ID, -request.StarCount, _gameDb, _redisDb);
            if (errorCode != ErrorCode.None)
            {
                // Rollback
                var innerErrorCode = await _gameDb.RollbackSendMailAsync(lastInsertId);
                if (innerErrorCode != ErrorCode.None)
                {
                    _logger.ZLogError($"{nameof(SendMailPost)} ErrorCode : {innerErrorCode}");
                }

                return errorCode;
            }
            return ErrorCode.None;
        }
    }
}