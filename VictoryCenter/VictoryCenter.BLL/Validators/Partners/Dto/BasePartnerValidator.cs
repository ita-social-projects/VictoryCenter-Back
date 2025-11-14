using FluentValidation;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.Partners;

namespace VictoryCenter.BLL.Validators.Partners.Dto;

public abstract class BasePartnerValidator<TPartnerDto> : AbstractValidator<TPartnerDto>
    where TPartnerDto : BasePartnerCreateUpdateDto
{
    protected BasePartnerValidator()
    {
        RuleFor(x => x.Description)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(BasePartnerCreateUpdateDto.Description)))
            .MinimumLength(PartnerConstants.PartnerDescriptionMinLength)
            .WithMessage(ErrorMessagesConstants.PropertyMustHaveAMinimumLengthOfNCharacters(
                nameof(BasePartnerCreateUpdateDto.Description), PartnerConstants.PartnerDescriptionMinLength))
            .MaximumLength(PartnerConstants.PartnerDescriptionMaxLength)
            .WithMessage(ErrorMessagesConstants.PropertyMustHaveAMaximumLengthOfNCharacters(
                nameof(BasePartnerCreateUpdateDto.Description), PartnerConstants.PartnerDescriptionMaxLength));
    }
}
