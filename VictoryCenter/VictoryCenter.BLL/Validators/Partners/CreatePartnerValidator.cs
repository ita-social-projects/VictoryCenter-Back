/*using FluentValidation;
using VictoryCenter.BLL.Constants;*/
using VictoryCenter.BLL.DTOs.Admin.Partners;

namespace VictoryCenter.BLL.Validators.Partners;

public class CreatePartnerValidator : BasePartnerValidator<CreatePartnerDto>
{
    public CreatePartnerValidator()
    {
/*        RuleFor(x => x.Image)
            .NotNull()
            .WithMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(CreatePartnerDto.Image)))
            .SetValidator(new CreatePartnerImageValidator());*/
    }
}
