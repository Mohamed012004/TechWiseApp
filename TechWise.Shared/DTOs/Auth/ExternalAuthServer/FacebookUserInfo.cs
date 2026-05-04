
using System.Text.Json.Serialization;

namespace TechWise.Shared.DTOs.Auth.ExternalAuthServer
{
    public class FacebookUserInfo
    {
        public string Id { get; set; }
        public string Email { get; set; }

        [JsonPropertyName("first_name")]
        public string FirstName { get; set; }

        [JsonPropertyName("last_name")]
        public string LastName { get; set; }
    }
}