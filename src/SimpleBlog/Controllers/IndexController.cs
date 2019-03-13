using Microsoft.AspNetCore.Mvc;
using SimpleBlog.Services;
using SimpleBlog.ViewModels;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace SimpleBlog.Controllers
{
    public class IndexController : Controller
    {
        private readonly PostService _postService;

        public IndexController(PostService postService)
        {
            _postService = postService;
        }

        public async Task<IActionResult> Index(int p = 1)
        {
            var list = await _postService.ListBy(posts =>
                posts.OrderByDescending(post => post.CreatedTime), p);

            return View(new IndexViewModel { List = list });
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
