using FluentValidation;
using VictoryCenter.BLL.DTOs.TeamMembers;

namespace VictoryCenter.BLL.Validators.TeamMembers;

public class ReorderTeamMembersValidator : AbstractValidator<ReorderTeamMembersDto>
{
    public ReorderTeamMembersValidator()
    {
        RuleFor(x => x.CategoryId)
            .GreaterThan(0)
            .WithMessage("CategoryId must be greater than 0");

        RuleFor(x => x.OrderedIds)
            .NotNull()
            .WithMessage("OrderedIds must be provided")
            .Must(ids => ids.Count > 0)
            .WithMessage("OrderedIds cannot be empty")
            .Must(ids => ids.Distinct().Count() == ids.Count)
            .WithMessage("OrderedIds must contain unique values");

        RuleForEach(x => x.OrderedIds)
            .GreaterThan(0)
            .WithMessage("Each ID in OrderedIds must be greater than 0");
    }
}
