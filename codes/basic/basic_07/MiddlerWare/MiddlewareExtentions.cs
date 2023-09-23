
/// <summary>
/// 이곳에 미들웨어를 등록합니다.
/// </summary>
public static class MiddlewareExtentions
{           
    public static IApplicationBuilder UseLoadRequestDataMiddlerWare(
        this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<LoadRequestDataMiddlerWare>();
    }

    public static IApplicationBuilder UseCheckUserSessionMiddleWare(
        this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<CheckUserSessionMiddleWare>();
    }  
}
