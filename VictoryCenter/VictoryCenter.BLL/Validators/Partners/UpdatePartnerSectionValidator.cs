using VictoryCenter.BLL.DTOs.Admin.Partners;

namespace VictoryCenter.BLL.Validators.Partners;

public class UpdatePartnerSectionValidator : BasePartnerSectionValidator<UpdatePartnerDto>
{
    public UpdatePartnerSectionValidator()
    {
        RuleForEach(x => x.Partners)
            .SetValidator(new UpdatePartnerValidator());
    }
}
