using Microsoft.AspNetCore.Mvc;
using BulletinBoard.Entities;

namespace BulletinBoard.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostController : ControllerBase
    {
        private readonly BulletinBoardContext _context;

        public PostController(BulletinBoardContext context)
        {
            _context = context;
        }

        // 1. 【查詢所有留言】 GET: api/Post
        [HttpGet]
        public ActionResult<IEnumerable<Post>> GetPosts()
        {
            var posts = _context.Post.ToList();
            return Ok(posts);
        }

        // 2. 【查詢單筆留言】 GET: api/Post/5
        [HttpGet("{id}")]
        public ActionResult<Post> GetPost(int id)
        {
            // 這裡改成 _context.Post
            var post = _context.Post.Find(id);

            if (post == null)
            {
                return NotFound("找不到這篇貼文！");
            }

            return Ok(post);
        }

        // 3. 【新增留言】 POST: api/Post
        [HttpPost]
        public ActionResult<Post> CreatePost(Post post)
        {
            // 這裡改成 _context.Post
            _context.Post.Add(post);
            _context.SaveChanges();

            return CreatedAtAction(nameof(GetPost), new { id = post.Id }, post);
        }

        // 4. 【修改留言】 PUT: api/Post/5
        [HttpPut("{id}")]
        public IActionResult UpdatePost(int id, Post updatedPost)
        {
            // 這裡改成 _context.Post
            var postInDb = _context.Post.Find(id);

            if (postInDb == null)
            {
                return NotFound("找不到該筆貼文，無法更新！");
            }

            postInDb.Title = updatedPost.Title;
            postInDb.Content = updatedPost.Content;

            _context.SaveChanges();

            return NoContent();
        }

        // 5. 【刪除留言】 DELETE: api/Post/5
        [HttpDelete("{id}")]
        public IActionResult DeletePost(int id)
        {
            // 這裡改成 _context.Post
            var post = _context.Post.Find(id);

            if (post == null)
            {
                return NotFound("找不到該筆貼文，無法刪除！");
            }

            // 這裡改成 _context.Post
            _context.Post.Remove(post);
            _context.SaveChanges();

            return Ok("貼文已成功刪除！");
        }
    }
}