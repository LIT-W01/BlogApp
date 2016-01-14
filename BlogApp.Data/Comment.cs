using System;

namespace BlogApp.Data
{
    public class Comment
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public string Name { get; set; }
        public int PostId { get; set; }
        public DateTime Date { get; set; }
    }
}