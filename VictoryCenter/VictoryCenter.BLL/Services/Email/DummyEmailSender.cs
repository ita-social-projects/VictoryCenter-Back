using FluentResults;
using VictoryCenter.BLL.DTOs.Common;
using VictoryCenter.BLL.Interfaces.Email;

namespace VictoryCenter.BLL.Services.Email;

public class DummyEmailSender : IEmailSender
{
    public Task<Result> SendEmailAsync(EmailDto emailToSend)
    {
        return Task.FromResult(Result.Ok());
    }
}
