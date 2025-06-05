using Microsoft.AspNetCore.Mvc;
using GameServer.Models.DTO;
using GameServer.Services.Interface;
using ZLogger;

namespace GameServer.Controllers.Contents;

[ApiController]
[Route("contents/mail")]
public class MailController : ControllerBase
{
    readonly ILogger<MailController> _logger;
    readonly IMailService _mailService;

    public MailController(ILogger<MailController> logger, IMailService mailService)
    {
        _logger = logger;
        _mailService = mailService;
    }

    [HttpPost("load")]
    public async Task<MailInfoResponse> GetMailInfoList([FromHeader] HeaderDTO header) 
    {
        MailInfoResponse response = new();
        (response.Result, response.MailList) = await _mailService.GetMailInfoList(header.AccountUid);
        return response;
    }

    [HttpPost("read")]
    public async Task<MailReadResponse> GetMailReadList([FromHeader] HeaderDTO header, [FromBody] MailReadRequest request)
    {
        MailReadResponse response = new();
        response.Result = await _mailService.ReadMail(header.AccountUid, request.MailId);
        return response;
    }

    [HttpPost("delete")]
    public async Task<MailDeleteResponse> GetMailDeleteList([FromHeader] HeaderDTO header, [FromBody] MailDeleteRequest request)
    {
        MailDeleteResponse response = new();
        response.Result= await _mailService.DeleteMail(header.AccountUid, request.MailId);
        return response;
    }
}
