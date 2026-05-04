using System.ComponentModel.DataAnnotations;

namespace TechWise.Shared.DTOs.Support
{
    public class ContactRequest
    {
        [Required]
        public string FullName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [MinLength(10)]
        [MaxLength(1000)]
        public string Message { get; set; }
    }
}