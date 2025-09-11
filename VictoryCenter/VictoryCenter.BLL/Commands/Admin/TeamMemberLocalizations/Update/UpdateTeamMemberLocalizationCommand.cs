using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Admin.TeamMemberLocalizations;

namespace VictoryCenter.BLL.Commands.Admin.TeamMemberLocalizations.Update;

public record UpdateTeamMemberLocalizationCommand(UpdateTeamMemberLocalizationDto UpdateTeamMemberLocalizationDto)
    : IRequest<Result<TeamMemberLocalizationDto>>;
