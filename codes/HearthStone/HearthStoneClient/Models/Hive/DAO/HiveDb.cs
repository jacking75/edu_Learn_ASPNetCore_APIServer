namespace HearthStoneWeb.Models.Hive;


public class HiveDBAccount
{
    public long account_uid { get; set; }         // 유니크 유저 번호 (PK)
    public string email_id { get; set; }          // 유저아이디(이메일)
    public string nickname { get; set; }          // 유저 닉네임
    public string pw { get; set; }                // 해싱된 비밀번호
    public string salt_value { get; set; }        // 암호화 값
    public DateTime create_dt { get; set; }       // 생성 일시
}
