using FluentValidation;
using VictoryCenter.BLL.Commands.Admin.Localization.TeamMembers.Create;

namespace VictoryCenter.BLL.Validators.Localization.TeamMembers;

public class CreateTeamMemberLocalizationsValidator : AbstractValidator<CreateTeamMemberLocalizationCommand>
{
    public CreateTeamMemberLocalizationsValidator(BaseTeamMemberLocalizationsValidator baseTeamMemberLocalizationsValidator)
    {
        RuleFor(c => c.CreateTeamMemberLocalizationDto).SetValidator(baseTeamMemberLocalizationsValidator);
    }
}
