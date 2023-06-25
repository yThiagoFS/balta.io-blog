using Blog.Models.Entities;

namespace Blog.Models
{
    public class Post : Base
    {
        public string Title { get; set; }

        public string Summary { get; set; }

        public string Body { get; set; }

        public string Slug { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime LastUpdateDate { get; set; }

        public Category Category { get; set; }

        public User Author { get; set; }

        public List<Tag> Tags { get; set; }
    }
}