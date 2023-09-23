using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Options;

namespace APIServer.Filters;

public class ResponseDataEncoder : IResultFilter
{
    public static Int32 HashSize { get; private set; } = 20;
    public static string Key { get; private set; } = "74BE3BD93600BA55";
    public static byte[] Iv { get; private set; } = { 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0 };

    private readonly bool _enableCrypto = false;

    public ResponseDataEncoder(IOptions<OptionFlags> optionFlags)
    {
        _enableCrypto = optionFlags.Value.EnableCrypto;
    }

    public void OnResultExecuting(ResultExecutingContext context)
    {
        if (_enableCrypto == false)
        {
            return;
        }

        var contextResult = context.Result as ObjectResult;

        var data = JsonSerializer.Serialize(contextResult.Value);
        var hash = SHA1.Create().ComputeHash(Encoding.UTF8.GetBytes(data));
        var encrypted = Encrypt(hash, data);

        contextResult.Value = encrypted;
    }

    public void OnResultExecuted(ResultExecutedContext context)
    {
        context.HttpContext.Items.Clear();
    }

    public string Encrypt(byte[] hash, string data)
    {
        byte[] encrypted;

        using (var aesAlg = Aes.Create())
        {
            aesAlg.Key = Encoding.UTF8.GetBytes(Key);
            aesAlg.IV = Iv;

            var encryptor = aesAlg.CreateEncryptor();

            using (var msEncrypt = new MemoryStream())
            {
                using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                {
                    using (var swEncrypt = new StreamWriter(csEncrypt))
                    {
                        // 암호화 할 데이터 쓰기
                        swEncrypt.Write(data);
                    }

                    // 암호화 된 데이터 가져오기
                    encrypted = msEncrypt.ToArray();
                }
            }
        }

        // 해시값과 암호화 된 데이터를 합친 후 Base64로 인코딩
        var results = new byte[hash.Length + encrypted.Length];
        hash.CopyTo(results, 0);
        encrypted.CopyTo(results, hash.Length);

        return Convert.ToBase64String(results);
    }
}
