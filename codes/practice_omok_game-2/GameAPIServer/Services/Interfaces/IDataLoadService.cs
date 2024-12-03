namespace GameAPIServer.Services.Interfaces;

public interface IDataLoadService
{
	public Task<(ErrorCode, LoadedUserData?)> LoadUserData(Int64 uid);

	public Task<(ErrorCode, LoadedItemData?)> LoadItemData(Int64 uid);

	public Task<(ErrorCode, LoadedProfileData?)> LoadUserProfile(Int64 uid);

}
