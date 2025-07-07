using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Categories;

namespace VictoryCenter.BLL.Queries.TeamMembers.GetPublished;

public record GetPublishedTeamMembersQuery
    : IRequest<Result<List<CategoryWithPublishedTeamMembersDto>>>;
