using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SimpleBlog.Infrastructures.GuardClauses;
using SimpleBlog.Models;
using SimpleBlog.Services;
using SimpleBlog.ViewModels;
using System;
using System.Diagnostics;
using System.Linq;
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

        public PostController(
            PostService postService,
            TagService tagService,
            IMapper mapper,
            UserManager<User> userManager)
        {
            _postService = postService;
            _tagService = tagService;
            _mapper = mapper;
            _userManager = userManager;
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
            var viewModel = new PostViewModel { Post = post };

            if (post.IsDraft)
                return RedirectToAction(nameof(Edit), new { post.Id });
            return View(viewModel);
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
            if (!User.Identity.IsAuthenticated)
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

        [ValidateAntiForgeryToken]
        [HttpPost("{slug}/comment")]
        public async Task<IActionResult> CreateComment(string slug, PostViewModel viewModel)
        {
            if (User.Identity.IsAuthenticated)
            {
                ModelState.Remove($"{nameof(PostViewModel.Comment)}.{nameof(PostViewModel.Comment.AuthorName)}");
                ModelState.Remove($"{nameof(PostViewModel.Comment)}.{nameof(PostViewModel.Comment.AuthorEmail)}");
            }

            if (!ModelState.IsValid)
            {
                viewModel.Post = await _postService.GetBySlug(slug);
                return View("Article", viewModel);
            }

            var comment = viewModel.Comment;
            var post = await _postService.GetBySlug(slug);
            if (User.Identity.IsAuthenticated)
            {
                var user = await _userManager.GetUserAsync(User);
                comment.RegisteredAuthor = user;
                comment.AuthorName = !string.IsNullOrEmpty(user.Name) ? user.Name : user.UserName;
                comment.AuthorEmail = user.Email;
            }

            post.Comments.Add(comment);
            await _postService.Update(post);

            return RedirectToAction(nameof(Article), new { slug });
        }

        [Authorize]
        [ValidateAntiForgeryToken]
        [HttpPost("{slug}/comment/delete/{id}")]
        public async Task<IActionResult> DeleteComment(string slug, int id)
        {
            var post = await _postService.GetBySlug(slug);

            var comment = post.Comments.FirstOrDefault(c => c.Id == id);
            Guard.Against.NullThrow404NotFound(comment, nameof(comment));

            post.Comments.Remove(comment);
            await _postService.Update(post);

            return RedirectToAction(nameof(Article), new { slug });
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        [Route("/Error")]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
