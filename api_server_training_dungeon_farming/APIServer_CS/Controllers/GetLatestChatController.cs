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
public class GetLatestChat : ControllerBase
{
    private readonly Int32 ChannelLatestChatGetCount = 5;

    private readonly ILogger<GetLatestChat> _logger;

    private readonly IMemoryDb _memoryDb;


    public GetLatestChat(ILogger<GetLatestChat> logger, IMemoryDb memoryDb)
    {
        _logger = logger;
        _memoryDb = memoryDb;
    }


    [HttpPost]
    public async Task<GetLatestChatResponse> Post(GetLatestChatRequest request)
    {
        var response = new GetLatestChatResponse();
        var userInfo = (CertifiedUser)HttpContext.Items[nameof(CertifiedUser)]!;

        response.MessageList = await GetLatestChatList(userInfo.ChannelNumber, request.MessageId);


        return response;
    }


    private async Task<List<ChatInfo>> GetLatestChatList(Int32 channelNumber, string messageId = null)
    {
        var (success, loadedDatas) = await _memoryDb.GetChannelChatInfoList(channelNumber, ChannelLatestChatGetCount, messageId);
        if (success == false)
        {
            return null;
        }

        return loadedDatas;
    }



}
