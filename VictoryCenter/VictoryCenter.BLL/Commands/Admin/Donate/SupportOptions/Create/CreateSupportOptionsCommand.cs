using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Admin.Donate.SupportOptions;

namespace VictoryCenter.BLL.Commands.Admin.Donate.SupportOptions.Create;

public record CreateSupportOptionsCommand(CreateSupportOptionsDto CreateSupportOptionsDto)
    : IRequest<Result<SupportOptionsDto>>;
