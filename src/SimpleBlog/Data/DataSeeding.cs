using Bogus;
using Microsoft.AspNetCore.Identity;
using SimpleBlog.Models;
using System;
using System.Linq;

namespace SimpleBlog.Data
{
    public class DataSeeding
    {
        public static void Seed(AppDbContext dbContext, UserManager<User> userManager)
        {
            dbContext.Database.EnsureCreated();

            if (dbContext.Posts.Any() || userManager.Users.Any())
                return;

            using (var transaction = dbContext.Database.BeginTransaction())
            {
                var admin = new User
                {
                    UserName = "admin@example.com",
                    Email = "admin@example.com"
                };
                userManager.CreateAsync(admin, "Passw0rd.").Wait();

                var categoryFaker = new Faker<Category>()
                    .RuleFor(c => c.Name, f => $"{f.Lorem.Word()} {f.Lorem.Word()} {f.UniqueIndex}");
                var fakeCategories = categoryFaker.Generate(10);
                fakeCategories[1].Parent = fakeCategories[0];
                fakeCategories[2].Parent = fakeCategories[0];
                fakeCategories[3].Parent = fakeCategories[4];
                fakeCategories[5].Parent = fakeCategories[1];
                dbContext.Categories.AddRange(fakeCategories);
                dbContext.SaveChanges();

                var tagFaker = new Faker<Tag>()
                    .RuleFor(t => t.Name, f => f.Lorem.Word() + f.UniqueIndex);
                var fakeTags = tagFaker.Generate(20);
                dbContext.Tags.AddRange();
                dbContext.SaveChanges();

                var commentFaker = new Faker<Comment>()
                    .RuleFor(c => c.AuthorName, f => f.Name.FullName())
                    .RuleFor(c => c.AuthorEmail, f => f.Internet.Email())
                    .RuleFor(c => c.Content, f => f.Lorem.Paragraph());

                var postFaker = new Faker<Post>()
                    .RuleFor(p => p.Title, f => f.Lorem.Sentence().TrimEnd('.'))
                    .RuleFor(p => p.Author, f => admin)
                    .RuleFor(p => p.IsDraft, f => false)
                    .RuleFor(p => p.Excerpt, f => f.Lorem.Paragraph())
                    .RuleFor(p => p.Content, f => Enumerable.Range(0, f.Random.Int(10, 50))
                        .Select(_ => $"<p>{f.Lorem.Paragraph()}</p>")
                        .Aggregate((sum, next) => sum + Environment.NewLine + next))
                    .RuleFor(p => p.Category, f => fakeCategories[f.Random.Int(0, fakeCategories.Count - 1)])
                    .RuleFor(p => p.TagPosts, (f, p) => Enumerable.Range(0, f.Random.Int(0, 5))
                        .Select(_ => f.PickRandom(fakeTags))
                        .Distinct()
                        .Select(t => new TagPost { Post = p, Tag = t })
                        .ToList())
                    .RuleFor(t => t.Comments, f => commentFaker.Generate(f.Random.Int(0, 5)));
                var fakePosts = postFaker.Generate(100);
                dbContext.Posts.AddRange(fakePosts);
                dbContext.SaveChanges();

                transaction.Commit();
            }
        }
    }
}
