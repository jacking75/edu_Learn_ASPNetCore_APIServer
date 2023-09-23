// 미들웨어 호출을 간단하게 하기 위한 확장 메소드
// IApplicationBuilder를 확장하고 있다.
public static class LoggingMiddlewareExtensions
{
    public static IApplicationBuilder UseLoggingMiddleware(this IApplicationBuilder builder)
    {
        // IApplicationBuilder 인터페이스의 UseMiddleware 메소드
        // 제너릭 타입에 작성한 미들웨어 클래스 타입을 지정하면
        // 미들웨어 등록 완료이다
        return builder.UseMiddleware<LoggingMiddleware>();
    }
}