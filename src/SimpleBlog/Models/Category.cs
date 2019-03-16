using SimpleBlog.SeedWorks;
using SimpleBlog.Utilities;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SimpleBlog.Models
{
    public class Category : Entity
    {
        private string _name;

        [Required]
        [StringLength(100)]
        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                Slug = value.ToSlug();
            }
        }

        [Required]
        [StringLength(100)]
        public string Slug { get; private set; }

        public int? ParentId { get; set; }

        public Category Parent { get; set; }

        public IList<Category> Subcategories { get; set; } = new List<Category>();

        public IList<Post> Posts { get; set; } = new List<Post>();
    }
}
