using System.Linq.Expressions;
using AutoMapper;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.DTOs.Admin.TeamMembers;
using VictoryCenter.BLL.DTOs.Common;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Enums;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.BLL.Queries.Admin.TeamMembers.GetByFilters;

public class GetTeamMembersByFiltersHandler : IRequestHandler<GetTeamMembersByFiltersQuery, Result<PaginationResult<TeamMemberDto>>>
{
    private readonly IMapper _mapper;
    private readonly IRepositoryWrapper _repository;

    public GetTeamMembersByFiltersHandler(IMapper mapper, IRepositoryWrapper repository)
    {
        _mapper = mapper;
        _repository = repository;
    }

    public async Task<Result<PaginationResult<TeamMemberDto>>> Handle(GetTeamMembersByFiltersQuery request, CancellationToken cancellationToken)
    {
        Status? status = request.TeamMembersFilterDto.Status;
        var categoryId = request.TeamMembersFilterDto.CategoryId;
        Expression<Func<TeamMember, bool>> filter =
            t => (status == null || t.Status == status) && (categoryId == null || t.TeamCategory.Id == categoryId);

        var queryOptions = new QueryOptions<TeamMember>
        {
            Offset = request.TeamMembersFilterDto.Offset is > 0 ? (int)request.TeamMembersFilterDto.Offset : 0,
            Limit = request.TeamMembersFilterDto.Limit is > 0 ? (int)request.TeamMembersFilterDto.Limit : 0,
            Filter = filter,
            Include = tm => tm.Include(member => member.Image!),
            OrderByASC = tm => tm.Priority
        };

        IEnumerable<TeamMember> teamMembers = await _repository.TeamMembersRepository.GetAllAsync(queryOptions);
        List<TeamMemberDto>? teamMembersDto = _mapper.Map<List<TeamMemberDto>>(teamMembers);
        var itemsTotalCount = await _repository.TeamMembersRepository.CountAsync(queryOptions with { Offset = 0, Limit = 0 });

        return Result.Ok(new PaginationResult<TeamMemberDto>([.. teamMembersDto], itemsTotalCount));
    }
}
