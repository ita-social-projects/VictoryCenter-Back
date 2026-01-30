using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Admin.FaqQuestions;
using VictoryCenter.BLL.DTOs.Common;

namespace VictoryCenter.BLL.Queries.Admin.FaqQuestions.Search;

public record SearchFaqQuestionQuery(SearchFaqQuestionDto SearchFaqQuestionDto)
    : IRequest<Result<PaginationResult<FaqQuestionDto>>>;
