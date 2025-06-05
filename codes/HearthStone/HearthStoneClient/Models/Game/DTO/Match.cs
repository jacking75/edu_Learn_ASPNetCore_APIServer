namespace HearthStoneWeb.Models.Game;

public class MatchInfo
{
    public int MatchType { get; set; } = 0; // 0: pvp, 1: pve
    public List<Int64> UserList { get; set; }
    public Guid MatchGUID { get; set; }
}

public class MatchWaiting
{
    public MatchInfo MatchInfo { get; set; }
    public List<Int64> MatchAcceptUserList { get; set; }
    public DateTime MatchingWaitingExpireTime { get; set; }
}

public class MatchAddReqeust
{
    public Int64 AccountUid { get; set; }
}

public class MatchAddResponse : ErrorCodeDTO
{
}

public class MatchWaitingResponse : ErrorCodeDTO
{
    public Guid MatchGUID { get; set; }
}

public class MatchStatusRequest
{
    public Guid MatchGUID { get; set; }
}

public class MatchStatusResponse : ErrorCodeDTO
{
    public HSGameInfo? GameInfo { get; set; }
}
