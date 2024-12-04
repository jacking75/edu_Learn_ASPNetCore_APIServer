using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using APIServer.ModelDB;

namespace APIServer.Services;

public interface IGameDb : IDisposable
{
    #region Insert

    public Task<Int64> CreateUserPlayDataAndGetId(Int16 level, Int32 exp);

    public Task<bool> CreateUserAttendanceBook(Int64 userId);

    public Task<bool> AddUserItem(Int64 userId, ItemInfo itemInfo, Int32 itemCount);

    public Task<Int64> AddUserItemAndGetId(Int64 userId, ItemInfo itemInfo, Int32 itemCount);

    public Task<bool> AddUserMail(Int64 userId, UserMail mailInfo);

    public Task<Int64> AddUserMailAndGetId(Int64 userId, UserMail mailInfo);

    public Task<bool> AddPaidInAppHistory(Int64 userId, Int32 pid, string receipt);

    public Task<bool> AddCompleteStageHistory(Int64 userId, Int32 stageCode);

    #endregion


    #region Select

    public Task<(ErrorCode, UserPlayData)> GetUserPlayData(Int64 userId);

    public Task<(ErrorCode, UserInventoryItem)> GetUserInventoryItem(Int64 inventoryItemId);

    public Task<(ErrorCode, List<UserInventoryItem>)> GetUserInventoryItemList(Int64 userId);

    public Task<(ErrorCode, UserMail)> GetUserMail(Int64 mailId);

    public Task<(ErrorCode, UserAttendanceBook)> GetUserAttendanceBook(Int64 userId);

    public Task<(ErrorCode, UserAttendanceBook)> GetUserAttendanceBookNotToday(Int64 userId, DateTime today);

    public Task<Int32> GetUserMailCount(Int64 userId);

    public Task<(ErrorCode, List<UserMail>)> GetUserMailList(Int64 userId, Int32 page, Int32 perPage);

    public Task<Int32> GetInventoryItemCount(Int64 userId, Int64 itemCode);

    public Task<(ErrorCode, List<Int32>)> GetCompletedStageList(Int64 userId);

    public Task<(ErrorCode, DateTime)> GetStageCompleteDate(Int64 userId, Int32 stageCode);

    public Task<(ErrorCode, bool)> IsCompleteStage(Int64 userId, Int32 stageCode);

    public Task<(ErrorCode, bool)> IsPaidInAppReceipt(string receipt);

    #endregion


    #region Delete

    public Task<ErrorCode> RemoveAllUserData(Int64 userId);

    public Task<ErrorCode> RemoveUserInventoryItem(Int64 inventoryItemId);

    public Task<ErrorCode> RemoveUserMailList(Int64 userId, List<Int64> mailIdList);

    public Task<ErrorCode> RemovePaidInAppHistory(string receipt);

    public Task<ErrorCode> RemoveCompletedStageHistory(Int64 userId, Int32 stageCode);

    #endregion



    #region Update

    public Task<ErrorCode> ResetUserAttendanceBook(Int64 userId);

    public Task<ErrorCode> UpdateUserAttendanceDay(Int64 userId, Int16 attendanceDay);

    public Task<ErrorCode> UpdateUserAttendanceDayAndDate(Int64 userId, Int16 attendanceDay, DateTime lastUpdateDate);

    public Task<ErrorCode> UpdateMailStatusAsReceive(Int64 mailId);

    public Task<ErrorCode> UpdateMailStatusAsNotReceive(Int64 mailId);

    public Task<Int64> UpdateInventoryItemCountAndGetId(Int64 userId, Int32 itemCode, Int32 itemCount);

    public Task<ErrorCode> UpdateEnhanceItemAttackPower(Int64 inventoryItemId, Int32 enhanceStage, Int64 attackPower);

    public Task<ErrorCode> UpdateEnhanceItemDefensivePower(Int64 inventoryItemId, Int32 enhanceStage, Int64 defensivePower);

    public Task<ErrorCode> UpdateUserExp(Int64 userId, Int32 exp);

    public Task<ErrorCode> UpdateCompletedStageHistory(Int64 userId, Int32 stageCode, DateTime completedDate);

    #endregion
}