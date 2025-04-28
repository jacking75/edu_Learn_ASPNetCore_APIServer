using GameShared.DTO;

namespace GameServer.Services.Interfaces;

public interface IDataLoadService
{
	public Task<(ErrorCode, LoadedUserData?)> LoadUserData(Int64 uid, bool loadItems = false, bool loadAttendance = false);
	public (ErrorCode, LoadedMasterData?) LoadMasterData();
}
