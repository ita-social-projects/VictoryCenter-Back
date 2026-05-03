namespace VictoryCenter.BLL.DTOs.Public.ContactUs;

public class SubmitContactUsFormDto : ContactUsFormDto
{
    public required string CaptchaResponseToken { get; init; }
}
