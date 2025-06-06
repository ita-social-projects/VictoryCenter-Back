using AutoMapper;
using FluentResults;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VictoryCenter.BLL.DTOs.TeamMember;
using VictoryCenter.DAL.Repositories.Interfaces.Base;

namespace VictoryCenter.BLL.Queries.TeamMembers;

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
        var teamMember = await _repository.TeamMembersRepository.GetFirstOrDefaultAsync(tm => tm.Id == request.Id);

        if (teamMember == null)
        {
            return Result.Fail("Team member not fould");
        }

        return Result.Ok(_mapper.Map<TeamMemberDto>(teamMember));
    }
}
