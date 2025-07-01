using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Categories;

namespace VictoryCenter.BLL.Queries.TeamMembers.GetPublic;

public record GetPublicTeamMembersQuery
    : IRequest<Result<List<PublicCategoryWithTeamMembersDto>>>;
