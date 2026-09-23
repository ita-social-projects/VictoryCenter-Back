using System.Net;
using Microsoft.Extensions.Logging;
using Moq;
using Resend;
using VictoryCenter.BLL.DTOs.Common;
using VictoryCenter.BLL.Services.Email;

namespace VictoryCenter.UnitTests.ServiceTests.Email;

public class ResendEmailSenderTests
{
    private readonly Mock<ILogger<ResendEmailSender>> _loggerMock = new();
    private readonly Mock<IResend> _resendMock = new();

    [Fact]
    public async Task SendEmailAsync_WhenResendSucceeds_ShouldReturnSuccess()
    {
        EmailMessage? sentMessage = null;
        _resendMock
            .Setup(resend => resend.EmailSendAsync(
                It.IsAny<EmailMessage>(),
                It.IsAny<CancellationToken>()))
            .Callback<EmailMessage, CancellationToken>((message, _) => sentMessage = message)
            .ReturnsAsync(new ResendResponse<Guid>(Guid.NewGuid(), new ResendRateLimit()));
        var sender = CreateSender();
        var email = CreateEmail();

        var result = await sender.SendEmailAsync(email);

        Assert.True(result.IsSuccess);
        Assert.NotNull(sentMessage);
        Assert.Equal(email.From, sentMessage.From);
        Assert.Equal(email.To, sentMessage.To.Select(address => address.Email));
        Assert.Equal(email.ReplyTo, sentMessage.ReplyTo?.Select(address => address.Email));
        Assert.Equal(email.Subject, sentMessage.Subject);
        Assert.Equal(email.TextBody, sentMessage.TextBody);
        Assert.Equal(email.HtmlBody, sentMessage.HtmlBody);
        _loggerMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task SendEmailAsync_WhenResendRejectsRequest_ShouldReturnFailure()
    {
        var rateLimit = new ResendRateLimit();
        var exception = new ResendException(
            HttpStatusCode.BadRequest,
            ErrorType.ValidationError,
            "Invalid email request.",
            rateLimit);
        _resendMock
            .Setup(resend => resend.EmailSendAsync(
                It.IsAny<EmailMessage>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ResendResponse<Guid>(exception, rateLimit));
        var sender = CreateSender();

        var result = await sender.SendEmailAsync(CreateEmail());

        Assert.True(result.IsFailed);
        Assert.Contains(result.Errors, error => error.Message == "Failed to send email via Resend API.");
        VerifyErrorWasLogged();
    }

    [Fact]
    public async Task SendEmailAsync_WhenResendThrows_ShouldReturnFailure()
    {
        _resendMock
            .Setup(resend => resend.EmailSendAsync(
                It.IsAny<EmailMessage>(),
                It.IsAny<CancellationToken>()))
            .ThrowsAsync(new HttpRequestException("Network failure."));
        var sender = CreateSender();

        var result = await sender.SendEmailAsync(CreateEmail());

        Assert.True(result.IsFailed);
        Assert.Contains(
            result.Errors,
            error => error.Message == "An unexpected error occurred while sending email via Resend API.");
        VerifyErrorWasLogged();
    }

    private ResendEmailSender CreateSender()
    {
        return new ResendEmailSender(_resendMock.Object, _loggerMock.Object);
    }

    private static EmailDto CreateEmail()
    {
        return new EmailDto
        {
            From = "sender@example.com",
            To = ["recipient@example.com"],
            ReplyTo = ["visitor@example.com"],
            Subject = "Test subject",
            TextBody = "Test text body",
            HtmlBody = "<p>Test HTML body</p>"
        };
    }

    private void VerifyErrorWasLogged()
    {
        _loggerMock.Verify(
            logger => logger.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }
}
