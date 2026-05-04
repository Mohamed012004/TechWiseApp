using TechWise.Shared.DTOs.Support;

namespace TechWise.Services.Abstractions.Support
{
    public interface ISupportService
    {
        Task SendContactMessageAsync(ContactRequest request);
        Task SendFeedbackAsync(string userId, FeedbackRequest request);
    }
}