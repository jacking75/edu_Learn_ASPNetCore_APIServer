using MatchAPIServer.DTO.User;
using MatchAPIServer.Models.GameDB;
using System.Threading.Tasks;

namespace MatchAPIServer.Servicies.Interfaces
{
    public interface IUserService
    {
        public Task<(ErrorCode, GdbUserInfo)> GetUserInfo(int uid);
        public Task<(ErrorCode, GdbUserMoneyInfo)> GetUserMoneyInfo(int uid);
        public Task<ErrorCode> SetUserMainChar(int uid, int charKey);
        public Task<(ErrorCode, OtherUserInfo)> GetOtherUserInfo(int uid);
    }
}
