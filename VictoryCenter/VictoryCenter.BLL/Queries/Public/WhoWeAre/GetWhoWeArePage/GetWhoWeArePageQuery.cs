using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Public.WhoWeArePage;

#pragma warning disable IDE0005

namespace VictoryCenter.BLL.Queries.Public.WhoWeAre.GetWhoWeArePage;

public record GetWhoWeArePageQuery : IRequest<Result<List<WhoWeArePageSectionDto>>>;
