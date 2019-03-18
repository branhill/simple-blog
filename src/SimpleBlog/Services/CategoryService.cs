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

        private readonly AppDbContext _dbContext;
        private readonly DbSet<Category> _categories;
        private readonly IMemoryCache _memoryCache;

        public CategoryService(AppDbContext dbContext, IMemoryCache memoryCache)
        {
            _categories = dbContext.Categories;
            _dbContext = dbContext;
            _memoryCache = memoryCache;
        }

        public async Task<Category> GetBySlug(string slug)
        {
            var category = await _categories
                .FirstOrDefaultAsync(c => c.Slug == slug);

            Guard.Against.NullOrEmptyThrow404NotFound(category, nameof(category));
            return category;
        }

        public async Task<IList<Category>> List(bool isFlat = false)
        {
            var key = GetCacheKeyPrefix() + isFlat;

            return await _memoryCache.GetOrCreateAsync(key, async entry =>
            {
                CacheKeys.Add(key);
                entry.SlidingExpiration = TimeSpan.FromDays(1);

                var list = await _categories
                    .Include(c => c.Parent)
                    .Include(c => c.Subcategories)
                    .ToListAsync();
                return isFlat
                    ? list
                    : list.Where(c => c.ParentId == null).ToList();
            });
        }

        public async Task<Category> Create(Category entity)
        {
            await _categories.AddAsync(entity);
            await _dbContext.SaveChangesAsync();

            EvictAllCache();
            return entity;
        }

        public async Task<Category> Update(Category entity)
        {
            _categories.Update(entity);
            await _dbContext.SaveChangesAsync();

            EvictAllCache();
            return entity;
        }

        public async Task Delete(Category entity)
        {
            _categories.Remove(entity);
            await _dbContext.SaveChangesAsync();
            EvictAllCache();
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
