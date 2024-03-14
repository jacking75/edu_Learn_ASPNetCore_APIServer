using System.ComponentModel.DataAnnotations;

namespace ApiServer.Model
{
    public class TableMail
    {
        // MailID - Auto Increament
        public Int64 postID { get; set; }
        
        [StringLength(45)] // 받을 유저 ID
        public string ID { get; set; } = "";

        // 선물 가능한 상품
        public Int32 StarCount { get; set; }
        // 선물 날짜/시간
        public DateTime Date { get; set; }
    }
}