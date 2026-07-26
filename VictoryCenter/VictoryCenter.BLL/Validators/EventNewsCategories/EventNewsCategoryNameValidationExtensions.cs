using FluentValidation;
using VictoryCenter.BLL.Constants;

namespace VictoryCenter.BLL.Validators.EventNewsCategories;

internal static class EventNewsCategoryNameValidationExtensions
{
    public static IRuleBuilderOptions<T, string> ValidEventNewsCategoryName<T>(
        this IRuleBuilderInitial<T, string> ruleBuilder)
    {
        return ruleBuilder
            .Must(name => !string.IsNullOrWhiteSpace(name))
            .WithMessage(ErrorMessagesConstants.PropertyIsRequired("Name"))
            .Must(name => string.IsNullOrWhiteSpace(name)
                || name.Trim().Length >= EventNewsCategoryConstants.MinNameLength)
            .WithMessage(ErrorMessagesConstants.PropertyMustHaveAMinimumLengthOfNCharacters(
                "Name",
                EventNewsCategoryConstants.MinNameLength))
            .Must(name => string.IsNullOrWhiteSpace(name)
                || name.Trim().Length <= EventNewsCategoryConstants.MaxNameLength)
            .WithMessage(ErrorMessagesConstants.PropertyMustHaveAMaximumLengthOfNCharacters(
                "Name",
                EventNewsCategoryConstants.MaxNameLength));
    }
}
