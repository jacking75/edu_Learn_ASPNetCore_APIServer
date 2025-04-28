using GameAPIServer.Models.DAO;
using GameAPIServer.Models.DTO;
using System.Threading.Tasks;

namespace GameAPIServer.Servicies.Interfaces;

public interface IUserService
{
    public Task<(ErrorCode, GdbUserInfo)> GetUserInfo(int uid);
    
    public Task<(ErrorCode, GdbUserMoneyInfo)> GetUserMoneyInfo(int uid);
    
    public Task<(ErrorCode, OtherUserInfo)> GetOtherUserInfo(int uid);
}
