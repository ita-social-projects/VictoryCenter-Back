using FluentValidation;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.Localization.MainPage;

namespace VictoryCenter.BLL.Validators.Localization.MainPage;

internal static class MainPageLocalizationValidationRules
{
    public static void AddTitleAndDescriptionRules<TDto>(
        this AbstractValidator<TDto> validator,
        int descriptionMaxLength)
        where TDto : BaseMainPageLocalizationDto
    {
        AddTitleAndDescriptionRules(validator, descriptionMaxLength, _ => true, _ => true);
    }

    public static void AddOptionalTitleAndDescriptionRules<TDto>(
        this AbstractValidator<TDto> validator,
        int descriptionMaxLength)
        where TDto : BaseMainPageLocalizationDto
    {
        AddTitleAndDescriptionRules(
            validator,
            descriptionMaxLength,
            dto => dto.Title is not null,
            dto => dto.Description is not null);
    }

    private static void AddTitleAndDescriptionRules<TDto>(
        AbstractValidator<TDto> validator,
        int descriptionMaxLength,
        Func<TDto, bool> titleCondition,
        Func<TDto, bool> descriptionCondition)
        where TDto : BaseMainPageLocalizationDto
    {
        validator.RuleFor(x => x.Title)
            .Cascade(CascadeMode.Stop)
            .Must(value => !string.IsNullOrWhiteSpace(value))
            .WithMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(BaseMainPageLocalizationDto.Title)))
            .MinimumLength(MainPageConstants.Localization.Title.MinLength)
            .WithMessage(ErrorMessagesConstants.PropertyMustHaveAMinimumLengthOfNCharacters(
                nameof(BaseMainPageLocalizationDto.Title), MainPageConstants.Localization.Title.MinLength))
            .MaximumLength(MainPageConstants.Localization.Title.MaxLength)
            .WithMessage(ErrorMessagesConstants.PropertyMustHaveAMaximumLengthOfNCharacters(
                nameof(BaseMainPageLocalizationDto.Title), MainPageConstants.Localization.Title.MaxLength))
            .When(titleCondition);

        validator.RuleFor(x => x.Description)
            .Cascade(CascadeMode.Stop)
            .Must(value => !string.IsNullOrWhiteSpace(value))
            .WithMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(BaseMainPageLocalizationDto.Description)))
            .MinimumLength(MainPageConstants.Localization.SectionDescription.MinLength)
            .WithMessage(ErrorMessagesConstants.PropertyMustHaveAMinimumLengthOfNCharacters(
                nameof(BaseMainPageLocalizationDto.Description), MainPageConstants.Localization.SectionDescription.MinLength))
            .MaximumLength(descriptionMaxLength)
            .WithMessage(ErrorMessagesConstants.PropertyMustHaveAMaximumLengthOfNCharacters(
                nameof(BaseMainPageLocalizationDto.Description), descriptionMaxLength))
            .When(descriptionCondition);
    }
}
