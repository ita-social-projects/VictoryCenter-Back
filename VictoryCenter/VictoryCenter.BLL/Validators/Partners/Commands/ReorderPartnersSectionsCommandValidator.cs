using FluentValidation;
using VictoryCenter.BLL.Commands.Admin.Partners.ReorderSections;
using VictoryCenter.BLL.Validators.Partners.Dto;

namespace VictoryCenter.BLL.Validators.Partners.Commands;

public class ReorderPartnersSectionsCommandValidator : AbstractValidator<ReorderPartnersSectionsCommand>
{
    public ReorderPartnersSectionsCommandValidator()
    {
        RuleFor(x => x.ReorderDto)
            .NotNull()
            .SetValidator(new ReorderPartnersSectionsValidator());
    }
}
