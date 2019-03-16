using Microsoft.AspNetCore.Mvc;
using SimpleBlog.Services;
using System.Linq;
using System.Threading.Tasks;

namespace SimpleBlog.Controllers
{
    [Route("t")]
    public class TagController : Controller
    {
        private readonly TagService _tagService;
        private readonly PostController _postController;

        public TagController(TagService tagService, PostController postController)
        {
            _tagService = tagService;
            _postController = postController;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string slug, [FromQuery(Name = "p")]int page = 1)
        {
            var list = await _tagService.List();
            return View(list);
        }

        [HttpGet("{slug}")]
        public async Task<IActionResult> PostList(string slug, [FromQuery(Name = "p")]int page = 1)
        {
            var tag = await _tagService.GetBySlug(slug);
            return await _postController.List(p => p.TagPosts.Any(tp => tp.TagId == tag.Id), page, tag.Name);
        }
    }
}
