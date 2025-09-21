using System.Linq.Expressions;
using AutoMapper;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.DTOs.Admin.TeamMembers;
using VictoryCenter.BLL.DTOs.Common;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.BLL.Queries.Admin.TeamMembers.GetByFilters;

public class GetTeamMembersByFiltersHandler : IRequestHandler<GetTeamMembersByFiltersQuery, Result<PaginationResult<TeamMemberDto>>>
{
    private readonly IMapper _mapper;
    private readonly IRepositoryWrapper _repositoryWrapper;

    public GetTeamMembersByFiltersHandler(IMapper mapper, IRepositoryWrapper repositoryWrapper)
    {
        _mapper = mapper;
        _repositoryWrapper = repositoryWrapper;
    }

    public async Task<Result<PaginationResult<TeamMemberDto>>> Handle(GetTeamMembersByFiltersQuery request, CancellationToken cancellationToken)
    {
        var status = request.TeamMembersFilterDto.Status;
        var categoryId = request.TeamMembersFilterDto.CategoryId;
        var offset = request.TeamMembersFilterDto.Offset is > 0 ? (int)request.TeamMembersFilterDto.Offset : 0;
        var limit = request.TeamMembersFilterDto.Limit is > 0 ? (int)request.TeamMembersFilterDto.Limit : 0;

        Expression<Func<TeamMember, bool>> filter =
            t => (status == null || t.Status == status) && (categoryId == null || t.Category.Id == categoryId);

        var teamMembers = _repositoryWrapper.TeamMembersRepository.GetAllAsync(
            new QueryOptions<TeamMember>
            {
                Filter = filter,
                Offset = offset,
                Limit = limit,
                Include = tm => tm.Include(member => member.Image!),
                OrderByASC = tm => tm.Priority
            });

        var totalItemsCount = await _repositoryWrapper.TeamMembersRepository.CountAsync(new QueryOptions<TeamMember>
        {
            Filter = filter
        });

        var teamMembersDto = _mapper.Map<TeamMemberDto[]>(teamMembers);

        return Result.Ok(new PaginationResult<TeamMemberDto>(teamMembersDto, totalItemsCount));
    }
}

/*using System.Linq.Expressions;
using AutoMapper;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.DTOs.Admin.TeamMembers;
using VictoryCenter.BLL.DTOs.Common;
using VictoryCenter.BLL.Interfaces.ReorderService;
using VictoryCenter.DAL.Entities;

namespace VictoryCenter.BLL.Queries.Admin.TeamMembers.GetByFilters;

public class GetTeamMembersByFiltersHandler : IRequestHandler<GetTeamMembersByFiltersQuery, Result<PaginationResult<TeamMemberDto>>>
{
    private readonly IMapper _mapper;
    private readonly IReorderService _reorderService;

    public GetTeamMembersByFiltersHandler(IMapper mapper, IReorderService reorderService)
    {
        _mapper = mapper;
        _reorderService = reorderService;
    }

    public async Task<Result<PaginationResult<TeamMemberDto>>> Handle(GetTeamMembersByFiltersQuery request, CancellationToken cancellationToken)
    {
        var status = request.TeamMembersFilterDto.Status;
        var categoryId = request.TeamMembersFilterDto.CategoryId;

        Expression<Func<TeamMember, bool>> filter =
            t => (status == null || t.Status == status) && (categoryId == null || t.Category.Id == categoryId);

        var paginationResult = await _reorderService.GetOrderedPageAsync(
            offset: request.TeamMembersFilterDto.Offset is > 0 ? (int)request.TeamMembersFilterDto.Offset : 0,
            limit: request.TeamMembersFilterDto.Limit is > 0 ? (int)request.TeamMembersFilterDto.Limit : 0,
            idSelector: tm => tm.Id,
            groupSelector: filter,
            include: tm => tm.Include(member => member.Image!));

        var teamMembersDto = _mapper.Map<List<TeamMemberDto>>(paginationResult.Items);

        return Result.Ok(new PaginationResult<TeamMemberDto>([.. teamMembersDto], paginationResult.TotalItemsCount));
    }
}
*/
