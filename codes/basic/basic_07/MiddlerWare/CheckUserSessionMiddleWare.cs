using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using ZLogger;

public class CheckUserSessionMiddleWare
{
    ILogger Logger;
    private readonly RequestDelegate _next;
    
    public CheckUserSessionMiddleWare(RequestDelegate next, ILoggerFactory loggerFactory)
    {
        _next = next;
    }
   
    public async Task InvokeAsync(HttpContext context)
    {
        if (context.Request.Path == "/Login")
        {
            StreamReader bodystream = new StreamReader(context.Request.Body, Encoding.UTF8);
            string body = bodystream.ReadToEndAsync().Result;            
        }

        await _next(context);   
    } 
}
