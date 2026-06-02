using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Admin.Localization.MainPage;

namespace VictoryCenter.BLL.Queries.Admin.Localization.MainPage.GetByLanguageId;

public record GetMainPageLocalizationByLanguageIdQuery(long EntityId, long LanguageId)
    : IRequest<Result<MainPageLocalizationDto>>;
