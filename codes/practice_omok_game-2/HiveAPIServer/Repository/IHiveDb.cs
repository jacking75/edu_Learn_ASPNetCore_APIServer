using System;
using System.Threading.Tasks;

namespace HiveAPIServer.Repository
{
	public interface IHiveDb : IDisposable
	{
		public Task<bool> CreateAsync<T>(string table, T data);

		public Task<bool> UpsertAsync<T>(string table, string primaryKey, T data);

		public Task<T> SelectAsync<T, S>(string table, string where, S value);

	}
}
