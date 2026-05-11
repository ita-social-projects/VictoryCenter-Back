using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Admin.Localization.MainPage;

namespace VictoryCenter.BLL.Commands.Admin.Localization.MainPage.Update;

public record UpdateMainPageLocalizationCommand(UpdateMainPageLocalizationDto Dto, long EntityId, long LanguageId)
    : IRequest<Result<MainPageLocalizationDto>>;
