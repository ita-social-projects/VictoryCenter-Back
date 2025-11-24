using FluentValidation;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.TeamMembers;
using VictoryCenter.DAL.Enums;

namespace VictoryCenter.BLL.Validators.TeamMembers;

public class BaseTeamMembersValidator : AbstractValidator<CreateTeamMemberDto>
{
    public BaseTeamMembersValidator()
    {
        RuleFor(x => x.FullName)
            .NotEmpty()
            .WithMessage(ErrorMessagesConstants.PropertyIsRequired(
                nameof(CreateTeamMemberDto.FullName)))
            .Matches(TeamMemberConstants.FullNameRegexPattern)
            .WithMessage(ErrorMessagesConstants.PropertyMustBeInAValidFormat(
                nameof(CreateTeamMemberDto.FullName)))
            .MinimumLength(TeamMemberConstants.FullNameMinLength)
            .WithMessage(ErrorMessagesConstants.PropertyMustHaveAMinimumLengthOfNCharacters(
                nameof(CreateTeamMemberDto.FullName),
                TeamMemberConstants.FullNameMinLength))
            .MaximumLength(TeamMemberConstants.FullNameMaxLength)
            .WithMessage(ErrorMessagesConstants.PropertyMustHaveAMaximumLengthOfNCharacters(
                nameof(CreateTeamMemberDto.FullName),
                TeamMemberConstants.FullNameMaxLength));

        RuleFor(x => x.CategoryId)
            .GreaterThan(0)
            .WithMessage(ErrorMessagesConstants.PropertyMustBePositive(
                nameof(CreateTeamMemberDto.CategoryId)));

        RuleFor(x => x.Status)
            .IsInEnum()
            .WithMessage(ErrorMessagesConstants.UnknownStatusValue);

        RuleFor(x => x.Description)
            .NotEmpty()
            .WithMessage(ErrorMessagesConstants.PropertyIsRequired(
                nameof(CreateTeamMemberDto.Description)))
            .MinimumLength(TeamMemberConstants.DescriptionNameMinLength)
            .WithMessage(ErrorMessagesConstants.PropertyMustHaveAMinimumLengthOfNCharacters(
                nameof(CreateTeamMemberDto.Description),
                TeamMemberConstants.DescriptionNameMinLength))
            .MaximumLength(TeamMemberConstants.DescriptionNameMaxLength)
            .WithMessage(ErrorMessagesConstants.PropertyMustHaveAMaximumLengthOfNCharacters(
                nameof(CreateTeamMemberDto.Description),
                TeamMemberConstants.DescriptionNameMaxLength))
            .When(x => x.Status == Status.Published);

        RuleFor(x => x.ImageId)
            .NotNull()
            .WithMessage(ErrorMessagesConstants.PropertyIsRequired(
                nameof(CreateTeamMemberDto.ImageId)))
            .GreaterThan(0)
            .WithMessage(ErrorMessagesConstants.PropertyMustBePositive(
                nameof(CreateTeamMemberDto.ImageId)))
            .When(x => x.Status == Status.Published);
    }
}
