using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Admin.Localization.TeamMembers;

namespace VictoryCenter.BLL.Commands.Admin.Localization.TeamMembers.Create;

public record CreateTeamMemberLocalizationCommand(CreateTeamMemberLocalizationDto CreateTeamMemberLocalizationDto)
    : IRequest<Result<TeamMemberLocalizationDto>>;
