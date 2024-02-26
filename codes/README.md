# API 게임 서버 
  
## API 서버간 통신 때 HttpClientFactory 사용하기
[MS Docs: ASP.NET Core에서 IHttpClientFactory를 사용하여 HTTP 요청 만들기](https://learn.microsoft.com/ko-kr/aspnet/core/fundamentals/http-requests?view=aspnetcore-8.0 )  
  
```
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddHttpClient("Google", httpClient =>{
    httpClient.BaseAddress = new Uri("https://www.google.com/");
});
builder.Services.AddHttpClient("BaiDu", httpClient =>{
    httpClient.BaseAddress = new Uri("https://www.baidu.com/");
});
```
  
```
interface IGoogleService{
    Task<string> GetContentAsync();
}
class GoogleService : IGoogleService{
    private readonly IHttpClientFactory _httpClientFactory;

    public GoogleService(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }
	
    public async Task<string> GetContentAsync()
    {
        var googleClient = _httpClientFactory.CreateClient("Google");
		var response = await googleClient.GetAsync("some-endpoint");
		...
    }
}
```  
    
    
IHttpClientFactory는 ASP.NET Core에서 HTTP 클라이언트를 관리하는 데 많은 장점을 제공한다.  
- 클라이언트 인스턴스 풀링: IHttpClientFactory는 HttpClient 인스턴스를 풀링하여 재사용합니다. 이로 인해 매 요청마다 새로운 HttpClient를 생성하는 비용을 줄이고 성능을 향상시킵니다.
- 수명 관리: IHttpClientFactory는 HttpClient 인스턴스의 수명을 관리합니다. 요청이 완료되면 HttpClient를 반환하고 재사용할 수 있도록 합니다. 이로 인해 메모리 누수를 방지하고 리소스를 효율적으로 사용할 수 있습니다.
- 구성 가능한 클라이언트: IHttpClientFactory를 사용하면 여러 클라이언트를 구성할 수 있습니다. 위의 코드에서 “Google” 및 “BaiDu” 클라이언트를 등록했습니다. 각 클라이언트는 서로 다른 BaseAddress를 가지며 다른 서비스에 대한 요청을 보낼 수 있습니다.
- DI (의존성 주입) 지원: IHttpClientFactory는 DI 컨테이너와 통합되어 의존성 주입을 통해 HttpClient를 쉽게 주입할 수 있습니다.
- 테스트 용이성: IHttpClientFactory를 사용하면 HttpClient를 목(Mock)으로 대체하여 단위 테스트를 수행할 수 있습니다. 이는 외부 서비스에 의존하는 코드를 테스트할 때 유용합니다.
  
  
요약하자면, IHttpClientFactory는 성능, 메모리 관리, 구성 가능성 및 테스트 용이성 측면에서 HTTP 클라이언트를 효율적으로 관리하는 데 도움이 됩니다.
       
