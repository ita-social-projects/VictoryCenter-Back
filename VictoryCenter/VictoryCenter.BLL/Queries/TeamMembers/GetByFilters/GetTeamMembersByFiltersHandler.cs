using AutoMapper;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using VictoryCenter.BLL.DTOs.TeamMember;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.Base;

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
        try
        {
            var offset = request.TeamMembersFilter.PageNumber > 0 ? request.TeamMembersFilter.PageNumber : 0;
            var limit = request.TeamMembersFilter.PageSize > 0 ? request.TeamMembersFilter.PageSize : 0;
            var status = request.TeamMembersFilter.Status;
            var categoryName = request.TeamMembersFilter.CategoryName;
            Expression<Func<TeamMember, bool>> filter =
                (t) => (status == null || t.Status == status) && (categoryName == null || t.Category.Name == categoryName);

            var teamMembers = await _repository.TeamMembersRepository.GetAllAsync(
                include: t => t.Include(t => t.Category),
                offset: offset,
                limit: limit,
                predicate: filter);
            var teamMembersDto = _mapper.Map<List<TeamMemberDto>>(teamMembers);

            return Result.Ok(teamMembersDto);
        }
        catch (Exception ex)
        {
            return Result.Fail<List<TeamMemberDto>>(ex.Message);
        }
    }
}
