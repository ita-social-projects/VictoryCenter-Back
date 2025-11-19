using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Admin.Localization.TeamMembers;

namespace VictoryCenter.BLL.Commands.Admin.Localization.TeamMembers.Update;

public record UpdateTeamMemberLocalizationCommand(
    UpdateTeamMemberLocalizationDto UpdateTeamMemberLocalizationDto,
    long TeamMemberId,
    long LanguageId)
    : IRequest<Result<TeamMemberLocalizationDto>>;
