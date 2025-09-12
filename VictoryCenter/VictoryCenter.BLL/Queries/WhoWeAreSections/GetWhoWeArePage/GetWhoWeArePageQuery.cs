using FluentResults;
using MediatR;
#pragma warning disable IDE0005
using VictoryCenter.BLL.DTOs.WhoWeAreSection;

namespace VictoryCenter.BLL.Queries.WhoWeAreSections.GetWhoWeArePage;

public record GetWhoWeArePageQuery : IRequest<Result<List<WhoWeArePageSectionDto>>>;
