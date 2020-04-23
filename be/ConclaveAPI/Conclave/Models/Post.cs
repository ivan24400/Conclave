using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Conclave.Models
{
    public class Post
    {
        public Post() { }
        [Key]
        public int Id { get; set; }
        public int UserSocialId { get; set; }
        public string Text { get; set; }
        public string Media { get; set; }

        public DateTime Date { get; set; }

        public UserSocial UserSocial { get; set; }

        public ICollection<PostMedia> PostMedia { get; set; }
    }
}
