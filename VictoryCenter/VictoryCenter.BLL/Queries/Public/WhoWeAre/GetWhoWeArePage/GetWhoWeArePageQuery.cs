using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Public.WhoWeArePage;

namespace VictoryCenter.BLL.Queries.Public.WhoWeAre.GetWhoWeArePage;

public record GetWhoWeArePageQuery : IRequest<Result<List<WhoWeArePageSectionDto>>>;
