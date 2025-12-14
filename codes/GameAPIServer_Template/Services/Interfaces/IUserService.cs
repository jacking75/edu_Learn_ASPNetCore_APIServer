using GameAPIServer.DTOs;
using GameAPIServer.Models;
using System;
using System.Threading.Tasks;

namespace GameAPIServer.Servicies.Interfaces;

public interface IUserService
{    
    public Task<(ErrorCode, GdbUserMoneyInfo)> GetUserMoneyInfo(Int64 uid);
    
    
}
