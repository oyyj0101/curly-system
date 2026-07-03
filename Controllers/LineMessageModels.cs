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
