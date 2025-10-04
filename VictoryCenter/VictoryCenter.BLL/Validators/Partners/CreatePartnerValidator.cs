using VictoryCenter.BLL.DTOs.Admin.Partners;

namespace VictoryCenter.BLL.Validators.Partners;

public class CreatePartnerValidator : BasePartnerValidator<CreatePartnerDto>
{
    public CreatePartnerValidator()
    {
        RuleFor(x => x.Image)
            .SetValidator(new CreatePartnerImageValidator());
    }
}
