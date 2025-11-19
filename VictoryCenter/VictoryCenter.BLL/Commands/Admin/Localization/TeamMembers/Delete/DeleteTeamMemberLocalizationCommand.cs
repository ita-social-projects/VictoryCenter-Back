using FluentResults;
using MediatR;

namespace VictoryCenter.BLL.Commands.Admin.Localization.TeamMembers.Delete;

public record DeleteTeamMemberLocalizationCommand(long EntityId, long LanguageId)
    : IRequest<Result<(long EntityId, long LanguageId)>>;
