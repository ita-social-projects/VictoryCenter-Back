using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Admin.Localization.MainPage;

namespace VictoryCenter.BLL.Commands.Admin.Localization.MainPage.Create;

public record CreateMainPageLocalizationCommand(CreateMainPageLocalizationDto Dto)
    : IRequest<Result<MainPageLocalizationDto>>;
