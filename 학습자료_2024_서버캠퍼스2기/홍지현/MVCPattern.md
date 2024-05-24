## MVC Pattern

### Model : 데이터 모델. (데이터를 표현하는 구조) 
- DAO, DTO 등 View(클라이언트) 또는 DB와 통신하는 데이터 형태
- **DTO** : 계층 간 데이터 교환을 하기 위한 객체, 로직을 가지지 않는 순수한 데이터 객체
- **DAO** : Database의 data에 접근하기 위한 객체 -> **Database에 접근하기 위한 로직**과 **비즈니스 로직**을 분리하기 위해 사용
### Controller : 데이터 가공, 필터링, 유효성 체크, 서비스 호출
- Model을 통해 전달된 데이터를 통해 특정 로직을 처리하는 부분
- 각종 Service를 통해 데이터를 가공함.
### View : 최종 결과물을 어떤 형태로 보여줄지
- 결과물
- **.net Core**에서는 .cshtml 이라는 **Razor View Page**형태가 존재

## WebAPI
- MVC에서 View가 달라지는 형태.
- View가 HTML을 반환하지 않고, JSON / XML 데이터를 반환하고 나머지는 동일하게 동작
- .NET Core에서 IActionResult 형태가 아닌 Json 형태를 리턴하는 방식
`return List<string>();`
  
- 예전 버전에서는 MVC와 WebAPI가 분리되어 있었으나, ASP.NET Core로 넘어오며 동일한 프레임워크를 사용한다.

## 전체적인 동작 방식
1) Request
2) Routing
3) Model Binding + Validtaion
4) Action (<-> Service ApplicationModel)
5) ViewModel vs ApiModel << 여기만 다름

-- View(Razor Template) vs Formatter (어떤 포맷으로? JSON)

6) Response