using FluentValidation;
using VictoryCenter.BLL.Commands.TeamMembers.Reorder;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.TeamMembers;

namespace VictoryCenter.BLL.Validators.TeamMembers;

public class ReorderTeamMembersValidator : AbstractValidator<ReorderTeamMembersCommand>
{
    public ReorderTeamMembersValidator()
    {
        RuleFor(x => x.ReorderTeamMembersDto.CategoryId)
            .GreaterThan(0)
            .WithMessage(ErrorMessagesConstants.PropertyMustBePositive(nameof(ReorderTeamMembersDto.CategoryId)));

        RuleFor(x => x.ReorderTeamMembersDto.OrderedIds)
            .Cascade(CascadeMode.Stop)
            .NotNull()
            .WithMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(ReorderTeamMembersDto.OrderedIds)))
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

    public static int MaxTeamMemberIds { get; } = 500;
}
