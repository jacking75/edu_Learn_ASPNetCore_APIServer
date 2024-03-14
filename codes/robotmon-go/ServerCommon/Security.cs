using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace ServerCommon
{
    public class Security
    {
        public static string MakeHashingPassWord(string? saltValue, string? pw)
        {
            // SHA 암호화하여 hash 값을 얻는다.
            var sha = new SHA256Managed();
            byte[] hash = sha.ComputeHash(Encoding.ASCII.GetBytes((saltValue + pw)));
            
            // 이후에 해시 내용을 stringBuilder에 옮긴다...
            StringBuilder stringBuilder = new StringBuilder();
            foreach (byte b in hash)
            {
                stringBuilder.AppendFormat("{0:x2}", b);
            }
            
            return stringBuilder.ToString();
        }
        
        public static string SaltString()
        {
            const string allowableCharacters = "abcdefghijklmnopqrstuvwxyz0123456789";
        
            var bytes = new byte[64];
            using (var random = RandomNumberGenerator.Create())
            {
                random.GetBytes(bytes);
            }
            return new string(bytes.Select(x => allowableCharacters[x % allowableCharacters.Length]).ToArray());
        }

        private const string AllowableCharacters = "abcdefghijklmnopqrstuvwxyz0123456789";
        public static string AuthToken()
        {
            var bytes = new byte[25];
            using (var random = RandomNumberGenerator.Create())
            {
                random.GetBytes(bytes);
            }
            return new string(bytes.Select(x => AllowableCharacters[x % AllowableCharacters.Length]).ToArray());
        }
    }
}