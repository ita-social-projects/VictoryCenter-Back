using VictoryCenter.BLL.DTOs.Common;
using VictoryCenter.BLL.Services.Email;

namespace VictoryCenter.UnitTests.ServiceTests.Email;

public class DummyEmailSenderTests
{
    [Fact]
    public async Task SendEmailAsync_ShouldReturnSuccess()
    {
        var email = CreateEmail();
        var sender = new DummyEmailSender();

        var result = await sender.SendEmailAsync(email);

        Assert.True(result.IsSuccess);
    }

    private static EmailDto CreateEmail()
    {
        return new EmailDto
        {
            From = "sender@example.com",
            To = ["recipient@example.com"],
            ReplyTo = ["visitor@example.com"],
            Subject = "Private subject",
            TextBody = "Private contact-form message"
        };
    }
}
