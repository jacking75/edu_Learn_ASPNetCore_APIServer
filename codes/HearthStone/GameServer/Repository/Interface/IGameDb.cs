using System.Data;
using GameServer.Models;

namespace GameServer.Repository.Interface;

public interface IGameDb : IDisposable
{
    IDbConnection GetDbConnection();

    // User
    public Task<GdbUserInfo> GetUserByAccountUid(Int64 accountUid);
    public Task<int> InsertUser(Int64 accountUid, IDbTransaction transaction);
    public Task<int> UpdateRecentLogin(Int64 accountUid);
    public Task<int> UpdateMainDeck(Int64 accountUid, int mainDeckId);

    // Deck
    public Task<List<GdbDeckInfo>> GetDeckInfoList(Int64 accountUid);
    public Task<GdbDeckInfo> GetDeckInfo(Int64 accountUid, int deckId);
    public Task<GdbDeckInfo> GetMainDeckInfo(Int64 accountUid);
    public Task<int> InsertDeck(Int64 accountUid, int deckId, string deckInfo, IDbTransaction transaction);
    public Task<int> UpdateDeck(Int64 accountUid, int deckId, string deckInfo);

    // Attendance
    public Task<List<AttendanceInfo>> GetAttendanceInfoList(Int64 accountUid);
    public Task<AttendanceInfo> GetAttendanceInfo(Int64 accountUid, int eventKey);
    public Task<int> InsertAttendanceInfoList(Int64 accountUid, List<MdbAttendanceInfo> attendanceInfoList, IDbTransaction transaction);
    public Task<int> InsertAttendance(Int64 accountUid, int eventKey, int dayCount);
    public Task<int> CheckAttendance(Int64 accountUid, int eventKey, IDbTransaction transaction);

    // Currency/Asset
    public Task<List<AssetInfo>> GetAssetInfoList(Int64 accountUid);
    public Task<AssetInfo> GetAssetInfo(Int64 accountUid, string asset_name);
    public Task<int> AddAssetInfo(Int64 accountUid, string asset_name, Int64 value, IDbTransaction transaction);
    public Task<int> DeleteAssetInfo(Int64 accountUid, string asset_name, Int64 value, IDbTransaction transaction);

    // Item
    public Task<List<ItemInfo>> GetItemInfoList(Int64 accountUid);
    public Task<List<ItemInfo>> GetItemInfoByItemId(Int64 accountUid, int itemId);
    public Task<ItemInfo> GetItemInfoByItemGuid(Int64 itemGUID);
    public Task<int> AddItemInfo(Int64 accountUid, int itemId, int itemCount, IDbTransaction transaction);
    public Task<int> DeleteItemInfo(Int64 accountUid, int itemId, int itemCount, IDbTransaction transaction);

    // Mail
    public Task<List<MailInfo>> GetMailList(Int64 accountUid, int page = 1, int pageSize = 10);
    public Task<int> ReceiveMail(Int64 accountUid, Int64 mailId, int mailStatus = 1);
    public Task<int> DeleteMail(Int64 accountUid, Int64 mailId);
    public Task<int> AddMailInfo(Int64 accountUid, MdbMailInfo mailInfo, IDbTransaction transaction);

    // HearthStone
    public Task<List<int>> GetDeckItemIdList(long accountUid);
}