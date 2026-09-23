using Microsoft.Extensions.Logging;
using Moq;
using VictoryCenter.BLL.DTOs.Common;
using VictoryCenter.BLL.Services.Email;

namespace VictoryCenter.UnitTests.ServiceTests.Email;

public class DummyEmailSenderTests
{
    private readonly Mock<ILogger<DummyEmailSender>> _loggerMock = new();

    [Fact]
    public async Task SendEmailAsync_ShouldReturnSuccessWithoutLoggingEmailContent()
    {
        var email = CreateEmail();
        var sender = new DummyEmailSender(_loggerMock.Object);

        var result = await sender.SendEmailAsync(email);

        Assert.True(result.IsSuccess);
        _loggerMock.Verify(
            logger => logger.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((state, _) =>
                    state.ToString() == "A dummy email was handled without external delivery."),
                null,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
        _loggerMock.VerifyNoOtherCalls();
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
