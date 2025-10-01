using FluentValidation;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.Partners;

namespace VictoryCenter.BLL.Validators.Partners;

public class UpdatePartnerValidator : AbstractValidator<UpdatePartnerDto>
{
    public static readonly int DescriptionMaxLength = 50;

    public UpdatePartnerValidator()
    {
        RuleFor(x => x.Description)
            .NotEmpty()
            .WithMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(CreatePartnerDto.Description)))
            .MaximumLength(DescriptionMaxLength)
            .WithMessage(ErrorMessagesConstants
                .PropertyMustHaveAMaximumLengthOfNCharacters(nameof(CreatePartnerDto.Description), DescriptionMaxLength));

        RuleFor(x => x.Image)
            .SetValidator(new UpdatePartnerImageValidator());
    }
}
