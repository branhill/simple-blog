using Microsoft.AspNetCore.Mvc;
using SimpleBlog.Services;
using System.Threading.Tasks;

namespace SimpleBlog.Controllers
{
    [Route("c")]
    public class CategoryController : Controller
    {
        private readonly CategoryService _categoryService;
        private readonly PostController _postController;

        public CategoryController(CategoryService categoryService, PostController postController)
        {
            _categoryService = categoryService;
            _postController = postController;
        }

        [HttpGet("{slug}")]
        public async Task<IActionResult> Index(string slug, [FromQuery(Name = "p")]int page = 1)
        {
            var category = await _categoryService.GetBySlug(slug);
            return await _postController.List(p => p.Category.Slug == slug, page, category.Name);
        }
    }
}
