using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Admin.HippotherapyLandingPage;

namespace VictoryCenter.BLL.Queries.Public.HippotherapyLandingPage.Get;

public record GetPublicHippotherapyLandingPageQuery : IRequest<Result<HippotherapyLandingPageDto>>;
