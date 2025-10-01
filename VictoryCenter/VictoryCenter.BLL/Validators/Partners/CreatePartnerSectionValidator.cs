using FluentValidation;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.Partners;

namespace VictoryCenter.BLL.Validators.Partners;

public class CreatePartnerSectionValidator : AbstractValidator<CreatePartnersSectionDto>
{
    public static readonly int TitleMaxLength = 50;
    public static readonly int DescriptionMaxLength = 100;
    public static readonly int PartnersMaxCount = 50;

    public CreatePartnerSectionValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty()
            .WithMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(CreatePartnersSectionDto.Title)))
            .MaximumLength(TitleMaxLength)
            .WithMessage(ErrorMessagesConstants
                .PropertyMustHaveAMaximumLengthOfNCharacters(nameof(CreatePartnersSectionDto.Title), TitleMaxLength));

        RuleFor(x => x.Description)
            .NotEmpty()
            .WithMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(CreatePartnersSectionDto.Description)))
            .MaximumLength(DescriptionMaxLength)
            .WithMessage(ErrorMessagesConstants
                .PropertyMustHaveAMaximumLengthOfNCharacters(nameof(CreatePartnersSectionDto.Description), DescriptionMaxLength));

        RuleFor(x => x.Partners)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage(ErrorMessagesConstants.CollectionCannotBeEmpty(nameof(CreatePartnersSectionDto.Partners)))
            .Must(partners => partners.Count <= PartnersMaxCount)
            .WithMessage(ErrorMessagesConstants
                .CollectionCannotContainMoreThan(nameof(CreatePartnersSectionDto.Partners), PartnersMaxCount));

        RuleForEach(x => x.Partners)
            .SetValidator(new CreatePartnerValidator());
    }
}
