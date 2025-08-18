using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Admin.FaqQuestions;

namespace VictoryCenter.BLL.Commands.Admin.FaqQuestions.Update;

public record UpdateFaqQuestionCommand(UpdateFaqQuestionDto UpdateFaqQuestionDto, long Id)
    : IRequest<Result<FaqQuestionDto>>;
