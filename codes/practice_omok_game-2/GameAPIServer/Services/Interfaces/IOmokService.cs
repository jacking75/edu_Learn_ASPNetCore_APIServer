
namespace GameAPIServer.Services.Interfaces;

public interface IOmokService
{
	public Task<(ErrorCode, byte[]?)> EnterGame(Int64 uid);
	public Task<(ErrorCode, byte[]?)> SetOmokStone(Int64 uid, int x, int y);
	public Task<(ErrorCode, byte[]?)> PeekGame(Int64 uid);
}
