using ApiServer.Model;
using ApiServer.Services;
using Microsoft.AspNetCore.Mvc;
using ServerCommon;
using ZLogger;

namespace ApiServer.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MailListController : ControllerBase
    {
        private readonly IGameDb _gameDb;
        private readonly ILogger<MailListController> _logger;

        public MailListController(ILogger<MailListController> logger, IGameDb gameDb)
        {
            _logger = logger;
            _gameDb = gameDb;
        }

        [HttpPost]
        public async Task<MailListResponse> CheckMailPost(MailListRequest request)
        {
            var response = new MailListResponse();
            
            // db에서 Mail된 정보 긁어오기
            var (errorCode, totalMailSize, MailList) = await _gameDb.CheckMailAsync(request.ID, request.PageIndex);
            
            // 예외 상황이 발생한 경우
            if(errorCode != ErrorCode.None)
            {
                response.Result = errorCode;
                _logger.ZLogError($"{nameof(CheckMailPost)} ErrorCode : {response.Result}");
                return response;
            }

            response.TotalSize = totalMailSize;
            response.MailInfo = MailList;
            return response;
        }
    }
}