using FluentValidation;
using VictoryCenter.BLL.Commands.TeamMembers.Update;

namespace VictoryCenter.BLL.Validators.TeamMembers;

public class UpdateTeamMemberValidator : AbstractValidator<UpdateTeamMemberCommand>
{
    public UpdateTeamMemberValidator()
    {
        RuleFor(command => command.updateTeamMemberDto.FirstName)
            .NotEmpty()
            .WithMessage("Name can't be empty");
    }
}
