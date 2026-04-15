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
            .MinimumLength(MainPageConstants.Title.MinLength)
            .WithMessage(ErrorMessagesConstants.PropertyMustHaveAMinimumLengthOfNCharacters(
                nameof(BaseMainAboutUsDto.Title), MainPageConstants.Title.MinLength))
            .MaximumLength(MainPageConstants.Title.MaxLength)
            .WithMessage(ErrorMessagesConstants.PropertyMustHaveAMaximumLengthOfNCharacters(
                nameof(BaseMainAboutUsDto.Title), MainPageConstants.Title.MaxLength));

        RuleFor(x => x.Description)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(BaseMainAboutUsDto.Description)))
            .MinimumLength(MainPageConstants.Description.MinLength)
            .WithMessage(ErrorMessagesConstants.PropertyMustHaveAMinimumLengthOfNCharacters(
                nameof(BaseMainAboutUsDto.Description), MainPageConstants.Description.MinLength))
            .MaximumLength(MainPageConstants.Description.MaxLength)
            .WithMessage(ErrorMessagesConstants.PropertyMustHaveAMaximumLengthOfNCharacters(
                nameof(BaseMainAboutUsDto.Description), MainPageConstants.Description.MaxLength));
    }
}
