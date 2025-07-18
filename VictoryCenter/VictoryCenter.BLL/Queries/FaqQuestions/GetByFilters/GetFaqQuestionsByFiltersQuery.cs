using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.FaqQuestions;

namespace VictoryCenter.BLL.Queries.FaqQuestions.GetByFilters;

public record GetFaqQuestionsByFiltersQuery(FaqQuestionsFilterDto FaqQuestionsFilterDto)
    : IRequest<Result<List<FaqQuestionDto>>>;
