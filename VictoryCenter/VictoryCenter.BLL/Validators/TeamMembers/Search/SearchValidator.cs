using FluentValidation;
using VictoryCenter.BLL.Queries.TeamMembers.Search;

namespace VictoryCenter.BLL.Validators.TeamMembers.Search;

public class SearchValidator : AbstractValidator<SearchTeamMemberQuery>
{
    private const int FullNameMinLength = 2;
    private const int FullNameMaxLength = 100;

    public SearchValidator()
    {
        RuleFor(x => x.SearchTeamMemberDto.FullName)
            .NotEmpty().WithMessage("FullName field is required")
            .MinimumLength(FullNameMinLength).WithMessage($"Full name must be at least {FullNameMinLength} characters long")
            .MaximumLength(FullNameMaxLength).WithMessage($"Full name must be no longer than {FullNameMaxLength} characters");
    }
}
