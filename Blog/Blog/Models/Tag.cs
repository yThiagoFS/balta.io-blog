using Blog.Models.Entities;

namespace Blog.Models
{
    public class Tag : Base
    {
        public string Name { get; set; }        

        public string Slug { get; set; }

        public List<Post> Posts { get; set; }
    }
}