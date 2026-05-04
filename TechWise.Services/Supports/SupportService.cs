using TechWise.Domains.Contracts;
using TechWise.Domains.Entities.Support;
using TechWise.Services.Abstractions.Email;
using TechWise.Services.Abstractions.Support;
using TechWise.Shared.DTOs.Support;

namespace TechWise.Services.Support
{
    public class SupportService(
        IUnitOfWork _unitOfWork,
        IEmailService _emailService
    ) : ISupportService
    {
        private const string AdminEmail = "support@techwise.com";

        public async Task SendContactMessageAsync(ContactRequest request)
        {
            //Store in DB
            var message = new ContactMessage
            {
                FullName = request.FullName,
                Email = request.Email,
                Message = request.Message
            };

            await _unitOfWork.Support.AddContactMessageAsync(message);

            // Send Email to Admin
            await _emailService.SendEmailAsync(
                AdminEmail,
                $"New Contact Message from {request.FullName}",
                $"""
                <h3>New Contact Message</h3>
                <p><strong>Name:</strong> {request.FullName}</p>
                <p><strong>Email:</strong> {request.Email}</p>
                <p><strong>Message:</strong></p>
                <p>{request.Message}</p>
                """
            );
        }

        public async Task SendFeedbackAsync(string userId, FeedbackRequest request)
        {
            var feedback = new Feedback
            {
                UserId = userId,
                Rating = request.Rating,
                Comment = request.Comment,
                Email = request.Email
            };

            await _unitOfWork.Support.AddFeedbackAsync(feedback);
        }
    }
}