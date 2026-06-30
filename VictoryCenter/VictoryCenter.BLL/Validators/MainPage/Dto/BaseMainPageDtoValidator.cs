using FluentValidation;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.MainPages;

namespace VictoryCenter.BLL.Validators.MainPage.Dto;

public abstract class BaseMainPageDtoValidator<TDto> : AbstractValidator<TDto>
    where TDto : BaseMainPageDto
{
    protected BaseMainPageDtoValidator()
    {
        RuleFor(x => x.Title)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(BaseMainPageDto.Title)))
            .MinimumLength(MainPageConstants.ValidationTitleRules.MinLen)
            .WithMessage(ErrorMessagesConstants.PropertyMustHaveAMinimumLengthOfNCharacters(
                nameof(BaseMainPageDto.Title), MainPageConstants.ValidationTitleRules.MinLen))
            .MaximumLength(MainPageConstants.ValidationTitleRules.MaxLen)
            .WithMessage(ErrorMessagesConstants.PropertyMustHaveAMaximumLengthOfNCharacters(
                nameof(BaseMainPageDto.Title), MainPageConstants.ValidationTitleRules.MaxLen));

        RuleFor(x => x.Description)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(BaseMainPageDto.Description)))
            .MinimumLength(MainPageConstants.ValidationDescriptionRules.MinLen)
            .WithMessage(ErrorMessagesConstants.PropertyMustHaveAMinimumLengthOfNCharacters(
                nameof(BaseMainPageDto.Description), MainPageConstants.ValidationDescriptionRules.MinLen))
            .MaximumLength(MainPageConstants.ValidationDescriptionRules.MaxLen)
            .WithMessage(ErrorMessagesConstants.PropertyMustHaveAMaximumLengthOfNCharacters(
                nameof(BaseMainPageDto.Description), MainPageConstants.ValidationDescriptionRules.MaxLen));
    }
}
