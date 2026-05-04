using System.ComponentModel.DataAnnotations;

namespace TechWise.Shared.DTOs.Reviews
{
    public class ReviewRequest
    {
        [Range(1, 5)]
        public int Rating { get; set; }

        [MaxLength(500)]
        public string? Comment { get; set; }
    }
}