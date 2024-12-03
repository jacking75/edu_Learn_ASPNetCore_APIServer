
public enum ErrorCode : UInt16
{

	None = 0,

	////////////////////////////////////////////////////////////////
	// Hive 100 ~

	HiveTokenNotFound = 100,
	HiveTokenInvalid = 101,
	HiveTokenExpired = 102,
	HiveTokenMismatch = 103,
	HiveTokenInvalidPlayerId = 104,
	HiveVerifyTokenFail = 105,
	HiveVerifyTokenFailException = 106,
	HiveSaveTokenFail = 107,
	HiveSaveTokenException = 108,

	HiveLoginFail = 110,
	HiveLoginFailUserNotFound = 111,
	HiveLoginFailPasswordInvalid = 112,
	HiveLoginFailException = 113,

	HiveCreateAccountFail = 120,
	HiveCreateAccountException = 121,

	HiveInsertFailException = 128,
	HiveSelectFailException = 129,
	HiveUpdateFailException = 130,

	////////////////////////////////////////////////////////////////
	// System 500 ~

	UnhandledException = 500,
	InvalidAppVersion = 501,
	InvalidMasterDataVersion = 502,
	MatchServerInternalError = 503,
	MatchServerRequestFail = 504,
	MatchServerRequestException = 505,
    MatchServerUserNotFound = 506,

    ////////////////////////////////////////////////////////////////
    // Authentication 1000 ~

    ClaimNotFound = 1000,
	ClaimInvalid = 1001,

	ClaimUidNotFound = 1010,
	ClaimUidInvalid = 1011,

	ClaimAuthTokenNotFound = 1020,
	ClaimAuthTokenInvalid = 1021,
	ClaimAuthTokenNull = 1022,
	ClaimAuthTokenExpired = 1023,
	ClaimAuthTokenUserNotFound = 1024,

	LoginFail = 1030,
	LoginFailUserNotFound = 1031,
	LoginFailPasswordInvalid = 1032,
	LoginFailException = 1033,
	LoginFailInvalidResponse = 1034,
	LoginFailBadRequest = 1035,

	RegisterFail = 1040,
	RegisterFailException = 1041,

	UpdateUserFail = 1050,
	UpdateUserException = 1051,
	UpdateUserFailBadRequest = 1052,

	////////////////////////////////////////////////////////////////
	// Database 2000 ~

	DbUserNotFound = 2000,
	DbUserFindPlayerIdFail = 2001,
	DbUserFindPlayerIdException = 2002,

	DbUserHiveTokenNotFound = 2050,
	DbUserHiveTokenException = 2051,

	DbUserInsertFail = 2060,
	DbUserInsertException = 2061,

	DbUserRecentLoginUpdateFail = 2070,
	DbUserRecentLoginUpdateException = 2071,
	DbUserNicknameUpdateFail = 2072,
	DbUserNicknameUpdateException = 2073,

	DbUserLoginFailPasswordInvalid = 2080,

	DbLoadUserInfoFail = 2100,
	DbLoadUserMoneyFail = 2101,
	DbLoadUserItemFail = 2102,
	DbLoadUserException = 2103,
	DbLoadUserNotFound = 2104,
	DbLoadUserProfileFail = 2105,
	DbLoadUserProfileException = 2106,

	DbGameResultInsertFail = 2110,
	DbGameResultInsertException = 2111,

    DbGameRewardInsertFail = 2120,
    DbGameRewardInsertException = 2121,

	DbAttendanceUpdateFail = 2130,
	DbAttendanceUpdateException = 2131,

	DbAttendanceGetFail = 2140,
	DbAttendanceGetException = 2141,

	DbAttendanceInsertFail = 2150,
	DbAttendanceInsertException = 2151,

	DbUserGameUpdateFail = 2160,
	DbUserGameUpdateException = 2161,

	DbMailGetFail = 2170,
	DbMailGetException = 2171,
	DbMailGetFailMailNotFound = 2172,

	DbMailInsertFail = 2180,
	DbMailInsertException = 2181,

	DbMailReceiveFail = 2190,
	DbMailReceiveException = 2191,

	DbItemInsertFail = 2200,
	DbItemInsertException = 2201,
	DbItemInsertItemNotFound = 2202,
	DbItemInsertTemplateNotFound = 2203,

	DbMailUpdateFail = 2210,
	DbMailUpdateException = 2211,

	DbMailDeleteFail = 2220,
	DbMailDeleteException = 2221,


	////////////////////////////////////////////////////////////////
	// Redis 3000 ~

	RedisUserLockNotFound = 3000,
	RedisUserLockInvalid = 3001,
	RedisUserLockOccupied = 3002,

	RedisDataNotFound = 3010,
	RedisDataInvalid = 3011,
	RedisDataDeleteFail = 3012,
	RedisDataGetException = 3013,
	RedisDataSetException = 3014,

	RedisMatchNotFound = 3020,
	RedisMatchInvalid = 3021,
	RedisMatchExpired = 3022,
	RedisMatchGetException = 3023,
	RedisMatchSetException = 3024,

	RedisGameNotFound = 3030,
	RedisGameInvalid = 3031,
	RedisGameNull = 3032,
	RedisGameGetException = 3033,
	RedisGameSetException = 3034,

	RedisGameEnterFailLock = 3040,
	RedisGameEnterException = 3041,

	RedisUserGameNotFound = 3050,
	RedisUserGameInvalid = 3051,
	RedisUserGameNull = 3052,

	RedisUserGameGetException = 3060,
	RedisUserGameSetException = 3061,

	RedisUserGameStoreFail = 3070,
	RedisTokenStoreFail = 3071,


	RedisGameLockOccupied = 3080,

	////////////////////////////////////////////////////////////////
	// Game 4000 ~

	GameNotFound = 4000,
	GameInvalidGameRoomId = 4001,

	GameGetException = 4010,
	GameGetFail = 4011,
	GameGetFailGameNotFound = 4012,
	GameGetFailInvalidGameData = 4013,

	GameEnterFail = 4020,
	GameEnterFailMatchNotFound = 4021,
	GameEnterFailGameNotFound = 4022,
	GameEnterGameException = 4023,
	GameEnterFailPlayerNotFound = 4024,
	GameEnterPlayerFail = 4025,
	GameEnterFailInvalidGameStatus = 4026,

	GameSaveGameFail = 4030,
	GameSaveGameException = 4031,
	GameSaveUserGameFail = 4032,
	GameSaveUserGameException = 4033,

	GameGetStoneFail = 4040,
	GameGetStoneFailGameNotFound = 4041,
	GameGetStoneFailInvalidPostion = 4042,
	GameGetStoneException = 4043,

	GameSaveStoneFail = 4050,
	GameSaveStoneException = 4051,
	GameSaveStoneFailGameNotFound = 4052,
	GameSaveStoneFailInvalidUser = 4053,
	GameSaveStoneFailInvalidPosition = 4054,
	GameSaveStoneFailInvalidTurn = 4055,
	GameSaveStoneFailInvalidGameStatus = 4056,
	GameSaveStoneFailInvalidParameters = 4057,
	GameSaveStoneFailExpiredTurn = 4058,

	GameCheckTurnFail = 4060,
	GameCheckTurnFailInvalidUser = 4061,
	GameCheckTurnFailGameNotFound = 4062,
	GameCheckTurnFailInvalidGameStatus = 4063,
	GameCheckTurnFailInvalidTurn = 4064,
	GameCheckTurnException = 4065,
	
	GameMatchCancelled = 4070,
	GameMatchTimeout = 4071,
	GameMatchDuplicate = 4072,
	GameMatchUserNotFound = 4073,
	GameMatchInvalidResponse = 4074,
	GameMatchInvalidData = 4075,
	GameMatchRequestFail = 4076,
	GameMatchCreateUserDataFail = 4077,
	GameMatchInvalidUserStatus = 4048,
    GameMatchBadRequest = 4079,

    GameMatchCheckFail = 4080,
	GameMatchCheckException = 4081,
	GameMatchCheckFailInvalidData = 4082,
	GameMatchCheckFailBadRequest = 4083,

	GamePlayFail = 4090,
	GamePlayException = 4091,
	GamePlayFailGameNotFound = 4092,
	GamePlayFailGameLoadFail = 4093,
	GamePlayFailInvalidData = 4094,
	GamePlayFailInvalidTurn = 4095,

	GamePeekFail = 4100,
	GamePeekFailGameNotFound = 4101,
	GamePeekFailInvalidData = 4102,
	GamePeekException = 4103,
	GamePeekCancelled = 4104,


	GameStartFail = 4110,
	GameStartException = 4111,

	GameSaveResultFail = 4120,
	GameSendRewardFail = 4121,

	GameLoadOpponentProfileFail = 4130,
	GameLoadOpponentProfileException = 4131,
	GameLoadOpponentFailGameNotFound = 4132,
	GameLoadOpponentFailBadRequest = 4133,


	////////////////////////////////////////////////////////////////
	// Attendance 5000 ~

	AttendanceGetNotFound = 5000,
	AttendanceGetBadRequest = 5001,
	AttendanceGetFail = 5002,
	AttendanceGetException = 5003,

	AttendanceUpdateFail = 5010,
	AttendanceUpdateException = 5011,
	AttendanceUpdateBadRequest = 5012,
	AttendanceUpdateFailAlreadyAttended = 5013,
	AttendanceUpdateFailUserNotFound = 5014,


	////////////////////////////////////////////////////////////////
	// Mail 6000 ~

	MailGetNotFound = 6000,
	MailGetBadRequest = 6001,
	MailGetFail = 6002,
	MailGetException = 6003,
	MailGetFailMailExpired = 6004,

	MailReceiveFailMailNotFound = 6004,
	MailReceiveFailRewardNotFound = 6005,
	MailReceiveFailItemNotFound = 6006,
	MailReceiveFail = 6007,
	MailReceiveException = 6008,
	MailReceiveBadRequest = 6009,

	MailReadFail = 6010,
	MailReadException = 6011,
	MailReadBadRequest = 6012,

	MailSendFail = 6020,
	MailSendException = 6021,
	MailSendBadRequest = 6022,

	MailSendRewardFail = 6030,
	MailSendRewardException = 6031,

	MailDeleteFail = 6040,
	MailDeleteException = 6041,
	MailDeleteBadRequest = 6042,

	MailReceiveFailAlreadyReceived = 6050,
	MailReceiveFailExpired = 6051,


	////////////////////////////////////////////////////////////////
	// User Item 7000 ~

	UserItemGetNotFound = 7000,
	UserItemGetBadRequest = 7001,
	UserItemGetFail = 7002,
	UserItemGetException = 7003,

}