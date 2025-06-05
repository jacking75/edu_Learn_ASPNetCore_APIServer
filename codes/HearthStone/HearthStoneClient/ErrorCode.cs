public enum ErrorCode : Int64
{
    None = 0,
    
    Fail = 1,
    CreateAccountFail = 100,

    LoginFail = 1000,
    HiveTokenInvalid = 1001,
    UserNotFound = 1002,
    CheckAuthFailNotExist = 1003,
    RegistTokenFail = 1004,
    LoginUpdateRecentLoginFail = 1005,
    LoginFailException = 1006,
    CreateUserFailNoNickname = 1007,
    CreateUserFailDuplicateNickname = 1008,
    CreateUserFailException = 1009,
    DatabaseError = 1010,

    TokenDoesNotExist= 2000,
    UidDoesNotExist = 2001,
    AuthTokenFailWrongAuthToken = 2002,
    AuthTokenKeyNotFound = 2003,
    AuthTokenFailSetNx = 2004,

    AttendanceInfoFailException = 3000,
    AttendanceCheckFailAlreadyChecked = 3001,
    AttendanceCheckFailException = 3002,
    AttendanceCheckFailUpdateMoney = 3003,
    AttendanceCheckFailUpdateItem = 3004,

    GachaReceiveFailException = 4000,

    MailInfoFailException = 5000,
    MailLoadFail = 5001,

    ItemInvalidRequest = 6000,
    ItemInfoFailException = 6001,
    ItemLoadFail = 6002,
    ItemRegistFail = 6003,

    CurrencyInvalidRequest = 7000,
    CurrencyLoadFail = 7001,
    CurrencyRegistFail = 7002,

    CharactorLoadFail = 8000,
    DeckLoadFail = 8003,

    MatchRequestFailException = 9001,
    MatchStatusCheckFailException = 9002,
    MatchCancelFailException = 9003,

    MAX,
}
