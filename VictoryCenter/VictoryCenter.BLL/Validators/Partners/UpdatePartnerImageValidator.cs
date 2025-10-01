using FluentValidation;
using VictoryCenter.BLL.DTOs.Admin.Partners;

namespace VictoryCenter.BLL.Validators.Partners;

public class UpdatePartnerImageValidator : AbstractValidator<UpdatePartnerImageDto>
{
    public UpdatePartnerImageValidator()
    {
        When(x => !x.ImageId.HasValue, () =>
        {
            RuleFor(x => x).SetValidator(new CreatePartnerImageValidator());
        });
    }
}
