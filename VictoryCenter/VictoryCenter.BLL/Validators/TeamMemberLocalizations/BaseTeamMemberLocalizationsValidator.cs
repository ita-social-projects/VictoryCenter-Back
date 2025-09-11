using FluentValidation;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.TeamMemberLocalizations;
using VictoryCenter.BLL.DTOs.Admin.TeamMembers;

namespace VictoryCenter.BLL.Validators.TeamMemberLocalizations;

public class BaseTeamMemberLocalizationsValidator : AbstractValidator<CreateTeamMemberLocalizationDto>
{
    public BaseTeamMemberLocalizationsValidator()
    {
        RuleFor(x => x.LanguageId)
            .GreaterThan(0).WithMessage(ErrorMessagesConstants.PropertyMustBePositive(nameof(CreateTeamMemberDto.CategoryId)));
        RuleFor(x => x.FullName)
            .NotEmpty().WithMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(CreateTeamMemberDto.FullName)))
            .MinimumLength(FullNameMinLength).WithMessage(ErrorMessagesConstants.PropertyMustHaveAMinimumLengthOfNCharacters(nameof(CreateTeamMemberDto.FullName), FullNameMinLength))
            .MaximumLength(FullNameMaxLength).WithMessage(ErrorMessagesConstants.PropertyMustHaveAMaximumLengthOfNCharacters(nameof(CreateTeamMemberDto.FullName), FullNameMaxLength));
        RuleFor(x => x.Description)
            .MaximumLength(DescriptionNameMaxLength)
            .WithMessage(ErrorMessagesConstants.PropertyMustHaveAMaximumLengthOfNCharacters(nameof(CreateTeamMemberDto.Description), DescriptionNameMaxLength));

        // RuleFor(x => x.Description)
        //    .NotEmpty().WithMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(CreateTeamMemberDto.Description)))
        //    .When(x => x.Status == Status.Published);
    }

    public static int FullNameMinLength { get; } = 2;
    public static int FullNameMaxLength { get; } = 100;
    public static int DescriptionNameMaxLength { get; } = 200;
}
