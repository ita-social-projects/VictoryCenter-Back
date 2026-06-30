using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Admin.Localization.MainPage;

namespace VictoryCenter.BLL.Queries.Admin.Localization.MainPage.GetStatuses;

public record GetMainPageTranslationStatusesQuery(long EntityId, long LanguageId)
    : IRequest<Result<List<MainPageTranslationStatusDto>>>;
