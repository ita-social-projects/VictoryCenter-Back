using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Admin.FaqQuestions;

namespace VictoryCenter.BLL.Queries.Admin.FaqQuestions.GetByFilters;

public record GetFaqQuestionsByFiltersQuery(FaqQuestionsFilterDto FaqQuestionsFilterDto)
    : IRequest<Result<List<FaqQuestionDto>>>;
