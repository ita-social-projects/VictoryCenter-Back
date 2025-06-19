using FluentValidation;
using VictoryCenter.BLL.Commands.TeamMembers.Reorder;

namespace VictoryCenter.BLL.Validators.TeamMembers;

public class ReorderTeamMembersValidator : AbstractValidator<ReorderTeamMembersCommand>
{
    public ReorderTeamMembersValidator()
    {
        RuleFor(x => x.ReorderTeamMembersDto.CategoryId)
            .GreaterThan(0)
            .WithMessage("CategoryId must be greater than 0");

        RuleFor(x => x.ReorderTeamMembersDto.OrderedIds)
            .NotNull()
            .WithMessage("OrderedIds must be provided")
            .Must(ids => ids.Count > 0)
            .WithMessage("OrderedIds cannot be empty")
            .Must(ids => ids.Distinct().Count() == ids.Count)
            .WithMessage("OrderedIds must contain unique values");

        RuleForEach(x => x.ReorderTeamMembersDto.OrderedIds)
            .GreaterThan(0)
            .WithMessage("Each ID in OrderedIds must be greater than 0");
    }
}
