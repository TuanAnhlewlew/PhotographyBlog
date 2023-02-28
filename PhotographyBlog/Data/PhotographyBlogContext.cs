using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace PhotographyBlog.Data;

public partial class PhotographyBlogContext : DbContext
{
    public PhotographyBlogContext()
    {
    }

    public PhotographyBlogContext(DbContextOptions<PhotographyBlogContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Album> Albums { get; set; }

    public virtual DbSet<Comment> Comments { get; set; }

    public virtual DbSet<Interest> Interests { get; set; }

    public virtual DbSet<Photo> Photos { get; set; }

    public virtual DbSet<Visitor> Visitors { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseMySql("name=photographyblogConnect", Microsoft.EntityFrameworkCore.ServerVersion.Parse("8.0.28-mysql"));

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("utf8mb4_0900_ai_ci")
            .HasCharSet("utf8mb4");

        modelBuilder.Entity<Album>(entity =>
        {
            entity.HasKey(e => e.AlbumId).HasName("PRIMARY");

            entity.ToTable("Album", tb => tb.HasComment("Store Album information"));

            entity.Property(e => e.AlbumId).HasColumnName("AlbumID");
            entity.Property(e => e.Active)
                .IsRequired()
                .HasDefaultValueSql("'1'");
            entity.Property(e => e.Description).HasMaxLength(1000);
            entity.Property(e => e.Name).HasMaxLength(256);
            entity.Property(e => e.TimeCreate).HasMaxLength(128);
            entity.Property(e => e.TimeLastEdit).HasMaxLength(128);
        });

        modelBuilder.Entity<Comment>(entity =>
        {
            entity.HasKey(e => e.CommentId).HasName("PRIMARY");

            entity.ToTable("Comment");

            entity.HasIndex(e => e.AlbumId, "Comment_FK_1");

            entity.HasIndex(e => e.PhotoId, "Comment_FK_2");

            entity.HasIndex(e => e.VisitorId, "Comment_FK_3");

            entity.Property(e => e.CommentId).HasColumnName("CommentID");
            entity.Property(e => e.AlbumId).HasColumnName("AlbumID");
            entity.Property(e => e.Comment1)
                .HasMaxLength(256)
                .HasColumnName("Comment");
            entity.Property(e => e.PhotoId).HasColumnName("PhotoID");
            entity.Property(e => e.Time).HasMaxLength(128);
            entity.Property(e => e.VisitorId).HasColumnName("VisitorID");

            entity.HasOne(d => d.Album).WithMany(p => p.Comments)
                .HasForeignKey(d => d.AlbumId)
                .HasConstraintName("Comment_FK_1");

            entity.HasOne(d => d.Photo).WithMany(p => p.Comments)
                .HasForeignKey(d => d.PhotoId)
                .HasConstraintName("Comment_FK_2");

            entity.HasOne(d => d.Visitor).WithMany(p => p.Comments)
                .HasForeignKey(d => d.VisitorId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Comment_FK_3");
        });

        modelBuilder.Entity<Interest>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("Interest");

            entity.HasIndex(e => e.VisitorId, "Interest_FK_1");

            entity.HasIndex(e => e.PhotoId, "Interest_FK_2");

            entity.HasIndex(e => e.AlbumId, "Interest_FK_3");

            entity.Property(e => e.AlbumId).HasColumnName("AlbumID");
            entity.Property(e => e.PhotoId).HasColumnName("PhotoID");
            entity.Property(e => e.Time).HasMaxLength(128);
            entity.Property(e => e.VisitorId).HasColumnName("VisitorID");

            entity.HasOne(d => d.Album).WithMany()
                .HasForeignKey(d => d.AlbumId)
                .HasConstraintName("Interest_FK_3");

            entity.HasOne(d => d.Photo).WithMany()
                .HasForeignKey(d => d.PhotoId)
                .HasConstraintName("Interest_FK_2");

            entity.HasOne(d => d.Visitor).WithMany()
                .HasForeignKey(d => d.VisitorId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Interest_FK_1");
        });

        modelBuilder.Entity<Photo>(entity =>
        {
            entity.HasKey(e => e.PhotoId).HasName("PRIMARY");

            entity.ToTable("Photo", tb => tb.HasComment("Store photo information"));

            entity.HasIndex(e => e.AlbumId, "Photo_FK_1");

            entity.Property(e => e.PhotoId).HasColumnName("PhotoID");
            entity.Property(e => e.Active)
                .IsRequired()
                .HasDefaultValueSql("'1'");
            entity.Property(e => e.AlbumId).HasColumnName("AlbumID");
            entity.Property(e => e.Link).HasMaxLength(1024);
            entity.Property(e => e.Name).HasMaxLength(256);
            entity.Property(e => e.TimeCreate).HasMaxLength(128);

            entity.HasOne(d => d.Album).WithMany(p => p.Photos)
                .HasForeignKey(d => d.AlbumId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Photo_FK_1");
        });

        modelBuilder.Entity<Visitor>(entity =>
        {
            entity.HasKey(e => e.VisitorId).HasName("PRIMARY");

            entity.ToTable("Visitor", tb => tb.HasComment("Visitor List"));

            entity.Property(e => e.VisitorId).HasColumnName("VisitorID");
            entity.Property(e => e.Name).HasMaxLength(10);
            entity.Property(e => e.TimeFirstVisit).HasMaxLength(128);
            entity.Property(e => e.TimeLastVisit).HasMaxLength(128);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
