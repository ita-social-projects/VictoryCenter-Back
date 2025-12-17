using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Admin.Localization.FaqQuestions;

namespace VictoryCenter.BLL.Queries.Admin.Localization.FaqQuestions.GetByFaqQuestionId;

public record GetByFaqQuestionIdQuery(long Id)
    : IRequest<Result<IEnumerable<FaqQuestionLocalizationDto>>>;
