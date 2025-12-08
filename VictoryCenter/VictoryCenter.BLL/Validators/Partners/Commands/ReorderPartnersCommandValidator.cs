using FluentValidation;
using VictoryCenter.BLL.Commands.Admin.Partners.ReorderPartners;
using VictoryCenter.BLL.Validators.Partners.Dto;

namespace VictoryCenter.BLL.Validators.Partners.Commands;

public class ReorderPartnersCommandValidator : AbstractValidator<ReorderPartnersCommand>
{
    public ReorderPartnersCommandValidator()
    {
        RuleFor(x => x.ReorderDto)
            .NotNull()
            .SetValidator(new ReorderPartnersValidator());
    }
}
