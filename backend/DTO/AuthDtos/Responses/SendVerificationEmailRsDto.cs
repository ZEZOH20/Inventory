namespace Inventory.DTO.AuthDtos.Responses;

public class SendVerificationEmailRsDto
{
    public string Otp { get; set; }
    public string UserKey { get; set; }
    public DateTime AvailableUntil { get; set; }
}