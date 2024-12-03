namespace GameAPIServer.Services.Interfaces;

public interface IMatchService
{
	public Task<(ErrorCode, MatchData?)> CheckMatch(Int64 uid);

	public Task<ErrorCode> StartMatch(Int64 uid);

}
