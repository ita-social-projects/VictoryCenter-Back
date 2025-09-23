using FluentValidation;
using VictoryCenter.BLL.Commands.Admin.TeamMembers.Update;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.TeamMembers;
using VictoryCenter.DAL.Enums;

namespace VictoryCenter.BLL.Validators.TeamMembers;

public class UpdateTeamMemberValidator : AbstractValidator<UpdateTeamMemberCommand>
{
    public UpdateTeamMemberValidator(BaseTeamMembersValidator baseTeamMembersValidator)
    {
        RuleFor(c => c.UpdateTeamMemberDto).SetValidator(baseTeamMembersValidator);

        RuleFor(x => x.UpdateTeamMemberDto.ImageId)
            .NotEmpty().WithMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(UpdateTeamMemberDto.ImageId)))
            .When(x => x.UpdateTeamMemberDto.Status == Status.Published);
    }
}
