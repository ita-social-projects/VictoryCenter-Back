using FluentValidation;
using VictoryCenter.BLL.Commands.Admin.TeamMemberLocalizations.Update;

namespace VictoryCenter.BLL.Validators.TeamMemberLocalizations;

public class UpdateTeamMemberLocalizationsValidator : AbstractValidator<UpdateTeamMemberLocalizationCommand>
{
    public UpdateTeamMemberLocalizationsValidator(BaseTeamMemberLocalizationsValidator baseTeamMemberLocalizationsValidator)
    {
        RuleFor(c => c.UpdateTeamMemberLocalizationDto).SetValidator(baseTeamMemberLocalizationsValidator);
    }
}
