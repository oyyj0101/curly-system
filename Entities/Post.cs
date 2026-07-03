using System;
using System.Collections.Generic;

namespace BulletinBoard.Entities
{
    /// <summary>
    /// 貼文
    /// </summary>
    public partial class Post
    {
        /// <summary>
        /// 流水編號
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// 標題
        /// </summary>
        public string Title { get; set; } = null!;
        /// <summary>
        /// 內文
        /// </summary>
        public string Content { get; set; } = null!;
    }
}
