namespace VictoryCenter.BLL.Commands.TeamMember.DeleteTeamMember;

public record DeleteTeamMemberCommand(int Id) : IRequest<Result<Unit>>;
