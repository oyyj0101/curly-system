using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using BulletinBoard.Entities;
using BulletinBoard.Models;

namespace BulletinBoard.Entities
{
    public class BulletinBoardContext : DbContext
    {
        public BulletinBoardContext(DbContextOptions<BulletinBoardContext> options) : base(options)
        {
        }

        public DbSet<Post> Post { get; set; } = null!;

        // 🎯 確保這段程式碼有躺在你的 Context 檔案裡
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // 強制定向大寫表格！
            modelBuilder.Entity<Post>().ToTable("Post");
        }
    }
}
