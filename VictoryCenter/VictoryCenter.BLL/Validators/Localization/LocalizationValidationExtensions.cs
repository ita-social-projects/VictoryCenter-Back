using FluentValidation;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.Helpers;

namespace VictoryCenter.BLL.Validators.Localization;

internal static class LocalizationValidationExtensions
{
    public static IRuleBuilderOptions<T, string> MustHaveVisibleLength<T>(
        this IRuleBuilderInitial<T, string> ruleBuilder,
        string propertyName,
        int minLength,
        int maxLength)
    {
        return ruleBuilder
            .NotEmpty()
                .WithMessage(ErrorMessagesConstants.PropertyIsRequired(propertyName))
            .Must(value => HtmlContentHelper.StripHtmlTags(value).Length >= minLength)
                .WithMessage(ErrorMessagesConstants.PropertyMustHaveAMinimumVisibleLengthOfNCharacters(propertyName, minLength))
            .Must(value => HtmlContentHelper.StripHtmlTags(value).Length <= maxLength)
                .WithMessage(ErrorMessagesConstants.PropertyMustHaveAMaximumVisibleLengthOfNCharacters(propertyName, maxLength));
    }
}
