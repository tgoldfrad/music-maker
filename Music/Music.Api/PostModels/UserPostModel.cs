using System.ComponentModel.DataAnnotations;

namespace Music.Api.PostModels
{
    public enum Role { Admin = 1, User = 2 }
    public class UserPostModel
    {


        public string Name { get; set; }

        public string Email { get; set; }

        public string Password { get; set; }
        public string Role { get; set; }


    }
}
