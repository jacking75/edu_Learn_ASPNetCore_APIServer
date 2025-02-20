namespace ContentDLL;

// 20000 ~ 24999
public enum ErrorCode : Int16
{
    None = 0,



    // 시나리오에서 발생하는 에러 (20101 ~ )
    HttpRequestFaile = 20101,
    JsonDeserializeFailed = 20102,

    HiveCreateAccountRequestFaile = 20201,
    HiveCreateAccountFailed = 20202,
    HiveCreateAccountException = 20203,

    End = 24999,
}