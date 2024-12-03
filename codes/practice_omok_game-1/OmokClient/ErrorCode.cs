namespace OmokClient;

public enum ErrorCode : UInt16
{
    None = 0,
    InvalidCredentials = 1,
    UserNotFound = 2,
    ServerError = 3,
    InternalServerError = 4,
    RequestTurnTimeout = 2505,
    TurnChangedByTimeout = 2510,

    ReqFriendFailPlayerNotExist = 2101,
    FriendRequestAlreadyPending = 2102,
    ReverseFriendRequestPending = 2103,
    AlreadyFriends = 2104,
    FriendRequestNotFound = 2105,

    FailToDeleteMailItemNotReceived = 8021,
    AttendanceCheckFailAlreadyChecked = 9002,

    RequestFailed = 10000,
}
