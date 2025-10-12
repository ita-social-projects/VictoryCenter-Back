using FluentResults;
using MediatR;

namespace VictoryCenter.BLL.Commands.Admin.Localization.TeamMembers.Delete;

public record DeleteTeamMemberLocalizationCommand(long TeamMemberId, long LanguageId)
    : IRequest<Result<(long TeamMemberId, long LanguageId)>>;
