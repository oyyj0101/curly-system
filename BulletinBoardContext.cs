using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using BulletinBoard.Entities;

namespace BulletinBoard
{
    public partial class BulletinBoardContext : DbContext
    {
        public BulletinBoardContext()
        {
        }

        public BulletinBoardContext(DbContextOptions<BulletinBoardContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Post> Post { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

    // 🎯 【就是這行大絕招】強制將 Post 實體對應到 PostgreSQL 中大寫的 "Post" 資料表
    modelBuilder.Entity<Post>().ToTable("Post");
            modelBuilder.UseCollation("utf8mb4_0900_ai_ci")
                .HasCharSet("utf8mb4");

            modelBuilder.Entity<Post>(entity =>
            {
                entity.ToTable("post");

                entity.HasComment("貼文");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasComment("流水編號");

                entity.Property(e => e.Content)
                    .HasColumnType("text")
                    .HasColumnName("content")
                    .HasComment("內文");

                entity.Property(e => e.Title)
                    .HasMaxLength(100)
                    .HasColumnName("title")
                    .HasComment("標題");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
