using System.ComponentModel.DataAnnotations;

namespace TechWise.Shared.DTOs.Auth.NormalAuthServer
{
    public class LoginRequest
    {
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [MinLength(7)]
        public string Password { get; set; }

        public bool RememberMe { get; set; }

    }
}
