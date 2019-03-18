using SimpleBlog.Models;

namespace SimpleBlog.ViewModels
{
    public class PostViewModel
    {
        public Post Post { get; set; }

        public Comment Comment { get; set; } = new Comment();
    }
}
