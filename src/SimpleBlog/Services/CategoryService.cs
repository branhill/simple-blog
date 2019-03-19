using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using SimpleBlog.Data;
using SimpleBlog.Infrastructures.Exceptions;
using SimpleBlog.Infrastructures.GuardClauses;
using SimpleBlog.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
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

        public async Task<Category> GetBy(Expression<Func<Category, bool>> predicate)
        {
            var category = await _categories
                .FirstOrDefaultAsync(predicate);

            Guard.Against.NullThrow404NotFound(category, nameof(category));
            return category;
        }

        public Task<Category> GetById(int id) =>
            GetBy(c => c.Id == id);

        public Task<Category> GetBySlug(string slug) =>
            GetBy(c => c.Slug == slug);

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
            Guard.Against.EqualsThrowBadRequest(entity.Id, entity.ParentId, nameof(entity.Id), nameof(entity.ParentId));
            using (var transaction = _dbContext.Database.BeginTransaction())
            {
                _categories.Update(entity);
                await _dbContext.SaveChangesAsync();

                var newList = await _categories
                    .Include(c => c.Parent)
                    .Include(c => c.Subcategories)
                    .ToListAsync();

                var self = newList.First(c => c.Id == entity.Id);
                var ids = new HashSet<int>
                {
                    self.Id
                };
                var parent = self.Parent;
                while (parent != null)
                {
                    if (ids.Contains(parent.Id))
                    {
                        transaction.Rollback();
                        throw new StatusCodeException(StatusCodes.Status400BadRequest,
                            "Category circular referencing detected.");
                    }

                    ids.Add(parent.Id);
                    parent = parent.Parent;
                }

                transaction.Commit();
            }

            EvictAllCache();
            return entity;
        }

        public async Task Delete(Category entity)
        {
            var canBeDelete = await CanBeDelete(entity.Id);
            if (!canBeDelete)
                throw new StatusCodeException(StatusCodes.Status400BadRequest,
                    "Please move all posts in the category before delete it.");

            _categories.Remove(entity);
            await _dbContext.SaveChangesAsync();
            EvictAllCache();
        }

        public async Task<bool> CanBeDelete(int id)
        {
            var result = await _dbContext.Posts.AnyAsync(p => p.CategoryId == id);
            return !result;
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
