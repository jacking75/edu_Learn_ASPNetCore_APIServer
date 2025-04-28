using GameServer.Services.Interfaces;
using GameShared.DTO;
using Microsoft.AspNetCore.Mvc;
using ServerShared.ServerCore;

namespace GameServer.Controllers;

[Route("[controller]")]
[ApiController]
public class OmokController : BaseController<OmokController>
{
	private readonly IOmokService _omokService;
	public OmokController(ILogger<OmokController> logger, IOmokService omokService) : base(logger)
	{
		_omokService = omokService;
	}

	[HttpPost("start")]
	public async Task<OmokEnterResponse> StartOmok()
	{
		OmokEnterResponse response = new();

		Int64 uid = GetUserUid();

		response.Result = await _omokService.EnterOmok(uid);

		if (response.Result == ErrorCode.None)
		{
			ActionLog(new 
			{
				uid
			});
		}

		return response;
	}

	[HttpPost("put")]
	public async Task<OmokPutResponse> PutOmok(OmokPutRequest request)
	{
		OmokPutResponse response = new()
		{
			Result = await _omokService.PutOmok(GetUserUid(), request.PosX, request.PosY)
		};

		return response;
	}

	[HttpPost("peek")]
	public async Task<OmokPeekResponse> PeekTurn([FromBody] OmokPeekRequest request)
	{
		OmokPeekResponse response = new();

		(response.Result,(response.TurnCount, response.OmokData)) = await _omokService.PeekTurn(GetUserUid(), request.TurnCount);

		return response;
	}
}
