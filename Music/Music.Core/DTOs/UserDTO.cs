using Music.Core.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using File = Music.Core.Entities.File;

namespace Music.Core.DTOs
{
    //public enum Role { Admin = 1, User = 2 }
    public class UserDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        //public int RoleId { get; set; }
        //[ForeignKey(nameof(RoleId))]
        public string Role { get; set; }
        public List<File> Files { get; set; } = new List<File>();

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
    }
}
