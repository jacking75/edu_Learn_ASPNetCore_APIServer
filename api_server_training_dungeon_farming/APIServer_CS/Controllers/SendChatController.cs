using System;
using System.Threading.Tasks;

using APIServer.ModelDB;
using APIServer.ModelReqRes;
using APIServer.Services;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
namespace APIServer.Controllers;

[ApiController]
[Route("[controller]")]
public class SendChat : ControllerBase
{
    private readonly ILogger<SendChat> _logger;

    readonly IMemoryDb _memoryDb;


    public SendChat(ILogger<SendChat> logger, IMemoryDb memoryDb)
    {
        _logger = logger;
        _memoryDb = memoryDb;
    }


    [HttpPost]
    public async Task<SendChatResponse> Post(SendChatRequest request)
    {
        var response = new SendChatResponse();
        var userInfo = (CertifiedUser)HttpContext.Items[nameof(CertifiedUser)]!;


        if (await AddChatMessage(userInfo.ChannelNumber, userInfo.Email, request.Message) == false)
        {
            response.Result = ErrorCode.FailedRedisRegist;
            return response;
        }


        return response;
    }


    private async Task<bool> AddChatMessage(Int32 channelNumber, string email, string message)
    {
        var info = CreateChatInfo(email, message);
        if (await _memoryDb.AddChannelChatInfo(channelNumber, info) == false)
        {
            return false;
        }
        return true;
    }


    private ChatInfo CreateChatInfo(string email, string message)
    {
        return new ChatInfo
        {
            Email = email,
            Message = message
        };
    }






}
