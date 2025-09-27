using FluentValidation;
using VictoryCenter.BLL.Commands.Admin.TeamMembers.Create;

namespace VictoryCenter.BLL.Validators.TeamMembers;

public class CreateTeamMemberValidator : AbstractValidator<CreateTeamMemberCommand>
{
    public CreateTeamMemberValidator(BaseTeamMembersValidator baseTeamMembersValidator)
    {
        RuleFor(c => c.CreateTeamMemberDto).SetValidator(baseTeamMembersValidator);
    }
}
