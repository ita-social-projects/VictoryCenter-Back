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
            .NotEmpty()
            .WithMessage(ErrorMessagesConstants.PropertyIsRequired(
                nameof(SearchTeamMemberDto.FullName)))
            .MinimumLength(GlobalSearchConstants.DefaultSearchQueryMinLength)
            .WithMessage(ErrorMessagesConstants.PropertyMustHaveAMinimumLengthOfNCharacters(
                nameof(SearchTeamMemberDto.FullName),
                GlobalSearchConstants.DefaultSearchQueryMinLength))
            .MaximumLength(GlobalSearchConstants.DefaultSearchQueryMaxLength)
            .WithMessage(ErrorMessagesConstants.PropertyMustHaveAMaximumLengthOfNCharacters(
                nameof(SearchTeamMemberDto.FullName),
                GlobalSearchConstants.DefaultSearchQueryMaxLength));
    }
}
