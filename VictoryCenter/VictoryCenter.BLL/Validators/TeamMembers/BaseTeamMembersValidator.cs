using FluentValidation;
using VictoryCenter.BLL.DTOs.TeamMembers;
using VictoryCenter.DAL.Enums;

namespace VictoryCenter.BLL.Validators.TeamMembers;

public class BaseTeamMembersValidator : AbstractValidator<CreateTeamMemberDto>
{
    private const int NameMinLength = 2;
    private const int NameMaxLength = 50;
    private const int DescriptionNameMaxLength = 200;
    public BaseTeamMembersValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("This field is required")
            .MinimumLength(NameMinLength).WithMessage($"First name must be at least {NameMinLength} characters long")
            .MaximumLength(NameMaxLength).WithMessage($"First name must be no longer than {NameMaxLength} characters");
        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("This field is required")
            .MinimumLength(NameMinLength).WithMessage($"Last name must be at least {NameMinLength} characters long")
            .MaximumLength(NameMaxLength).WithMessage($"Last name must be no longer than {NameMaxLength} characters");
        RuleFor(x => x.CategoryId)
            .GreaterThan(0).WithMessage("CategoryId must be positive value");
        RuleFor(x => x.Status)
            .IsInEnum().WithMessage("Unknown status value");
        RuleFor(x => x.Description)
            .MaximumLength(DescriptionNameMaxLength)
            .WithMessage($"the description length cannot exceed {DescriptionNameMaxLength} characters");
        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Description is required for publishing")
            .When(x => x.Status == Status.Published);
    }
}
