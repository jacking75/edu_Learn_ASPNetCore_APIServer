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
    BuyError = 4001,

    MailInfoFailException = 5000,
    MailLoadFail = 5001,

    ItemInvalidRequest = 6000,
    ItemInfoFailException = 6001,
    ItemLoadFail = 6002,
    ItemRegistFail = 6003,

    DeckSaveFail = 6004,
    DeckSetMainFail = 6005,
    DeckLoadFail = 6006,
    DeckNotFound = 6007,
    DeckInvalidFormat = 6008,

    CurrencyInvalidRequest = 7000,
    CurrencyLoadFail = 7001,
    CurrencyRegistFail = 7002,

    CharactorLoadFail = 8000,

    MatchRequestFailException = 9001,
    MatchStatusCheckFailException = 9002,
    MatchCancelFailException = 9003,
    MatchingWaiting = 9004,
    MatchAddUserFailAboutDeck = 9005,

    CardInitFail = 9100,
    CardReplaceFail = 9101,
    NotPlayersTurn = 9102,
    NoCardsInDeck = 9103,
    CardDrawFail = 9104,
    PlayerStateNotFound = 9105,
    CardNotInHand = 9106,
    NotEnoughMana = 9107,
    CardPlayFail = 9108,
    CardNotOnField = 9109,
    OpponentNotFound = 9110,
    TargetCardNotFound = 9111,
    AttackFail = 9112,
    EndTurnFail = 9113,
    FieldCardListFull = 9114,
    PlayerNotFound=9115,
    HandCardListFull=9116,
    CardDrawLimitReached=9117,
    FieldCardsFull = 9118,
    HandCardsFull = 9119,
    CardAlreadyAttacked=9120,


    MAX,
}
