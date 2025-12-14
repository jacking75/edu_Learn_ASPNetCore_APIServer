using GameAPIServer.DTOs;
using System;
using System.Threading.Tasks;

namespace GameAPIServer.Servicies.Interfaces;

public interface IDataLoadService
{
    public Task<(ErrorCode, DataLoadUserInfo)> LoadUserData(Int64 uid);
        
}
