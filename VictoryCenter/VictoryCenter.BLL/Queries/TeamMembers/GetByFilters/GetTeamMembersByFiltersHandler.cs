using System.Linq.Expressions;
using AutoMapper;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.DTOs.TeamMembers;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Enums;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.BLL.Queries.TeamMembers.GetByFilters;

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
        Status? status = request.TeamMembersFilter.Status;
        var categoryName = request.TeamMembersFilter.CategoryName;
        Expression<Func<TeamMember, bool>> filter =
            t => (status == null || t.Status == status) && (categoryName == null || t.Category.Name == categoryName);

        var queryOptions = new QueryOptions<TeamMember>
        {
            Offset = request.TeamMembersFilter.Offset is not null and > 0 ? (int)request.TeamMembersFilter.Offset : 0,
            Limit = request.TeamMembersFilter.Limit is not null and > 0 ? (int)request.TeamMembersFilter.Limit : 0,
            Filter = filter,
            Include = t => t.Include(t => t.Image),
            OrderByASC = t => t.Priority
        };

        IEnumerable<TeamMember> teamMembers = await _repository.TeamMembersRepository.GetAllAsync(queryOptions);
        List<TeamMemberDto>? teamMembersDto = _mapper.Map<List<TeamMemberDto>>(teamMembers);

        return Result.Ok(teamMembersDto);
    }
}
