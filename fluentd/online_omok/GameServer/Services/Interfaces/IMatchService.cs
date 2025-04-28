
namespace GameServer.Services.Interfaces;

public interface IMatchService
{
	public Task<(ErrorCode, string)> CheckMatch(Int64 uid);

	public Task<ErrorCode> StartMatch(Int64 uid);
}
