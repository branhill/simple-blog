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
        private readonly DbSet<Post> _posts;

        public PostService(AppDbContext dbContext)
        {
            _posts = dbContext.Posts;
        }

        public async Task<Post> GetBy(Expression<Func<Post, bool>> predicate)
        {
            var post = await _posts.AsNoTracking()
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

        public Task<Post> GetById(int id)
        {
            return GetBy(p => p.Id == id);
        }

        public Task<Post> GetBySlug(string slug)
        {
            return GetBy(p => p.Slug == slug);
        }

        public async Task<PaginatedList<Post>> ListBy(Func<IQueryable<Post>, IQueryable<Post>> expression,
            int pageIndex, int pageSize = 10)
        {
            var query = expression(_posts.AsNoTracking());
            var list = await PaginatedList<Post>.CreateAsync(query, pageIndex, pageSize);

            Guard.Against.NullOrEmptyThrow404NotFound(list, nameof(list));
            return list;
        }
    }
}
