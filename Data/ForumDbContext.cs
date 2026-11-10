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
    public DbSet<User> Users => Set<User>();
    public DbSet<UserSession> UserSessions => Set<UserSession>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // utf8mb4 is required to store Lithuanian letters (ą, č, ę...) correctly.
        modelBuilder.HasCharSet("utf8mb4");

        ConfigureCategory(modelBuilder);
        ConfigurePost(modelBuilder);
        ConfigureComment(modelBuilder);
        ConfigureUser(modelBuilder);
        ConfigureUserSession(modelBuilder);

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

            post.HasOne(p => p.Author)
                .WithMany(u => u.Posts)
                .HasForeignKey(p => p.AuthorId)
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

            comment.HasOne(c => c.Author)
                .WithMany(u => u.Comments)
                .HasForeignKey(c => c.AuthorId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }

    private static void ConfigureUser(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(user =>
        {
            user.Property(u => u.UserName).IsRequired().HasMaxLength(User.UserNameMaxLength);
            user.Property(u => u.Email).IsRequired().HasMaxLength(User.EmailMaxLength);
            user.Property(u => u.PasswordHash).IsRequired();
            user.Property(u => u.Role).IsRequired().HasMaxLength(User.RoleMaxLength);
            user.HasIndex(u => u.UserName).IsUnique();
            user.HasIndex(u => u.Email).IsUnique();
        });
    }

    private static void ConfigureUserSession(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<UserSession>(session =>
        {
            session.Property(s => s.RefreshTokenHash)
                .IsRequired()
                .HasMaxLength(UserSession.RefreshTokenHashLength)
                .IsFixedLength();

            session.HasOne(s => s.User)
                .WithMany(u => u.Sessions)
                .HasForeignKey(s => s.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
