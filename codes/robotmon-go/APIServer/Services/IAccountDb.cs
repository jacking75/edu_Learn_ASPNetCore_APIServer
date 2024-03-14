using System;
using System.Threading.Tasks;
using ServerCommon;

namespace ApiServer
{
    public interface IAccountDb : IDisposable
    {
        // DB 열기.
        public void Open();
        
        // DB 닫기.
        public void Close();

        public Task<Tuple<ErrorCode, Int64>> CreateAccountDataAsync(string id, string pw, string salt);

        public Task<ErrorCode> RollbackCreateAccountDataAsync(Int64 createIdx);
        // 유저의 Password, Salt 값 반환
        public Task<ErrorCode> TryPasswordAsync(string id, string pw);
    }
}