# Controller

## 필수적인 형태
- `[ApiController]` : Model Binding을 Check 해주기 위해서 추가해줘야 하는 **Attribute** 입니다.
- `[Route([controller])]` : Routing Path를 지정하기 위한 **Attribute** 입니다. 
  - `[controller]`는 TestController에서 Controller만 제거되고 Path로 지정됨. -> `/test`
- `[ControllerBase]` : 상속받아야하는 클래스로, http 요청을 처리하기 위한 메서드와 속성을 제공합니다.
``` cs
[ApiController]
[Route("[controller]")]
public class TestController : ControllerBase
{
    ...
}
```
## Method 형태
- `[Post]` : 전송
- `[Get]` : 가져오기
- `[Delete]` : 삭제
- `[Patch]` : 변경
- `[HTTPGet("~~~")]` -> `[HttpGet]` + `[Route("~~~")]` 와 동일
- 함수에 해당하는 Method를 Attribute로 추가하면 됨.
- 추가적으로 각 Method의 Path를 따로 지정할 수 있음.
``` cs
[HttpPost]
public LoginRes Post([FromBody] LoginReq request)
{
   ...
}

[HttpGet]
public LoginRes Get()
{
   ...
}

[HttpDelete]
public LoginRes Delete([FromBody] LoginReq request)
{
   ...
}

[HttpPatch]
public LoginRes Patch([FromBody] LoginReq request)
{
   ...
}
```

## Binding
- `[FromHeader]` : HeaderValues에서 찾아라
- `[FromQuery]` : QueryString에서 찾아라
- `[FromRoute]` : Route Parameter에서 찾아라
- `[FromBody]` : 그냥 Body에서 찾아라(디폴스 JSON -> 다른 형태로도 세팅 가능)
``` cs
 [HttpPost("register")]
public async RegisterRes RegisterAsync(
    [FromHeader(Name = "trace-id")] string traceId,
    [FromBody] RegisterDTO register)
{
    await _authService.RegisterAsync(register, traceId);

    return new RegisterRes();
}
```