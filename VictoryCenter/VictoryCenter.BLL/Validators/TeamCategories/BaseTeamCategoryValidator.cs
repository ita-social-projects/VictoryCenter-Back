using FluentValidation;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.TeamCategories;

namespace VictoryCenter.BLL.Validators.TeamCategories;

public class BaseTeamCategoryValidator : AbstractValidator<CreateTeamCategoryDto>
{
    public BaseTeamCategoryValidator()
    {
        RuleFor(dto => dto.Name)
            .NotEmpty()
            .WithMessage(ErrorMessagesConstants.PropertyIsRequired(
                nameof(CreateTeamCategoryDto.Name)))
            .MinimumLength(TeamCategoryConstants.MinNameLength)
            .WithMessage(ErrorMessagesConstants.PropertyMustHaveAMinimumLengthOfNCharacters(
                nameof(CreateTeamCategoryDto.Name),
                TeamCategoryConstants.MinNameLength))
            .MaximumLength(TeamCategoryConstants.MaxNameLength)
            .WithMessage(ErrorMessagesConstants.PropertyMustHaveAMaximumLengthOfNCharacters(
                nameof(CreateTeamCategoryDto.Name),
                TeamCategoryConstants.MaxNameLength));

        RuleFor(dto => dto.Description)
            .NotEmpty()
            .WithMessage(ErrorMessagesConstants.PropertyIsRequired(
                nameof(CreateTeamCategoryDto.Description)))
            .MinimumLength(TeamCategoryConstants.MinDescriptionLength)
            .WithMessage(ErrorMessagesConstants.PropertyMustHaveAMinimumLengthOfNCharacters(
                nameof(CreateTeamCategoryDto.Description),
                TeamCategoryConstants.MinDescriptionLength))
            .MaximumLength(TeamCategoryConstants.MaxDescriptionLength)
            .WithMessage(ErrorMessagesConstants.PropertyMustHaveAMaximumLengthOfNCharacters(
                nameof(CreateTeamCategoryDto.Description),
                TeamCategoryConstants.MaxDescriptionLength));
    }
}
