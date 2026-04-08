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
            .MinimumLength(MainPageConstants.Title.MinLength)
            .WithMessage(ErrorMessagesConstants.PropertyMustHaveAMinimumLengthOfNCharacters(
                nameof(BaseMainPageDto.Title), MainPageConstants.Title.MinLength))
            .MaximumLength(MainPageConstants.Title.MaxLength)
            .WithMessage(ErrorMessagesConstants.PropertyMustHaveAMaximumLengthOfNCharacters(
                nameof(BaseMainPageDto.Title), MainPageConstants.Title.MaxLength));

        RuleFor(x => x.Description)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(BaseMainPageDto.Description)))
            .MinimumLength(MainPageConstants.Description.MinLength)
            .WithMessage(ErrorMessagesConstants.PropertyMustHaveAMinimumLengthOfNCharacters(
                nameof(BaseMainPageDto.Description), MainPageConstants.Description.MinLength))
            .MaximumLength(MainPageConstants.Description.MaxLength)
            .WithMessage(ErrorMessagesConstants.PropertyMustHaveAMaximumLengthOfNCharacters(
                nameof(BaseMainPageDto.Description), MainPageConstants.Description.MaxLength));
    }
}
