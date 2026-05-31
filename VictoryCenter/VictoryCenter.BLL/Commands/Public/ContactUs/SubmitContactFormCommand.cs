using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Public.ContactUs;

namespace VictoryCenter.BLL.Commands.Public.ContactUs;

public record SubmitContactFormCommand(SubmitContactUsFormDto Dto) : IRequest<Result<ContactUsFormDto>>;
