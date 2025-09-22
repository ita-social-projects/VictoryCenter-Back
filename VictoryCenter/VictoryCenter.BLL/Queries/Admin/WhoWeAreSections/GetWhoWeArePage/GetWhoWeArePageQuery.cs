using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Admin.WhoWeAreSection;

#pragma warning disable IDE0005

namespace VictoryCenter.BLL.Queries.Admin.WhoWeAreSections.GetWhoWeArePage;

public record GetWhoWeArePageQuery : IRequest<Result<List<WhoWeArePageSectionDto>>>;
