﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using BlogHost.BLL.ServiceInterfaces;
using BlogHost.WEB.Models;
using BlogHost.BLL.DTO;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using BlogHost.WEB.Models.MappingProfiles;

namespace BlogHost.Controllers
{
    public class PostController : Controller
    {
        private readonly IBlogService _blogService;
        private readonly IPostService _postService;

        public PostController(
            IBlogService blogService,
            IPostService postService)
        {
            _blogService = blogService;
            _postService = postService;
        }

        public IActionResult Create(int? blogId)
        {
            if (_blogService.GetBlog(blogId, User) == null)
            {
                return NotFound();
            }

            PostViewModel viewModel = new PostViewModel()
            {
                BlogId = (int)blogId
            };
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(PostViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                _postService.Create(viewModel.ToDTO(), User, viewModel.BlogId);

                return RedirectToAction("Show", "Blog", new { id = viewModel.BlogId });
            }

            return View(viewModel);
        }

        public IActionResult Edit(int? id)
        {
            PostViewModel post = _postService.GetPost(id, User).ToVM();
            if (post == null)
            {
                return NotFound();
            }

            return View(post);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(PostViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                _postService.Edit(viewModel.ToDTO());

                return RedirectToAction("Show", "Post", new { id = viewModel.Id });
            }

            return View(viewModel);
        }

        [AllowAnonymous]
        public IActionResult Show(int? id)
        {
            PostViewModel post = _postService.GetPost(id, User).ToVM();
            if (post == null)
            {
                return NotFound();
            }

            return View(post);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int? id)
        {
            int? blogId = _postService.GetPost(id, User)?.Blog.Id;
            _postService.Delete(id, User);

            if (blogId == null)
            {
                return NotFound();
            }
            return RedirectToAction("Show", "Blog", new { id = blogId });
        }

        //public IActionResult PutLike(int? id)
        //{
        //    return View();
        //}

        //public IActionResult CleanLike(int? id)
        //{
        //    return View();
        //}
    }
}