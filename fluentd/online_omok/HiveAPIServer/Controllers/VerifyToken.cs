using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using HiveAPIServer.Services;
using ZLogger;
using System.Threading.Tasks;
using GameShared.DTO;


namespace HiveAPIServer.Controllers;

[ApiController]
[Route("[controller]")]
public class VerifyToken : ControllerBase
{
    readonly ILogger<VerifyToken> _logger;
    readonly IHiveService _hiveService;

    public VerifyToken(ILogger<VerifyToken> logger, IHiveService hiveService)
    {
        _logger = logger;
		_hiveService = hiveService;
    }

    [HttpPost]
    public async Task<HiveVerifyResponse> Verify([FromBody] HiveVerifyRequest request)
    {
		HiveVerifyResponse response = new();
        response.Result = await _hiveService.VerifyToken(request.PlayerId, request.Token);


		if (ErrorCode.None != response.Result)
        {
            _logger.ZLogError($"[VerifyToken] ErrorCode: {response.Result}");
            response.Result = ErrorCode.HiveVerifyTokenFail;
        }

        return response;
    }
}
