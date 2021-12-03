using BlogNet.Data;
using BlogNet.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace BlogNet.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;
        private const int POSTS_PER_PAGE = 3;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        [Route("c/{slug}")]
        [Route("")]
        public IActionResult Index(string slug, string q, int pn = 1)
        {
            ViewBag.Slug = slug;
            IQueryable<Post> posts = _context.Posts.Where(x => x.IsPublished);
            Category category = null;
            if (!string.IsNullOrEmpty(slug))
            {
                posts = posts.Where(x => x.Category.Slug == slug);
                category = _context.Categories.FirstOrDefault(x => x.Slug == slug);
            }

            if (!string.IsNullOrEmpty(q))
            {
                posts = posts.Where(x => x.Title.Contains(q) || x.Content.Contains(q));
            }

            int totalItems = posts.Count();
            int totalPages = (int)Math.Ceiling((decimal)totalItems / POSTS_PER_PAGE);
            posts = posts
                .OrderByDescending(x => x.CreatedTime)
                .Skip((pn - 1) * POSTS_PER_PAGE).Take(POSTS_PER_PAGE);
            var postsList = posts.ToList();

            var vm = new HomeViewModel()
            {
                Category = category,
                Posts = postsList,
                PaginationInfo = new PaginationViewModel()
                {
                    CurrentPage = pn,
                    HasNewer = pn > 1,
                    HasOlder = pn < totalPages,
                    ItemsOnPage = postsList.Count,
                    TotalItems = totalItems,
                    TotalPages = totalPages,
                    ItemsPerPage = POSTS_PER_PAGE,
                    ResultsStart = (pn - 1) * POSTS_PER_PAGE + 1,
                    ResultsEnd = (pn - 1) * POSTS_PER_PAGE + postsList.Count
                },
                SearchCriteria = q
            };

            return View(vm);
        }

        [Route("p/{slug}")]
        public IActionResult ShowPost(string slug)
        {
            return View(_context.Posts
                .Include(x => x.Category)
                .FirstOrDefault(x => x.Slug == slug));
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

int[] PageNumbers(int current, int last)
{
    int delta = 2, l = 0;

    int left = current - delta;
    int right = current + delta + 1;

    List<int> range = new List<int>();
    List<int> rangeWithDots = new List<int>();

    for (int i = 1; i <= last; i++)
    {
        if (i == 1 || i == last || i >= left && i < right)
            range.Add(i);
    }

    foreach (var i in range)
    {
        if (l != 0)
        {
            if (i - l == 2)
                rangeWithDots.Add(l + 1);
            else if (i - l != 1)
                rangeWithDots.Add(-1);
        }
        rangeWithDots.Add(i);
        l = i;
    }

    return rangeWithDots.ToArray();
}
    }
}
