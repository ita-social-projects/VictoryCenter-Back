namespace VictoryCenter.BLL.DTOs.Public.ContactUs;

public class ContactUsFormDto
{
    public required string FromName { get; init; }

    public required string FromEmail { get; init; }

    public required string Subject { get; init; }

    public required string Message { get; init; }
}
