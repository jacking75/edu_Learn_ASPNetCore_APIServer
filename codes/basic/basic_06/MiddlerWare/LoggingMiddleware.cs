public class LoggingMiddleware
{
    private readonly Microsoft.AspNetCore.Http.RequestDelegate _next;
    private readonly ILogger<LoggingMiddleware> _logger;

    // 생성자에 RequestDelegate 타입의 인수를 받고 있다
    // 로그에 출력하므로 ILogger 인터페이스도 인수로 받는다
    // logger에 로그 구현이 DI 되어 있다
    public LoggingMiddleware(Microsoft.AspNetCore.Http.RequestDelegate next, ILogger<LoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    // 미들웨어 실행 메소드
    public async Task Invoke(Microsoft.AspNetCore.Http.HttpContext httpContext)
    {
        this._logger.LogInformation("처리 시작:" + httpContext.Request.Path);
        // 다음 미들웨어가 호출된다. 이 전후로 처리 시작, 처리 종료 로그를 출력하고 있다
        await _next(httpContext);
        this._logger.LogInformation("처리 종료");
    }
}