using APIServer.DTO.DataLoad;
using System.Threading.Tasks;

namespace APIServer.Servicies.Interfaces
{
    public interface IDataLoadService
    {
        public Task<(ErrorCode, DataLoadUserInfo)> LoadUserData(int uid);
        public Task<(ErrorCode, DataLoadGameInfo)> LoadGameData(int uid);
        public Task<(ErrorCode, DataLoadSocialInfo)> LoadSocialData(int uid);
    }
}
