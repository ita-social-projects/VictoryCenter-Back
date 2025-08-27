using FluentValidation;
using VictoryCenter.BLL.Commands.Admin.TeamMembers.Reorder;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.TeamMembers;

namespace VictoryCenter.BLL.Validators.TeamMembers;

public class ReorderTeamMembersValidator : AbstractValidator<ReorderTeamMembersCommand>
{
    private const int MaxTeamMemberIds = 500;

    public ReorderTeamMembersValidator()
    {
        RuleFor(x => x.ReorderTeamMembersDto.CategoryId)
            .GreaterThan(0)
            .WithMessage(ErrorMessagesConstants.PropertyMustBePositive(nameof(ReorderTeamMembersDto.CategoryId)));

        RuleFor(x => x.ReorderTeamMembersDto.OrderedIds)
            .Cascade(CascadeMode.Stop)
            .NotNull()
            .WithMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(ReorderTeamMembersDto.OrderedIds)))
            .NotEmpty()
            .WithMessage(ErrorMessagesConstants.CollectionCannotBeEmpty(nameof(ReorderTeamMembersDto.OrderedIds)))
            .Must(ids => ids.Count <= MaxTeamMemberIds)
            .WithMessage(ErrorMessagesConstants
                .CollectionCannotContainMoreThan(nameof(ReorderTeamMembersDto.OrderedIds), MaxTeamMemberIds))
            .Must(ids => ids.Distinct().Count() == ids.Count)
            .WithMessage(ErrorMessagesConstants
                .CollectionMustContainUniqueValues(nameof(ReorderTeamMembersDto.OrderedIds)));

        RuleForEach(x => x.ReorderTeamMembersDto.OrderedIds)
            .GreaterThan(0)
            .WithMessage(ErrorMessagesConstants
                .PropertyMustBePositive($"Each {nameof(ReorderTeamMembersDto.OrderedIds)} element"));
    }
}
