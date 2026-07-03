using Microsoft.AspNetCore.Mvc;
using BulletinBoard.Entities;
using BulletinBoard.Models;
using System.Text.Json;
using System.Text;
using System.Net.Http.Headers;

namespace BulletinBoard.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LineBotController : ControllerBase
    {
        private readonly string _channelAccessToken = "oU6vzFdBk0+B8SqEg34N5cB2/JInDe7t727gFPEB5paqNMu9g1fv2qibhZ/emt5+JHrb6Apo45UVtIuCxe+Cj/cB6KirDcN82vkuDZlwVkv7tn8irTX5nAv14ii+xY/wJHIoXV0A/67+mQapBLnJHAdB04t89/1O/w1cDnyilFU=";

        [HttpPost]
        public async Task<IActionResult> Post()
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

                    // 🎯 【關鍵修正】把原本前面的 // 拿掉，真正啟用發送功能！
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
