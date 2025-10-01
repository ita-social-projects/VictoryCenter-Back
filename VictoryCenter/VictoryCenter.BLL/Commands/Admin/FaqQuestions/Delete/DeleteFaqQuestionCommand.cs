using FluentResults;
using MediatR;

namespace VictoryCenter.BLL.Commands.Admin.FaqQuestions.Delete;

public record DeleteFaqQuestionCommand(long Id)
    : IRequest<Result<long>>;
