using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITS.DAL.Models
{
    public class User
    {
        public int Id { get; set; }

        [Required, StringLength(50)]
        public string Name { get; set; }

        [Required, StringLength(50)]
        public string Username { get; set; }

        [Required, StringLength(100)]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

        public string Role { get; set; } = "User"; // Default role

        public virtual ICollection<Issue> Issues { get; set; } = new List<Issue>();
        public virtual ICollection<Token> Tokens { get; set; } = new List<Token>();
    }
}
