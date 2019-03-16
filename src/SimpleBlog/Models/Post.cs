using SimpleBlog.SeedWorks;
using SimpleBlog.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SimpleBlog.Models
{
    public class Post : Entity
    {
        private string _title;

        [Required, StringLength(200)]
        public string Title
        {
            get => _title;
            set
            {
                _title = value;
                Slug = value.ToSlug();
            }
        }

        [Required, StringLength(100)]
        public string Slug { get; private set; }

        [Required, StringLength(450)]
        public string AuthorId { get; set; }

        public User Author { get; set; }

        public DateTime CreatedTime { get; set; } = DateTime.UtcNow;

        public DateTime? ModifiedTime { get; set; }

        public bool IsDraft { get; set; }

        [Required, StringLength(1000)]
        public string Excerpt { get; set; }

        [Required]
        public string Content { get; set; }

        public int CategoryId { get; set; }

        public Category Category { get; set; }

        public IList<TagPost> TagPosts { get; set; } = new List<TagPost>();

        public IList<Comment> Comments { get; set; } = new List<Comment>();
    }
}
