using System.ComponentModel.DataAnnotations;
using SampleServer.Services;

namespace SampleServer.DTO;

public class LogRequest
{
	[Required]
	public string Message { get; set; }

	[Required]
	public LogType LogType { get; set; }
}
