using FluentValidation;
using VictoryCenter.BLL.Commands.Admin.Localization.TeamMembers.Update;

namespace VictoryCenter.BLL.Validators.Localization.TeamMembers;

public class UpdateTeamMemberLocalizationsValidator : AbstractValidator<UpdateTeamMemberLocalizationCommand>
{
    public UpdateTeamMemberLocalizationsValidator(BaseTeamMemberLocalizationsValidator baseTeamMemberLocalizationsValidator)
    {
        RuleFor(c => c.UpdateTeamMemberLocalizationDto).SetValidator(baseTeamMemberLocalizationsValidator);
    }
}
