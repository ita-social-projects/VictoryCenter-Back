using FluentValidation;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.Constants.Localization;
using VictoryCenter.BLL.DTOs.Admin.Localization.TeamCategories;

namespace VictoryCenter.BLL.Validators.Localization.TeamCategories;
public class BaseTeamCategoryLocalizationValidator : AbstractValidator<UpdateTeamCategoryLocalizationDto>
{
    public BaseTeamCategoryLocalizationValidator()
    {
        RuleFor(x => x.FullName)
            .NotEmpty().WithMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(UpdateTeamCategoryLocalizationDto.FullName)))
            .MinimumLength(TeamCategoryLocalizationConstants.FullNameMinLength).WithMessage(ErrorMessagesConstants.PropertyMustHaveAMinimumLengthOfNCharacters(nameof(UpdateTeamCategoryLocalizationDto.FullName), TeamCategoryLocalizationConstants.FullNameMinLength))
            .MaximumLength(TeamCategoryLocalizationConstants.FullNameMaxLength).WithMessage(ErrorMessagesConstants.PropertyMustHaveAMaximumLengthOfNCharacters(nameof(UpdateTeamCategoryLocalizationDto.FullName), TeamCategoryLocalizationConstants.FullNameMaxLength));
        RuleFor(x => x.Description)
            .MinimumLength(TeamCategoryLocalizationConstants.DescriptionNameMinLength).WithMessage(ErrorMessagesConstants.PropertyMustHaveAMinimumLengthOfNCharacters(nameof(UpdateTeamCategoryLocalizationDto.Description), TeamCategoryLocalizationConstants.DescriptionNameMinLength))
            .MaximumLength(TeamCategoryLocalizationConstants.DescriptionNameMaxLength).WithMessage(ErrorMessagesConstants.PropertyMustHaveAMaximumLengthOfNCharacters(nameof(UpdateTeamCategoryLocalizationDto.Description), TeamCategoryLocalizationConstants.DescriptionNameMaxLength));
    }
}
