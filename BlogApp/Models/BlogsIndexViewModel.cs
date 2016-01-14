using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BlogApp.Data;

namespace BlogApp.Models
{
    public class BlogsIndexViewModel
    {
        public IEnumerable<Post> Posts { get; set; } 
    }
}