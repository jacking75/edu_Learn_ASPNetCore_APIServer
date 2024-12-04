
using System;
using System.Collections.Generic;

using Microsoft.Extensions.Logging;

using ZLogger;

public static class LogManager
{
    public enum EventType
    {
        // GameDb (1000 ~ 1999)
        CreateUserPlayDataAndGetId = 1001,
        CreateUserAttendanceBook = 1002,
        AddUserItem = 1003,
        AddUserItemAndGetId = 1004,
        AddUserMail = 1005,
        AddUserMailAndGetId = 1006,
        AddPaidInAppHistory = 1007,
        AddCompleteStageHistory = 1008,
        GetUserPlayData = 1101,
        GetUserInventoryItem = 1102,
        GetUserInventoryItemList = 1103,
        GetUserMail = 1104,
        GetUserAttendanceBook = 1105,
        GetUserAttendanceBookNotToday = 1106,
        GetUserMailCount = 1107,
        GetUserMailList = 1108,
        GetInventoryItemCount = 1109,
        GetCompletedStageList = 1110,
        GetStageCompleteDate = 1111,
        IsCompleteStage = 1112,
        IsPaidInAppReceipt = 1113,
        RemoveAllUserData = 1201,
        RemoveUserInventoryItem = 1202,
        RemoveUserMailList = 1203,
        RemovePaidInAppHistory = 1204,
        RemoveCompletedStageHistory = 1205,
        ResetUserAttendanceBook = 1301,
        UpdateUserAttendanceDay = 1302,
        UpdateUserAttendanceDayAndDate = 1303,
        UpdateMailStatusAsReceive = 1304,
        UpdateMailStatusAsNotReceive = 1305,
        UpdateInventoryItemCountAndGetId = 1306,
        UpdateEnhanceItemAttackPower = 1307,
        UpdateEnhanceItemDefensivePower = 1308,
        UpdateUserExp = 1309,
        UpdateCompletedStageHistory = 1310,


        // AccountDb


        // MemoryDb (3000 ~ 3999)
        LoadGameNotice = 3001,
        CheckCertifiedUser = 3002,
        RegistCertifiedUserInfo = 3101,
        UpdateCertifiedUserInfo = 3102,
        RegistUserBattleInfo = 3103,
        UpdateUserBattleInfo = 3104,
        TryLockUserRequest = 3105,
        AddChannelChatInfo = 3106,
        GetCertifiedUser = 3207,
        GetUserBattleInfo = 3208,
        GetChannelChatInfoList = 3209,
        UnlockUserRequest = 3210,
        RemoveUserBattleInfo = 3301,


        // Controllers (4000 ~ 4999)
        CreateAccount = 4001,
        Login = 4002,
        EnhanceItem = 4003,
    }


    public static Dictionary<EventType, EventId> EventIdDic = new()
    {
        { EventType.CreateUserPlayDataAndGetId, new EventId((Int32)EventType.CreateUserPlayDataAndGetId, nameof(EventType.CreateUserPlayDataAndGetId)) },
        { EventType.CreateUserAttendanceBook, new EventId((Int32)EventType.CreateUserAttendanceBook, nameof(EventType.CreateUserAttendanceBook)) },
        { EventType.AddUserItem, new EventId((Int32)EventType.AddUserItem, nameof(EventType.AddUserItem)) },
        { EventType.AddUserItemAndGetId, new EventId((Int32)EventType.AddUserItemAndGetId, nameof(EventType.AddUserItemAndGetId)) },
        { EventType.AddUserMail, new EventId((Int32)EventType.AddUserMail, nameof(EventType.AddUserMail)) },
        { EventType.AddUserMailAndGetId, new EventId((Int32)EventType.AddUserMailAndGetId, nameof(EventType.AddUserMailAndGetId)) },
        { EventType.AddPaidInAppHistory, new EventId((Int32)EventType.AddPaidInAppHistory, nameof(EventType.AddPaidInAppHistory)) },
        { EventType.AddCompleteStageHistory, new EventId((Int32)EventType.AddCompleteStageHistory, nameof(EventType.AddCompleteStageHistory)) },
        { EventType.GetUserPlayData, new EventId((Int32)EventType.GetUserPlayData, nameof(EventType.GetUserPlayData)) },
        { EventType.GetUserInventoryItem, new EventId((Int32)EventType.GetUserInventoryItem, nameof(EventType.GetUserInventoryItem)) },
        { EventType.GetUserInventoryItemList, new EventId((Int32)EventType.GetUserInventoryItemList, nameof(EventType.GetUserInventoryItemList)) },
        { EventType.GetUserMail, new EventId((Int32)EventType.GetUserMail, nameof(EventType.GetUserMail)) },
        { EventType.GetUserAttendanceBook, new EventId((Int32)EventType.GetUserAttendanceBook, nameof(EventType.GetUserAttendanceBook)) },
        { EventType.GetUserAttendanceBookNotToday, new EventId((Int32)EventType.GetUserAttendanceBookNotToday, nameof(EventType.GetUserAttendanceBookNotToday)) },
        { EventType.GetUserMailCount, new EventId((Int32)EventType.GetUserMailCount, nameof(EventType.GetUserMailCount)) },
        { EventType.GetUserMailList, new EventId((Int32)EventType.GetUserMailList, nameof(EventType.GetUserMailList)) },
        { EventType.GetInventoryItemCount, new EventId((Int32)EventType.GetInventoryItemCount, nameof(EventType.GetInventoryItemCount)) },
        { EventType.GetCompletedStageList, new EventId((Int32)EventType.GetCompletedStageList, nameof(EventType.GetCompletedStageList)) },
        { EventType.GetStageCompleteDate, new EventId((Int32)EventType.GetStageCompleteDate, nameof(EventType.GetStageCompleteDate)) },
        { EventType.IsCompleteStage, new EventId((Int32)EventType.IsCompleteStage, nameof(EventType.IsCompleteStage)) },
        { EventType.IsPaidInAppReceipt, new EventId((Int32)EventType.IsPaidInAppReceipt, nameof(EventType.IsPaidInAppReceipt)) },
        { EventType.RemoveAllUserData, new EventId((Int32)EventType.RemoveAllUserData, nameof(EventType.RemoveAllUserData)) },
        { EventType.RemoveUserInventoryItem, new EventId((Int32)EventType.RemoveUserInventoryItem, nameof(EventType.RemoveUserInventoryItem)) },
        { EventType.RemoveUserMailList, new EventId((Int32)EventType.RemoveUserMailList, nameof(EventType.RemoveUserMailList)) },
        { EventType.RemovePaidInAppHistory, new EventId((Int32)EventType.RemovePaidInAppHistory, nameof(EventType.RemovePaidInAppHistory)) },
        { EventType.RemoveCompletedStageHistory, new EventId((Int32)EventType.RemoveCompletedStageHistory, nameof(EventType.RemoveCompletedStageHistory)) },
        { EventType.ResetUserAttendanceBook, new EventId((Int32) EventType.ResetUserAttendanceBook, nameof(EventType.ResetUserAttendanceBook)) },
        { EventType.UpdateUserAttendanceDay, new EventId((Int32)EventType.UpdateUserAttendanceDay, nameof(EventType.UpdateUserAttendanceDay)) },
        { EventType.UpdateUserAttendanceDayAndDate, new EventId((Int32)EventType.UpdateUserAttendanceDayAndDate, nameof(EventType.UpdateUserAttendanceDayAndDate)) },
        { EventType.UpdateMailStatusAsReceive, new EventId((Int32)EventType.UpdateMailStatusAsReceive, nameof(EventType.UpdateMailStatusAsReceive)) },
        { EventType.UpdateMailStatusAsNotReceive, new EventId((Int32)EventType.UpdateMailStatusAsNotReceive, nameof(EventType.UpdateMailStatusAsNotReceive)) },
        { EventType.UpdateInventoryItemCountAndGetId, new EventId((Int32)EventType.UpdateInventoryItemCountAndGetId, nameof(EventType.UpdateInventoryItemCountAndGetId)) },
        { EventType.UpdateEnhanceItemAttackPower, new EventId((Int32)EventType.UpdateEnhanceItemAttackPower, nameof(EventType.UpdateEnhanceItemAttackPower)) },
        { EventType.UpdateEnhanceItemDefensivePower, new EventId((Int32)EventType.UpdateEnhanceItemDefensivePower, nameof(EventType.UpdateEnhanceItemDefensivePower)) },
        { EventType.UpdateUserExp, new EventId((Int32)EventType.UpdateUserExp, nameof(EventType.UpdateUserExp)) },
        { EventType.UpdateCompletedStageHistory, new EventId((Int32)EventType.UpdateCompletedStageHistory, nameof(EventType.UpdateCompletedStageHistory)) },

        { EventType.LoadGameNotice, new EventId((Int32) EventType.LoadGameNotice, nameof(EventType.LoadGameNotice)) },
        { EventType.CheckCertifiedUser, new EventId((Int32)EventType.CheckCertifiedUser, nameof(EventType.CheckCertifiedUser)) },
        { EventType.RegistCertifiedUserInfo, new EventId((Int32)EventType.RegistCertifiedUserInfo, nameof(EventType.RegistCertifiedUserInfo)) },
        { EventType.UpdateCertifiedUserInfo, new EventId((Int32)EventType.UpdateCertifiedUserInfo, nameof(EventType.UpdateCertifiedUserInfo)) },
        { EventType.TryLockUserRequest, new EventId((Int32)EventType.TryLockUserRequest, nameof(EventType.TryLockUserRequest)) },
        { EventType.AddChannelChatInfo, new EventId((Int32)EventType.AddChannelChatInfo, nameof(EventType.AddChannelChatInfo)) },
        { EventType.GetCertifiedUser, new EventId((Int32)EventType.GetCertifiedUser, nameof(EventType.GetCertifiedUser)) },
        { EventType.GetUserBattleInfo, new EventId((Int32)EventType.GetUserBattleInfo, nameof(EventType.GetUserBattleInfo)) },
        { EventType.GetChannelChatInfoList, new EventId((Int32)EventType.GetChannelChatInfoList, nameof(EventType.GetChannelChatInfoList)) },
        { EventType.UnlockUserRequest, new EventId((Int32)EventType.UnlockUserRequest, nameof(EventType.UnlockUserRequest)) },
        { EventType.RemoveUserBattleInfo, new EventId((Int32)EventType.RemoveUserBattleInfo, nameof(EventType.RemoveUserBattleInfo)) },

        { EventType.CreateAccount, new EventId((Int32)EventType.CreateAccount, nameof(EventType.CreateAccount)) },
        { EventType.Login, new EventId((Int32)EventType.Login, nameof(EventType.Login)) },
        { EventType.EnhanceItem, new EventId((Int32)EventType.EnhanceItem, nameof(EventType.EnhanceItem)) },
    };


    public static ILogger Logger { get; private set; }


    private static ILoggerFactory s_loggerFactory;


    public static void SetLoggerFactory(ILoggerFactory loggerFactory, string categoryName)
    {
        s_loggerFactory = loggerFactory;
        Logger = loggerFactory.CreateLogger(categoryName);
    }


    public static ILogger<T> GetLogger<T>() where T : class
    {
        return s_loggerFactory.CreateLogger<T>();
    }


    public static void LoggingForInformation(ILogger logger, EventType eventType, string message, object payload = null)
    {
        logger.ZLogInformationWithPayload(EventIdDic[eventType], payload, message);
    }


    public static void LoggingForError(ILogger logger, EventType eventType, string message, object payload = null)
    {
        logger.ZLogErrorWithPayload(EventIdDic[eventType], payload, message);
    }



}