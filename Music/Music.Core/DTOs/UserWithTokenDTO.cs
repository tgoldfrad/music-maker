using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Music.Core.DTOs
{
    public class UserWithTokenDTO
    {
        public UserDTO UserDto { get; set; }
        public string Token { get; set; }
    }
}
