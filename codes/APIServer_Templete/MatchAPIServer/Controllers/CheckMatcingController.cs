using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ZLogger;
using static APIServer.Controllers.CheckMatching;

namespace APIServer.Controllers;

[ApiController]
[Route("[controller]")]
public class CheckMatching : Controller
{
    IMatchWoker _matchWorker;


    public CheckMatching(IMatchWoker matchWorker)
    {
        _matchWorker = matchWorker;
    }

    [HttpPost]
    public CheckMatchingRes Post(CheckMatchingReq request)
    {
        CheckMatchingRes response = new();

        (var result, var completeMatchingData) = _matchWorker.GetCompleteMatching(request.UserID);
        
        //TODO: 결과를 담아서 보낸다

        return response;
    }


}

public class CheckMatchingReq
{
    public string UserID { get; set; }
}


public class CheckMatchingRes
{
    public ErrorCode Result { get; set; } = ErrorCode.None;
    public string ServerAddress { get; set; } = "";
    public int RoomNumber { get; set; } = 0;    
}
