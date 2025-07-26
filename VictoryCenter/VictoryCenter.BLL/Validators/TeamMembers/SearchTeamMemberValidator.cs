using FluentValidation;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.TeamMembers;
using VictoryCenter.BLL.Queries.TeamMembers.Search;

namespace VictoryCenter.BLL.Validators.TeamMembers;

public class SearchTeamMemberValidator : AbstractValidator<SearchTeamMemberQuery>
{
    private const int FullNameMinLength = 2;
    private const int FullNameMaxLength = 100;

    public SearchTeamMemberValidator()
    {
        RuleFor(x => x.SearchTeamMemberDto.FullName)
            .NotEmpty().WithMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(SearchTeamMemberDto.FullName)))
            .MinimumLength(FullNameMinLength).WithMessage(ErrorMessagesConstants
            .PropertyMustHaveAMinimumLengthOfNCharacters(nameof(SearchTeamMemberDto.FullName), FullNameMinLength))
            .MaximumLength(FullNameMaxLength).WithMessage(ErrorMessagesConstants
            .PropertyMustHaveAMaximumLengthOfNCharacters(nameof(SearchTeamMemberDto.FullName), FullNameMaxLength));
    }
}
