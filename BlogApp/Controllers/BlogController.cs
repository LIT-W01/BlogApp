using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using BlogApp.Data;
using BlogApp.Models;

namespace BlogApp.Controllers
{
    public class BlogController : Controller
    {
        private string _connectionString = @"Data Source=.\sqlexpress;Initial Catalog=Blog;Integrated Security=True";
        public ActionResult Index()
        {
            var db = new BlogDb(_connectionString);
            var viewModel = new BlogsIndexViewModel
            {
                Posts = db.GetLastFive()
            };
            foreach (Post p in viewModel.Posts)
            {
                p.Text = StripTagsRegex(p.Text).Substring(0, 300) + "...";
            }
            return View(viewModel);
        }

        public ActionResult Post(int postId)
        {
            var db = new BlogDb(_connectionString);
            var vm = new BlogSingleViewModel();
            vm.Post = db.GetPostById(postId);
            vm.Comments = db.GetCommentsForPost(postId);
            vm.Tags = db.GetTagsForPost(postId);
            return View(vm);
        }

        public ActionResult Comment(string name, string text, int postId)
        {
            var db = new BlogDb(_connectionString);
            Comment comment = new Comment
            {
                Date = DateTime.Now,
                Name = name,
                Text = text,
                PostId = postId
            };

            db.AddComment(comment);
            return RedirectToAction("Post", new {postId = postId});
        }

        private static string StripTagsRegex(string source)
        {
            return Regex.Replace(source, "<.*?>", string.Empty);
        }
    }
}
