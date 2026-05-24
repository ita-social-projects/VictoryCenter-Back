using FluentResults;
using VictoryCenter.BLL.DTOs.Common;

namespace VictoryCenter.BLL.Interfaces.Email;

public interface IEmailSender
{
    Task<Result> SendEmailAsync(EmailDto emailToSend);
}
