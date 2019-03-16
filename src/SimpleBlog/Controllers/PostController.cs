using Microsoft.AspNetCore.Mvc;
using SimpleBlog.Models;
using SimpleBlog.Services;
using SimpleBlog.ViewModels;
using System;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace SimpleBlog.Controllers
{
    [Route("p")]
    public class PostController : Controller
    {
        private readonly PostService _postService;

        public PostController(PostService postService)
        {
            _postService = postService;
        }

        [HttpGet("/")]
        public Task<IActionResult> Index([FromQuery(Name = "p")]int page = 1)
        {
            return List(_ => true, page, null);
        }

        [HttpGet("{slug}")]
        public async Task<IActionResult> Article(string slug)
        {
            var post = await _postService.GetBySlug(slug);
            return View(post);
        }

        [NonAction]
        public async Task<IActionResult> List(Expression<Func<Post, bool>> predicate, int page, string title)
        {
            var list = await _postService.ListBy(predicate, page);

            if (page > 1)
            {
                if (!string.IsNullOrEmpty(title))
                    title += " ";
                title += $"Page {page}";
            }

            return View("List", new PostListViewModel { Title = title, List = list });
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        [Route("/Error")]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
