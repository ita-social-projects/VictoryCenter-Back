using FluentValidation;
using VictoryCenter.BLL.Commands.Admin.Localization.TeamMembers.Update;

namespace VictoryCenter.BLL.Validators.Localization.TeamMembers;

public class UpdateTeamMemberLocalizationValidator : AbstractValidator<UpdateTeamMemberLocalizationCommand>
{
    public UpdateTeamMemberLocalizationValidator(BaseTeamMemberLocalizationValidator baseTeamMemberLocalizationsValidator)
    {
        RuleFor(c => c.UpdateTeamMemberLocalizationDto).SetValidator(baseTeamMemberLocalizationsValidator);
    }
}
