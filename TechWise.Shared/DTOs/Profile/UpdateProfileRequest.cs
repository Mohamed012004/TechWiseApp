
using System.ComponentModel.DataAnnotations;

namespace TechWise.Shared.DTOs.Profile
{
    public class UpdateProfileRequest
    {
        [Required]
        [MinLength(2)]
        [MaxLength(50)]
        public string FirstName { get; set; }

        [Required]
        [MinLength(2)]
        [MaxLength(50)]
        public string LastName { get; set; }

        public string? PhoneNumber { get; set; }
        public string? Location { get; set; }
    }
}