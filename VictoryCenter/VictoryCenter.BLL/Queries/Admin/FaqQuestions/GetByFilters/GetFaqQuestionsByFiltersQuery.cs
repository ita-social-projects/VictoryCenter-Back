using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Admin.FaqQuestions;
using VictoryCenter.BLL.DTOs.Common;

namespace VictoryCenter.BLL.Queries.Admin.FaqQuestions.GetByFilters;

public record GetFaqQuestionsByFiltersQuery(FaqQuestionsFilterDto FaqQuestionsFilterDto)
    : IRequest<Result<PaginationResult<FaqQuestionDto>>>;
