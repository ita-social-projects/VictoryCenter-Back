using FluentValidation;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.TeamMembers;
using VictoryCenter.BLL.Queries.Admin.TeamMembers.Search;

namespace VictoryCenter.BLL.Validators.TeamMembers;

public class SearchTeamMemberValidator : AbstractValidator<SearchTeamMemberQuery>
{
    public SearchTeamMemberValidator()
    {
        RuleFor(x => x.SearchTeamMemberDto.FullName)
            .NotEmpty().WithMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(SearchTeamMemberDto.FullName)))
            .MinimumLength(FullNameMinLength).WithMessage(ErrorMessagesConstants
            .PropertyMustHaveAMinimumLengthOfNCharacters(nameof(SearchTeamMemberDto.FullName), FullNameMinLength))
            .MaximumLength(FullNameMaxLength).WithMessage(ErrorMessagesConstants
            .PropertyMustHaveAMaximumLengthOfNCharacters(nameof(SearchTeamMemberDto.FullName), FullNameMaxLength));
    }

    public static int FullNameMinLength { get; } = 2;
    public static int FullNameMaxLength { get; } = 100;
}
