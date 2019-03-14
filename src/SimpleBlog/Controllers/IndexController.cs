﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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

        [HttpGet]
        public async Task<IActionResult> Index([FromQuery(Name = "p")]int page = 1)
        {
            var list = await _postService.ListBy(posts => posts
                .Include(p => p.Author)
                .OrderByDescending(p => p.CreatedTime), page);

            return View(new IndexViewModel { List = list });
        }

        [HttpGet("p/{slug}")]
        public async Task<IActionResult> Article(string slug)
        {
            var post = await _postService.GetBySlug(slug);
            return View(post);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
