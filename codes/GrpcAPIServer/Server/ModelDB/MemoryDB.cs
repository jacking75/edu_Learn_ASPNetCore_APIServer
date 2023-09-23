namespace ModelMemoryDB;
public enum UserState
{
    Default = 0,
    Login = 1,
    Matching = 2,
    Playing = 3
}


public class AuthUser
{
    public string Email { get; set; } = "";
    public string AuthToken { get; set; } = "";
    public Int64 AccountId { get; set; } = 0;
    public string State { get; set; } = ""; // enum UserState    
}

public class RediskeyExpireTime
{
    public const ushort NxKeyExpireSecond = 3;
    public const ushort RegistKeyExpireSecond = 6000;
    public const ushort LoginKeyExpireMin = 60;
    public const ushort TicketKeyExpireSecond = 6000; // 현재 테스트를 위해 티켓은 10분동안 삭제하지 않는다. 
}