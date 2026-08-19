using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Admin.HippotherapyLandingPage;

namespace VictoryCenter.BLL.Queries.Admin.HippotherapyLandingPage.Get;

public record GetHippotherapyLandingPageQuery : IRequest<Result<HippotherapyLandingPageDto>>;
