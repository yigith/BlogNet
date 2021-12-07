﻿using BlogNet.Areas.Admin.Models;
using BlogNet.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlogNet.Areas.Admin.Controllers
{
    public class DashboardController : AdminBaseController
    {
        private readonly ApplicationDbContext _db;

        public DashboardController(ApplicationDbContext db)
        {
            _db = db;
        }

        public IActionResult Index()
        {
            var vm = new DashboardViewModel()
            {
                CategoryCount = _db.Categories.Count(),
                PostCount = _db.Posts.Count(),
                UserCount = _db.Users.Count(),
                CommentCount = _db.Comments.Count()
                
            };
            return View(vm);
        }
    }
}
