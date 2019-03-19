using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SimpleBlog.Services;
using SimpleBlog.ViewModels;
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

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var categories = await _categoryService.List();
            return View(new CategoryListViewModel { Categories = categories });
        }

        [HttpGet("{slug}")]
        public async Task<IActionResult> PostList(string slug, [FromQuery(Name = "p")]int page = 1)
        {
            var category = await _categoryService.GetBySlug(slug);
            return await _postController.List(p => p.CategoryId == category.Id, page, category.Name);
        }

        [Authorize]
        [ValidateAntiForgeryToken]
        [HttpPost("new")]
        [HttpPost("edit/{id}")]
        public async Task<IActionResult> CreateOrUpdate(CategoryListViewModel viewModel, int? id)
        {
            if (!ModelState.IsValid)
            {
                viewModel.Categories = await _categoryService.List();
                return View("Index", viewModel);
            }

            var category = viewModel.Category;
            if (id == null)
            {
                await _categoryService.Create(category);
            }
            else
            {
                category.Id = id.Value;
                await _categoryService.Update(category);
            }

            return RedirectToAction(nameof(Index));
        }

        [Authorize]
        [ValidateAntiForgeryToken]
        [HttpPost("[action]/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var category = await _categoryService.GetById(id);
            await _categoryService.Delete(category);

            return RedirectToAction(nameof(Index));
        }

        [Authorize]
        [HttpGet("[action]/{id}")]
        public async Task<IActionResult> CanBeDelete(int id)
        {
            var result = await _categoryService.CanBeDelete(id);
            return Ok(result);
        }
    }
}
