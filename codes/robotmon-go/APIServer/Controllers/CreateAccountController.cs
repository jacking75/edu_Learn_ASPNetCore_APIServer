using System;
using System.Threading.Tasks;
using ApiServer.Model;
using ApiServer.Services;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ServerCommon;
using ApiServer.Model.Data;
using ZLogger;

namespace ApiServer.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CreateAccountController : ControllerBase
    {
        // ControllerBase 객체는 ASPNET MVC에서 제공하는 객체로 모델 바인딩 하기 위해서 사용됨.
        private readonly IAccountDb _accountDb;
        private readonly IGameDb _gameDb;
        private readonly IRedisDb _redisDb;
        private readonly IRankingManager _rankingManager;
        private readonly ILogger<CreateAccountController> _logger;

        public CreateAccountController(ILogger<CreateAccountController> logger, IAccountDb accountDb, IGameDb gameDb, IRedisDb redisDb, IRankingManager ranking)
        {
            _logger = logger;
            _accountDb = accountDb;
            _gameDb = gameDb;
            _redisDb = redisDb;
            _rankingManager = ranking;
        }

        [HttpPost]
        public async Task<CreateAccountResponse> CreateAccountPost(CreateAccountRequest request)
        {
            var response = new CreateAccountResponse();

            // PW 암호화 ( Salt + HashingPassword )
            var saltValue = Security.SaltString();
            var hashingPassword = Security.MakeHashingPassWord(saltValue, request.PW);

            var (errorCode, lastCreateIndex) = await _accountDb.CreateAccountDataAsync(
                    request.ID, hashingPassword, saltValue);
            if (errorCode != ErrorCode.None)
            {
                response.Result = errorCode;
                _logger.ZLogError($"{nameof(CreateAccountPost)} ErrorCode : {response.Result}");
                return response;
            }

            // GameDB에 유저 기본 초기화 정보 세팅하기
            (errorCode, var lastGameInfoIndex) = await InitUserGameInfoAsync(request, lastCreateIndex);
            if (errorCode != ErrorCode.None)
            {                
                response.Result = errorCode;
                _logger.ZLogError($"{nameof(CreateAccountPost)} ErrorCode : {response.Result}");
                return response;
            }

            // 출석체크 정보 초기화
            errorCode = await InitDailyCheckAsync(request, lastCreateIndex, lastGameInfoIndex);
            if (errorCode != ErrorCode.None)
            {
                response.Result = errorCode;
                _logger.ZLogError($"{nameof(CreateAccountPost)} ErrorCode : {response.Result}");
                return response;
            }

            // StarCount 0인 상태로 Redis에 등록하기
            errorCode = await InitStarCountAsync(request, lastCreateIndex, lastGameInfoIndex);
            if (errorCode != ErrorCode.None)
            {                
                response.Result = errorCode;
                _logger.ZLogError($"{nameof(CreateAccountPost)} ErrorCode : {response.Result}");
                return response;  
            }
            
            return response;
        }

        private async Task<Tuple<ErrorCode, Int64>> InitUserGameInfoAsync(CreateAccountRequest request, Int64 lastCreateIndex)
        {
            // GameDB에 유저 기본 초기화 정보 세팅하기
            var (errorCode, lastGameInfoIndex) = await _gameDb.InitUserGameInfoAsync(request.ID, new UserGameInfo(1, 0, 0, 0));
            if (errorCode != ErrorCode.None)
            {
                // Rollback 계정 생성
                var innerErrorCode = await _accountDb.RollbackCreateAccountDataAsync(lastCreateIndex);
                if (innerErrorCode != ErrorCode.None)
                {
                    _logger.ZLogError($"{nameof(CreateAccountPost)} ErrorCode : {innerErrorCode}");
                }

                return new Tuple<ErrorCode, Int64>(errorCode, 0);
            }
            return new Tuple<ErrorCode, Int64>(ErrorCode.None, lastGameInfoIndex);
        }

        private async Task<ErrorCode> InitDailyCheckAsync(CreateAccountRequest request, Int64 lastCreateIndex, Int64 lastGameInfoIndex)
        {
            var errorCode = await _gameDb.InitDailyCheckAsync(request.ID);
            if (errorCode != ErrorCode.None)
            {
                // Rollback 계정 생성
                var innerErrorCode = await _accountDb.RollbackCreateAccountDataAsync(lastCreateIndex);
                if (innerErrorCode != ErrorCode.None)
                {
                    _logger.ZLogError($"{nameof(CreateAccountPost)} ErrorCode : {innerErrorCode}");
                }

                // Rollback gameInfo 
                innerErrorCode = await _gameDb.RollbackInitUserGameInfoAsync(lastGameInfoIndex);
                if (innerErrorCode != ErrorCode.None)
                {
                    _logger.ZLogError($"{nameof(CreateAccountPost)} ErrorCode : {innerErrorCode}");
                }

                return errorCode;
            }
            return ErrorCode.None;
        }

        private async Task<ErrorCode> InitStarCountAsync(CreateAccountRequest request, Int64 lastCreateIndex, Int64 lastGameInfoIndex)
        {
            var errorCode = await _rankingManager.UpdateStarCount(request.ID, 0, _gameDb, _redisDb);
            if (errorCode != ErrorCode.None)
            {
                // Rollback 계정 생성
                var innerErrorCode = await _accountDb.RollbackCreateAccountDataAsync(lastCreateIndex);
                if (innerErrorCode != ErrorCode.None)
                {
                    _logger.ZLogError($"{nameof(CreateAccountPost)} ErrorCode : {innerErrorCode}");
                }

                // Rollback gameInfo 
                innerErrorCode = await _gameDb.RollbackInitUserGameInfoAsync(lastGameInfoIndex);
                if (innerErrorCode != ErrorCode.None)
                {
                    _logger.ZLogError($"{nameof(CreateAccountPost)} ErrorCode : {innerErrorCode}");
                }

                // Rollback DailyCheck
                innerErrorCode = await _gameDb.RollbackInitDailyCheckAsync(request.ID);
                if (innerErrorCode != ErrorCode.None)
                {
                    _logger.ZLogError($"{nameof(CreateAccountPost)} ErrorCode : {innerErrorCode}");
                }

                return errorCode;
            }
            return ErrorCode.None;
        }
    }
}