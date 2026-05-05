using FluentValidation;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.Localization.MainPage;

namespace VictoryCenter.BLL.Validators.Localization.MainPage;

public class BaseMainPageLocalizationDtoValidator : AbstractValidator<BaseMainPageLocalizationDto>
{
    public BaseMainPageLocalizationDtoValidator()
    {
        RuleFor(x => x.Title)
            .MinimumLength(MainPageConstants.Title.MinLength)
            .WithMessage(ErrorMessagesConstants.PropertyMustHaveAMinimumLengthOfNCharacters(
                nameof(BaseMainPageLocalizationDto.Title), MainPageConstants.Title.MinLength))
            .MaximumLength(MainPageConstants.Title.MaxLength)
            .WithMessage(ErrorMessagesConstants.PropertyMustHaveAMaximumLengthOfNCharacters(
                nameof(BaseMainPageLocalizationDto.Title), MainPageConstants.Title.MaxLength))
            .When(x => x.Title != null);

        RuleFor(x => x.Description)
            .MinimumLength(MainPageConstants.Description.MinLength)
            .WithMessage(ErrorMessagesConstants.PropertyMustHaveAMinimumLengthOfNCharacters(
                nameof(BaseMainPageLocalizationDto.Description), MainPageConstants.Description.MinLength))
            .MaximumLength(MainPageConstants.Description.MaxLength)
            .WithMessage(ErrorMessagesConstants.PropertyMustHaveAMaximumLengthOfNCharacters(
                nameof(BaseMainPageLocalizationDto.Description), MainPageConstants.Description.MaxLength))
            .When(x => x.Description != null);
    }
}
