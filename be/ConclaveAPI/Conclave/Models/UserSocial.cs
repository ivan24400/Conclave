using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Conclave.Models
{
    public class UserSocial
    {
        [Key]
        public int Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Provider { get; set; }

        public ICollection<Post> Posts { get; set; }
    }
}
