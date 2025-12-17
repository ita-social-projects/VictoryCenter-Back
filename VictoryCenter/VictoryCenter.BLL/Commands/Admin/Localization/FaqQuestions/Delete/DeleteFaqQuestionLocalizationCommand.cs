using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Admin.Localization.FaqQuestions;

namespace VictoryCenter.BLL.Commands.Admin.Localization.FaqQuestions.Delete;

public record DeleteFaqQuestionLocalizationCommand(long EntityId, long LanguageId)
    : IRequest<Result<DeleteFaqQuestionLocalizationDto>>;
