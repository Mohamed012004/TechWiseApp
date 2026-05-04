using TechWise.Domains.Entities.Support;

namespace TechWise.Domains.Contracts
{
    public interface ISupportRepository
    {
        Task AddContactMessageAsync(ContactMessage message);
        Task AddFeedbackAsync(Feedback feedback);
        Task<List<ContactMessage>> GetContactMessagesAsync();
        Task<List<Feedback>> GetFeedbacksAsync();
        Task MarkContactAsReadAsync(int messageId);
    }
}