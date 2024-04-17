using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.Security.Cryptography;
using Microsoft.Extensions.Primitives;

public class LoadRequestDataMiddlerWare
{
    const string PACKET_TYPE = "PACKET_TYPE";
    const string PACKET_TYPE_ENCRYPT = "ENCRYPT_PACKET";

    const string DEF_SIGN = "YGSIGN";
    const string DEF_SIGNATURE_KEY_V1 = "dkssudgktpdbc";

    private readonly RequestDelegate _next;

    public LoadRequestDataMiddlerWare(RequestDelegate next)
    {
        _next = next;
    }
    
    public async Task InvokeAsync(HttpContext context)
    {
		//아래 라이브러리를 사용하면 stream에서의 읽기 위치 이동을 하지 않을 수 있다.
		//Peeking at HttpContext.Request.Body, without consuming it
		//https://dev.to/alialp/peeking-at-httpcontext-request-body-without-consuming-it-4if5
		
        StreamReader bodystream = new StreamReader(context.Request.Body, Encoding.UTF8);
        string body = bodystream.ReadToEndAsync().Result;
        
        StringValues protocol;
        context.Request.Headers.TryGetValue(PACKET_TYPE, out protocol);

        switch (protocol)
        {
            case PACKET_TYPE_ENCRYPT:

                /*string data = string.Format("{0}:{1}", body, DEF_SIGNATURE_KEY_V1);
                string sign = Encrypter.SHA1Hash(data);

                if (!context.Request.Headers.ContainsKey(DEF_SIGN))
                {
                    return;
                }

                if (string.Compare(context.Request.Headers[DEF_SIGN], sign) != 0)
                {
                    return;
                }

                var result = Encrypter.DecryptIt(body);

                JObject obj = (JObject)JsonConvert.DeserializeObject(result);
                if (obj == null || CheckPacketParams(obj, DEF_COMMON_PARAMS) == false)
                {
                    return;
                }

                var refObj = new JObject();
                refObj = obj;*/

                // 요청 패스 변경
                //context.Request.Path = "/" + "Test/API/" + obj["api"];

                //
                //context.Request.Body = new MemoryStream(Encoding.UTF8.GetBytes(obj["reqdata"].ToString()));
                break;

            default:
				//위에 Body를 읽을 때 위치가 이동해서 다시 덮어쓴다.
                context.Request.Body = new MemoryStream(Encoding.UTF8.GetBytes(body));
                //context.Request.Body.Seek(0, SeekOrigin.Begin); // Seek 함수는 지원한지 않는다.
                break;
        }

        await _next(context);           
    }


    readonly static string[] DEF_COMMON_PARAMS = new string[] { "api", "apiver", "sid", "seq", "timestamp", "user_id", "reqdata" };

    /*static bool CheckPacketParams(JObject obj, string[] paramList)
    {
        foreach (string param in paramList)
        {
            if (!obj.ContainsKey(param))
            {
                return false;
            }
        }

        return true;
    }*/
}
