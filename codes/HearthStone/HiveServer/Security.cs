using System.Net.NetworkInformation;
using System.Security.Cryptography;
using System.Text;

namespace HiveServer;

public class Security
{
    const string AllowableChars = "abcdefghijklmnopqrstuvwxyz0123456789";

    public static string Generate(Int32 length) 
    {
        var bytes = new Byte[length];
        using (var random = RandomNumberGenerator.Create())
        {
            random.GetBytes(bytes);
        }

        return new string(bytes.Select(x => AllowableChars[x % AllowableChars.Length]).ToArray());
    }

    public static string GenerateSaltString() 
    {
        return Generate(64);
    }

    public static string GenerateAuthToken() 
    {
        return Generate(25);
    }

    public static string MakeHashingPassWord(string saltValue, string password) 
    {
        var sha = SHA256.Create();
        var hash = sha.ComputeHash(Encoding.ASCII.GetBytes(saltValue + password));
        var stringBuilder = new StringBuilder();
        foreach (var b in hash)
        {
            stringBuilder.AppendFormat("{0:x2}", b);
        }

        return stringBuilder.ToString();
    }

    public static string MakeHashingToken(string saltValue, Int64 accountUid)
    {
        var sha = SHA256.Create();
        var hash = sha.ComputeHash(Encoding.ASCII.GetBytes(saltValue + accountUid));
        var stringBuilder = new StringBuilder();
        foreach (var b in hash)
        {
            stringBuilder.AppendFormat("{0:x2}", b);
        }

        return stringBuilder.ToString();
    }
}
