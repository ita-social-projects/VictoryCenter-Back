using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Admin.Localization.TeamMembers;

namespace VictoryCenter.BLL.Commands.Admin.Localization.TeamMembers.Delete;

public record DeleteTeamMemberLocalizationCommand(long EntityId, long LanguageId)
    : IRequest<Result<DeleteTeamMemberLocalizationDto>>;
