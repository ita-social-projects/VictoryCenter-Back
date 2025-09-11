using FluentValidation;
using VictoryCenter.BLL.Commands.Admin.TeamMemberLocalizations.Create;

namespace VictoryCenter.BLL.Validators.TeamMemberLocalizations;

public class CreateTeamMemberLocalizationsValidator : AbstractValidator<CreateTeamMemberLocalizationCommand>
{
    public CreateTeamMemberLocalizationsValidator(BaseTeamMemberLocalizationsValidator baseTeamMemberLocalizationsValidator)
    {
        RuleFor(c => c.CreateTeamMemberLocalizationDto).SetValidator(baseTeamMemberLocalizationsValidator);
    }
}
