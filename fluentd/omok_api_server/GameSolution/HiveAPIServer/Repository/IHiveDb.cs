using HiveAPIServer.Model.DAO;
using System;
using System.Threading.Tasks;

namespace HiveAPIServer.Repository;

public interface IHiveDb : IDisposable
{
	public Task<bool> CreateAsync<T>(string table, T data);
	public Task<T> SelectAsync<T, S>(string table, string where, S value);
	public Task<bool> UpsertToken(Token token);

}
