using FluentValidation;
using VictoryCenter.BLL.Commands.Admin.TeamMembers.Create;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.TeamMembers;

namespace VictoryCenter.BLL.Validators.TeamMembers;

public class CreateTeamMemberValidator : AbstractValidator<CreateTeamMemberCommand>
{
    public CreateTeamMemberValidator(BaseTeamMembersValidator baseTeamMembersValidator)
    {
        RuleFor(c => c.CreateTeamMemberDto).SetValidator(baseTeamMembersValidator);
        RuleFor(c => c.CreateTeamMemberDto.FullName)
                    .Matches(@"^[\p{L}'\- ]+$")
                    .WithMessage(ErrorMessagesConstants.PropertyMustBeInAValidFormat(nameof(CreateTeamMemberDto.FullName)));
    }
}
