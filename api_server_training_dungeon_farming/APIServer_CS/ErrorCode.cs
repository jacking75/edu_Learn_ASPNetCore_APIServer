using System;

// 1000 ~ 19999
public enum ErrorCode : UInt16
{
    None = 0,

    // Middleware 1000 ~ 1999
    AuthTokenFailSetNx = 1001,
    AuthTokenFailWrongKeyword = 1002,
    AuthTokenFailWrongAuthToken = 1003,
    InvalidRequestHttpBody = 1004,
    InvalidAppVersion = 1005,
    InvalidMasterDataVersion = 1006,


    // AccountDB Services 2000 ~ 2999
    FailedCreateAccount = 2001,
    FailedDeleteAccount = 2002,
    NotExistAccount = 2011,
    InvalidPassword = 2051,
    AccountDbException = 2999,


    // GameDB Services 3000 ~ 3999
    FailedCreateUserPlayData = 3001,
    FailedCreateUserAttendanceBook = 3002,
    FailedAddUserItem = 3003,
    FailedAddUserMail = 3004,
    FailedUpdateUserAttendanceBook = 3005,
    FailedUpdateUserMail = 3006,
    FailedAddInAppProductReceipt = 3007,
    FailedUpdateUserExp = 3008,
    FailedAddDompletedStageHistory = 3009,
    NotExistUserPlayData = 3011,
    NotExistUserItem = 3012,
    NotExistUserMail = 3013,
    NotExistUserAttendanceBook = 3014,
    InvalidAttendanceDay = 3051,
    GameDbException = 3999,


    // Redis Services 5000 ~ 5999
    FailedRedisRegist = 5001,
    FailedRedisUpdate = 5002,
    NotExsitCertifiedUser = 5011,
    NotExistsUserBattleInfo = 5012,
    RedisException = 5999,


    // Controller 6000 ~ 19999
    ImposibbleEnhanceItem = 6205,
    InvalidItemType = 6207,
    InvalidBattleInfo = 6208,
    InvalidChannelNumber = 6209,
    InvalidChannelUserId = 6210,
    NotCertifiedUser = 5201,
    AlreadyReceivedRecept = 6303,
    AlreadyChannelUser = 6304,
    FailedAddFarmingItem = 6304,
    ImpossibleEnterStage = 6401,
    ImpossibleAppearanceItem = 6402,
    RangeOverDefeatedCount = 6403,
    NotCompletedStage = 6404,
    ChannelIsFull = 6405,
}