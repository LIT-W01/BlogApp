using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BlogApp.Data;

namespace BlogApp.Controllers
{
    public class AdminController : Controller
    {

        private string _connectionString = @"Data Source=.\sqlexpress;Initial Catalog=Blog;Integrated Security=True";
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult AddPost(string title, string text, string tags)
        {
            var db = new BlogDb(_connectionString);
            var post = new Post { Date = DateTime.Now, Text = text, Title = title };
            db.AddPost(post);
            IEnumerable<Tag> postTags = tags.Split(',').Select(t =>
            {
                int tagId = db.AddTag(t);
                return new Tag
                {
                    Id = tagId,
                    Name = t
                };
            });

            db.AddTagsToPost(post, postTags);
            return RedirectToAction("Index", "Blog");
        }
    }
}
