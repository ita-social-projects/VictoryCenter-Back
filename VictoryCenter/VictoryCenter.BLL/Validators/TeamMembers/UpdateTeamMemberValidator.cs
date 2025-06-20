using FluentValidation;
using VictoryCenter.BLL.Commands.TeamMembers.Update;

namespace VictoryCenter.BLL.Validators.TeamMembers;

public class UpdateTeamMemberValidator : AbstractValidator<UpdateTeamMemberCommand>
{
    public UpdateTeamMemberValidator(BaseTeamMembersValidator baseTeamMembersValidator)
    {
        RuleFor(c => c.updateTeamMemberDto).SetValidator(baseTeamMembersValidator);
    }
}
