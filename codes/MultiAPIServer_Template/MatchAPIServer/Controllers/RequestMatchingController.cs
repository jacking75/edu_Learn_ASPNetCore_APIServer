// 클라이언트가 플랫폼 서버에서 인증을 받았는지 확인해 준다
// 클라이언트는 플랫폼 서버에서 받은 인증토큰과 자신의 인증ID(계정 ID 혹은 플랫폼에서 만들어준 ID)로 보낸다.
// 사용할 수 있는 인증ID와 인증토큰은 이미 정해져 있다.
// 게임 서버는 인증이 성공하면
// - 이 유저의 default 게임데이터가 없다면 생성해줘야 한다.

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ZLogger;


namespace MatchAPIServer.Controllers;

[ApiController]
[Route("[controller]")]
public class RequestMatching : ControllerBase
{
    IMatchWoker _matchWorker;

    public RequestMatching(IMatchWoker matchWorker)
    {
        _matchWorker = matchWorker;
    }

    [HttpPost]
    public MatchResponse Post(MatchingRequest request)
    {
        MatchResponse response = new();

        _matchWorker.AddUser(request.UserID);

        return response;
    }

    
}

public class MatchingRequest
{
    public string UserID { get; set; }
}

public class MatchResponse
{
    [Required] public ErrorCode Result { get; set; } = ErrorCode.None;
}
