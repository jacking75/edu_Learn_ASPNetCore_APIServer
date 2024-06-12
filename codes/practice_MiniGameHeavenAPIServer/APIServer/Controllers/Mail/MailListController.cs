using APIServer.DTO;
using APIServer.DTO.Mail;
using APIServer.Servicies.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using ZLogger;

namespace APIServer.Controllers.Mail;

[ApiController]
[Route("[controller]")]
public class MailList : ControllerBase
{
    readonly ILogger<MailList> _logger;
    readonly IMailService _mailService;

    public MailList(ILogger<MailList> logger, IMailService mailService)
    {
        _logger = logger;
        _mailService = mailService;
    }

    /// <summary>
    /// 메일 목록 정보 API
    /// 유저의 메일 목록 정보를 가져옵니다.
    /// </summary>
    [HttpPost]
    public async Task<MailboxInfoResponse> GetMailList([FromHeader] HeaderDTO header)
    {
        MailboxInfoResponse response = new();

        (response.Result, response.MailList) = await _mailService.GetMailList(header.Uid);

        _logger.ZLogInformation($"[MailList] Uid : {header.Uid}");
        return response;
    }
}
