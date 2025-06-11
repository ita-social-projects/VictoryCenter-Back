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
            .MinimumLength(FirstNameMinLength).WithMessage($"First name must be at least {FirstNameMinLength} characters long");

        RuleFor(x => x.CategoryId)
            .GreaterThan(0).WithMessage("CategoryId must be positive value");
        RuleFor(x => x.Status)
            .IsInEnum().WithMessage("Unknown status value");
        RuleFor(x => x.Description)
            .MaximumLength(DescriptionNameMaxLength).WithMessage($"the description length cannot exceed {DescriptionNameMaxLength} characters");
        
    }
}