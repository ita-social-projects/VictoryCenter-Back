using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Admin.FaqQuestions;

namespace VictoryCenter.BLL.Queries.Admin.FaqQuestions.GetById;

public record GetFaqQuestionByIdQuery(long Id)
    : IRequest<Result<FaqQuestionDto>>;
