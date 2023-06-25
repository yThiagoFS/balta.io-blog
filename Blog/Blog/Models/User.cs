using Blog.Models.Entities;

namespace Blog.Models
{
    public class User : Base
    {
        public string Name { get; set; }

        public string Email { get; set; }

        public string PasswordHash { get; set; }

        public string Bio { get; set; }

        public string Image { get; set; }

        public string Slug { get; set; }

        public List<Post> Posts { get; set; }

        public List<Role> Roles { get; set; }
    }
}