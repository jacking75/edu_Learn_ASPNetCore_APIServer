namespace GameAPIServer.Repositories.Interfaces;

public interface IUserRepository : IDisposable
{
	public Task<UserInfo?> GetUserByUid(Int64 uid);
	public Task<IEnumerable<UserMoneyInfo>?> GetUserMoneyByUid(Int64 uid);
	public Task<IEnumerable<UserItemInfo>?> GetUserItemByUid(Int64 uid);
	public Task<UserInfo?> GetUserByPlayerId(Int64 playerId);
	public Task<UserInfo?> GetUserByNickname(string nickname);
	public Task<(ErrorCode, int)> InsertUser(Int64 playerId);
	public Task<bool> UpdateRecentLoginTime(Int64 uid);
	public Task<bool> UpdateUserNickname(Int64 uid, string nickname);

	public Task<int> GetTotalUserPlayCountByUid(Int64 uid);
	public Task<int> GetTotalUserWinCountByUid(Int64 uid);

}
