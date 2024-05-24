# C# 기본 사항

## 1. 주요 구성 요소

C#은 다양한 애플리케이션 개발에 사용되는 강력하고 유연한 객체 지향 프로그래밍 언어입니다. .NET 플랫폼에서 주로 사용되며, 아래와 같은 주요 구성 요소들로 이루어져 있습니다.

### 객체 지향 프로그래밍 (OOP)
- C#은 객체 지향 프로그래밍 언어로, 클래스와 객체를 중심으로 프로그래밍합니다.
- 이를 통해 코드 재사용성, 유지 관리성, 그리고 확장성이 향상됩니다.

### 통합 개발 환경 (IDE) 지원
- Visual Studio와 같은 IDE를 통해 C# 프로그래밍을 보다 효율적으로 할 수 있습니다.
- 코드 자동 완성, 디버깅, 테스트 도구 등 다양한 기능을 제공합니다.

### .NET Framework 및 .NET Core
- **.NET 지원**: C#은 .NET Framework와 .NET Core에서 실행되며, 다양한 유형의 애플리케이션을 개발할 수 있도록 지원합니다.

### 이벤트 및 대리자 (Delegates and Events)
- **대리자**: 대리자는 메서드를 가리키는 타입 안전한 객체로, 이벤트 처리나 콜백 메서드 구현에 주로 사용됩니다.
- **이벤트**: C#에서 이벤트는 객체가 특정 행동을 외부에 알리는 방법을 제공합니다. 이벤트와 대리자는 C#에서 강력한 이벤트 구동 프로그래밍을 가능하게 합니다.
  ```csharp
  public delegate void MessageDelegate(string message);
  
  public class Publisher
  {
      public event MessageDelegate MessageReceived;
  
      public void SendMessage(string message)
      {
          MessageReceived?.Invoke(message);
      }
  }
  
  public class Subscriber
  {
      public void OnMessageReceived(string message)
      {
          Console.WriteLine($"Message received: {message}");
      }
  }
  
  var publisher = new Publisher();
  var subscriber = new Subscriber();
  
  publisher.MessageReceived += subscriber.OnMessageReceived;
  publisher.SendMessage("Hello, world!");
  ```

### 람다 식 및 LINQ (Lambda Expressions and LINQ)
- **람다 식**: 람다 식은 간결한 문법으로 메서드를 표현할 수 있게 해주는 익명 함수입니다. LINQ 쿼리 작성과 이벤트 핸들러 등에 널리 사용됩니다.
- **LINQ (Language Integrated Query)**: LINQ는 컬렉션, 데이터베이스, XML과 같은 다양한 데이터 소스에 대한 쿼리를 간편하게 작성할 수 있게 해줍니다.
  ```csharp
  List<int> numbers = new List<int> { 1, 2, 3, 4, 5 };
  var evenNumbers = numbers.Where(n => n % 2 == 0);
  
  foreach (var num in evenNumbers)
  {
      Console.WriteLine(num);
  }
  ```
### 비동기 프로그래밍 (Asynchronous Programming)
- **비동기 메서드**: C#에서 `async`와 `await` 키워드를 사용하여 비동기적으로 작업을 수행할 수 있습니다.
- 이는 I/O 바운드 및 CPU 바운드 작업의 성능을 향상시키며, 응용 프로그램의 반응성을 높입니다.
  ```csharp
  List<int> numbers = new List<int> { 1, 2, 3, 4, 5 };
  var evenNumbers = numbers.Where(n => n % 2 == 0);
  
  foreach (var num in evenNumbers)
  {
      Console.WriteLine(num);
  }
  ```

### 속성 및 인덱서 (Properties and Indexers)
- **속성**: 속성은 클래스의 데이터를 캡슐화하며, 필드에 대한 보호된 접근을 제공합니다.
- **인덱서**: 인덱서를 사용하면 객체를 배열처럼 인덱스를 사용해 접근할 수 있습니다. 이는 컬렉션 또는 클래스의 내부 데이터에 배열과 유사한 방식으로 접근하게 해줍니다.
  ```csharp
  public class Company
  {
      private List<string> employees = new List<string>();
  
      public string this[int index]
      {
          get { return employees[index]; }
          set { employees.Insert(index, value); }
      }
  }
  
  Company company = new Company();
  company[0] = "John Doe";
  Console.WriteLine(company[0]);  // Output: John Doe
  ```

## 2. 데이터 형식과 변수

- C#은 강 타입 언어로, 모든 변수와 데이터 타입은 명시적으로 선언되어야 합니다. 
- 이는 프로그램의 안정성과 성능을 향상시키는 데 도움을 줍니다.

### 기본 데이터 형식
- **정수형**: `int`, `long`, `short`
- **실수형**: `float`, `double`, `decimal`
- **논리형**: `bool`
- **문자형**: `char`
- **문자열형**: `string`

### 변수 선언
```csharp
int age = 30;
bool isCSharpFun = true;
string name = "John Doe";
```

## 3. 예외 처리 및 가비지 컬렉션
### 예외 처리
C#에서 예외 처리는 프로그램에서 예상치 못한 오류를 관리하기 위해 사용됩니다. 
try, catch, finally 블록을 사용하여 예외를 처리합니다.

#### 기본 예외 처리 구조
```csharp
try
{
    // Code that may throw an exception
    int divisor = 0;
    int result = 12 / divisor;
}
catch (DivideByZeroException ex)
{
    Console.WriteLine($"Divide by zero error: {ex.Message}");
}
catch (Exception ex)
{
    // General exception handler
    Console.WriteLine($"An error occurred: {ex.Message}");
}
finally
{
    // Code that runs after the try/catch blocks regardless of an exception
    Console.WriteLine("Cleanup code can run here.");
}
```
- 이 코드는 DivideByZeroException을 특별히 처리하며, 다른 모든 유형의 예외를 일반적인 예외 처리기에서 처리합니다. 
- finally 블록은 예외의 발생 여부에 관계없이 항상 실행되어 리소스 정리 등의 작업을 안전하게 수행할 수 있습니다.


### 가비지 컬렉션 (Garbage Collection) 
- C#의 가비지 컬렉션은 .NET의 CLR(Common Language Runtime)에 의해 자동으로 관리되며, 개발자가 메모리 관리에 신경 쓰지 않아도 되게 합니다. 
- 가비지 컬렉터는 힙 메모리 영역에서 더 이상 참조되지 않는 객체를 자동으로 찾아내고 해제하여 메모리를 효율적으로 관리합니다.
#### 가비지 컬렉션의 작동 원리
```csharp
public class Person
{
    public string Name { get; set; }
}

public class Demo
{
    public static void Main()
    {
        Person person = new Person { Name = "John Doe" };
        person = null; // Now 'Person' object is eligible for garbage collection

        GC.Collect(); // Forcing garbage collection for demonstration purposes
        Console.WriteLine("Garbage collection performed.");
    }
}
```
- 위 코드에서 Person 객체는 person 변수가 null로 설정된 후 가비지 컬렉션 대상이 됩니다. 
- GC.Collect() 메서드 호출은 일반적인 프랙티스가 아니며, 여기서는 단지 가비지 컬렉션의 효과를 시연하기 위해 사용됩니다.
- 이러한 메커니즘은 메모리 누수를 방지하고, 애플리케이션의 성능을 유지하는 데 중요한 역할을 합니다.
