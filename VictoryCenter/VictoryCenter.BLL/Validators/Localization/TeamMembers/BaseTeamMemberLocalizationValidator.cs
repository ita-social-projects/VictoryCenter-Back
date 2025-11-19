using FluentValidation;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.Localization.TeamMembers;

namespace VictoryCenter.BLL.Validators.Localization.TeamMembers;

public class BaseTeamMemberLocalizationValidator : AbstractValidator<UpdateTeamMemberLocalizationDto>
{
    public BaseTeamMemberLocalizationValidator()
    {
        RuleFor(x => x.FullName)
            .NotEmpty().WithMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(UpdateTeamMemberLocalizationDto.FullName)))
            .MinimumLength(FullNameMinLength).WithMessage(ErrorMessagesConstants.PropertyMustHaveAMinimumLengthOfNCharacters(nameof(UpdateTeamMemberLocalizationDto.FullName), FullNameMinLength))
            .MaximumLength(FullNameMaxLength).WithMessage(ErrorMessagesConstants.PropertyMustHaveAMaximumLengthOfNCharacters(nameof(UpdateTeamMemberLocalizationDto.FullName), FullNameMaxLength));
        RuleFor(x => x.Description)
            .MinimumLength(DescriptionNameMinLength).WithMessage(ErrorMessagesConstants.PropertyMustHaveAMinimumLengthOfNCharacters(nameof(UpdateTeamMemberLocalizationDto.Description), DescriptionNameMinLength))
            .MaximumLength(DescriptionNameMaxLength).WithMessage(ErrorMessagesConstants.PropertyMustHaveAMaximumLengthOfNCharacters(nameof(UpdateTeamMemberLocalizationDto.Description), DescriptionNameMaxLength));
    }

    public static int FullNameMinLength { get; } = 2;
    public static int FullNameMaxLength { get; } = 100;
    public static int DescriptionNameMinLength { get; } = 10;
    public static int DescriptionNameMaxLength { get; } = 200;
}
