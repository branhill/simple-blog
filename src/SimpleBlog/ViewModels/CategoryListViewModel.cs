using SimpleBlog.Models;
using System.Collections.Generic;

namespace SimpleBlog.ViewModels
{
    public class CategoryListViewModel
    {
        public IList<Category> Categories { get; set; }

        public Category Category { get; set; } = new Category();
    }
}
