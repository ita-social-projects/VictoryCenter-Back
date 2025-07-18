using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.FaqQuestions;

namespace VictoryCenter.BLL.Queries.FaqQuestions.GetById;

public record GetFaqQuestionByIdQuery(long Id)
    : IRequest<Result<FaqQuestionDto>>;
