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
            .MinimumLength(TeamCategoryLocalizationConstants.NameMinLength).WithMessage(ErrorMessagesConstants.PropertyMustHaveAMinimumLengthOfNCharacters(nameof(UpdateTeamCategoryLocalizationDto.FullName), TeamCategoryLocalizationConstants.NameMinLength))
            .MaximumLength(TeamCategoryLocalizationConstants.NameMaxLength).WithMessage(ErrorMessagesConstants.PropertyMustHaveAMaximumLengthOfNCharacters(nameof(UpdateTeamCategoryLocalizationDto.FullName), TeamCategoryLocalizationConstants.NameMaxLength));
        RuleFor(x => x.Description)
            .NotEmpty().WithMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(UpdateTeamCategoryLocalizationDto.Description)))
            .MinimumLength(TeamCategoryLocalizationConstants.DescriptionMinLength).WithMessage(ErrorMessagesConstants.PropertyMustHaveAMinimumLengthOfNCharacters(nameof(UpdateTeamCategoryLocalizationDto.Description), TeamCategoryLocalizationConstants.DescriptionMinLength))
            .MaximumLength(TeamCategoryLocalizationConstants.DescriptionMaxLength).WithMessage(ErrorMessagesConstants.PropertyMustHaveAMaximumLengthOfNCharacters(nameof(UpdateTeamCategoryLocalizationDto.Description), TeamCategoryLocalizationConstants.DescriptionMaxLength));
    }
}
