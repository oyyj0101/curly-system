using Microsoft.AspNetCore.Mvc;
using BulletinBoard.Entities; // 確保引用了你的資料庫實體命名空間
using BulletinBoard.Models;   // 引用剛剛建立的 DTO 命名空間
using System.Text.Json;
using System.Text;
using System.Net.Http.Headers;

namespace BulletinBoard.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LineBotController : ControllerBase
    {
        private readonly BulletinBoardContext _context;
        private readonly HttpClient _httpClient;

        // Channel Access Token金鑰
        private readonly string _channelAccessToken = "oU6vzFdBk0 + B8SqEg34N5cB2 / JInDe7t727gFPEB5paqNMu9g1fv2qibhZ / emt5 + JHrb6Apo45UVtIuCxe + Cj / cB6KirDcN82vkuDZlwVkv7tn8irTX5nAv14ii + xY / wJHIoXV0A / 67 + mQapBLnJHAdB04t89 / 1O/w1cDnyilFU=";

        // 注入資料庫上下文
        public LineBotController(BulletinBoardContext context)
        {
            _context = context;
            _httpClient = new HttpClient();
        }

        [HttpPost]
        public async Task<IActionResult> Webhook()
        {
            try
            {
                using var reader = new StreamReader(Request.Body);
                var json = await reader.ReadToEndAsync();

                // 1. 解析 LINE 傳來的 JSON
                using var doc = JsonDocument.Parse(json);
                var root = doc.RootElement;

                // 確保有 events 且長度大於 0
                if (root.TryGetProperty("events", out var eventsProp) && eventsProp.GetArrayLength() > 0)
                {
                    var firstEvent = eventsProp[0];

                    // 檢查事件類型是否為訊息事件 (message)
                    if (firstEvent.GetProperty("type").GetString() == "message")
                    {
                        var messageProp = firstEvent.GetProperty("message");

                        // 確保使用者傳送的是文字訊息
                        if (messageProp.GetProperty("type").GetString() == "text")
                        {
                            string userText = messageProp.GetProperty("text").GetString() ?? "";
                            string replyToken = firstEvent.GetProperty("replyToken").GetString() ?? "";

                            // 2. 去資料庫進行關鍵字模糊搜尋 (撈取 Title 或 Content 包含關鍵字的貼文)
                            var matchedPosts = _context.Post
                                .Where(p => p.Title.Contains(userText) || p.Content.Contains(userText))
                                .Take(5) // 最多撈出 5 筆，避免訊息太長
                                .ToList();

                            // 3. 組裝回應給使用者的文字
                            string replyText = "";
                            if (matchedPosts.Any())
                            {
                                replyText = $"🔍 幫您找到以下與「{userText}」相關的貼文：\n\n";
                                foreach (var post in matchedPosts)
                                {
                                    replyText += $"📌【{post.Title}】\n📝 {post.Content}\n\n";
                                }
                            }
                            else
                            {
                                replyText = $" 抱歉，資料庫中找不到與「{userText}」相關的貼文。";
                            }

                            // 4. 呼叫 LINE API 回傳訊息
                            await ReplyToLineAsync(replyToken, replyText);
                        }
                    }
                }

                return Ok();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error: {ex.Message}");
                return Ok();
            }
        }

        // 發送 POST 請求給 LINE 回覆 API 的方法
        private async Task ReplyToLineAsync(string replyToken, string text)
        {
            var url = "https://api.line.me/v2/bot/message/reply";

            // 建立 LINE 規定的 DTO 物件
            var responseData = new LineReplyMessageDto
            {
                replyToken = replyToken,
                messages = new List<LineMessageDto>
                {
                    new LineMessageDto { text = text }
                }
            };

            // 轉成 JSON 字串
            var payload = JsonSerializer.Serialize(responseData);
            var content = new StringContent(payload, Encoding.UTF8, "application/json");

            // 設定認證 Header (Bear Token)
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", _channelAccessToken);

            // 發送請求
            var response = await _httpClient.PostAsync(url, content);

            if (!response.IsSuccessStatusCode)
            {
                var errorLog = await response.Content.ReadAsStringAsync();
                System.Diagnostics.Debug.WriteLine($"LINE 回覆失敗原因: {errorLog}");
            }
        }
    }
}