using FluentValidation;
using VictoryCenter.BLL.Commands.Admin.Localization.TeamMembers.Create;

namespace VictoryCenter.BLL.Validators.Localization.TeamMembers;

public class CreateTeamMemberLocalizationValidator : AbstractValidator<CreateTeamMemberLocalizationCommand>
{
    public CreateTeamMemberLocalizationValidator(BaseTeamMemberLocalizationValidator baseTeamMemberLocalizationsValidator)
    {
        RuleFor(c => c.CreateTeamMemberLocalizationDto).SetValidator(baseTeamMemberLocalizationsValidator);
    }
}
