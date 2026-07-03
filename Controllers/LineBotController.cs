using Microsoft.AspNetCore.Mvc;
using BulletinBoard.Entities; // 確保引用了你的資料庫實體命名空間
using BulletinBoard.Models;   // 引用剛剛建立的 DTO 命名空間
using System.Text.Json;
using System.Text;
using System.Net.Http.Headers;

namespace BulletinBoard.Controllers
{
    [ApiController]
    [Route("api/[controller]")] // 確保網址對接是 /api/LineBot
    public class LineBotController : ControllerBase
    {
        // 暫時把資料庫註解掉，或留著但不使用它
        // private readonly BulletinBoardContext _context; 

        [HttpPost]
        public async Task<IActionResult> Post() // 習慣上改成 Post()
        {
            using var reader = new StreamReader(Request.Body);
            var json = await reader.ReadToEndAsync();

            using var doc = JsonDocument.Parse(json);
            var root = doc.RootElement;

            // 【防禦機制 1】如果 LINE 只是來 Verify 驗證的（空的封包），直接回傳 200 OK
            if (!root.TryGetProperty("events", out var eventsProp) || eventsProp.GetArrayLength() == 0)
            {
                return Ok();
            }

            var firstEvent = eventsProp[0];
            if (firstEvent.TryGetProperty("type", out var typeProp) && typeProp.GetString() == "message")
            {
                var messageProp = firstEvent.GetProperty("message");
                if (messageProp.TryGetProperty("type", out var msgTypeProp) && msgTypeProp.GetString() == "text")
                {
                    string userText = messageProp.GetProperty("text").GetString() ?? "";
                    string replyToken = firstEvent.GetProperty("replyToken").GetString() ?? "";

                    // 【防禦機制 2】改用寫死的假資料測試，不連本機資料庫
                    string replyMessage = $"你說了：{userText}！不關機測試成功！";

                    // TODO: 這裡放你原本回傳給 LINE 的 HttpClient 發送代碼 (ReplyMessage)
                    // await ReplyToLineAsync(replyToken, replyMessage); 
                }
            }

            return Ok(); // 確保一定回傳 200 OK
        }
    }
}