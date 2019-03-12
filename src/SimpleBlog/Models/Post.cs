﻿using SimpleBlog.SeedWorks;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SimpleBlog.Models
{
    public class Post : Entity
    {
        [Required, StringLength(200)]
        public string Title { get; set; }

        [Required, StringLength(100)]
        public string Slug { get; set; }

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

        public IList<CategoryPost> CategoryPosts { get; } = new List<CategoryPost>();

        public IList<Comment> Comments { get; } = new List<Comment>();
    }
}