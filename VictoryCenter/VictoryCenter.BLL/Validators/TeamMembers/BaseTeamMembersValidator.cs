using FluentValidation;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.TeamMembers;
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
            .NotEmpty().WithMessage(ErrorMessagesConstants.PropertyIsRequired("Full Name"))
            .MinimumLength(FullNameMinLength).WithMessage(ErrorMessagesConstants.PropertyMustHaveAMinimumLengthOfNCharacters("Full Name", FullNameMinLength))
            .MaximumLength(FullNameMaxLength).WithMessage(ErrorMessagesConstants.PropertyMustHaveAMaximumLengthOfNCharacters("Full Name", FullNameMaxLength));
        RuleFor(x => x.CategoryId)
            .GreaterThan(0).WithMessage(ErrorMessagesConstants.PropertyMustBePositive("CategoryId"));
        RuleFor(x => x.Status)
            .IsInEnum().WithMessage(TeamMemberConstants.UnknownStatusValue);
        RuleFor(x => x.Description)
            .MaximumLength(DescriptionNameMaxLength)
            .WithMessage(ErrorMessagesConstants.PropertyMustHaveAMaximumLengthOfNCharacters("Description", DescriptionNameMaxLength));
        RuleFor(x => x.Description)
            .NotEmpty().WithMessage(ErrorMessagesConstants.PropertyIsRequired("Description"))
            .When(x => x.Status == Status.Published);
    }
}
