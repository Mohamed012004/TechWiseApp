using Microsoft.AspNetCore.Identity;

namespace TechWise.Domains.Entities.Identity
{
    public class AppUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public bool IsTermsAccepted { get; set; }
        public Address Address { get; set; }

        public bool IsDeleted { get; set; } = false;
        public DateTime? DeletedAt { get; set; }
        public string? OriginalEmail { get; set; }


        public string? ProfilePhoto { get; set; }
        public string? Location { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;


    }
}
