using System.ComponentModel.DataAnnotations;

namespace Conclave.Models
{
    public class PostMedia
    {
        [Key]
        public int Id { get; set; }
        public int PostId { get; set; }
        public string Filetype { get; set; }
        public string Path { get; set; }

        public Post Post { get; set; }

    }
}
