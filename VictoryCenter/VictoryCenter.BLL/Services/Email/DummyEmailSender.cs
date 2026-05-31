using System.Text.Json;
using FluentResults;
using Microsoft.Extensions.Logging;
using VictoryCenter.BLL.DTOs.Common;
using VictoryCenter.BLL.Interfaces.Email;

namespace VictoryCenter.BLL.Services.Email;

public class DummyEmailSender : IEmailSender
{
    private readonly ILogger<DummyEmailSender> _logger;

    public DummyEmailSender(ILogger<DummyEmailSender> logger)
    {
        _logger = logger;
    }

    public async Task<Result> SendEmailAsync(EmailDto emailToSend)
    {
        var emailJson = JsonSerializer.Serialize(emailToSend, new JsonSerializerOptions
        {
            WriteIndented = true,
        });
        _logger.LogInformation("A dummy email was sent: {EmailData}", emailJson);

        return Result.Ok();
    }
}
