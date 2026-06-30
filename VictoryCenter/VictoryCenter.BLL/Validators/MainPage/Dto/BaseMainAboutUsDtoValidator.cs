using FluentValidation;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.MainAboutUs;

namespace VictoryCenter.BLL.Validators.MainPage.Dto;

public abstract class BaseMainAboutUsDtoValidator<TDto> : AbstractValidator<TDto>
    where TDto : BaseMainAboutUsDto
{
    protected BaseMainAboutUsDtoValidator()
    {
        RuleFor(x => x.Title)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(BaseMainAboutUsDto.Title)))
            .MinimumLength(MainPageConstants.ValidationTitleRules.MinLen)
            .WithMessage(ErrorMessagesConstants.PropertyMustHaveAMinimumLengthOfNCharacters(
                nameof(BaseMainAboutUsDto.Title), MainPageConstants.ValidationTitleRules.MinLen))
            .MaximumLength(MainPageConstants.ValidationTitleRules.MaxLen)
            .WithMessage(ErrorMessagesConstants.PropertyMustHaveAMaximumLengthOfNCharacters(
                nameof(BaseMainAboutUsDto.Title), MainPageConstants.ValidationTitleRules.MaxLen));

        RuleFor(x => x.Description)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(BaseMainAboutUsDto.Description)))
            .MinimumLength(MainPageConstants.ValidationDescriptionRules.MinLen)
            .WithMessage(ErrorMessagesConstants.PropertyMustHaveAMinimumLengthOfNCharacters(
                nameof(BaseMainAboutUsDto.Description), MainPageConstants.ValidationDescriptionRules.MinLen))
            .MaximumLength(MainPageConstants.ValidationDescriptionRules.MaxLen)
            .WithMessage(ErrorMessagesConstants.PropertyMustHaveAMaximumLengthOfNCharacters(
                nameof(BaseMainAboutUsDto.Description), MainPageConstants.ValidationDescriptionRules.MaxLen));
    }
}
