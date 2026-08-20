using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Admin.HippotherapyLandingPage;

namespace VictoryCenter.BLL.Commands.Admin.HippotherapyLandingPage.Update;

public record UpdateHippotherapyLandingPageCommand(UpdateHippotherapyLandingPageDto Dto)
    : IRequest<Result<HippotherapyLandingPageDto>>;
