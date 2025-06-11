using FluentValidation;
using VictoryCenter.BLL.DTOs.TeamMembers;

namespace VictoryCenter.BLL.Validators.TeamMembers;

public class BaseTeamMembersValidator : AbstractValidator<CreateTeamMemberDto>
{
    private const int FirstNameMinLength = 2;
    private const int DescriptionNameMaxLength = 200;
    
    public BaseTeamMembersValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("This field is required")
            .MinimumLength(FirstNameMinLength).WithMessage("name must be longer than 2 characters");

        RuleFor(x => x.CategoryId)
            .NotNull().WithMessage("CategoryId can't be null")
            .GreaterThan(0).WithMessage("CategoryId must be positive value");
        RuleFor(x => x.Status)
            .NotNull().WithMessage("Status cant be null");
        RuleFor(x => x.Description)
            .MaximumLength(DescriptionNameMaxLength).WithMessage("the description length cannot exceed 200 characters");
        
    }
}