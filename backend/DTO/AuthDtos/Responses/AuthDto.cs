using System.IdentityModel.Tokens.Jwt;

namespace Inventory.DTO.AuthDtos.Responses
{
    public class AuthDto
    {
        public string Id { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;
        public DateTime ExpiresOn { get; set; }
    }
}