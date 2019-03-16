using Microsoft.EntityFrameworkCore;
using SimpleBlog.Data;
using SimpleBlog.Infrastructures.GuardClauses;
using SimpleBlog.Models;
using System.Collections.Generic;
using System.Linq;
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
            var category = await _categories.AsNoTracking()
                .FirstOrDefaultAsync(c => c.Slug == slug);

            Guard.Against.NullOrEmptyThrow404NotFound(category, nameof(category));
            return category;
        }

        public async Task<IList<Category>> ListByParent(int? parentId = null, string parentSlug = null)
        {
            if (parentId == null && !string.IsNullOrEmpty(parentSlug))
                parentId = GetBySlug(parentSlug).Id;

            return await _categories
                .Where(c => c.ParentId == parentId)
                .Include(c => c.Parent)
                .Include(c => c.Subcategories)
                    .ThenInclude(sc => sc.Subcategories)
                .AsNoTracking()
                .ToListAsync();
        }
    }
}
