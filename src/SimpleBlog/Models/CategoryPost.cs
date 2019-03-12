namespace SimpleBlog.Models
{
    public class CategoryPost
    {
        public int CategoryId { get; set; }

        public Category Category { get; set; }

        public int PostId { get; set; }

        public Post Post { get; set; }
    }
}
