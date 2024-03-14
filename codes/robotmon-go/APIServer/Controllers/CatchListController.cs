using ApiServer.Model;
using ApiServer.Services;
using Microsoft.AspNetCore.Mvc;
using ServerCommon;
using ZLogger;

namespace ApiServer.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CatchListController : ControllerBase
    {
        private readonly IGameDb _gameDb;
        private readonly ILogger<CatchListController> _logger;

        public CatchListController(ILogger<CatchListController> logger, IGameDb gameDb)
        {
            _logger = logger;
            _gameDb = gameDb;
        }

        [HttpPost]
        public async Task<CatchListResponse> CheckCatchPost(CatchListRequest request)
        {
            var response = new CatchListResponse();

            var (errorCode, monsterList) = await _gameDb.GetCatchListAsync(request.ID);

            if (errorCode != ErrorCode.None)
            {
                response.Result = errorCode;
                _logger.ZLogError($"{nameof(CheckCatchPost)} ErrorCode : {response.Result}");
                return response;
            }

            response.MonsterInfoList = monsterList;
            return response;
        }
    }
}