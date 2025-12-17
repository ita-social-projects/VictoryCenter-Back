using VictoryCenter.BLL.DTOs.Common;
using VictoryCenter.DAL.Enums;

namespace VictoryCenter.BLL.DTOs.Admin.Localization.FaqQuestions;

public record FaqQuestionLocalizationDto
{
    public long EntityId { get; init; }

    public LocalizationInfoDto LocalizationInfoDto { get; init; } = null!;

    public string QuestionText { get; init; } = null!;

    public string AnswerText { get; init; } = null!;

    public TranslationStatus TranslationStatus { get; init; }
}
