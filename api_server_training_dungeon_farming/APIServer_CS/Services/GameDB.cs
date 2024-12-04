using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

using APIServer.ModelDB;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using MySqlConnector;

using SqlKata.Execution;

using static LogManager;

namespace APIServer.Services;

public class GameDb : IGameDb
{
    private readonly ILogger<GameDb> _logger;

    private readonly IOptions<DbConfig> _dbConfig;

    private readonly SqlKata.Compilers.MySqlCompiler _compiler;

    private readonly QueryFactory _queryFactory;

    private IDbConnection _dbConn;

    public GameDb(ILogger<GameDb> logger, IOptions<DbConfig> dbConfig)
    {
        _dbConfig = dbConfig;
        _logger = logger;

        Open();

        _compiler = new SqlKata.Compilers.MySqlCompiler();
        _queryFactory = new SqlKata.Execution.QueryFactory(_dbConn, _compiler);
    }

    public void Dispose()
    {
        Close();
    }

    #region Insert

    public async Task<Int64> CreateUserPlayDataAndGetId(Int16 level, Int32 exp)
    {
        try
        {
            var insertedId = await _queryFactory
                .Query("GameDB.user_play_data")
                .InsertGetIdAsync<Int64>(new
                {
                    level = level,
                    exp = exp
                });

            LoggingForInformation(_logger, EventType.CreateUserPlayDataAndGetId, "Success insert from GameDB.user_play_data", new { Level = level, Exp = exp });
            return insertedId;
        }
        catch (Exception ex)
        {
            LoggingForError(_logger, EventType.CreateUserPlayDataAndGetId, ex.Message, new { Level = level, Exp = exp });
            return 0;
        }
    }


    public async Task<bool> CreateUserAttendanceBook(Int64 userId)
    {
        var lastUpdateDate = DateTime.MinValue;

        try
        {
            _ = await _queryFactory
                .Query("GameDB.user_attendance_book")
                .InsertAsync(new
                {
                    user_id = userId,
                    last_update_date = DateTime.MinValue
                });

            LoggingForInformation(_logger, EventType.CreateUserAttendanceBook, "Success insert from GameDB.user_attendance_book", new { UserId = userId });
            return true;
        }
        catch (Exception ex)
        {
            LoggingForError(_logger, EventType.CreateUserAttendanceBook, ex.Message, new { UserId = userId, LastUpdateDate = lastUpdateDate });
            return false;
        }
    }

    public async Task<bool> AddUserItem(Int64 userId, ItemInfo itemInfo, Int32 itemCount)
    {
        try
        {
            _ = await _queryFactory
                .Query("GameDB.user_inventory_item")
                .InsertAsync(new
                {
                    user_id = userId,
                    item_code = itemInfo.item_code,
                    item_attack_power = itemInfo.item_attack_power,
                    item_defensive_power = itemInfo.item_defensive_power,
                    item_magic = itemInfo.item_magic,
                    item_count = itemCount
                });

            LoggingForInformation(_logger, EventType.AddUserItem, "Success insert from GameDB.user_inventory_item",
                new { UserId = userId, ItemCode = itemInfo.item_code, ItemCount = itemCount });

            return true;
        }
        catch (Exception ex)
        {
            LoggingForError(_logger, EventType.AddUserItem, ex.Message, new { UserId = userId, ItemCode = itemInfo.item_code, ItemCount = itemCount });
            return false;
        }
    }

    public async Task<Int64> AddUserItemAndGetId(Int64 userId, ItemInfo itemInfo, Int32 itemCount)
    {
        try
        {
            var insertedId = await _queryFactory
                .Query("GameDB.user_inventory_item")
                .InsertGetIdAsync<Int64>(new
                {
                    user_id = userId,
                    item_code = itemInfo.item_code,
                    item_attack_power = itemInfo.item_attack_power,
                    item_defensive_power = itemInfo.item_defensive_power,
                    item_magic = itemInfo.item_magic,
                    item_count = itemCount
                });

            LoggingForInformation(_logger, EventType.AddUserItemAndGetId, "Success insert from GameDB.user_inventory_item",
                new { InsertedId = insertedId, UserId = userId, ItemCode = itemInfo.item_code, ItemCount = itemCount });

            return insertedId;
        }
        catch (Exception ex)
        {
            LoggingForError(_logger, EventType.AddUserItemAndGetId, ex.Message, new { UserId = userId, ItemCode = itemInfo.item_code, ItemCount = itemCount });
            return 0;
        }
    }


    public async Task<bool> AddUserMail(Int64 userId, UserMail mailInfo)
    {
        try
        {
            _ = await _queryFactory
                .Query("GameDB.user_mailbox")
                .InsertAsync(new
                {
                    user_id = userId,
                    mail_type = mailInfo.mail_type,
                    mail_title = mailInfo.mail_title,
                    item_code = mailInfo.item_code,
                    item_count = mailInfo.item_count,
                    expire_date = mailInfo.expire_date,
                });

            LoggingForInformation(_logger, EventType.AddUserMail, "Success insert from GameDB.user_mailbox",
                new { UserId = userId, MailType = mailInfo.mail_type, MailTitle = mailInfo.mail_title });

            return true;
        }
        catch (Exception ex)
        {
            LoggingForError(_logger, EventType.AddUserMail, ex.Message, new { UserId = userId, MailType = mailInfo.mail_type, MailTitle = mailInfo.mail_title });
            return false;
        }
    }


    public async Task<Int64> AddUserMailAndGetId(Int64 userId, UserMail mailInfo)
    {
        try
        {
            var insertedId = await _queryFactory
                .Query("GameDB.user_mailbox")
                .InsertGetIdAsync<Int64>(new
                {
                    user_id = userId,
                    mail_type = mailInfo.mail_type,
                    mail_title = mailInfo.mail_title,
                    item_code = mailInfo.item_code,
                    item_count = mailInfo.item_count,
                    expire_date = mailInfo.expire_date,
                });

            LoggingForInformation(_logger, EventType.AddUserMailAndGetId, "Success insert from GameDB.user_mailbox",
                new { InsertedId = insertedId, UserId = userId, MailType = mailInfo.mail_type, MailTitle = mailInfo.mail_title });

            return insertedId;

        }
        catch (Exception ex)
        {
            LoggingForError(_logger, EventType.AddUserMailAndGetId, ex.Message, new { UserId = userId, MailType = mailInfo.mail_type, MailTitle = mailInfo.mail_title });
            return 0;
        }
    }


    public async Task<bool> AddPaidInAppHistory(Int64 userId, Int32 pid, string receipt)
    {
        try
        {
            _ = await _queryFactory
                .Query("GameDB.user_inapp_purchase_history")
                .InsertAsync(new
                {
                    user_id = userId,
                    pid = pid,
                    receipt = receipt
                });

            LoggingForInformation(_logger, EventType.AddPaidInAppHistory, "Success insert from GameDB.user_inapp_purchase_history", new { UserId = userId, PID = pid, Receipt = receipt });
            return true;
        }
        catch (Exception ex)
        {
            LoggingForError(_logger, EventType.AddPaidInAppHistory, ex.Message, new { UserId = userId, PID = pid, Receipt = receipt });
            return false;
        }
    }


    public async Task<bool> AddCompleteStageHistory(Int64 userId, Int32 stageCode)
    {
        try
        {
            _ = await _queryFactory
                .Query("GameDB.user_cleared_dungeon_stage")
                .InsertAsync(new
                {
                    user_id = userId,
                    stage_code = stageCode
                });

            LoggingForInformation(_logger, EventType.AddCompleteStageHistory, "Success insert from GameDB.user_cleared_dungeon_stage", new { UserId = userId, StageCode = stageCode });
            return true;
        }
        catch (Exception ex)
        {
            LoggingForError(_logger, EventType.AddCompleteStageHistory, ex.Message, new { UserId = userId, StageCode = stageCode });
            return false;
        }
    }

    #endregion

    #region Select

    public async Task<(ErrorCode, UserPlayData)> GetUserPlayData(Int64 userId)
    {
        try
        {
            var loadedData = await _queryFactory
                .Query("GameDB.user_play_data")
                .Where("user_id", userId)
                .FirstOrDefaultAsync<UserPlayData>();

            return (ErrorCode.None, loadedData);
        }
        catch (Exception ex)
        {
            LoggingForError(_logger, EventType.GetUserPlayData, ex.Message, new { UserId = userId });
            return (ErrorCode.GameDbException, null);
        }
    }


    public async Task<(ErrorCode, UserInventoryItem)> GetUserInventoryItem(Int64 inventoryItemId)
    {
        try
        {
            var loadedData = await _queryFactory
                .Query("GameDB.user_inventory_item")
                .Where("inventory_item_id", inventoryItemId)
                .FirstOrDefaultAsync<UserInventoryItem>();

            return (ErrorCode.None, loadedData);
        }
        catch (Exception ex)
        {
            LoggingForError(_logger, EventType.GetUserInventoryItem, ex.Message, new { InventoryItemId = inventoryItemId });
            return (ErrorCode.GameDbException, null);
        }

    }


    public async Task<(ErrorCode, List<UserInventoryItem>)> GetUserInventoryItemList(Int64 userId)
    {
        try
        {
            var loadedData = await _queryFactory
                .Query("GameDB.user_inventory_item")
                .Where("user_id", userId)
                .GetAsync<UserInventoryItem>();

            return (ErrorCode.None, loadedData.ToList());
        }
        catch (Exception ex)
        {
            LoggingForError(_logger, EventType.GetUserInventoryItemList, ex.Message, new { UserId = userId });
            return (ErrorCode.GameDbException, null);
        }
    }


    public async Task<(ErrorCode, UserMail)> GetUserMail(Int64 mailId)
    {
        try
        {
            var loadedData = await _queryFactory
                .Query("GameDB.user_mailbox")
                .Where("mail_id", mailId)
                .WhereFalse("is_receive")
                .FirstOrDefaultAsync<UserMail>();

            return (ErrorCode.None, loadedData);
        }
        catch (Exception ex)
        {
            LoggingForError(_logger, EventType.GetUserMail, ex.Message, new { MailId = mailId });
            return (ErrorCode.GameDbException, null);
        }

    }


    public async Task<(ErrorCode, UserAttendanceBook)> GetUserAttendanceBook(Int64 userId)
    {
        try
        {
            var loadedData = await _queryFactory
                .Query("GameDB.user_attendance_book")
                .Where("user_id", userId)
                .FirstOrDefaultAsync<UserAttendanceBook>();

            return (ErrorCode.None, loadedData);
        }
        catch (Exception ex)
        {
            LoggingForError(_logger, EventType.GetUserAttendanceBook, ex.Message, new { UserId = userId });
            return (ErrorCode.GameDbException, null);
        }

    }


    public async Task<(ErrorCode, UserAttendanceBook)> GetUserAttendanceBookNotToday(Int64 userId, DateTime today)
    {
        try
        {
            var loadedData = await _queryFactory
                .Query("GameDB.user_attendance_book")
                .Where("user_id", userId)
                .WhereNotDate("last_update_date", today)
                .FirstOrDefaultAsync<UserAttendanceBook>();

            return (ErrorCode.None, loadedData);
        }
        catch (Exception ex)
        {
            LoggingForError(_logger, EventType.GetUserAttendanceBookNotToday, ex.Message, new { UserId = userId, Today = today });
            return (ErrorCode.GameDbException, null);
        }

    }

    public async Task<Int32> GetUserMailCount(Int64 userId)
    {
        try
        {
            return await _queryFactory
                .Query("GameDB.user_mailbox")
                .Where("user_id", userId)
                .CountAsync<Int32>();
        }
        catch (Exception ex)
        {
            LoggingForError(_logger, EventType.GetUserMailCount, ex.Message, new { UserId = userId });
            return -1;
        }
    }

    public async Task<(ErrorCode, List<UserMail>)> GetUserMailList(Int64 userId, Int32 page, Int32 perPage)
    {
        try
        {
            var loadedData = await _queryFactory
                .Query("GameDB.user_mailbox")
                .Where("user_id", userId)
                .OrderByDesc("send_date")
                .ForPage(page, perPage)
                .GetAsync<UserMail>();

            return (ErrorCode.None, loadedData.ToList());
        }
        catch (Exception ex)
        {
            LoggingForError(_logger, EventType.GetUserMailList, ex.Message, new { UserId = userId, Page = page, PerPage = perPage });
            return (ErrorCode.GameDbException, null);
        }
    }

    public async Task<Int32> GetInventoryItemCount(Int64 userId, Int64 itemCode)
    {
        try
        {
            return await _queryFactory
                .Query("GameDB.user_inventory_item")
                .Select("item_count")
                .Where("user_id", userId)
                .Where("item_code", itemCode)
                .FirstOrDefaultAsync<Int32>();
        }
        catch (Exception ex)
        {
            LoggingForError(_logger, EventType.GetInventoryItemCount, ex.Message, new { UserId = userId, ItemCode = itemCode });
            return 0;
        }
    }

    public async Task<(ErrorCode, List<Int32>)> GetCompletedStageList(Int64 userId)
    {
        try
        {
            var loadedData = await _queryFactory
                .Query("GameDB.user_cleared_dungeon_stage")
                .Select("stage_code")
                .Where("user_id", userId)
                .GetAsync<Int32>();

            return (ErrorCode.None, loadedData.ToList());
        }
        catch (Exception ex)
        {
            LoggingForError(_logger, EventType.GetCompletedStageList, ex.Message, new { UserId = userId });
            return (ErrorCode.GameDbException, null);
        }
    }

    public async Task<(ErrorCode, DateTime)> GetStageCompleteDate(Int64 userId, Int32 stageCode)
    {
        try
        {
            var completedDate = await _queryFactory
                .Query("GameDB.user_cleared_dungeon_stage")
                .Select("cleared_date")
                .Where("user_id", userId)
                .Where("stage_code", stageCode)
                .FirstOrDefaultAsync<DateTime>();

            return (ErrorCode.None, completedDate);
        }
        catch (Exception ex)
        {
            LoggingForError(_logger, EventType.GetStageCompleteDate, ex.Message, new { UserId = userId, StageCode = stageCode });
            return (ErrorCode.GameDbException, DateTime.MinValue);
        }
    }

    public async Task<(ErrorCode, bool)> IsCompleteStage(Int64 userId, Int32 stageCode)
    {
        try
        {
            var exist = await _queryFactory
                .Query("GameDB.user_cleared_dungeon_stage")
                .Where("user_id", userId)
                .Where("stage_code", stageCode)
                .ExistsAsync();

            return (ErrorCode.None, exist);
        }
        catch (Exception ex)
        {
            LoggingForError(_logger, EventType.IsCompleteStage, ex.Message, new { UserId = userId, StageCode = stageCode });
            return (ErrorCode.GameDbException, false);
        }
    }

    public async Task<(ErrorCode, bool)> IsPaidInAppReceipt(string receipt)
    {
        try
        {
            var exist = await _queryFactory
                .Query("GameDB.user_inapp_purchase_history")
                .Where("receipt", receipt)
                .ExistsAsync();

            return (ErrorCode.None, exist);
        }
        catch (Exception ex)
        {
            LoggingForError(_logger, EventType.IsPaidInAppReceipt, ex.Message, new { Receipt = receipt });
            return (ErrorCode.GameDbException, true);
        }
    }

    #endregion

    #region Delete Services

    public async Task<ErrorCode> RemoveAllUserData(Int64 userId)
    {
        try
        {
            await _queryFactory.Query("GameDB.user_attendance_book").Where("user_id", userId).DeleteAsync();
            await _queryFactory.Query("GameDB.user_mailbox").Where("user_id", userId).DeleteAsync();
            await _queryFactory.Query("GameDB.user_inventory_item").Where("user_id", userId).DeleteAsync();
            await _queryFactory.Query("GameDB.user_play_data").Where("user_id", userId).DeleteAsync();

            return ErrorCode.None;
        }
        catch (Exception ex)
        {
            LoggingForError(_logger, EventType.RemoveAllUserData, ex.Message, new { UserId = userId });
            return ErrorCode.GameDbException;
        }
    }

    public async Task<ErrorCode> RemoveUserInventoryItem(Int64 inventoryItemId)
    {
        try
        {
            _ = await _queryFactory
                .Query("GameDB.user_inventory_item")
                .Where("inventory_item_id", inventoryItemId)
                .DeleteAsync();

            return ErrorCode.None;
        }
        catch (Exception ex)
        {
            LoggingForError(_logger, EventType.RemoveUserInventoryItem, ex.Message, new { InventoryItemId = inventoryItemId });
            return ErrorCode.GameDbException;
        }
    }



    public async Task<ErrorCode> RemoveUserMailList(Int64 userId, List<Int64> mailIdList)
    {
        try
        {
            _ = await _queryFactory
                .Query("GameDB.user_mailbox")
                .Where("user_id", userId)
                .WhereIn("mail_id", mailIdList)
                .DeleteAsync();

            return ErrorCode.None;
        }
        catch (Exception ex)
        {
            LoggingForError(_logger, EventType.RemoveUserMailList, ex.Message, new { UserId = userId, MailCount = mailIdList.Count });
            return ErrorCode.GameDbException;
        }
    }


    public async Task<ErrorCode> RemovePaidInAppHistory(string receipt)
    {
        try
        {
            _ = await _queryFactory
                .Query("GameDB.user_inapp_purchase_history")
                .Where("receipt", receipt)
                .DeleteAsync();

            return ErrorCode.None;
        }
        catch (Exception ex)
        {
            LoggingForError(_logger, EventType.RemovePaidInAppHistory, ex.Message, new { Receipt = receipt });
            return ErrorCode.GameDbException;
        }
    }


    public async Task<ErrorCode> RemoveCompletedStageHistory(Int64 userId, Int32 stageCode)
    {
        try
        {
            _ = await _queryFactory
                .Query("GameDB.user_cleared_dungeon_stage")
                .Where("user_id", userId)
                .Where("stage_code", stageCode)
                .DeleteAsync();

            return ErrorCode.None;
        }
        catch (Exception ex)
        {
            LoggingForError(_logger, EventType.RemoveCompletedStageHistory, ex.Message, new { UserId = userId, StageCode = stageCode });
            return ErrorCode.GameDbException;
        }
    }

    #endregion

    #region Update Services

    public async Task<ErrorCode> ResetUserAttendanceBook(Int64 userId)
    {
        try
        {
            _ = await _queryFactory
                .Query("GameDB.user_attendance_book")
                .Where("user_id", userId)
                .UpdateAsync(new
                {
                    last_attendance_day = 1,
                    start_update_date = DateTime.Now,
                    last_update_date = DateTime.Now,
                });

            return ErrorCode.None;
        }
        catch (Exception ex)
        {
            LoggingForError(_logger, EventType.ResetUserAttendanceBook, ex.Message, new { UserId = userId });
            return ErrorCode.GameDbException;
        }
    }


    public async Task<ErrorCode> UpdateUserAttendanceDay(Int64 userId, Int16 attendanceDay)
    {
        try
        {
            _ = await _queryFactory
                .Query("GameDB.user_attendance_book")
                .Where("user_id", userId)
                .UpdateAsync(new
                {
                    last_attendance_day = attendanceDay,
                    last_update_date = DateTime.Now,
                });

            return ErrorCode.None;
        }

        catch (Exception ex)
        {
            LoggingForError(_logger, EventType.UpdateUserAttendanceDay, ex.Message, new { UserId = userId, AttendanceDay = attendanceDay });
            return ErrorCode.GameDbException;
        }

    }

    public async Task<ErrorCode> UpdateUserAttendanceDayAndDate(Int64 userId, Int16 attendanceDay, DateTime lastUpdateDate)
    {
        try
        {
            _ = await _queryFactory
                .Query("GameDB.user_attendance_book")
                .Where("user_id", userId)
                .UpdateAsync(new
                {
                    last_attendance_day = attendanceDay,
                    last_update_date = lastUpdateDate,
                });

            return ErrorCode.None;
        }
        catch (Exception ex)
        {
            LoggingForError(_logger, EventType.UpdateUserAttendanceDayAndDate, ex.Message, new { UserId = userId, AttendanceDay = attendanceDay, LastUpdateDate = lastUpdateDate });
            return ErrorCode.GameDbException;
        }

    }

    public async Task<ErrorCode> UpdateMailStatusAsReceive(Int64 mailId)
    {
        try
        {
            _ = await _queryFactory
                .Query("GameDB.user_mailbox")
                .Where("mail_id", mailId)
                .WhereFalse("is_receive")
                .UpdateAsync(new
                {
                    is_receive = true,
                    receive_date = DateTime.Now
                });

            return ErrorCode.None;

        }
        catch (Exception ex)
        {
            LoggingForError(_logger, EventType.UpdateMailStatusAsReceive, ex.Message, new { MailId = mailId });
            return ErrorCode.GameDbException;
        }

    }

    public async Task<ErrorCode> UpdateMailStatusAsNotReceive(Int64 mailId)
    {
        try
        {
            _ = await _queryFactory
                .Query("GameDB.user_mailbox")
                .Where("mail_id", mailId)
                .WhereTrue("is_receive")
                .UpdateAsync(new
                {
                    is_receive = false,
                    receive_date = DateTime.MinValue
                });

            return ErrorCode.None;
        }
        catch (Exception ex)
        {
            LoggingForError(_logger, EventType.UpdateMailStatusAsNotReceive, ex.Message, new { MailId = mailId });
            return ErrorCode.GameDbException;
        }
    }

    public async Task<Int64> UpdateInventoryItemCountAndGetId(Int64 userId, Int32 itemCode, Int32 itemCount)
    {
        try
        {
            var inventoryItemId = await _queryFactory
                .Query("GameDB.user_inventory_item")
                .Select("inventory_item_id")
                .Where("user_id", userId)
                .Where("item_code", itemCode)
                .FirstOrDefaultAsync<Int64>();

            if (inventoryItemId == 0)
            {
                return 0;
            }

            _ = await _queryFactory
                .Query("GameDB.user_inventory_item")
                .Where("user_id", userId)
                .Where("inventory_item_id", inventoryItemId)
                .UpdateAsync(new
                {
                    item_count = itemCount
                });

            return inventoryItemId;
        }

        catch (Exception ex)
        {
            LoggingForError(_logger, EventType.UpdateInventoryItemCountAndGetId, ex.Message, new { UserId = userId, ItemCode = itemCode, ItemCount = itemCount });
            return 0;
        }
    }

    public async Task<ErrorCode> UpdateEnhanceItemAttackPower(Int64 inventoryItemId, Int32 enhanceStage, Int64 attackPower)
    {
        try
        {
            _ = await _queryFactory
                .Query("GameDB.user_inventory_item")
                .Where("inventory_item_id", inventoryItemId)
                .UpdateAsync(new
                {
                    item_attack_power = attackPower,
                    enhance_stage = enhanceStage,
                });

            return ErrorCode.None;
        }
        catch (Exception ex)
        {
            LoggingForError(_logger, EventType.UpdateEnhanceItemAttackPower, ex.Message, new { InventoryItemId = inventoryItemId, EnhanceStage = enhanceStage, AttackPower = attackPower });
            return ErrorCode.GameDbException;
        }
    }

    public async Task<ErrorCode> UpdateEnhanceItemDefensivePower(Int64 inventoryItemId, Int32 enhanceStage, Int64 defensivePower)
    {
        try
        {
            _ = await _queryFactory
                .Query("GameDB.user_inventory_item")
                .Where("inventory_item_id", inventoryItemId)
                .UpdateAsync(new
                {
                    item_defensive_power = defensivePower,
                    enhance_stage = enhanceStage,
                });

            return ErrorCode.None;
        }
        catch (Exception ex)
        {
            LoggingForError(_logger, EventType.UpdateEnhanceItemDefensivePower, ex.Message, new { InventoryItemId = inventoryItemId, EnhanceStage = enhanceStage, DefensivePower = defensivePower });
            return ErrorCode.GameDbException;
        }
    }

    public async Task<ErrorCode> UpdateUserExp(Int64 userId, Int32 amount)
    {
        try
        {
            var existExp = await _queryFactory
                .Query("GameDB.user_play_data")
                .Select("exp")
                .Where("user_id", userId)
                .FirstOrDefaultAsync<Int32>();


            _ = await _queryFactory
                .Query("GameDB.user_play_data")
                .Where("user_id", userId)
                .UpdateAsync(new
                {
                    exp = existExp + amount
                });

            return ErrorCode.None;
        }
        catch (Exception ex)
        {
            LoggingForError(_logger, EventType.UpdateUserExp, ex.Message, new { UserId = userId, Amount = amount });
            return ErrorCode.GameDbException;
        }
    }

    public async Task<ErrorCode> UpdateCompletedStageHistory(Int64 userId, Int32 stageCode, DateTime completedDate)
    {
        try
        {
            _ = await _queryFactory
                .Query("GameDB.user_cleared_dungeon_stage")
                .Where("user_id", userId)
                .Where("stage_code", stageCode)
                .UpdateAsync(new
                {
                    cleared_date = completedDate
                });

            return ErrorCode.None;
        }
        catch (Exception ex)
        {
            LoggingForError(_logger, EventType.UpdateCompletedStageHistory, ex.Message, new { UserId = userId, StageCode = stageCode, CompletedDate = completedDate });
            return ErrorCode.GameDbException;
        }
    }


    #endregion

    private void Open()
    {
        _dbConn = new MySqlConnection(_dbConfig.Value.GameDb);

        _dbConn.Open();
    }

    private void Close()
    {
        _dbConn.Close();
    }


}