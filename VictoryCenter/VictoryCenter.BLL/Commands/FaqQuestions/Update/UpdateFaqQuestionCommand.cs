using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.FaqQuestions;

namespace VictoryCenter.BLL.Commands.FaqQuestions.Update;

public record UpdateFaqQuestionCommand(UpdateFaqQuestionDto UpdateFaqQuestionDto, long Id)
    : IRequest<Result<FaqQuestionDto>>;
