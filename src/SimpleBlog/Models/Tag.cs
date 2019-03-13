using SimpleBlog.SeedWorks;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SimpleBlog.Models
{
    public class Tag : Entity
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [Required]
        [StringLength(100)]
        public string Slug { get; set; }

        public IList<TagPost> TagPosts { get; } = new List<TagPost>();
    }
}
