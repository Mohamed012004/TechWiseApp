using Microsoft.EntityFrameworkCore;
using TechWise.Domains.Contracts;
using TechWise.Domains.Entities.Support;

namespace TechWise.Persistence.Store.Repositories
{
    public class SupportRepository(StoreDbContext _context) : ISupportRepository
    {
        public async Task AddContactMessageAsync(ContactMessage message)
        {
            await _context.ContactMessages.AddAsync(message);
            await _context.SaveChangesAsync();
        }

        public async Task AddFeedbackAsync(Feedback feedback)
        {
            await _context.Feedbacks.AddAsync(feedback);
            await _context.SaveChangesAsync();
        }

        public async Task<List<ContactMessage>> GetContactMessagesAsync()
        {
            return await _context.ContactMessages
                .OrderByDescending(m => m.CreatedAt)
                .ToListAsync();
        }

        public async Task<List<Feedback>> GetFeedbacksAsync()
        {
            return await _context.Feedbacks
                .OrderByDescending(f => f.CreatedAt)
                .ToListAsync();
        }

        public async Task MarkContactAsReadAsync(int messageId)
        {
            var message = await _context.ContactMessages.FindAsync(messageId);
            if (message is null) return;

            message.IsRead = true;
            await _context.SaveChangesAsync();
        }
    }
}