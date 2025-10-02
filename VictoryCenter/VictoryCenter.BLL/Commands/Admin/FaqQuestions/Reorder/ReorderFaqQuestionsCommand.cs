using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Admin.FaqQuestions;

namespace VictoryCenter.BLL.Commands.Admin.FaqQuestions.Reorder;

public record ReorderFaqQuestionsCommand(ReorderFaqQuestionsDto ReorderFaqQuestionsDto)
    : IRequest<Result<Unit>>;
