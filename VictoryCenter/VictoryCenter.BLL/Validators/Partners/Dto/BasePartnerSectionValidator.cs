using FluentValidation;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.Partners;

namespace VictoryCenter.BLL.Validators.Partners.Dto;

public abstract class BasePartnerSectionValidator<TPartnerDto> : AbstractValidator<TPartnerDto>
    where TPartnerDto : BasePartnerSectionCreateUpdateDto
{
    protected BasePartnerSectionValidator()
    {
        RuleFor(x => x.Title)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(BasePartnerSectionCreateUpdateDto.Title)))
            .MinimumLength(PartnerConstants.PartnersSectionTitleMinLength)
            .WithMessage(ErrorMessagesConstants.PropertyMustHaveAMinimumLengthOfNCharacters(
                nameof(BasePartnerSectionCreateUpdateDto.Title), PartnerConstants.PartnersSectionTitleMinLength))
            .MaximumLength(PartnerConstants.PartnersSectionTitleMaxLength)
            .WithMessage(ErrorMessagesConstants.PropertyMustHaveAMaximumLengthOfNCharacters(
                nameof(BasePartnerSectionCreateUpdateDto.Title), PartnerConstants.PartnersSectionTitleMaxLength));

        RuleFor(x => x.Description)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(BasePartnerSectionCreateUpdateDto.Description)))
            .MinimumLength(PartnerConstants.PartnersSectionDescriptionMinLength)
            .WithMessage(ErrorMessagesConstants.PropertyMustHaveAMinimumLengthOfNCharacters(
                nameof(BasePartnerSectionCreateUpdateDto.Description), PartnerConstants.PartnersSectionDescriptionMinLength))
            .MaximumLength(PartnerConstants.PartnersSectionDescriptionMaxLength)
            .WithMessage(ErrorMessagesConstants.PropertyMustHaveAMaximumLengthOfNCharacters(
                nameof(BasePartnerSectionCreateUpdateDto.Description), PartnerConstants.PartnersSectionDescriptionMaxLength));
    }
}
