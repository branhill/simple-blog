using Microsoft.EntityFrameworkCore;
using SimpleBlog.Data;
using SimpleBlog.Infrastructures.GuardClauses;
using SimpleBlog.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SimpleBlog.Services
{
    public class TagService
    {
        private readonly DbSet<Tag> _tags;

        public TagService(AppDbContext dbContext)
        {
            _tags = dbContext.Tags;
        }

        public async Task<Tag> GetBySlug(string slug)
        {
            var tag = await _tags
                .FirstOrDefaultAsync(t => t.Slug == slug);

            Guard.Against.NullThrow404NotFound(tag, nameof(tag));
            return tag;
        }

        public async Task<IList<Tag>> List()
        {
            return await _tags.ToListAsync();
        }
    }
}
