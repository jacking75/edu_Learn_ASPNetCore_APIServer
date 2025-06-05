using GameServer.Models.DTO;
namespace GameServer.Services.Interface;

public interface IDataLoadService
{
    public Task<DataLoadResponse> LoadUserData(Int64 accountUid);
    public TableLoadReponse LoadTableData();
}
