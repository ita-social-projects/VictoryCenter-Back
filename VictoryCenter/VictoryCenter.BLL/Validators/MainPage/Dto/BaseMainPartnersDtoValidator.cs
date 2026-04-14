using FluentValidation;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.MainPartners;

namespace VictoryCenter.BLL.Validators.MainPage.Dto;

public abstract class BaseMainPartnersDtoValidator<TDto> : AbstractValidator<TDto>
    where TDto : BaseMainPartnersDto
{
    protected BaseMainPartnersDtoValidator()
    {
        RuleFor(x => x.Title)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(BaseMainPartnersDto.Title)))
            .MinimumLength(MainPageConstants.Title.MinLength)
            .WithMessage(ErrorMessagesConstants.PropertyMustHaveAMinimumLengthOfNCharacters(
                nameof(BaseMainPartnersDto.Title), MainPageConstants.Title.MinLength))
            .MaximumLength(MainPageConstants.Title.MaxLength)
            .WithMessage(ErrorMessagesConstants.PropertyMustHaveAMaximumLengthOfNCharacters(
                nameof(BaseMainPartnersDto.Title), MainPageConstants.Title.MaxLength));

        RuleFor(x => x.Description)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(BaseMainPartnersDto.Description)))
            .MinimumLength(MainPageConstants.Description.MinLength)
            .WithMessage(ErrorMessagesConstants.PropertyMustHaveAMinimumLengthOfNCharacters(
                nameof(BaseMainPartnersDto.Description), MainPageConstants.Description.MinLength))
            .MaximumLength(MainPageConstants.Description.MaxLength)
            .WithMessage(ErrorMessagesConstants.PropertyMustHaveAMaximumLengthOfNCharacters(
                nameof(BaseMainPartnersDto.Description), MainPageConstants.Description.MaxLength));
    }
}
