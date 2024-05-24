# Filter
필터를 넣는 시점을 다양하게 할 수 있음.
1) Authorization Filter
- 권한이 있는지 확인. 가장 먼저 실행
- 권한이 없으면 -> 흐름을 가로채서 바로 빠져나감
2) Route Filter
- 1번 다음으로 추가, 맨 마지막에도 가능
- 공용 코드를 넣는다거나
3) Action Filter
- Action 호출 전후에 처리
4) Exception Filter
- 예외가 일어났을 때
5) Result Filter
- IActionResult 전후에 처리
  
미들웨어 vs 필터
1) 방향성
- 미들웨어는 항상 양방향 (IN/OUT)
- 필터 (Resource, Action, Result 두번, 나머진 1번)
1) Request 종류
- 미들웨어는 모든 Request에 대해
필터는 MVC 미들웨어와 연관된 Request에 대해서만 실행
AddControllersWithViews가 MVC 미들웨어
미들웨어는 더 일반적으로 광범위하다
필터는 MVC 미들웨어에서 동작하는 2차 미들웨어 정도로 이해 가능
1) 적용 범위
필터는 전체 / Controller / Action 적용 범위를 골라서 적용
필터는 MVC의 ModelState, IActionResults 등 세부 정보에 접근 가능


명시적으로 순서를 지정할 수도 있음
IOrderedFilter 사용
Order = 0인데 작을수록 먼저 실행
Order가 같을 경우 기본 실행 순서로 실행됨.


``` cs
public class TestResourceFilter : Attribute, IResourceFilter, IOrderedFilter
{
    // 우선순위 지정
    public int Order => -1;

    // 전
    public void OnResourceExecuting(ResourceExecutingContext context)
    {
        Console.WriteLine("Resource Executing");
    }

    // 후
    public void OnResourceExecuted(ResourceExecutedContext context)
    {
        throw new NotImplementedException();
    }
}

// Authorize
// 기본적으로 있는 인증 필터
// Controller
[TestResourceFilter]
[Authorize]
public class FilterController : Controller
{
    public FilterController()
    {

    }

    // Action
    [TestResourceFilter]
    public IActionResult Index()
    {
        return BadRequest();
    }

    public IActionResult ACtions()
    {
        // 이런 기능을 ActionFilter에서 대신 할 수 있음.
        if(!ModelState.IsValid)
            return BadRequest();

        return NotFound();
    }
}
```