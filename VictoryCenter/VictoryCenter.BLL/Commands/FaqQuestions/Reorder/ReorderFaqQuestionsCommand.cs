using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.FaqQuestions;

namespace VictoryCenter.BLL.Commands.FaqQuestions.Reorder;

public record ReorderFaqQuestionsCommand(ReorderFaqQuestionsDto ReorderFaqQuestionsDto)
    : IRequest<Result<Unit>>;
