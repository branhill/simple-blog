using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SimpleBlog.Models;

namespace SimpleBlog.Data
{
    public class AppDbContext : IdentityDbContext<User>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<Post> Posts { get; set; }

        public DbSet<Comment> Comments { get; set; }

        public DbSet<Category> Categories { get; set; }

        public DbSet<CategoryPost> CategoryPosts { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Post>(ConfigurePost);
            builder.Entity<Category>(ConfigureCategory);
            builder.Entity<CategoryPost>(ConfigureCategoryPost);
        }

        private static void ConfigurePost(EntityTypeBuilder<Post> builder)
        {
            builder.HasIndex(p => p.Slug)
                .IsUnique();
        }

        private static void ConfigureCategory(EntityTypeBuilder<Category> builder)
        {
            builder.HasIndex(c => c.Slug)
                .IsUnique();

            builder.HasOne(c => c.Parent)
                .WithMany(c => c.Subcategories);
        }

        private static void ConfigureCategoryPost(EntityTypeBuilder<CategoryPost> builder)
        {
            builder.HasKey(cp => new { cp.CategoryId, cp.PostId });
            builder.HasOne(cp => cp.Category)
                .WithMany(c => c.CategoryPosts)
                .HasForeignKey(cp => cp.CategoryId);
            builder.HasOne(cp => cp.Post)
                .WithMany(p => p.CategoryPosts)
                .HasForeignKey(cp => cp.PostId);
        }
    }
}
