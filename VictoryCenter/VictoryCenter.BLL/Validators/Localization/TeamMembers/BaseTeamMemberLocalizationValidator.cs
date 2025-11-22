using FluentValidation;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.Constants.Localization;
using VictoryCenter.BLL.DTOs.Admin.Localization.TeamMembers;

namespace VictoryCenter.BLL.Validators.Localization.TeamMembers;

public class BaseTeamMemberLocalizationValidator : AbstractValidator<UpdateTeamMemberLocalizationDto>
{
    public BaseTeamMemberLocalizationValidator()
    {
        RuleFor(x => x.FullName)
            .NotEmpty().WithMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(UpdateTeamMemberLocalizationDto.FullName)))
            .MinimumLength(TeamMemberLocalizationConstants.FullNameMinLength).WithMessage(ErrorMessagesConstants.PropertyMustHaveAMinimumLengthOfNCharacters(nameof(UpdateTeamMemberLocalizationDto.FullName), TeamMemberLocalizationConstants.FullNameMinLength))
            .MaximumLength(TeamMemberLocalizationConstants.FullNameMaxLength).WithMessage(ErrorMessagesConstants.PropertyMustHaveAMaximumLengthOfNCharacters(nameof(UpdateTeamMemberLocalizationDto.FullName), TeamMemberLocalizationConstants.FullNameMaxLength));
        RuleFor(x => x.Description)
            .MinimumLength(TeamMemberLocalizationConstants.DescriptionNameMinLength).WithMessage(ErrorMessagesConstants.PropertyMustHaveAMinimumLengthOfNCharacters(nameof(UpdateTeamMemberLocalizationDto.Description), TeamMemberLocalizationConstants.DescriptionNameMinLength))
            .MaximumLength(TeamMemberLocalizationConstants.DescriptionNameMaxLength).WithMessage(ErrorMessagesConstants.PropertyMustHaveAMaximumLengthOfNCharacters(nameof(UpdateTeamMemberLocalizationDto.Description), TeamMemberLocalizationConstants.DescriptionNameMaxLength));
    }
}
