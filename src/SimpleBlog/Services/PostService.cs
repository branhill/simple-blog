using Microsoft.EntityFrameworkCore;
using SimpleBlog.Data;
using SimpleBlog.Infrastructures.GuardClauses;
using SimpleBlog.Models;
using SimpleBlog.SeedWorks;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace SimpleBlog.Services
{
    public class PostService
    {
        private readonly AppDbContext _dbContext;
        private readonly DbSet<Post> _posts;

        public PostService(AppDbContext dbContext)
        {
            _dbContext = dbContext;
            _posts = dbContext.Posts;
        }

        public async Task<Post> GetBy(Expression<Func<Post, bool>> predicate)
        {
            var post = await _posts
                .Include(p => p.Author)
                .Include(p => p.Category)
                .Include(p => p.TagPosts)
                    .ThenInclude(tp => tp.Tag)
                .Include(p => p.Comments)
                    .ThenInclude(c => c.RegisteredAuthor)
                .FirstOrDefaultAsync(predicate);

            Guard.Against.NullThrow404NotFound(post, nameof(post));
            return post;
        }

        public Task<Post> GetById(int id) =>
            GetBy(p => p.Id == id);

        public Task<Post> GetBySlug(string slug) =>
            GetBy(p => p.Slug == slug);

        public async Task<PaginatedList<Post>> ListBy(Expression<Func<Post, bool>> predicate,
            int pageIndex, int pageSize = 10)
        {
            var query = _posts
                .Where(predicate)
                .Include(p => p.Author)
                .Include(p => p.Category)
                .OrderByDescending(p => p.CreatedTime);
            return await PaginatedList<Post>.CreateAsync(query, pageIndex, pageSize);
        }

        public async Task<Post> Create(Post entity)
        {
            await _posts.AddAsync(entity);
            await _dbContext.SaveChangesAsync();

            return entity;
        }

        public async Task<Post> Update(Post entity)
        {
            _posts.Update(entity);
            await _dbContext.SaveChangesAsync();

            return entity;
        }

        public async Task Delete(Post entity)
        {
            _posts.Remove(entity);
            await _dbContext.SaveChangesAsync();
        }
    }
}
