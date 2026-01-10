namespace Inventory.DTO.AuthDtos.Requests
{
    public class RegisterDto
    {
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public string UserKey { get; set; } = string.Empty;
        public string Otp { get; set; } = string.Empty;
    }
}