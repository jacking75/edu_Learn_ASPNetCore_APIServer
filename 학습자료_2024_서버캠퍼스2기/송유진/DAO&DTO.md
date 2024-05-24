# DTO와 DAO란 무엇일까?

### DAO (Data Access Object)
  - DB의 데이터에 접근하기 위한 객체
  - “DB에 접근하기 위한 로직”과 “비지니스 로직”을 분리하기 위해 사용
### DTO (Data Transfer Object)
  - 계층(controller, view, business layer) 간 데이터 교환을 위한
  - 데이터 로직을 가지지 않는 순수한 데이터 객체
      - 따라서 getter, setter만 가짐
          - 여기서 setter가 있기에 값이 변할 수 있음

→ [유저가 입력한 데이터를 DB에 넣는 과정]

- 유저가 데이터를 입력하여 form에 있는 데이터를 DTO에 넣어서 전송
- 해당 DTO를 받은 서버가 DAO를 이용하여 데이터베이스로 데이터를 집어넣음
- VO (Value Object)
    - Read-only 속성을 가진 **값 오브젝트**
    - getter
