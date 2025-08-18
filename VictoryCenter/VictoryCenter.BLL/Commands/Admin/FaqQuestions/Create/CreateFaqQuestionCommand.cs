using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Admin.FaqQuestions;

namespace VictoryCenter.BLL.Commands.Admin.FaqQuestions.Create;

public record CreateFaqQuestionCommand(CreateFaqQuestionDto CreateFaqQuestionDto)
    : IRequest<Result<FaqQuestionDto>>;
