using FluentValidation;
using VictoryCenter.BLL.DTOs.TeamMembers;
using VictoryCenter.DAL.Enums;

namespace VictoryCenter.BLL.Validators.TeamMembers;

public class BaseTeamMembersValidator : AbstractValidator<CreateTeamMemberDto>
{
    private const int FullNameMinLength = 2;
    private const int FullNameMaxLength = 100;
    private const int DescriptionNameMaxLength = 200;
    public BaseTeamMembersValidator()
    {
        RuleFor(x => x.FullName)
            .NotEmpty().WithMessage("FullName field is required")
            .MinimumLength(FullNameMinLength).WithMessage($"Full name must be at least {FullNameMinLength} characters long")
            .MaximumLength(FullNameMaxLength).WithMessage($"Full name must be no longer than {FullNameMaxLength} characters");
        RuleFor(x => x.CategoryId)
            .GreaterThan(0).WithMessage("CategoryId must be positive value");
        RuleFor(x => x.Status)
            .IsInEnum().WithMessage("Unknown status value");
        RuleFor(x => x.Description)
            .MaximumLength(DescriptionNameMaxLength)
            .WithMessage($"The description length cannot exceed {DescriptionNameMaxLength} characters");
        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Description is required for publishing")
            .When(x => x.Status == Status.Published);
    }
}
