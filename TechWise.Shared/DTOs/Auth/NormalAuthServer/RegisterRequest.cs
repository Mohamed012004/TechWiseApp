using System.ComponentModel.DataAnnotations;

namespace TechWise.Shared.DTOs.Auth.NormalAuthServer
{
    public class RegisterRequest
    {
        [Required]
        [MinLength(2)]
        [MaxLength(50)]
        public string FirstName { get; set; }

        [Required]
        [MinLength(2)]
        [MaxLength(50)]
        public string LastName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [MinLength(7)]
        public string Password { get; set; }

        [Required]
        [Compare("Password", ErrorMessage = "Passwords do not match")]
        public string ConfirmPassword { get; set; }

        [Range(typeof(bool), "true", "true",
            ErrorMessage = "You must accept terms")]
        public bool IsTermsAccepted { get; set; }
    }
}
