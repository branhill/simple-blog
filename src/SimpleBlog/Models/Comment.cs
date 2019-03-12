using SimpleBlog.SeedWorks;
using System;
using System.ComponentModel.DataAnnotations;

namespace SimpleBlog.Models
{
    public class Comment : Entity
    {
        [Required]
        public string AuthorName { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; }

        public string RegisteredAuthorId { get; set; }

        public User RegisteredAuthor { get; set; }

        public int PostId { get; set; }

        public Post Post { get; set; }

        public DateTime CreatedTime { get; set; } = DateTime.UtcNow;

        [Required]
        public string Content { get; set; }
    }
}
