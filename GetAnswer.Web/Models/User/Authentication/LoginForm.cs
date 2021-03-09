using System.ComponentModel.DataAnnotations;

namespace GetAnswer.Web.Models.User.Authentication
{
    public class LoginForm
    {
        [Required]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
        public bool RememberMe { get; set; }
    }
}