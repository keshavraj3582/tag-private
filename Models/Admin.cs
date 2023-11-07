using System.ComponentModel.DataAnnotations;

namespace School_Login_SignUp.Models
{
    public class Admin
    {
        [Required]
        public string Email { get; set; }
        //public string Role{ get; set; }
        [Required]
        public string Password { get; set; }
    }
}
