using System.ComponentModel.DataAnnotations;

namespace TechWise.Domains.Entities.SuperAdmin
{
    public class CreateAdminRequest
    {
        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [MinLength(7)]
        public string Password { get; set; }
    }
}