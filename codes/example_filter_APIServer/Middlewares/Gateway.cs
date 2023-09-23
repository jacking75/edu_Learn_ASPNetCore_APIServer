/**
 * 
 * Request DTO의 구조가 다음과 같을 때
 * {
 *     "seq": 1000,
 *     "userUid": 0,
 *     "sessionKey": "",
 *     "apiName": "",
 *     "apiVer": 1,
 *     "timestamp": 1687415716,
 *     "result": {
 *         "code": 0,
 *         "msg": ""
 *     },
 *     "requestData": {}
 * }
 * 
 * 클라이언트는 서버에 패킷을 송신할 때 URL에 API를 입력하지 않고, HTTP Body 부분에 API를 명시하게 된다.
 * 따라서 ASP.NET Core의 라우팅 기능을 사용하여 컨트롤러를 자동으로 라우팅할 수 없는 구조이기 때문에 해당 미들웨어를 통해
 * API를 출력하여, HTTPReuqset.Path를 변경하여 컨트롤러를 라우팅하도록 한다.
 * 
 * 또한, 패킷의 공통 부분을 추출하여 HttpContext.Items에 저장한 후 (해당 데이터는 ResponseDataConverter에서 사용된다.)
 * API별 요청 데이터를 추출하여 기존 Request BodyData를 덮어쓴다. 이렇게하면, 컨트롤러에서 각각의 RequestDTO로 자동 Deserialization이 가능하다.
 * 
 */

using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

using APIServer.Filters;
using APIServer.Models.DTO;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace APIServer.Middlewares;

public class Gateway
{
    private readonly bool _enableCrypto = false;
    private readonly RequestDelegate _next;
    private readonly Int32 StreamReaderBufferSize = 4096;

    public Gateway(RequestDelegate next, IOptions<OptionFlags> optionFlags)
    {
        _enableCrypto = optionFlags.Value.EnableCrypto;
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        // https://devblogs.microsoft.com/dotnet/re-reading-asp-net-core-request-bodies-with-enablebuffering/
        // 다중 읽기 허용 함수 -> 파일 형식으로 임시 변환
        context.Request.EnableBuffering();

        using (var reader = new StreamReader(
            context.Request.Body,
            Encoding.UTF8,
            true,
            StreamReaderBufferSize,
            true))
        {
            var bodyData = await reader.ReadToEndAsync();

            // 암호화를 사용한다면 복후화한다.
            if (_enableCrypto == true)
            {
                bodyData = Decrypt(bodyData);
            }

            // DTO의 공통 부분만 추출해서 HttpContext에 저장한다.
            var reqCommonDTO = JsonSerializer.Deserialize<ReqCommonDTO>(bodyData);
            context.Items[nameof(ReqCommonDTO)] = reqCommonDTO;

            // DTO에서 API별 요청 데이터를 추출해서 기존 Request BodyData를 덮어쓴다.
            // 그럼 컨트롤러에서 각각의 RequestDTO로 자동 Deserialization이 가능하다.
            using (var document = JsonDocument.Parse(bodyData))
            {
                var requsetData = document.RootElement.GetProperty("requestData").ToString();
                context.Request.Body = new MemoryStream(Encoding.UTF8.GetBytes(requsetData));
                context.Request.Body.Position = 0;
            }

            // ASP.Net Core가 컨트롤러를 찾을 수 있게끔 request.Path를 변경한다.
            context.Request.Path = $"/{reqCommonDTO.apiName}";
        }

        await _next(context);
    }

    public string Decrypt(string data)
    {
        var hashSize = ResponseDataEncoder.HashSize;

        // Base64로 인코딩 된 데이터를 해시값과 암호화 된 데이터로 분리
        var stringData = Convert.FromBase64String(data);

        var decryptedHash = new byte[hashSize];
        Buffer.BlockCopy(stringData, 0, decryptedHash, 0, hashSize);

        var encryptedData = new byte[stringData.Length - hashSize];
        Buffer.BlockCopy(stringData, hashSize, encryptedData, 0, encryptedData.Length);

        // 해시값을 제외한 데이터 복호화 진행.
        var decryptedData = string.Empty;

        using (Aes aesAlg = Aes.Create())
        {
            aesAlg.Key = Encoding.UTF8.GetBytes(ResponseDataEncoder.Key);
            aesAlg.IV = ResponseDataEncoder.Iv;

            var decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

            using (var msDecrypt = new MemoryStream(encryptedData))
            {
                using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                {
                    using (var srDecrypt = new StreamReader(csDecrypt))
                    {
                        decryptedData = srDecrypt.ReadToEnd();
                    }
                }
            }
        }

        // 복호화 된 데이터로 생성한 해시값과 수신 받은 패킷의 해시값을 비교
        var hash = SHA1.Create().ComputeHash(Encoding.UTF8.GetBytes(decryptedData));
        if (hash.SequenceEqual(decryptedHash) == false)
        {
            return string.Empty;
        }

        return decryptedData;
    }
}
