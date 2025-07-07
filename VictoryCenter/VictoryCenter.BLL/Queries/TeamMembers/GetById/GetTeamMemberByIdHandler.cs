using AutoMapper;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.DTOs.TeamMembers;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.BLL.Queries.TeamMembers.GetById;

public class GetTeamMemberByIdHandler : IRequestHandler<GetTeamMemberByIdQuery, Result<TeamMemberDto>>
{
    private readonly IMapper _mapper;
    private readonly IRepositoryWrapper _repository;

    public GetTeamMemberByIdHandler(IMapper mapper, IRepositoryWrapper repository)
    {
        _mapper = mapper;
        _repository = repository;
    }

    public async Task<Result<TeamMemberDto>> Handle(GetTeamMemberByIdQuery request, CancellationToken cancellationToken)
    {
        var queryOptions = new QueryOptions<TeamMember>
        {
            Filter = tm => tm.Id == request.Id,
            Include = t => t.Include(t => t.Category)
        };

        var teamMember = await _repository.TeamMembersRepository.GetFirstOrDefaultAsync(queryOptions);

        if (teamMember == null)
        {
            return Result.Fail<TeamMemberDto>(ErrorMessagesConstants.NotFound(request.Id, typeof(TeamMember)));
        }

        return Result.Ok(_mapper.Map<TeamMemberDto>(teamMember));
    }
}
