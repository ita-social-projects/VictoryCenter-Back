using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Admin.TeamMemberLocalizations;

namespace VictoryCenter.BLL.Commands.Admin.TeamMemberLocalizations.Create;

public record CreateTeamMemberLocalizationCommand(CreateTeamMemberLocalizationDto CreateTeamMemberLocalizationDto)
    : IRequest<Result<TeamMemberLocalizationDto>>;
