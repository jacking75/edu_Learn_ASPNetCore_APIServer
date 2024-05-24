# Model
## Model Binding
1) Form Values
- Request의 Body에서 보낸 값(HTTP POST 방식의 요청)
2) Routes Values
- URL 매칭, Default Value
3) Query String Values
- URL 끝에 붙이는 방법 ?Name (HTTP GET 방식의 요청)

## DataAnotation
데이터의 형태를 검증해주는 Attribute임. DTO, DAO 등에 붙여서 사용함.
- `[Required]` : 무조건 있어야 한다.
- `[CreditCard]` : 올바른 결제카드 번호인지
- `[EmailAddress]` : 올바른 이메일 주소인지
- `[StringLength(max)]` : String 길이가 최대 max인지
- `[MinLength(min)]` : Collection의 크기가 최소 min인지
- `[Phone]` : 올바른 전화번호 인지
- `[Range(min, max)]` : Min~Max 사이의 값인지
- `[Url]` : 올바른 URL인지
- `[Compare]` : 2개의 Property 비교(Password, ConfirmPassword)

``` cs
public class EmailRequestDTO
{
    [Required(ErrorMessage = "4000")]
    [EmailAddress(ErrorMessage = "4001")]
    public string Email { get; set; } = null!;
}
```