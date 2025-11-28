using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Admin.Localization.FaqQuestions;

namespace VictoryCenter.BLL.Commands.Admin.Localization.FaqQuestions.Create;

public record CreateFaqQuestionLocalizationCommand(CreateFaqQuestionLocalizationDto CreateFaqQuestionLocalizationDto)
   : IRequest<Result<FaqQuestionLocalizationDto>>;
