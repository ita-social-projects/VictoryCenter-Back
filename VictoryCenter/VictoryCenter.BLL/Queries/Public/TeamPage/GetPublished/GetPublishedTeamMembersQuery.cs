using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Public.TeamPage;

namespace VictoryCenter.BLL.Queries.Public.TeamPage.GetPublished;

public record GetPublishedTeamMembersQuery
    : IRequest<Result<List<CategoryWithPublishedTeamMembersDto>>>;
