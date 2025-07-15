using FluentValidation;
using VictoryCenter.BLL.Commands.TeamMembers.Reorder;
using VictoryCenter.BLL.Constants;

namespace VictoryCenter.BLL.Validators.TeamMembers;

public class ReorderTeamMembersValidator : AbstractValidator<ReorderTeamMembersCommand>
{
    private const int MaxTeamMemberIds = 500;

    public ReorderTeamMembersValidator()
    {
        RuleFor(x => x.ReorderTeamMembersDto.CategoryId)
            .GreaterThan(0)
            .WithMessage(ErrorMessagesConstants.PropertyMustBePositive("CategoryId"));

        RuleFor(x => x.ReorderTeamMembersDto.OrderedIds)
            .Cascade(CascadeMode.Stop)
            .NotNull()
            .WithMessage(ErrorMessagesConstants.PropertyIsRequired("Ordered Ids"))
            .Must(ids => ids.Count > 0)
            .WithMessage(TeamMemberConstants.OrderedIdsCannotBeEmpty)
            .Must(ids => ids.Count <= MaxTeamMemberIds)
            .WithMessage(TeamMemberConstants.OrderedIdsCannotContainMoreThanNElements(MaxTeamMemberIds))
            .Must(ids => ids.Distinct().Count() == ids.Count)
            .WithMessage(TeamMemberConstants.OrderedIdsMustContainUniqueValues);

        RuleForEach(x => x.ReorderTeamMembersDto.OrderedIds)
            .GreaterThan(0)
            .WithMessage(ErrorMessagesConstants.PropertyMustBeGreaterThan("Each ID in OrderedIDS", 0));
    }
}
