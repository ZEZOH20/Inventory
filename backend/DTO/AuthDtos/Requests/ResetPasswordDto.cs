namespace Inventory.DTO.AuthDtos.Requests;

public class ResetPasswordDto
{
    public string Email { get; set; }
    public string Otp { get; set; }
    public string UserKey { get; set; }
    public string NewPassword { get; set; }
}