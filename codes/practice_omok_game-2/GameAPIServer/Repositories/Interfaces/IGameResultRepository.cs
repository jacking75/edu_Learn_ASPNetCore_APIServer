using GameAPIServer.Models.GameDb;
namespace GameAPIServer.Repositories.Interfaces;

public interface IGameResultRepository : IDisposable
{
	public Task<IEnumerable<GameResult>?> GetGameResultByUserUid(Int64 uid);

	public Task<ErrorCode> InsertGameResult(GameResult gameResult);
}
