# DB 프로그래밍 Tips

## DB 테이블 필드 이름에 맞추어 클래스 멤버 이름 맵핑하기  
테이블의 필드 이름은 소문자로 시작하는데 클래스의 public 멤버는 대문자로 시작하는 경우 맵핑이 안되는 문제가 발생하는데 아래처럼 속성을 사용하여 해결한다.  
  
```
namespace aspnet_minimal_sample.Models
{
    public class Category
    {
        public int Id { get; set; }
        public string Slug { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? Image { get; set; }
        
        [Column("sortid")]
        public int SortId { get; set; } = 0;
        
        public int Display { get; set; } = 1;
        
        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        
        [Column("deleted_at")]
        public DateTime? DeletedAt { get; set; }
    }
}
```