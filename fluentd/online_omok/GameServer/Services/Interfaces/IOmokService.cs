namespace GameServer.Services.Interfaces;

public interface IOmokService
{
	public Task<ErrorCode> EnterOmok(Int64 uid);

	public Task<ErrorCode> PutOmok(Int64 uid, int posX, int posY);

	public Task<(ErrorCode, (int, byte[]?))> PeekTurn(Int64 uid, int lastCount);

}
