using FluentValidation;
using VictoryCenter.BLL.DTOs.Admin.TeamCategories;

namespace VictoryCenter.BLL.Validators.TeamCategories;

public class BaseTeamCategoryValidator : AbstractValidator<CreateTeamCategoryDto>
{
    public BaseTeamCategoryValidator()
    {
        RuleFor(dto => dto.Name)
            .ValidTeamCategoryName(nameof(CreateTeamCategoryDto.Name));

        RuleFor(dto => dto.Description)
            .ValidTeamCategoryDescription(nameof(CreateTeamCategoryDto.Description));
    }
}
