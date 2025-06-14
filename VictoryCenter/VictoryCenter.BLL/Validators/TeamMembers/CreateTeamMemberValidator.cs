using FluentValidation;
using VictoryCenter.BLL.Commands.TeamMembers.CreateTeamMember;
using VictoryCenter.BLL.DTOs.TeamMembers;

namespace VictoryCenter.BLL.Validators.TeamMembers;

public class CreateTeamMemberValidator : AbstractValidator<CreateTeamMemberCommand>
{
 public CreateTeamMemberValidator(BaseTeamMembersValidator baseTeamMembersValidator)
 {
  RuleFor(c => c.createTeamMemberDto).SetValidator(baseTeamMembersValidator);
 }
    
}
