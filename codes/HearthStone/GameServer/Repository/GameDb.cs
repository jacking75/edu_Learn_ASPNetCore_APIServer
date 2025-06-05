using GameServer.Repository.Interface;
using Microsoft.Extensions.Options;
using MySqlConnector;
using SqlKata.Execution;
using System.Data;
using GameServer.Models;
using ZLogger;
using Dapper;

public class GameDb : IGameDb
{
    readonly ILogger<GameDb> _logger;
    readonly IOptions<DbConfig> _dbConfig;
    IDbConnection _dbConn;
    SqlKata.Compilers.MySqlCompiler _compiler;
    QueryFactory _queryFactory;

    public GameDb(ILogger<GameDb> logger, IOptions<DbConfig> dbConfig)
    {
        _logger = logger;
        _dbConfig = dbConfig;

        Open();

        _compiler = new SqlKata.Compilers.MySqlCompiler();
        _queryFactory = new SqlKata.Execution.QueryFactory(_dbConn, _compiler);
    }

    void Open()
    {
        _dbConn = new MySqlConnection(_dbConfig.Value.GameDb);
        _dbConn.Open();
    }

    void Close()
    {
        _dbConn.Close();
    }

    public void Dispose()
    {
        Close();
    }

    public IDbConnection GetDbConnection()
    {
        return _queryFactory.Connection;
    }

    // User
    public async Task<GdbUserInfo> GetUserByAccountUid(Int64 accountUid)
    {
        return await _queryFactory.Query("user").Where("account_uid", accountUid)
            .FirstOrDefaultAsync<GdbUserInfo>();
    }

    public async Task<int> InsertUser(Int64 accountUid, IDbTransaction transaction)
    {
        return await _queryFactory.Query("user").InsertGetIdAsync<int>(new
        {
            account_uid = accountUid,
            main_deck_id = 0,
            last_login_dt = DateTime.Now
        }, transaction);
    }

    public async Task<int> UpdateRecentLogin(Int64 accountUid)
    {
        return await _queryFactory.Query("user").Where("account_uid", accountUid)
            .UpdateAsync(new { last_login_dt = DateTime.Now });
    }

    public async Task<int> UpdateMainDeck(Int64 accountUid, int mainDeckId)
    {
        return await _queryFactory.Query("user")
            .Where("account_uid", accountUid)
            .UpdateAsync(new
            {
                main_deck_id = mainDeckId
            });
    }

    // Deck
    public async Task<List<GdbDeckInfo>> GetDeckInfoList(Int64 accountUid)
    {
        return (await _queryFactory.Query("user_deck")
            .Where("account_uid", accountUid)
            .GetAsync<GdbDeckInfo>()).ToList();
    }

    public async Task<GdbDeckInfo> GetDeckInfo(Int64 accountUid, int deckId)
    {
        return await _queryFactory.Query("user_deck")
            .Where("account_uid", accountUid)
            .Where("deck_id", deckId)
            .FirstOrDefaultAsync<GdbDeckInfo>();
    }

    public async Task<int> InsertDeck(Int64 accountUid, int deckId, string deckInfo, IDbTransaction transaction)
    {
        return await _queryFactory.Query("user_deck").InsertAsync(new
        {
            account_uid = accountUid,
            deck_id = deckId,
            deck_list = deckInfo,
            create_dt = DateTime.Now
        }, transaction);
    }

    public async Task<int> UpdateDeck(Int64 accountUid, int deckId, string deckInfo)
    {
        return await _queryFactory.Query("user_deck")
            .Where("account_uid", accountUid)
            .Where("deck_id", deckId)
            .UpdateAsync(new
            {
                deck_list = deckInfo
            });
    }

    // Currency/Asset 
    public async Task<List<AssetInfo>> GetAssetInfoList(Int64 accountUid)
    {
        return (await _queryFactory.Query("user_asset")
            .Where("account_uid", accountUid)
            .GetAsync<AssetInfo>()).ToList();
    }
    public async Task<AssetInfo> GetAssetInfo(Int64 accountUid, string assetName)
    {
        return await _queryFactory.Query("user_asset")
            .Where("account_uid", accountUid)
            .Where("asset_name", assetName)
            .FirstOrDefaultAsync<AssetInfo>();
    }
    public async Task<int> AddAssetInfo(Int64 accountUid, string assetName, Int64 assetAmount, IDbTransaction transaction)
    {
        try
        {
            string updateSql =
                @"INSERT INTO user_asset (account_uid, asset_name, asset_amount) VALUES (@accountUid, @assetName, @assetAmount) " +
                "ON DUPLICATE KEY UPDATE asset_amount = asset_amount + @assetAmount ";

            int result = await _dbConn.ExecuteAsync(
                updateSql,
                new { accountUid, assetName, assetAmount },
                transaction);

            return result;
        
        }
        catch(Exception e)
        {
            _logger.ZLogError(e, $"[AddAssetInfo] Failed to insert/update currency for accountUid: {accountUid}, asset_name: {assetName}, value: {assetAmount}");
            return 0;
        }
    }

    public async Task<int> DeleteAssetInfo(Int64 accountUid, string assetName, Int64 assetAmount, IDbTransaction transaction)
    {
        try
        {
            // 현재 보유 금액 조회
            var assetInfo = await _queryFactory.Query("user_asset")
                .Where("account_uid", accountUid)
                .Where("asset_name", assetName)
                .FirstOrDefaultAsync<AssetInfo>();

            if (assetInfo == null)
            {
                _logger.ZLogError($"[DeleteAssetInfo] Currency not found for accountUid: {accountUid}, asset_name: {assetName}");
                return 0;
            }

            // 차감 후 금액 계산
            Int64 remainingValue = assetInfo.asset_amount - assetAmount;

            // 남은 금액이 0 이하이면 row 삭제
            if (remainingValue <= 0)
            {
                return await _queryFactory.Query("user_asset")
                    .Where("account_uid", accountUid)
                    .Where("asset_name", assetName)
                    .DeleteAsync(transaction);
            }
            // 아니면 금액만 업데이트
            else
            {
                return await _queryFactory.Query("user_asset")
                    .Where("account_uid", accountUid)
                    .Where("asset_name", assetName)
                    .UpdateAsync(new
                    {
                        asset_amount = remainingValue
                    }, transaction);
            }
        }
        catch (Exception e)
        {
            _logger.ZLogError(e, $"[DeleteAssetInfo] Failed to update/delete currency for accountUid: {accountUid}, asset_name: {assetName}, value: {assetAmount}");
            return 0;
        }
    }

    // Attendance
    public async Task<List<AttendanceInfo>> GetAttendanceInfoList(Int64 accountUid)
    {
        return (await _queryFactory.Query("user_attendance")
            .Where("account_uid", accountUid)
            .GetAsync<AttendanceInfo>()).ToList();
    }

    public async Task<AttendanceInfo> GetAttendanceInfo(Int64 accountUid, int eventKey)
    {
        return await _queryFactory.Query("user_attendance")
            .Where("account_uid", accountUid)
            .Where("event_id", eventKey)
            .FirstOrDefaultAsync<AttendanceInfo>();
    }

    public async Task<int> InsertAttendanceInfoList(Int64 accountUid, List<MdbAttendanceInfo> attendanceInfoList, IDbTransaction transaction)
    {
        try
        {
            if (attendanceInfoList == null || attendanceInfoList.Count == 0)
            {
                return 0;
            }

            int totalResult = 0;
            foreach (var info in attendanceInfoList)
            {
                totalResult += await _queryFactory.Query("user_attendance")
                    .InsertAsync(new
                    {
                        account_uid = accountUid,
                        event_id = info.event_id,
                        attendance_no = 0,
                        attendance_dt = DateTime.Now.AddDays(-1)
                    }, transaction);
            }

            return totalResult;
        }
        catch (Exception e)
        {
            _logger.ZLogError(e, $"[InsertAttendanceInfoList] Failed to bulk insert attendance info list for accountUid: {accountUid}");
            return 0;
        }
    }

    public async Task<int> InsertAttendance(Int64 accountUid, int eventKey, int dayCount)
    {
        return await _queryFactory.Query("user_attendance").InsertAsync(new
        {
            account_uid = accountUid,
            event_id = eventKey,
            attendance_no = dayCount,
            attendance_dt = DateTime.Now
        });
    }

    public async Task<int> CheckAttendance(Int64 accountUid, int eventKey, IDbTransaction transaction)
    {
        string updateSql =
            "UPDATE user_attendance " +
            "SET attendance_no = attendance_no + 1, attendance_dt = @now " +
            "WHERE account_uid = @accountUid AND event_id = @eventKey AND attendance_dt < @today";

        return await _dbConn.ExecuteAsync(
            updateSql,
            new { accountUid, eventKey, now = DateTime.Now, today = DateTime.Today },
            transaction);
    }

    // Item
    public async Task<List<ItemInfo>> GetItemInfoList(Int64 accountUid)
    {
        return (await _queryFactory.Query("user_item")
            .Where("account_uid", accountUid)
            .GetAsync<ItemInfo>()).ToList();
    }

    public async Task<List<ItemInfo>> GetItemInfoByItemId(Int64 accountUid, int itemId)
    {
        return (await _queryFactory.Query("user_item")
            .Where("account_uid", accountUid)
            .Where("item_id", itemId)
            .GetAsync<ItemInfo>()).ToList();
    }

    public async Task<ItemInfo> GetItemInfoByItemGuid(Int64 itemGUID)
    {
        return await _queryFactory.Query("user_item")
            .Where("item_guid", itemGUID)
            .FirstOrDefaultAsync<ItemInfo>();
    }

    public async Task<int> AddItemInfo(Int64 accountUid, int itemId, int itemCount, IDbTransaction transaction)
    {
        string updateSql =
            "UPDATE user_item SET item_cnt = item_cnt + @itemCount " +
            "WHERE account_uid = @accountUid AND item_id = @itemId";

        int result = await _dbConn.ExecuteAsync(
            updateSql,
            new { accountUid, itemId, itemCount },
            transaction);

        if (result == 0)
        {
            result = await _queryFactory.Query("user_item")
                .InsertAsync(new
                {
                    account_uid = accountUid,
                    item_id = itemId,
                    item_cnt = itemCount
                }, transaction);
        }

        return result;
    }

    public async Task<int> DeleteItemInfo(Int64 accountUid, int itemId, int itemCount, IDbTransaction transaction)
    {
        try
        {
            // 먼저 해당 아이템의 현재 수량을 조회
            var itemInfo = await _queryFactory.Query("user_item")
                .Where("account_uid", accountUid)
                .Where("item_id", itemId)
                .FirstOrDefaultAsync<ItemInfo>();

            if (itemInfo == null)
            {
                _logger.ZLogError($"[DeleteItemInfo] Item not found for accountUid: {accountUid}, itemId: {itemId}");
                return 0;
            }

            // 차감 후 수량 계산
            int remainingCount = itemInfo.item_cnt - itemCount;

            // 남은 수량이 0 이하면 아이템 삭제
            if (remainingCount <= 0)
            {
                return await _queryFactory.Query("user_item")
                    .Where("account_uid", accountUid)
                    .Where("item_id", itemId)
                    .DeleteAsync(transaction);
            }
            // 그렇지 않으면 수량만 업데이트
            else
            {
                return await _queryFactory.Query("user_item")
                    .Where("account_uid", accountUid)
                    .Where("item_id", itemId)
                    .UpdateAsync(new
                    {
                        item_cnt = remainingCount
                    }, transaction);
            }
        }
        catch (Exception e)
        {
            _logger.ZLogError(e, $"[DeleteItemInfo] Failed to update/delete item for accountUid: {accountUid}, itemId: {itemId}, count: {itemCount}");
            return 0;
        }
    }

    // Mail
    public async Task<List<MailInfo>> GetMailList(Int64 accountUid, int page = 1, int pageSize = 10)
    {
        int offset = (page - 1) * pageSize;
        return (await _queryFactory.Query("user_mail")
            .Where("account_uid", accountUid)
            .Where("status", 0)
            .Where("expire_dt", ">=", DateTime.Now)
            .OrderByDesc("received_dt")
            .Skip(offset)
            .Take(pageSize)
            .GetAsync<MailInfo>()).ToList();
    }

    public async Task<int> ReceiveMail(Int64 accountUid, Int64 mailId, int mailStatus = 1)
    {
        return await _queryFactory.Query("user_mail")
            .Where("account_uid", accountUid)
            .Where("mail_id", mailId)
            .Where("expire_dt", ">=", DateTime.Now)
            .UpdateAsync(new
            {
                status = mailStatus
            });
    }

    public async Task<int> DeleteMail(Int64 accountUid, Int64 mailId)
    {
        try
        {
            return await _queryFactory.Query("user_mail")
                .Where("account_uid", accountUid)
                .Where("mail_id", mailId)
                .DeleteAsync();
        }
        catch (Exception e)
        {
            _logger.ZLogError($"fail delete mail : {accountUid}-{mailId}-{e}");
            return 0;
        }
    }

    public async Task<GdbDeckInfo> GetMainDeckInfo(Int64 accountUid)
    {
        try
        {
            var user = await _queryFactory.Query("user")
                                .Select("main_deck_id")
                                .Where("account_uid", accountUid)
                                .FirstOrDefaultAsync<GdbUserInfo>();

            if (user == null || user.main_deck_id == 0)
                return null;

            return await _queryFactory.Query("user_deck")
                .Where("account_uid", accountUid)
                .Where("deck_id", user.main_deck_id)
                .FirstOrDefaultAsync<GdbDeckInfo>();

        }catch(Exception e)
        {
            _logger.ZLogError(e, $"[GetMainDeckInfo] Failed to get main deck info for accountUid: {accountUid}");
            return null;
        }
    }

    public async Task<List<int>> GetDeckItemIdList(long accountUid)
    {
        try
        {
            var deck = await GetMainDeckInfo(accountUid);
            if (deck == null || string.IsNullOrEmpty(deck.deck_list)) return new List<int>();

            var itemIds = deck.deck_list.Split(',')
                .Select(x => int.TryParse(x, out var id) ? id : (int?)null)
                .Where(x => x.HasValue)
                .Select(x => x.Value)
                .ToList();

            return itemIds;
        }
        catch (Exception e)
        {
            _logger.ZLogError(e, $"[GetDeckItemIds] Failed to get deck item IDs for accountUid: {accountUid}");
            return new List<int>();
        }
    }

    public async Task<int> AddMailInfo(Int64 accountUid, MdbMailInfo mailInfo, IDbTransaction transaction)
    {
        try
        {
            return await _queryFactory.Query("user_mail")
                .InsertAsync(new
                {
                    account_uid = accountUid,
                    mail_id = mailInfo.mail_id,
                    status = mailInfo.status,
                    reward_key = mailInfo.reward_key,
                    mail_desc = mailInfo.mail_desc,
                    received_dt = mailInfo.received_dt,
                    expire_dt = mailInfo.expire_dt
                }, transaction);
        }
        catch (Exception e)
        {
            _logger.ZLogError(e, $"[AddMailInfo] Failed to insert mail for accountUid: {accountUid}");
            return 0;
        }
    }
}
