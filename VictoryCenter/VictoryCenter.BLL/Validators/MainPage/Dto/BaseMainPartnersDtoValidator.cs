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
            .MinimumLength(MainPageConstants.ValidationTitleRules.MinLen)
            .WithMessage(ErrorMessagesConstants.PropertyMustHaveAMinimumLengthOfNCharacters(
                nameof(BaseMainPartnersDto.Title), MainPageConstants.ValidationTitleRules.MinLen))
            .MaximumLength(MainPageConstants.ValidationTitleRules.MaxLen)
            .WithMessage(ErrorMessagesConstants.PropertyMustHaveAMaximumLengthOfNCharacters(
                nameof(BaseMainPartnersDto.Title), MainPageConstants.ValidationTitleRules.MaxLen));

        RuleFor(x => x.Description)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(BaseMainPartnersDto.Description)))
            .MinimumLength(MainPageConstants.ValidationDescriptionRules.MinLen)
            .WithMessage(ErrorMessagesConstants.PropertyMustHaveAMinimumLengthOfNCharacters(
                nameof(BaseMainPartnersDto.Description), MainPageConstants.ValidationDescriptionRules.MinLen))
            .MaximumLength(MainPageConstants.ValidationDescriptionRules.MaxLen)
            .WithMessage(ErrorMessagesConstants.PropertyMustHaveAMaximumLengthOfNCharacters(
                nameof(BaseMainPartnersDto.Description), MainPageConstants.ValidationDescriptionRules.MaxLen));
    }
}
