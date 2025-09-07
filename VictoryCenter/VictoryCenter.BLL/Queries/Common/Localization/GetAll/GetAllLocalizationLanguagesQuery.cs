using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Common;

namespace VictoryCenter.BLL.Queries.Common.Localization.GetAll;

public record GetAllLocalizationLanguagesQuery
    : IRequest<Result<IEnumerable<LocalizationLanguageDto>>>;
