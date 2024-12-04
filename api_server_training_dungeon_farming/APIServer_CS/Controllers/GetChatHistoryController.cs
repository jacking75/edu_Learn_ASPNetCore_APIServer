using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using APIServer.ModelDB;
using APIServer.ModelReqRes;
using APIServer.Services;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
namespace APIServer.Controllers;

[ApiController]
[Route("[controller]")]
public class GetChatHistory : ControllerBase
{
    static readonly Int32 ChannelChatHistoryGetCount = 10;

    private readonly ILogger<GetChatHistory> _logger;

    private readonly IMemoryDb _memoryDb;


    public GetChatHistory(ILogger<GetChatHistory> logger, IMemoryDb memoryDb)
    {
        _logger = logger;
        _memoryDb = memoryDb;
    }


    [HttpPost]
    public async Task<GetChatHistoryResponse> Post(GetChatHistoryRequest request)
    {
        var response = new GetChatHistoryResponse();
        var userInfo = (CertifiedUser)HttpContext.Items[nameof(CertifiedUser)]!;

        response.MessageList = await GetMessageList(userInfo.ChannelNumber, request.MessageId);

        return response;
    }


    private async Task<List<ChatInfo>> GetMessageList(Int32 channelNumber, string messageId)
    {
        var (success, loadedDatas) = await _memoryDb.GetChannelChatInfoList(channelNumber, ChannelChatHistoryGetCount, messageId);
        if (success == false)
        {
            return null;
        }

        return loadedDatas;
    }



}
