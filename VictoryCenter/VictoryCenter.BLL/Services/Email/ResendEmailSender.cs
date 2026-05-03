using FluentResults;
using Microsoft.Extensions.Logging;
using Resend;
using VictoryCenter.BLL.DTOs.Common;
using VictoryCenter.BLL.Errors;
using VictoryCenter.BLL.Interfaces.Email;

namespace VictoryCenter.BLL.Services.Email;

public class ResendEmailSender : IEmailSender
{
    private readonly ILogger<ResendEmailSender> _logger;
    private readonly IResend _resend;

    public ResendEmailSender(IResend resend, ILogger<ResendEmailSender> logger)
    {
        _resend = resend;
        _logger = logger;
    }

    public async Task<Result> SendEmailAsync(EmailDto emailToSend)
    {
        var toEmailAddressList = EmailAddressList.From(emailToSend.To);
        var replyToAddressList = emailToSend.ReplyTo is null ? null : EmailAddressList.From(emailToSend.ReplyTo);

        var message = new EmailMessage
        {
            From = emailToSend.From,
            Subject = emailToSend.Subject,
            HtmlBody = emailToSend.HtmlBody,
            To = toEmailAddressList,
            TextBody = emailToSend.TextBody,
            ReplyTo = replyToAddressList
        };

        var response = await _resend.EmailSendAsync(message);

        if (!response.Success)
        {
            const string errorMessage = "Failed to send email via Resend API.";
            _logger.LogError(response.Exception, errorMessage);

            return Result.Fail(new InternalError(errorMessage));
        }

        return Result.Ok();
    }
}
