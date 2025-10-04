using VictoryCenter.BLL.DTOs.Admin.Partners;

namespace VictoryCenter.BLL.Validators.Partners;

public class CreatePartnerSectionValidator : BasePartnerSectionValidator<CreatePartnerDto>
{
    public CreatePartnerSectionValidator()
    {
        RuleForEach(x => x.Partners)
            .SetValidator(new CreatePartnerValidator());
    }
}
