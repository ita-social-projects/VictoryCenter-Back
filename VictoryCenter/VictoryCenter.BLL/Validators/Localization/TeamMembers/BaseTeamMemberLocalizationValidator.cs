using FluentValidation;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.Localization.TeamMembers;
using VictoryCenter.BLL.DTOs.Admin.TeamMembers;

namespace VictoryCenter.BLL.Validators.Localization.TeamMembers;

public class BaseTeamMemberLocalizationValidator : AbstractValidator<CreateTeamMemberLocalizationDto>
{
    public BaseTeamMemberLocalizationValidator()
    {
        RuleFor(x => x.LanguageId)
            .GreaterThan(0).WithMessage(ErrorMessagesConstants.PropertyMustBePositive(nameof(CreateTeamMemberDto.CategoryId)));
        RuleFor(x => x.FullName)
            .NotEmpty().WithMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(CreateTeamMemberDto.FullName)))
            .MinimumLength(FullNameMinLength).WithMessage(ErrorMessagesConstants.PropertyMustHaveAMinimumLengthOfNCharacters(nameof(CreateTeamMemberDto.FullName), FullNameMinLength))
            .MaximumLength(FullNameMaxLength).WithMessage(ErrorMessagesConstants.PropertyMustHaveAMaximumLengthOfNCharacters(nameof(CreateTeamMemberDto.FullName), FullNameMaxLength));
        RuleFor(x => x.Description)
            .MinimumLength(DescriptionNameMinLength).WithMessage(ErrorMessagesConstants.PropertyMustHaveAMinimumLengthOfNCharacters(nameof(CreateTeamMemberDto.Description), DescriptionNameMinLength))
            .MaximumLength(DescriptionNameMaxLength).WithMessage(ErrorMessagesConstants.PropertyMustHaveAMaximumLengthOfNCharacters(nameof(CreateTeamMemberDto.Description), DescriptionNameMaxLength));
    }

    public static int FullNameMinLength { get; } = 2;
    public static int FullNameMaxLength { get; } = 100;
    public static int DescriptionNameMinLength { get; } = 10;
    public static int DescriptionNameMaxLength { get; } = 200;
}
