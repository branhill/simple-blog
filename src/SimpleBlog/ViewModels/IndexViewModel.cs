using SimpleBlog.Models;
using SimpleBlog.SeedWorks;

namespace SimpleBlog.ViewModels
{
    public class IndexViewModel
    {
        public string Title { get; set; }

        public PaginatedList<Post> List { get; set; }
    }
}
