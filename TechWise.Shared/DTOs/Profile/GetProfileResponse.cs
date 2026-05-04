
namespace TechWise.Shared.DTOs.Profile
{
    public class GetProfileResponse
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Location { get; set; }
        public string? ProfilePhoto { get; set; }
        public bool IsVerified { get; set; }
        public string MemberSince { get; set; }
    }
}