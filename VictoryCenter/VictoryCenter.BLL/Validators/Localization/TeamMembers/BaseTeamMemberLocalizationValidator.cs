using FluentValidation;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.Localization.TeamMembers;

namespace VictoryCenter.BLL.Validators.Localization.TeamMembers;

public class BaseTeamMemberLocalizationValidator : AbstractValidator<CreateTeamMemberLocalizationDto>
{
    public BaseTeamMemberLocalizationValidator()
    {
        RuleFor(x => x.LanguageId)
            .NotNull().WithMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(CreateTeamMemberLocalizationDto.LanguageId)))
            .GreaterThan(0).WithMessage(ErrorMessagesConstants.PropertyMustBePositive(nameof(CreateTeamMemberLocalizationDto.LanguageId)));
        RuleFor(x => x.TeamMemberId)
            .NotNull().WithMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(CreateTeamMemberLocalizationDto.TeamMemberId)))
            .GreaterThan(0).WithMessage(ErrorMessagesConstants.PropertyMustBePositive(nameof(CreateTeamMemberLocalizationDto.TeamMemberId)));
        RuleFor(x => x.FullName)
            .NotEmpty().WithMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(CreateTeamMemberLocalizationDto.FullName)))
            .MinimumLength(FullNameMinLength).WithMessage(ErrorMessagesConstants.PropertyMustHaveAMinimumLengthOfNCharacters(nameof(CreateTeamMemberLocalizationDto.FullName), FullNameMinLength))
            .MaximumLength(FullNameMaxLength).WithMessage(ErrorMessagesConstants.PropertyMustHaveAMaximumLengthOfNCharacters(nameof(CreateTeamMemberLocalizationDto.FullName), FullNameMaxLength));
        RuleFor(x => x.Description)
            .MinimumLength(DescriptionNameMinLength).WithMessage(ErrorMessagesConstants.PropertyMustHaveAMinimumLengthOfNCharacters(nameof(CreateTeamMemberLocalizationDto.Description), DescriptionNameMinLength))
            .MaximumLength(DescriptionNameMaxLength).WithMessage(ErrorMessagesConstants.PropertyMustHaveAMaximumLengthOfNCharacters(nameof(CreateTeamMemberLocalizationDto.Description), DescriptionNameMaxLength));
    }

    public static int FullNameMinLength { get; } = 2;
    public static int FullNameMaxLength { get; } = 100;
    public static int DescriptionNameMinLength { get; } = 10;
    public static int DescriptionNameMaxLength { get; } = 200;
}
