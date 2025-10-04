using FluentValidation;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.Partners;

namespace VictoryCenter.BLL.Validators.Partners;

public abstract class BasePartnerSectionValidator<TPartnerDto> : AbstractValidator<BasePartnerSectionCreateUpdateDto<TPartnerDto>>
    where TPartnerDto : BasePartnerCreateUpdateDto
{
    protected BasePartnerSectionValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty()
            .WithMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(BasePartnerSectionCreateUpdateDto<TPartnerDto>.Title)))
            .MaximumLength(PartnerConstants.TitleMaxLength)
            .WithMessage(ErrorMessagesConstants.PropertyMustHaveAMaximumLengthOfNCharacters(
                nameof(BasePartnerSectionCreateUpdateDto<TPartnerDto>.Title), PartnerConstants.TitleMaxLength));

        RuleFor(x => x.Description)
            .NotEmpty()
            .WithMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(BasePartnerSectionCreateUpdateDto<TPartnerDto>.Description)))
            .MaximumLength(PartnerConstants.DescriptionMaxLength)
            .WithMessage(ErrorMessagesConstants.PropertyMustHaveAMaximumLengthOfNCharacters(
                nameof(BasePartnerSectionCreateUpdateDto<TPartnerDto>.Description), PartnerConstants.DescriptionMaxLength));

        RuleFor(x => x.Partners)
            .NotEmpty()
            .WithMessage(ErrorMessagesConstants.CollectionCannotBeEmpty(nameof(BasePartnerSectionCreateUpdateDto<TPartnerDto>.Partners)))
            .Must(partners => partners.Count <= PartnerConstants.PartnersMaxCount)
            .WithMessage(ErrorMessagesConstants.CollectionCannotContainMoreThan(
                nameof(BasePartnerSectionCreateUpdateDto<TPartnerDto>.Partners), PartnerConstants.PartnersMaxCount));
    }
}
