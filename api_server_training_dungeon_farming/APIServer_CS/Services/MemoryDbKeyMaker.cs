using System;

namespace APIServer.Services;

public class MemoryDbKeyMaker
{
    private static readonly string UserRequestLockKey = "USER_REQUEST_LOCK_";

    private static readonly string ChannelKey = "CAHNNEL_";

    private static readonly string CertifiedUserKey = "CERTIFIE_USER_";

    private static readonly string BattleInfoKey = "BATTLE_INFO_";


    public static string MakeUserRequestLockKey(string id) => UserRequestLockKey + id;


    public static string MakeCertifiedUserKey(string id) => CertifiedUserKey + id;


    public static string MakeBattleInfoKey(string id) => BattleInfoKey + id;


    public static string MakeChannelKey(Int32 channelNumber) => ChannelKey + channelNumber.ToString();


    public static string GetChannelKey() => ChannelKey;
}



public class MemoryDbKeyExpireTime
{
    public const UInt16 UserRequestLockSecond = 3;

    public const UInt16 LoginMin = 60;

    public const UInt16 BattleMin = 10;
}