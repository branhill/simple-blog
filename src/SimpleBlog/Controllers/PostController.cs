using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SimpleBlog.Models;
using SimpleBlog.Services;
using SimpleBlog.ViewModels;
using System;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SimpleBlog.Controllers
{
    [Route("p")]
    public class PostController : Controller
    {
        private readonly PostService _postService;
        private readonly TagService _tagService;
        private readonly IMapper _mapper;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;

        public PostController(
            PostService postService,
            TagService tagService,
            IMapper mapper,
            UserManager<User> userManager,
            SignInManager<User> signInManager)
        {
            _postService = postService;
            _tagService = tagService;
            _mapper = mapper;
            _userManager = userManager;
            _signInManager = signInManager;
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

            if (post.IsDraft)
                return RedirectToAction(nameof(Edit), new { post.Id });
            return View(post);
        }

        [Authorize]
        [HttpGet("[action]")]
        public IActionResult New()
        {
            var model = new PostInputModel
            {
                IsDraft = true
            };

            return View("Edit", model);
        }

        [Authorize]
        [HttpGet("[action]/{id}")]
        public async Task<IActionResult> Edit(int id)
        {
            var post = await _postService.GetById(id);

            return View(_mapper.Map<PostInputModel>(post));
        }

        [Authorize]
        [ValidateAntiForgeryToken]
        [HttpPost("new")]
        [HttpPost("edit/{id}")]
        public async Task<IActionResult> CreateOrUpdate(PostInputModel model, int? id)
        {
            if (!ModelState.IsValid)
                return View("Edit", model);

            Post post;
            var tagList = await _tagService.List();
            if (id == null)
            {
                // Create
                post = _mapper.Map<Post>(model);
                post.AuthorId = _userManager.GetUserId(User);
                post.TagPosts = model.GetTagPosts(tagList);
                if (string.IsNullOrWhiteSpace(post.Excerpt))
                {
                    var lineBreak = post.Content.IndexOf(Environment.NewLine, StringComparison.Ordinal);
                    if (lineBreak < 0)
                        lineBreak = post.Content.Length;
                    var length = Math.Min(lineBreak, 500);
                    var sub = post.Content.Substring(0, length);
                    post.Excerpt = Regex.Replace(sub, "<[^>]*(>|$)", "");
                }

                await _postService.Create(post);
            }
            else
            {
                // Update
                post = await _postService.GetById(id.Value);
                if (post.IsDraft)
                {
                    post.CreatedTime = DateTime.UtcNow;
                    post.ModifiedTime = null;
                }
                else
                {
                    post.ModifiedTime = DateTime.UtcNow;
                }

                _mapper.Map(model, post);
                post.TagPosts = model.GetTagPosts(tagList);

                await _postService.Update(post);
            }

            return RedirectToAction(nameof(Article), new { post.Slug });
        }

        [Authorize]
        [ValidateAntiForgeryToken]
        [HttpPost("[action]/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var post = await _postService.GetById(id);
            await _postService.Delete(post);

            return RedirectToAction(nameof(Index));
        }

        [NonAction]
        public async Task<IActionResult> List(Func<Post, bool> predicate, int page, string title)
        {
            if (!_signInManager.IsSignedIn(User))
            {
                var oldPredicate = predicate;
                predicate = p => p.IsDraft == false && oldPredicate(p);
            }

            var list = await _postService.ListBy(p => predicate(p), page);

            if (page > 1)
            {
                if (!string.IsNullOrEmpty(title))
                    title += " ";
                title += $"Page {page}";
            }

            return View("/Views/Post/List.cshtml", new PostListViewModel { Title = title, List = list });
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        [Route("/Error")]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
