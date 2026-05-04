
using System.ComponentModel.DataAnnotations;

namespace TechWise.Shared.DTOs.Auth.ExternalAuthServer
{
    public class SocialLoginRequest
    {
        [Required]
        public string IdToken { get; set; }
    }
}