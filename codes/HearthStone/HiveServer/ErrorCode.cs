public enum ErrorCode : Int64
{
    None = 0,
    
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
    DuplicateNickname = 1011,
}
