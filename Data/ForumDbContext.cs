using DndForum.Models;
using Microsoft.EntityFrameworkCore;

namespace DndForum.Data;

public class ForumDbContext : DbContext
{
    public ForumDbContext(DbContextOptions<ForumDbContext> options) : base(options)
    {
    }

    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Post> Posts => Set<Post>();
    public DbSet<Comment> Comments => Set<Comment>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // utf8mb4 is required to store Lithuanian letters (ą, č, ę...) correctly.
        modelBuilder.HasCharSet("utf8mb4");

        ConfigureCategory(modelBuilder);
        ConfigurePost(modelBuilder);
        ConfigureComment(modelBuilder);

        ForumSeedData.Apply(modelBuilder);
    }

    private static void ConfigureCategory(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Category>(category =>
        {
            category.Property(c => c.Name).IsRequired().HasMaxLength(Category.NameMaxLength);
            category.Property(c => c.Description).HasMaxLength(Category.DescriptionMaxLength);
            category.HasIndex(c => c.Name).IsUnique();
        });
    }

    private static void ConfigurePost(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Post>(post =>
        {
            post.Property(p => p.Title).IsRequired().HasMaxLength(Post.TitleMaxLength);
            post.Property(p => p.Content).IsRequired().HasMaxLength(Post.ContentMaxLength);

            post.HasOne(p => p.Category)
                .WithMany(c => c.Posts)
                .HasForeignKey(p => p.CategoryId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }

    private static void ConfigureComment(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Comment>(comment =>
        {
            comment.Property(c => c.Content).IsRequired().HasMaxLength(Comment.ContentMaxLength);

            comment.HasOne(c => c.Post)
                .WithMany(p => p.Comments)
                .HasForeignKey(c => c.PostId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
