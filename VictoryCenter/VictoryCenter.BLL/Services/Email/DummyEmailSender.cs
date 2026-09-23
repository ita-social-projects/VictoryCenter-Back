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

    public Task<Result> SendEmailAsync(EmailDto emailToSend)
    {
        _logger.LogInformation("A dummy email was handled without external delivery.");

        return Task.FromResult(Result.Ok());
    }
}
