using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using SimpleBlog.Data;
using SimpleBlog.Infrastructures.GuardClauses;
using SimpleBlog.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace SimpleBlog.Services
{
    public class CategoryService
    {
        private static readonly HashSet<string> CacheKeys = new HashSet<string>();

        private readonly DbSet<Category> _categories;
        private readonly IMemoryCache _memoryCache;

        public CategoryService(AppDbContext dbContext, IMemoryCache memoryCache)
        {
            _categories = dbContext.Categories;
            _memoryCache = memoryCache;
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
            var key = GetCacheKeyPrefix() + parentId + parentSlug;
            CacheKeys.Add(key);

            return await _memoryCache.GetOrCreateAsync(key, async entry =>
            {
                entry.SlidingExpiration = TimeSpan.FromDays(1);

                if (parentId == null && !string.IsNullOrEmpty(parentSlug))
                    parentId = GetBySlug(parentSlug).Id;

                return await _categories
                    .Where(c => c.ParentId == parentId)
                    .Include(c => c.Parent)
                    .Include(c => c.Subcategories)
                        .ThenInclude(sc => sc.Subcategories)
                    .AsNoTracking()
                    .ToListAsync();
            });
        }

        public void EvictAllCache()
        {
            foreach (var key in CacheKeys.ToList())
            {
                _memoryCache.Remove(key);
                CacheKeys.Remove(key);
            }
        }

        private static string GetCacheKeyPrefix([CallerMemberName] string callerName = "") =>
            nameof(CategoryService) + callerName;
    }
}
