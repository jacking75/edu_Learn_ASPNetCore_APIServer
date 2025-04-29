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
public class MailDelete : ControllerBase
{
    readonly ILogger<MailDelete> _logger;
    readonly IMailService _mailService;

    public MailDelete(ILogger<MailDelete> logger, IMailService mailService)
    {
        _logger = logger;
        _mailService = mailService;
    }

    /// <summary>
    /// 메일 삭제 API
    /// 메일함에서 메일을 삭제합니다.
    /// </summary>
    [HttpPost]
    public async Task<MailDeleteResponse> DeleteMail([FromHeader] HeaderDTO header, MailDeleteRequest request)
    {
        MailDeleteResponse response = new();

        response.Result = await _mailService.DeleteMail(header.Uid, request.MailSeq);

        _logger.ZLogInformation($"[MailDelete] Uid : {header.Uid}");
        return response;
    }
}
