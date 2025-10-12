using FluentValidation;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.Partners;

namespace VictoryCenter.BLL.Validators.Partners.Dto;

public class UpdatePartnerValidator : BasePartnerValidator<UpdatePartnerDto>
{
    public UpdatePartnerValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0)
            .WithMessage(ErrorMessagesConstants.PropertyMustBeGreaterThan(nameof(UpdatePartnerDto.Id), 0));
    }
}
