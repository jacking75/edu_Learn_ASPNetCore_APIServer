using MatchAPIServer.DTO;
using MatchAPIServer.DTO.Mail;
using MatchAPIServer.Servicies.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using ZLogger;

namespace MatchAPIServer.Controllers.Mail;

[ApiController]
[Route("[controller]")]
public class MailReceive : ControllerBase
{
    readonly ILogger<MailReceive> _logger;
    readonly IMailService _mailService;

    public MailReceive(ILogger<MailReceive> logger, IMailService mailService)
    {
        _logger = logger;
        _mailService = mailService;
    }

    /// <summary>
    /// 메일 보상 수령 API
    /// 메일에 포함된 보상을 모두 수령하고, 수령한 보상을 반환합니다.
    /// </summary>
    [HttpPost]
    public async Task<MailReceiveResponse> ReceiveMail([FromHeader] HeaderDTO header, MailReceiveRequest request)
    {
        MailReceiveResponse response = new();

        (response.Result, response.Rewards) = await _mailService.ReceiveMail(header.Uid, request.MailSeq);

        _logger.ZLogInformation($"[MailReceive] Uid : {header.Uid}");
        return response;
    }
}
