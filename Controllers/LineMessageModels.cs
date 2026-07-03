using System.ComponentModel.DataAnnotations.Schema;

namespace BulletinBoard.Models // 💡 如果你的命名空間是 Entities，就改成 .Entities
{
    [Table("Post")] // 🎯 關鍵就是這行，強制對應大寫的 Post 表格
    public class Post
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public string Content { get; set; } = null!;
    }
}

namespace BulletinBoard.Models
{
    // 傳送回覆給 LINE 的基本外殼
    public class LineReplyMessageDto
    {
        public string replyToken { get; set; } = string.Empty;
        public List<LineMessageDto> messages { get; set; } = new();
    }

    // 訊息本體
    public class LineMessageDto
    {
        public string type { get; set; } = "text"; // 固定為 text
        public string text { get; set; } = string.Empty;
    }
}
