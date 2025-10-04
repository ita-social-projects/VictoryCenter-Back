using FluentValidation;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.Partners;

namespace VictoryCenter.BLL.Validators.Partners;

public class UpdatePartnerValidator : BasePartnerValidator<UpdatePartnerDto>
{
    public UpdatePartnerValidator()
    {
        RuleFor(x => x.Image)
            .NotNull()
            .WithMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(UpdatePartnerDto.Image)))
            .SetValidator(new UpdatePartnerImageValidator());
    }
}
