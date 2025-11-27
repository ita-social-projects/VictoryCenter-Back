using FluentValidation;
using VictoryCenter.BLL.Commands.Admin.Localization.TeamMembers.Create;
using VictoryCenter.BLL.DTOs.Admin.Localization.TeamMembers;
using VictoryCenter.BLL.Validators.Localization.Base;

namespace VictoryCenter.BLL.Validators.Localization.TeamMembers;

public class CreateTeamMemberLocalizationValidator : AbstractValidator<CreateTeamMemberLocalizationCommand>
{
    public CreateTeamMemberLocalizationValidator(BaseTeamMemberLocalizationValidator baseTeamMemberLocalizationsValidator)
    {
        RuleFor(x => x.CreateTeamMemberLocalizationDto)
            .SetValidator(new LocalizationIdentityValidator<CreateTeamMemberLocalizationDto>());

        RuleFor(c => c.CreateTeamMemberLocalizationDto).SetValidator(baseTeamMemberLocalizationsValidator);
    }
}
