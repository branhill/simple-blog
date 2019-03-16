using Microsoft.EntityFrameworkCore;
using SimpleBlog.Data;
using SimpleBlog.Infrastructures.GuardClauses;
using SimpleBlog.Models;
using System.Threading.Tasks;

namespace SimpleBlog.Services
{
    public class CategoryService
    {
        private readonly DbSet<Category> _categories;

        public CategoryService(AppDbContext dbContext)
        {
            _categories = dbContext.Categories;
        }

        public async Task<Category> GetBySlug(string slug)
        {
            var list = await _categories.AsNoTracking()
                .FirstOrDefaultAsync(c => c.Slug == slug);

            Guard.Against.NullOrEmptyThrow404NotFound(list, nameof(list));
            return list;
        }
    }
}
