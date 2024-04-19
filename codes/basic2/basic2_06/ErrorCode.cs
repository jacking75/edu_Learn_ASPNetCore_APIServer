using System;

// 1000 ~ 19999
public enum ErrorCode : UInt16
{
    None = 0,

    UnhandleException = 1001,
    RedisFailException = 1002,
    InValidRequestHttpBody = 1003,
    AuthTokenFailWrongAuthToken = 1006,
    TokenDoesNotExist = 1007,
    UserIDDoesNotExist = 1008,
    AuthTokenKeyNotFound = 1009,

    
    LoginFailException = 2002,
    LoginFailUserNotExist = 2003,
    LoginFailPwNotMatch = 2004,
    LoginFailSetAuthToken = 2005,
    LoginFailAddRedis = 2006,

    AttendanceDataNull = 2011,
}