using SimpleBlog.SeedWorks;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SimpleBlog.Models
{
    public class Category : Entity
    {
        [Required]
        [StringLength(200)]
        public string Name { get; set; }

        [Required]
        [StringLength(100)]
        public string Slug { get; set; }

        public int? ParentId { get; set; }

        public Category Parent { get; set; }

        public IList<Category> Subcategories { get; } = new List<Category>();

        public IList<CategoryPost> CategoryPosts { get; } = new List<CategoryPost>();
    }
}
