using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Admin.Donate.SupportOptions;

namespace VictoryCenter.BLL.Commands.Admin.Donate.SupportOptions.Update;

public record UpdateSupportOptionsCommand(UpdateSupportOptionsDto UpdateSupportOptionsDto, long Id)
    : IRequest<Result<SupportOptionsDto>>;
