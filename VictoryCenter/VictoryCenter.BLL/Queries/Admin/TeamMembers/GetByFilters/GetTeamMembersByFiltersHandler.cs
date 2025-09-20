using System.Linq.Expressions;
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
    private readonly IIndexReorderService _reorderService;

    public GetTeamMembersByFiltersHandler(IMapper mapper, IIndexReorderService reorderService)
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
            groupSelector: filter,
            include: tm => tm.Include(member => member.Image!));

        var teamMembersDto = _mapper.Map<List<TeamMemberDto>>(paginationResult.Items);

        return Result.Ok(new PaginationResult<TeamMemberDto>([.. teamMembersDto], paginationResult.TotalItemsCount));
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
