using FluentValidation;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.Partners;

namespace VictoryCenter.BLL.Validators.Partners;

public abstract class BasePartnerValidator<TPartnerDto> : AbstractValidator<TPartnerDto>
    where TPartnerDto : BasePartnerCreateUpdateDto
{
    protected BasePartnerValidator()
    {
        RuleFor(x => x.Description)
            .NotEmpty().WithMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(BasePartnerCreateUpdateDto.Description)))
            .MaximumLength(PartnerConstants.PartnerDescriptionMaxLength)
            .WithMessage(ErrorMessagesConstants.PropertyMustHaveAMaximumLengthOfNCharacters(
                nameof(BasePartnerCreateUpdateDto.Description), PartnerConstants.PartnerDescriptionMaxLength));
    }
}
