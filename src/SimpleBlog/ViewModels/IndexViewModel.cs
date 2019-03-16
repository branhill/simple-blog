using SimpleBlog.Models;
using SimpleBlog.SeedWorks;

namespace SimpleBlog.ViewModels
{
    public class PostListViewModel
    {
        public string Title { get; set; }

        public PaginatedList<Post> List { get; set; }
    }
}
