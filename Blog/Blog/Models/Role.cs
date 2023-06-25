using Blog.Models.Entities;

namespace Blog.Models
{
    public class Role : Base
    {
        public string Name { get; set; }

        public string Slug { get; set; }

        public List<User> Users { get; set; }
    }
}