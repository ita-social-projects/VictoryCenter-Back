using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Admin.Localization.FaqQuestions;

namespace VictoryCenter.BLL.Queries.Admin.Localization.FaqQuestions.GetByLanguageId;

public record GetByLanguageIdQuery(long Id)
    : IRequest<Result<IEnumerable<FaqQuestionLocalizationDto>>>;
