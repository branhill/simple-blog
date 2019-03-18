using SimpleBlog.SeedWorks;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace SimpleBlog.Models
{
    public class Comment : Entity
    {
        [Required]
        [DisplayName("Name")]
        public string AuthorName { get; set; }

        [Required, EmailAddress]
        [DisplayName("Email")]
        public string AuthorEmail { get; set; }

        public bool IsRegistered { get; set; }

        public string RegisteredAuthorId { get; set; }

        public User RegisteredAuthor { get; set; }

        public int PostId { get; set; }

        public Post Post { get; set; }

        public DateTime CreatedTime { get; set; } = DateTime.UtcNow;

        [Required]
        [DisplayName("Comment")]
        public string Content { get; set; }
    }
}
