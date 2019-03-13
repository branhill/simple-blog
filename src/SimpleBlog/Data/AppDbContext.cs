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

        public DbSet<Tag> Tags { get; set; }

        public DbSet<TagPost> TagPosts { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Post>(ConfigurePost);
            builder.Entity<Category>(ConfigureCategory);
            builder.Entity<Tag>(ConfigureTag);
            builder.Entity<TagPost>(ConfigureTagPost);
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

        private static void ConfigureTag(EntityTypeBuilder<Tag> builder)
        {
            builder.HasIndex(t => t.Slug)
                .IsUnique();
        }

        private static void ConfigureTagPost(EntityTypeBuilder<TagPost> builder)
        {
            builder.HasKey(tp => new { tp.TagId, tp.PostId });
        }
    }
}
