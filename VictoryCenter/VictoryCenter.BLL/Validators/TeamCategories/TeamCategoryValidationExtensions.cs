using FluentValidation;
using VictoryCenter.BLL.Constants;

namespace VictoryCenter.BLL.Validators.TeamCategories;

internal static class TeamCategoryValidationExtensions
{
    public static IRuleBuilderOptions<T, string> ValidTeamCategoryName<T>(
        this IRuleBuilderInitial<T, string> ruleBuilder,
        string propertyName)
    {
        return ruleBuilder
            .Cascade(CascadeMode.Stop)
            .Must(name => !string.IsNullOrWhiteSpace(name))
                .WithMessage(ErrorMessagesConstants.PropertyIsRequired(propertyName))
            .Must(name => name == name.Trim())
                .WithMessage(ErrorMessagesConstants.PropertyMustNotHaveLeadingOrTrailingSpaces(propertyName))
            .Must(name => name.Length >= TeamCategoryConstants.MinNameLength)
                .WithMessage(ErrorMessagesConstants.PropertyMustHaveAMinimumLengthOfNCharacters(
                    propertyName, TeamCategoryConstants.MinNameLength))
            .Must(name => name.Length <= TeamCategoryConstants.MaxNameLength)
                .WithMessage(ErrorMessagesConstants.PropertyMustHaveAMaximumLengthOfNCharacters(
                    propertyName, TeamCategoryConstants.MaxNameLength));
    }

    public static IRuleBuilderOptions<T, string> ValidTeamCategoryDescription<T>(
        this IRuleBuilderInitial<T, string> ruleBuilder,
        string propertyName)
    {
        return ruleBuilder
            .Cascade(CascadeMode.Stop)
            .Must(description => !string.IsNullOrWhiteSpace(description))
                .WithMessage(ErrorMessagesConstants.PropertyIsRequired(propertyName))
            .Must(description => description == description.Trim())
                .WithMessage(ErrorMessagesConstants.PropertyMustNotHaveLeadingOrTrailingSpaces(propertyName))
            .Must(description => description.Length >= TeamCategoryConstants.MinDescriptionLength)
                .WithMessage(ErrorMessagesConstants.PropertyMustHaveAMinimumLengthOfNCharacters(
                    propertyName, TeamCategoryConstants.MinDescriptionLength))
            .Must(description => description.Length <= TeamCategoryConstants.MaxDescriptionLength)
                .WithMessage(ErrorMessagesConstants.PropertyMustHaveAMaximumLengthOfNCharacters(
                    propertyName, TeamCategoryConstants.MaxDescriptionLength));
    }
}
