using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using HiveAPIServer.Services;
using ZLogger;
using System.Threading.Tasks;


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
    public async Task<HiveVerifyTokenResponse> Verify([FromBody] HiveVerifyTokenRequest request)
    {
		HiveVerifyTokenResponse response = new();
        response.Result = await _hiveService.VerifyToken(request.PlayerId, request.HiveToken);


		if (ErrorCode.None != response.Result)
        {
            _logger.ZLogError($"[VerifyToken] ErrorCode: {response.Result}");
            //response.Result = ErrorCode.Hive_VerifyTokenFail;
        }

        return response;
    }
}
