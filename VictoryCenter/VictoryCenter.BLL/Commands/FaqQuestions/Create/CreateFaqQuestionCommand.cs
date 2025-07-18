using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.FaqQuestions;

namespace VictoryCenter.BLL.Commands.FaqQuestions.Create;

public record CreateFaqQuestionCommand(CreateFaqQuestionDto CreateFaqQuestionDto)
    : IRequest<Result<FaqQuestionDto>>;
