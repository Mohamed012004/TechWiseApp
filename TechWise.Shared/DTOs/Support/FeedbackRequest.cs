using System.ComponentModel.DataAnnotations;

namespace TechWise.Shared.DTOs.Support
{
    public class FeedbackRequest
    {
        [Range(1, 5)]
        public int Rating { get; set; }

        [MaxLength(500)]
        public string? Comment { get; set; }

        [EmailAddress]
        public string? Email { get; set; }
    }
}