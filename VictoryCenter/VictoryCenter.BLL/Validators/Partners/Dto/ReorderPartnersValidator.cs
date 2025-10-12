using FluentValidation;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.Partners;
using VictoryCenter.BLL.Validators.Common;

namespace VictoryCenter.BLL.Validators.Partners.Dto;

public class ReorderPartnersValidator : BaseReorderValidator<ReorderPartnersDto>
{
    public ReorderPartnersValidator()
        : base(PartnerConstants.PartnersMaxCount)
    {
        RuleFor(x => x.PartnersSectionId)
            .GreaterThan(0)
            .WithMessage(ErrorMessagesConstants.PropertyMustBePositive(nameof(ReorderPartnersDto.PartnersSectionId)));
    }
}
