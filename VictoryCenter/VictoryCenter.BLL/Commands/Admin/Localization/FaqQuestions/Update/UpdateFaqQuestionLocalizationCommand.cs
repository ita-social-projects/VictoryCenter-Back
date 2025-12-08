using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Admin.Localization.FaqQuestions;

namespace VictoryCenter.BLL.Commands.Admin.Localization.FaqQuestions.Update;

public record UpdateFaqQuestionLocalizationCommand(
    UpdateFaqQuestionLocalizationDto UpdateFaqQuestionLocalizationDto,
    long EntityId,
    long LanguageId)
    : IRequest<Result<FaqQuestionLocalizationDto>>;
