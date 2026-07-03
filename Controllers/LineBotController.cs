using Microsoft.AspNetCore.Mvc;
using BulletinBoard.Entities;
using BulletinBoard.Models;
using System.Text.Json;
using System.Text;
using System.Net.Http.Headers;
using Microsoft.EntityFrameworkCore;

namespace BulletinBoard.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LineBotController : ControllerBase
    {
        private readonly string _channelAccessToken = "oU6vzFdBk0+B8SqEg34N5cB2/JInDe7t727gFPEB5paqNMu9g1fv2qibhZ/emt5+JHrb6Apo45UVtIuCxe+Cj/cB6KirDcN82vkuDZlwVkv7tn8irTX5nAv14ii+xY/wJHIoXV0A/67+mQapBLnJHAdB04t89/1O/w1cDnyilFU=";

        private readonly BulletinBoardContext _context;     //宣告資料庫上下文

        public LineBotController(BulletinBoardContext context)      //建構子注入資料庫上下文
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> Post()
        {
            using var reader = new StreamReader(Request.Body);
            var json = await reader.ReadToEndAsync();

            using var doc = JsonDocument.Parse(json);
            var root = doc.RootElement;

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

                    string replyMessage = "";

                    //資料表Title與使用者輸入內容模糊比對，回復Content內容
                    var matchedPost = await _context.Post
                        .FirstOrDefaultAsync(p => EF.Functions.ILike(p.Title, $"%{userText}%"));

                    if (matchedPost != null)
                    {
                        replyMessage = matchedPost.Content;
                    }
                    else
                    {
                        replyMessage = "抱歉，找不到相關的文章。";

                    }

                    // 發送回復給Line
                    await ReplyToLineAsync(replyToken, replyMessage);
                }
            }

            return Ok();
        }

        // 這個方法用來回覆 LINE 的訊息
        private async Task ReplyToLineAsync(string replyToken, string message)
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {_channelAccessToken}");

            var requestBody = new
            {
                replyToken = replyToken,
                messages = new[]
                {
                    new { type = "text", text = message }
                }
            };

            var jsonPayload = JsonSerializer.Serialize(requestBody);
            var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

            // 發送到 LINE 官方 API 節點
            await client.PostAsync("https://api.line.me/v2/bot/message/reply", content);
        }
    }
}
