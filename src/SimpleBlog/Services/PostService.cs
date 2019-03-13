using Microsoft.EntityFrameworkCore;
using SimpleBlog.Data;
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

        public Task<Post> GetBy(Expression<Func<Post, bool>> predicate)
        {
            return _posts.AsNoTracking()
                .Include(p => p.Category)
                .Include(p => p.TagPosts)
                .ThenInclude(tp => tp.Tag)
                .Include(p => p.Comments)
                .SingleAsync(predicate);
        }

        public Task<Post> GetById(int id)
        {
            return GetBy(p => p.Id == id);
        }

        public Task<Post> GetBySlug(string slug)
        {
            return GetBy(p => p.Slug == slug);
        }

        public Task<PaginatedList<Post>> ListBy(Func<IQueryable<Post>, IQueryable<Post>> expression,
            int pageIndex, int pageSize)
        {
            var query = expression(_posts.AsNoTracking());
            return PaginatedList<Post>.CreateAsync(query, pageIndex, pageSize);
        }
    }
}
