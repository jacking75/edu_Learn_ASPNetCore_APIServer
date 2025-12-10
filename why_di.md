# 왜 DI 패턴을 사용해야하나?
from: ChatGPT  
  
ASP.NET Core에서 DI(Dependency Injection) 패턴을 사용하는 이유는 **“DB가 바뀔 가능성” 하나 때문이 아니라, 애플리케이션 구조 전반의 품질을 높이기 위한 기본 설계 원칙을 지키기 위해서**다.
레포지터리가 MySQL에서 다른 DB로 바뀔 확률이 거의 없다 하더라도 DI는 여전히 의미가 크다.

아래에서 핵심 이유를 정리한다.

---

## 1. **결합도 감소 — 테스트하기 쉬운 구조를 만든다**

DI를 빼고 `new MySqlRepository()` 같은 방식으로 직접 객체를 생성하면, 해당 클래스는 MySQL 구현체에 **강하게 결합된다**.
그러면 다음 문제가 생긴다:

* 유닛 테스트 시 실제 DB 없이 테스트하기 어려움
* Mock Repository를 주입해 테스트하려면 코드 구조를 갈아엎어야 함
* 서비스 로직이 특정 저장소 구현체에 종속적이 되어 재사용성 감소

DI를 적용하면 테스트 시에는 Mock을, 런타임에서는 실제 Repository를 넣는 식으로 깔끔하게 처리할 수 있다.
**테스트 편의성 하나만으로도 DI의 가치는 충분하다**고 평가된다.

---

## 2. **관심사의 분리(SoC) — 서비스는 “무엇”을 하는지에만 집중한다**

DI는 서비스 레이어가 *레포지터리를 어떻게 생성하는지*에 대해 전혀 신경 쓰지 않도록 만들어준다.

예:

```csharp
public class UserService
{
    private readonly IUserRepository _repo;
    public UserService(IUserRepository repo)
    {
        _repo = repo;
    }

    public Task<User> GetUser(int id)
    {
        return _repo.Get(id);
    }
}
```

UserService는 **레포지터리가 MySQL인지, PostgreSQL인지, MongoDB인지 전혀 모른다**.
서비스는 오직 “유저를 가져온다”는 책임만 갖게 되고, 인프라와 비즈니스 로직이 분리된다.

---

## 3. **유연성 — DB가 안 바뀌어도 ‘구현체 변경’은 자주 일어난다**

DB 타입은 안 바뀔지 몰라도, 다음은 얼마든지 바뀔 수 있다:

* MySQL 드라이버 변경 (예: Dapper → EF Core → 자체 구현)
* 캐시 레이어 추가
  (예: Decorator 패턴으로 캐시 추가한 CacheRepository)
* 로깅/성능 측정을 위한 래퍼
* 분산 트랜잭션 적용 여부
* 샤딩/리딩-레플리카 구현 변경

이런 변경을 할 때 DI가 없으면 기존 코드가 죄다 영향을 받는데,
DI 구조에서는 구현체만 DI Container에서 바꾸면 끝이다.

즉, **DB 스펙이 바뀌지 않아도 구현 디테일은 자주 바뀐다**.

---

## 4. **ASP.NET Core 생태계 자체가 DI 중심으로 설계됨**

ASP.NET Core의 대부분 기능이 DI 컨테이너를 중심으로 동작한다:

* Logging
* Configuration
* Authentication/Authorization
* EF Core DbContext
* Middleware

따라서 DI를 쓰지 않으면 .NET Core 기본 철학과 맞지 않는 구조가 되고, 확장성도 떨어진다.

---

## 5. **테스트, 유지보수, 확장성 측면에서의 비용 대비 효과가 매우 큼**

DI 패턴은 코드량이 늘어서 비용이 있다고 여겨지지만, 장기 유지보수에서는 비용이 오히려 줄어든다.

* 버그 수정 시 영향 범위가 좁아짐
* 테스트 자동화가 쉬워짐
* 새 기능을 붙일 때 기존 서비스 코드 수정량이 줄어듦
* 의존성 구조가 명확해져서 팀원 간 협업이 쉬워짐

**“DB가 안 바뀌니까 DI가 필요 없다”는 단기적 관점이고,
DI는 장기 유지보수 비용을 줄이는 구조를 만든다는 게 핵심이다**.

---

## 결론

레포지터리 DB 타입이 바뀔 가능성이 낮아도 DI를 사용하는 이유는 다음과 같다:

* 테스트 용이성 확보
* 결합도 낮추기
* 관심사 분리
* 향후 구현체 변경 가능성 대응
* ASP.NET Core 생태계와의 통일성 확보

즉, **유연성과 유지보수성**을 위해 DI는 필수적인 설계 패턴이다.  