using Microsoft.AspNetCore.Mvc;
using SampleServer.DTO;
using SampleServer.Services;
using ZLogger;

namespace SampleServer.Controllers;

[Route("[controller]")]
[ApiController]
public class LoggingController : ControllerBase
{
	private readonly ILoggingService _loggingService;


	public LoggingController(ILoggingService loggingService)
	{
		_loggingService = loggingService;
	}
	[HttpPost]
	public ActionResult Log([FromBody] LogRequest request)
	{
		_loggingService.Log(request.Message, request.LogType);

		return Ok(request.Message);
	}
}
