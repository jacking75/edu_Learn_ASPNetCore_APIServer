using System;
using System.Threading.Tasks;
using HiveAPIServer.Model.DAO;

namespace HiveAPIServer.Services
{
	public interface IHiveService
	{
		public Task<ErrorCode> CreateAccount(string email, string pw);
		public Task<(ErrorCode, HdbTokenInfo)> LoginHive(string email, string pw);
		public Task<ErrorCode> VerifyToken(Int64 playerId, string token);
	}
}
