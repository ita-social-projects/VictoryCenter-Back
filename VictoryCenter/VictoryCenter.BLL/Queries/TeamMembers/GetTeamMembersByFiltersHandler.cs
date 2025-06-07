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

public class GetTeamMembersByFiltersHandler : IRequestHandler<GetTeamMembersByFiltersQuery, Result<List<TeamMemberDto>>>
{
    private readonly IMapper _mapper;
    private readonly IRepositoryWrapper _repository;

    public GetTeamMembersByFiltersHandler(IMapper mapper, IRepositoryWrapper repository)
    {
        _mapper = mapper;
        _repository = repository;
    }

    public async Task<Result<List<TeamMemberDto>>> Handle(GetTeamMembersByFiltersQuery request, CancellationToken cancellationToken)
    {
        var offset = request.TeamMembersFilter.PageNumber > 0 ? request.TeamMembersFilter.PageNumber : 0;
        var limit = request.TeamMembersFilter.PageSize > 0 ? request.TeamMembersFilter.PageSize : 0;
        var status = request.TeamMembersFilter.Status;
        var categoryId = request.TeamMembersFilter.CategoryId;

        var teamMembers = await _repository.TeamMembersRepository.GetTeamMembersAsync(offset, limit, categoryId, status);
        var teamMembersDto = _mapper.Map<List<TeamMemberDto>>(teamMembers);

        return Result.Ok(teamMembersDto);
    }
}
