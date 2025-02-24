# SqlKata  
https://sqlkata.com/docs/ 여기의 글을 번역 정리하였다.  
  

## 소개
우아한 쿼리 빌더 및 실행기는 우아하고 예측 가능한 방식으로 SQL 쿼리를 처리할 수 있도록 도와줍니다.  
모두가 좋아하는 언어인 C#으로 작성되었으며, 소스 코드는 [Github의 SqlKata](https://github.com/sqlkata/querybuilder )에서 확인할 수 있습니다.  
  
매개 변수 바인딩 기술을 사용하여 SQL 인젝션 공격으로부터 애플리케이션을 보호합니다.  
바인딩으로 전달되는 문자열을 정리할 필요가 없습니다.    
  
이 기술은 SQL 인젝션 공격으로부터 보호할 뿐 아니라 매개변수가 변경되더라도 SQL 엔진이 동일한 쿼리 계획을 캐시하고 재사용하도록 하여 쿼리 실행 속도를 높여줍니다.  
  
```
IEnumerable<Post> posts = await db.Query("Posts")
    .Where("Likes", ">", 10)
    .WhereIn("Lang", new [] {"en", "fr"})
    .WhereNotNull("AuthorId")
    .OrderByDesc("Date")
    .Select("Id", "Title")
    .GetAsync<Post>();
```  
  
```  
SELECT [Id], [Title] FROM [Posts] WHERE
  [Likes] > @p1 AND
  [Lang] IN ( @p2, @p3 ) AND
  [AuthorId] IS NOT NULL
ORDER BY [Date] DESC
```  
     
  
## 설치
Nuget으로 설치한다.   
```
dotnet add package SqlKata
dotnet add package SqlKata.Execution
```
   
  
  
## 시작  
```
using SqlKata;
using SqlKata.Execution;
using System.Data.SqlClient; // Sql Server Connection Namespace

// Setup the connection and compiler
var connection = new SqlConnection("Data Source=MyDb;User Id=User;Password=TopSecret");
var compiler = new SqlServerCompiler();

var db = new QueryFactory(connection, compiler);

// You can register the QueryFactory in the IoC container

var user = db.Query("Users").Where("Id", 1).Where("Status", "Active").First();
```  
  
Sql output  
```
SELECT TOP(1) * FROM [Users] WHERE [Id] = @p0 AND [Status] = @p1
```  
where @p0, @p1 are equivalent to 1, "Active" respectively.
  
  
  
## 컴파일만 하는 예제 
쿼리를 실행할 필요가 없는 경우 SqlKata를 사용하여 쿼리를 바인딩 배열이 있는 SQL 문자열로 빌드하고 컴파일할 수 있습니다. 물론 여기에는 연결 인스턴스가 필요하지 않습니다. 시작하는 가장 간단한 방법은 테이블 이름을 전달하여 쿼리 객체의 새 인스턴스를 만드는 것입니다.  
```
using SqlKata;
using SqlKata.Compilers;

// Create an instance of SQLServer
var compiler = new SqlServerCompiler();

var query = new Query("Users").Where("Id", 1).Where("Status", "Active");

SqlResult result = compiler.Compile(query);

string sql = result.Sql;
List<object> bindings = result.Bindings; // [ 1, "Active" ]
```  
다음 SQL 문자열이 생성됩니다.  
```
SELECT * FROM [Users] WHERE [Id] = @p0 AND [Status] = @p1
```   
  

<br>   

## Compilers
컴파일러는 쿼리 인스턴스를 데이터베이스 엔진에서 직접 실행할 수 있는 SQL 문자열로 변환하는 컴포넌트입니다. 
  
### 지원되는 컴파일러 
현재 SqlKata 쿼리 빌더는 기본적으로 다음 컴파일러를 지원합니다. Sql Server, SQLite, MySql, PostgreSql, Oracle 및 Firebird.  
    
### 몇 가지 눈에 띄는 차이점 
이론적으로 다른 컴파일러의 출력은 유사해야 하며, 이는 80%의 경우에 해당되지만 일부 에지 케이스에서는 출력이 매우 다를 수 있습니다. 예를 들어 각 컴파일러에서 Limit 및 Offset 절이 어떻게 컴파일되는지 살펴보십시오.    
  
```  
new Query("Posts").Limit(10).Offset(20); 
```
  
Sql Server  
```
SELECT * FROM [Posts] ORDER BY (SELECT 0) OFFSET 20 ROWS FETCH NEXT 10 ROWS ONLY
```  
  
Legacy Sql Server (< 2012)  
```
SELECT * FROM (
    SELECT *, ROW_NUMBER() OVER (ORDER BY (SELECT 0)) AS [row_num] FROM [Posts]
) WHERE [row_num] BETWEEN 21 AND 30
```  
  
MySql   
```
SELECT * FROM `Posts` LIMIT 10 OFFSET 20
```  
   
PostgreSql  
```
SELECT * FROM "Posts" LIMIT 10 OFFSET 20 
```  
  
이 문서에서는 출력이 동일하지 않은 쿼리를 제외하고 SqlServer 컴파일러에 의해 컴파일된 쿼리만 표시합니다.  
  
  

## Select 
   
### Column
하나 또는 여러 개의 열을 선택  
```
new Query("Posts").Select("Id", "Title", "CreatedAt as Date"); 
```  
```  
SELECT [Id], [Title], [CreatedAt] AS [Date] FROM [Posts]
```  
  
### 하위 쿼리
하위 쿼리에서 선택  
```
var countQuery = new Query("Comments").WhereColumns("Comments.PostId", "=", "Posts.Id").AsCount();
var query = new Query("Posts").Select("Id").Select(countQuery, "CommentsCount");try 
```  
```
SELECT [Id], (SELECT COUNT(*) AS [count] FROM [Comments] WHERE [Comments].[PostId] = [Posts].[Id]) AS [CommentsCount] FROM [Posts]
```  
  
### Raw
자유롭게 기술하고 싶을 때   
```
new Query("Posts").Select("Id").SelectRaw("count(1) over(partition by AuthorId) as PostsByAuthor") 
```  
```
SELECT [Id], count(1) over(partition by AuthorId) as PostsByAuthor FROM [Posts]
```  
   
### Raw 내부의 열과 테이블 식별
식별자를 [ 및 ]로 감싸서 SqlKata에서 식별자로 인식하도록 할 수 있으므로 위의 동일한 예제를 다음과 같이 다시 작성할 수 있습니다.  
```
new Query("Posts").Select("Id").SelectRaw("count(1) over(partition by [AuthorId]) as [PostsByAuthor]")
```  
  
이제 AuthorId 및 PostsByAuthor가 컴파일러 식별자로 래핑되므로 특히 대소문자를 구분하는 PostgreSql과 같은 엔진에 유용합니다.
  
SQLServer 에서  
```
SELECT [Id], count(1) over(partition by [AuthorId]) as [PostsByAuthor] FROM [Posts]
```
  
Postgres 에서 
```
SELECT "Id", count(1) over(partition by "AuthorId") as "PostsByAuthor" FROM "Posts"
```  
  
MySQL 에서  
```
SELECT `Id`, count(1) over(partition by `AuthorId`) as `PostsByAuthor` FROM `Posts`
```  
   

### 열 확장 표현식(중괄호 확장)
v1.1.2 부터 Braces Expansions 기능을 사용하여 동시에 여러 열을 선택할 수 있습니다. 이를 통해 동일한 쿼리를 더 간결하게 작성할 수 있습니다.   
```
new Query("Users")
    .Join("Profiles", "Profiles.UserId", "Users.Id")
    .Select(
        "Users.{Id, Name, LastName}",
        "Profiles.{GithubUrl, Website, Stars}"
    )
```  	 
  
동일하게 기술한 것   
```
new Query("Users")
    .Join("Profiles", "Profiles.UserId", "Users.Id")
    .Select(
        "Users.Id",
        "Users.Name",
        "Users.LastName",
        "Profiles.GithubUrl",
        "Profiles.Website",
        "Profiles.Stars"
    )
```  
```	 
SELECT
  [Users].[Id],
  [Users].[Name],
  [Users].[LastName],
  [Profiles].[GithubUrl],
  [Profiles].[Website],
  [Profiles].[Stars]
FROM
  [Users]
  INNER JOIN [Profiles] ON [Profiles].[UserId] = [Users].[Id] 
```  
     

## From  
  
### 테이블이나 뷰에서
```
new Query("Posts");
```  
or   
```
new Query().From("Posts");
```  
  
```  
SELECT * FROM [Posts]
```  
  
   
### 별명(Alias)
테이블에 별칭을 지정하려면 as 구문을 사용해야 합니다.  
```
new Query("Posts as p")
```  
   
```   
SELECT * FROM [Posts] AS [p]
```  
  

### From a Sub Query
쿼리 인스턴스를 From 메서드에 전달하여 하위 쿼리에서 선택하거나 람다 함수 오버로드를 사용할 수 있습니다.  
```
var fewMonthsAgo = DateTime.UtcNow.AddMonths(-6);
var oldPostsQuery = new Query("Posts").Where("Date", "<", fewMonthsAgo).As("old");

var query = new Query().From(oldPostsQuery).OrderByDesc("Date");
```  
  
```  
SELECT * FROM (SELECT * FROM [Posts] WHERE [Date] < '2017-06-01 6:31:26') AS [old] ORDER BY [Date] DESC
```  
  

### From a Raw expression
이 FromRaw 방법을 사용하면 원시 표현식을 작성할 수 있습니다.
  
예를 들어 SqlServer에서는 TABLESAMPLE을 사용하여 주석 테이블의 전체 행 중 10% 샘플을 가져올 수 있습니다.  
```
var query = new Query().FromRaw("Comments TABLESAMPLE SYSTEM (10 PERCENT)")
```
  
```  
SELECT * FROM Comments TABLESAMPLE SYSTEM (10 PERCENT)
```  
  
  
## Where
SqlKata는 Where 조건을 쉽게 작성할 수 있는 많은 유용한 메서드를 제공합니다.  
이러한 모든 메서드에는 NOT 및 OR 연산자에 대한 오버로드가 함께 제공됩니다.  
따라서 부울 OR 연산자를 적용하려면 WhereNotNull 또는 OrWhereNotNull를 사용하고 조건을 무효화하려면 OrWhereNull를 사용할 수 있습니다.  
  
### 기본 Where
where 메서드의 두 번째 매개변수는 선택 사항이며 생략하면 기본적으로 `=`가 되므로 이 두 문은 완전히 동일합니다.  
```
new Query("Posts").Where("Id", 10); 

// `=`는 기본 연산자이므로 
new Query("Posts").Where("Id", "=", 10);
```  
```  
new Query("Posts").WhereFalse("IsPublished").Where("Score", ">", 10);
```  
```
SELECT * FROM [Posts] WHERE [IsPublished] = 0 AND [Score] > 10
```  
  
주: WhereNot, OrWhere 및 OrWhereNot에도 동일하게 적용됩니다.   
  

### 여러 필드
여러 필드에 대해 쿼리를 필터링하려면 col/value을 나타내는 객체를 전달합니다.  
```
var query = new Query("Posts").Where(new {
    Year = 2017 ,
    CategoryId = 198 ,
    IsPublished = true,
});
```   
```
SELECT * FROM [Posts] WHERE [Year] = 2017 AND [CategoryId] = 198 AND [IsPublished] = True
```  
  
  
### WhereNull, WhereTrue 및 WhereFalse
NULL, true 및 false 값에 대해 필터링하려면 다음을 수행합니다.   
```
db.Query("Users").WhereFalse("IsActive").OrWhereNull("LastActivityDate");
```  
```
SELECT * FROM [Users] WHERE [IsActive] = cast(0 as bit) OR [LastActivityDate] IS NULL
```   
  

### 하위 쿼리(Sub Query)
Query 인스턴스를 전달하여 열을 하위 쿼리와 비교할 수 있습니다.  
```
var averageQuery = new Query("Posts").AsAverage("score");

var query = new Query("Posts").Where("Score", ">", averageQuery);
```   
```
SELECT * FROM [Posts] WHERE [Score] > (SELECT AVG([score]) AS [avg] FROM [Posts])
```  
  
주: 하위 쿼리는 비교할 스칼라 셀 하나를 반환해야 하므로 필요한 경우 Limit(1)를 설정하고 하나의 열을 선택해야 할 수 있습니다.  
   

### 중첩된 조건 및 그룹화
조건을 그룹화하려면 조건을 다른 Where 블록 안에 래핑하면 됩니다.  
```
new Query("Posts").Where(q =>
    q.WhereFalse("IsPublished").OrWhere("CommentsCount", 0)
);
```  
``` 
SELECT * FROM [Posts] WHERE ([IsPublished] = 0 OR [CommentsCount] = 0)
```  
    

### Comparing two columns
두 열을 함께 비교하려는 경우 이 방법을 사용합니다.  
```
new Query("Posts").WhereColumns("Upvotes", ">", "Downvotes");
```  
```
SELECT * FROM [Posts] WHERE [Upvotes] > [Downvotes]
```  
  

### Where In
IEnumerable<T>을 전달하여 SQL WHERE IN 조건을 적용합니다.  
```
new Query("Posts").WhereNotIn("AuthorId", new []{1, 2, 3, 4, 5});
```  
``` 
SELECT * FROM [Posts] WHERE [AuthorId] NOT IN (1), 2, 3, 4, 5)
```  
  
하위 쿼리에 대해 필터링하기 위해 Query 인스턴스를 전달할 수 있습니다.  
```
var blocked = new Query("Authors").Where("Status", "blocked").Select("Id");

new Query("Posts").WhereNotIn("AuthorId", blocked);
```   
``` 
SELECT * FROM [Posts] WHERE [AuthorId] NOT IN (SELECT [Id] FROM [Authors] WHERE [Status] = 'blocked')
```    
      
주: 하위 쿼리는 하나의 열을 반환해야 합니다.  
  

### Where Exists
댓글이 하나 이상 있는 모든 글을 선택하려면 다음과 같이 하세요.   
```
new Query("Posts").WhereExists(q => q.From("Comments").WhereColumns("Comments.PostId", "=", "Posts.Id") ); 
```  
    
Sql Server에서  
```
SELECT * FROM [Posts] WHERE EXISTS (SELECT TOP (1) 1 FROM [댓글] WHERE [Id] = [Posts].[Id])
```  
  
PostgreSql에서   
```
SELECT * FROM "Posts" WHERE EXISTS (SELECT 1 FROM "Comments" WHERE "Id" = "Posts"."Id" LIMIT 1)
```  
  
SqlKata는 모든 데이터베이스 엔진에서 일관된 동작을 제공하기 위해 선택한 열을 무시하고 결과를 EXISTS로 제한하여 1로 쿼리를 최적화하려고 시도합니다.  
  

### Where Raw
WhereRaw 메서드를 사용하면 위의 메서드에서 지원되지 않는 모든 것을 작성할 수 있으므로 최대한의 유연성을 얻을 수 있습니다.
```
new Query("Posts").WhereRaw("lower(Title) = ?", "sql");
```   
```
SELECT * FROM [Posts] WHERE lower(Title) = 'sql'
```  
  
때때로 테이블/열을 엔진 식별자로 감싸는 것이 유용할 때가 있는데, 이는 PostgreSql에서처럼 데이터베이스가 대소문자를 구분하는 경우에 유용하며, 이렇게 하려면 문자열을 [ 및 ]로 감싸면 SqlKata가 해당 식별자를 넣습니다.  
```
new Query("Posts").WhereRaw("lower([Title]) = ?", "sql"); 
```   

Sql Server에서   
```
SELECT * FROM [Posts] WHERE lower([Title]) = 'sql'
```   
  
PostgreSql에서   
```
SELECT * FROM "Posts" WHERE lower("Title") = 'sql'
```   
  

## String Operations  
https://sqlkata.com/docs/where-string   



## Date Operations       
https://sqlkata.com/docs/where-date  
   


## Limit and Offset  
Limit 및 Offset를 사용하면 데이터베이스에서 반환되는 결과 수를 제한할 수 있으며, 이 방법은 OrderBy 및 OrderByDesc 방법과 높은 상관 관계가 있습니다.   
```
// 최신 게시물 
var query = new Query("Posts").OrderByDesc("Date").Limit(10)
```   
   
Sql Server에서  
```
SELECT TOP (10) * FROM [Posts] ORDER BY [Date] DESC
```    
  
PostgreSql에서  
```
SELECT * FROM "Posts" ORDER BY "Date" DESC LIMIT 10
```    
  
MySql에서   
```
SELECT * FROM `Posts` ORDER BY `Date` DESC LIMIT 10
```    
    
    
### 레코드 건너뛰기(오프셋)
일부 레코드를 건너뛰려면 오프셋 방법을 사용합니다.  
  
```
// latest posts
var query = new Query("Posts").OrderByDesc("Date").Limit(10).Offset(5); 
```  
       
In Sql Server  
```
SELECT * FROM [Posts] ORDER BY [Date] DESC OFFSET 5 ROWS FETCH NEXT 10 ROWS
```  
In Legacy Sql Server (< 2012)  
```
SELECT * FROM (SELECT *, ROW_NUMBER() OVER (ORDER BY [Date] DESC) AS [row_num] FROM [Posts]) AS [subquery] WHERE [row_num] BETWEEN 6 AND 15
```   
In PostgreSql  
```
SELECT * FROM "Posts" ORDER BY "Date" DESC LIMIT 10 OFFSET 5
```  
In MySql    
```
SELECT * FROM `Posts` ORDER BY `Date` DESC LIMIT 10 OFFSET 5
```   
  

### 데이터 페이지 매김
ForPage 메서드를 사용하여 데이터를 쉽게 페이지 매김할 수 있습니다.   
   
```
var posts = new Query("Posts").OrderByDesc("Date").ForPage(2);
```
   
기본적으로 이 메서드는 페이지당 15 행을 반환하며, 두 번째 매개변수로 정수를 전달하여 이 값을 재정의할 수 있습니다.  
  
주: ForPage는 1 기반이므로 첫 번째 페이지에 1을 전달합니다.  
  
```
var posts = new Query("Posts").OrderByDesc("Date").ForPage(3, 50);
```  


### Skip & Take   
Linq 배경에서 오신다면 여기에 보너스가 있습니다. 스킵 및 테이크 메서드를 오프셋 및 리밋의 별칭으로 사용할 수 있으니 즐겨보세요 :)

  
   
## Join
https://sqlkata.com/docs/join  
  
   

## Group
https://sqlkata.com/docs/group  
  
   

## Order 
https://sqlkata.com/docs/order  



## Having
https://sqlkata.com/docs/having  



## 여러 쿼리 결합하기  
  
### Union / Except / Intersect
SqlKata를 사용하면 Union, UnionAll, Intersect, IntersectAll, Except 및 ExceptAll 메서드를 제공하여 사용 가능한 연산자 union, intersect 및 except 중 하나를 사용하여 여러 쿼리를 결합할 수 있습니다.  
  
위의 메서드는 Query 인스턴스 또는 labmda 식을 허용합니다.  
```
var phones = new Query("Phones");
var laptops = new Query("Laptops");

var mobiles = laptops.Union(phones);
```   

```    
(SELECT * FROM [Laptops]) UNION (SELECT * FROM [Phones])
```  
  
또는 랩엠다 오버로드를 사용하여
```  
var mobiles = new Query("Laptops").ExceptAll(q => q.From("OldLaptops"));
```   
   
```   
(SELECT * FROM [노트북]) EXCEPT ALL (SELECT * FROM [OldLaptops])
```   
  
  
### 원시 표현식 결합
언제든지 CombineRaw 메서드를 사용하여 원시 식을 추가할 수 있습니다.  
```
var mobiles = new Query("Laptops").CombineRaw("union all select * from OldLaptops");
```  
   
```
SELECT * FROM [Laptops] union all select * from OldLaptops 
```     
  
물론 테이블 식별자 문자 [ 및 ]를 사용하여 테이블/열 키워드를 래핑하도록 SqlKata에 지시할 수 있습니다.   
```
var mobiles = new Query("Laptops").CombineRaw("[OldLaptops]에서 * 모두 선택");
```  
    
```
SELECT * FROM [Laptops] union all select * from [OldLaptops]
```   
     
    
## Common Table Expression
https://sqlkata.com/docs/cte  


   
## Advanced methods
https://sqlkata.com/docs/advanced  
  


## Insert, Update and Delete  
참고: 현재 삽입, 업데이트 및 삭제 문에서는 다음과 같은 절이 완전히 무시됩니다: 사용, 주문 기준, 그룹화 기준, 가지고, 조인, 제한, 오프셋 및 구별.  
  
### Insert  
```
var query = new Query("Books").AsInsert(new {
    Title = "Toyota Kata",
    CreatedAt = new DateTime(2009, 8, 4),
    Author = "Mike Rother"
});
```  

```   
INSERT INTO [Books] ([Title], [CreatedAt], [Author]) VALUES ('Toyota Kata', '2009-08-04 00:00:00', 'Mike Rother')
```  
  
Note: 쿼리를 실행하는 동안 InsertGetId() 메서드를 사용하여 삽입된 ID를 가져올 수 있습니다.   
  

### Insert Many
you can use the insert many overload to insert multiple records  
  
```
var cols = new [] {"Name", "Price"};

var data = new [] {
    new object[] { "A", 1000 },
    new object[] { "B", 2000 },
    new object[] { "C", 3000 },
};

var query = new Query("Products")
    .AsInsert(cols, data);try 
```  
   
```
INSERT INTO [Products] ([Name], [Price]) VALUES ("A", 1000), ("B", 2000), ("C", 3000)
```  
  

### Insert from Query
다른 선택 쿼리 결과에 대한 레코드를 삽입할 수도 있습니다.  
```
var cols = new [] { "Id", "Name", "Address" };
new Query("ActiveUsers").AsInsert(cols, new Query("Users").Where("Active", 1));
```    
   
```
INSERT INTO [ActiveUsers] ([Id], [Name], [Address]) SELECT * FROM [Users] WHERE [Active] = 1
```  
   

### Update
```
var query = new Query("Posts").WhereNull("AuthorId").AsUpdate(new {
    AuthorId = 10
});
```    
   
```
UPDATE [Posts] SET [AuthorId] = 10 WHERE [AuthorId] IS NULL
```  
   

### Delete  
```
var query = new Query("Posts").Where("Date", ">", DateTime.UtcNow.AddDays(-30)).AsDelete();
```   
  
```
DELETE FROM [Posts] WHERE [Date] > ?  
```   
  
   
   
## Update Data   
SqlKata는 데이터베이스에 대한 업데이트/삽입/삭제에 도움이 되는 다음과 같은 메서드를 제공합니다:   
- Update()
- Insert()
- InsertGetId()
- Delete()  
   
```
var db = new QueryFactory(connection, new SqlServerCompiler());
```  
  

### Update Existing Data
```
int affected = db.Query("Books").Where("Id", 1).Update(new {
    Price = 18,
    Status = "active",
});
```  
   

### Insert One Record
```
int affected = db.Query("Books").Insert(new {
    Title = "Introduction to C#",
    Price = 18,
    Status = "active",
});
```  
  

### Insert One Record and get the Inserted Id
  
```
var id = db.Query("Books").InsertGetId<int>(new {
    Title = "Introduction to Dart",
    Price = 0,
    Status = "active"
});
```  
  
Note: 현재 이 메서드는 단일 삽입 문에 대한 ID를 가져올 수 있습니다. 다중 레코드는 아직 지원되지 않습니다.  
  

### Insert Multiple Record
```
var cols = new [] {"Name", "Price"};

var data = new [] {
    new object[] { "A", 1000 },
    new object[] { "B", 2000 },
    new object[] { "C", 3000 },
};

db.Query("Products").Insert(cols, data);
```  
   

### Insert From Existing Query
```
var articlesQuery = new Query("Articles").Where("Type", "Book").Limit(100);
var columns = new [] { "Title", "Price", "Status" };

int affected = db.Query("Books").Insert(columns, articlesQuery);
```  
  

### Delete
```
int affected = db.Query("Books").Where("Status", "inactive").Delete();   
```  
   

## Fetching Records   
SqlKata는 쿼리 실행을 돕기 위해 다음과 같은 메서드를 제공합니다:  
- Get()
- First()
- FirstOrDefault()
- Paginate()
- Chunk()  
   

### 레코드 검색
기본적으로 Get 메서드를 호출하면 `IEnumerable<dynamic>` 이 반환되므로 최대한의 유연성을 제공합니다.  
```
var db = new QueryFactory(connection, new SqlServerCompiler()); 
IEnumerable<dynamic> users = db.Query("Users").Get();
```  
  
그러나 강력한 유형을 선호하는 경우 일반 오버로드를 대신 사용할 수 있습니다.  
```
IEnumerable<User< users = db.Query("Users").Get<User>();
```  
  

### 하나의 레코드 가져오기
쿼리의 첫 번째 레코드를 가져오려면 First 또는 FirstOrDefault를 사용합니다.  
```
var book = db.Query("Books").Where("Id", 1).First<Book>();
```  

주: First 및 FirstOrDefault는 Limit(1) 절을 쿼리에 암시적으로 추가하므로 직접 추가할 필요가 없습니다.  
  


### 데이터 페이지 매기기
데이터 페이지 매김을 하려면 Get 대신 Paginate(pageNumber, perPage?) 메서드를 사용하세요.  
  
Paginate 메서드는 페이지 번호(1 기준)와 기본값이 25인 선택적 페이지당의 두 매개 변수를 허용하고 PaginationResult 형식의 인스턴스를 반환합니다.  
  
PaginationResult는 반환된 데이터를 안전하게 반복할 수 있도록 Enumerable 인터페이스를 구현하는 Each 속성을 노출합니다.  
```
var users = query.Paginate(1, 10);

foreach(var user in users.Each)
{
    Console.WriteLine($"Id: {user.Id}, Name: {user.Name}");
}
```  
  

### 다음 및 이전
다음 및 이전 메서드를 호출하여 각각 다음/이전 페이지를 가져올 수 있습니다.  
```
var page1 = query.Paginate(1);

foreach(var item in page1.Each)
{
    // print items in the first page
}

var page2 = page1.Next(); // same as query.Paginate(2)

foreach(var item in page2.Each)
{
    // print items in the 2nd page
}
```  
     
   
### 다음 및 이전 쿼리
때로는 다음 및 이전 메서드에 대한 기본 쿼리에 액세스해야 할 수 있습니다. 이 경우 각각 다음 쿼리 및 이전 쿼리를 사용합니다.  

쿼리에 액세스하는 것이 추가적인 제약 조건을 추가하는 등 더 많은 제어를 원하는 경우 더 유용할 수 있습니다.  
```
var currentPage = query.Paginate(1, 10);

foreach(var item in currentPage.Each)
{
    // print all books in the first page
}

var publishedInPage2 = currentPage.NextQuery().WhereTrue("IsPublished").Get();

foreach(var item in publishedInPage2.Each)
{
    // print published books only in page 2
}
```  
  

### 모든 레코드 반복 예제
이 예제는 실제 사례에서는 사용되지 않을 수 있으며, 이러한 기능이 필요한 경우 대신 Chunk 메서드를 사용하십시오.  
```
var currentPage = db.Query("Books").OrderBy("Date").Paginate(1);

while(currentPage.HasNext)
{
    Console.WriteLine($"Looping over the page: {currentPage.Page}");

    foreach(var book in currentPage.Each)
    {
        // process book
    }

    currentPage = currentPage.Next();
}
```   
   

### 데이터 청크
전체 테이블이 메모리에 한 번 로드되는 것을 방지하기 위해 데이터를 청크로 검색하고 싶을 때가 있는데, 이 경우 청크 메서드를 사용할 수 있습니다.
  
이 방법은 수천 개의 레코드가 있는 상황에서 유용합니다.   
```
query.Chunk(100, (rows, page) => {

    Console.WriteLine($"Fetching page: {page}");

    foreach(var row in rows)
    {
        // do something with row
    }

});
```  
   
청크 검색을 중지하려면 `Chunk(int chunkSize, Func<IEnumerable<dynamic>, int, bool> func)` 오버로드를 사용하고 호출된 액션에서 false를 반환하기만 하면 됩니다.    
```
query.Chunk(100, (rows, page) => {

    // process rows

    if(page == 3) {

        // stop retrieving other chunks
        return false;

    }

    // return true to continue
    return true;

```  
  
  
### Execute Raw Statements
QueryFactory.Select 및 QueryFactory.Statement 메서드를 사용합니다.  
```
var users = db.Select("exec sp_get_users_by_date @date", new {date = DateTime.UtcNow}); 
```  
      
QueryFactory.Statement를 사용하면 truncate table, create database 등과 같은 임의의 문을 실행할 수 있습니다.   
```
db.Statement("truncate table Users");
```  
