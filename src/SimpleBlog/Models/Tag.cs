using SimpleBlog.SeedWorks;
using SimpleBlog.Utilities;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SimpleBlog.Models
{
    public class Tag : Entity
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

        public IList<TagPost> TagPosts { get; } = new List<TagPost>();
    }
}
