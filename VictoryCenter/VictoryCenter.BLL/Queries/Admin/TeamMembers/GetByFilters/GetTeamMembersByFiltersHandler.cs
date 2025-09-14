using System.Linq.Expressions;
using AutoMapper;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.DTOs.Admin.TeamMembers;
using VictoryCenter.BLL.DTOs.Common;
using VictoryCenter.BLL.Services.ReorderService;
using VictoryCenter.DAL.Entities;

namespace VictoryCenter.BLL.Queries.Admin.TeamMembers.GetByFilters;

public class GetTeamMembersByFiltersHandler : IRequestHandler<GetTeamMembersByFiltersQuery, Result<PaginationResult<TeamMemberDto>>>
{
    /*    private readonly IMapper _mapper;
        private readonly IRepositoryWrapper _repository;

        public GetTeamMembersByFiltersHandler(IMapper mapper, IRepositoryWrapper repository)
        {
            _mapper = mapper;
            _repository = repository;
        }*/

    private readonly IMapper _mapper;
    private readonly IReorderService _reorderService;

    public GetTeamMembersByFiltersHandler(IMapper mapper, IReorderService reorderService)
    {
        _mapper = mapper;
        _reorderService = reorderService;
    }

    /*    public async Task<Result<PaginationResult<TeamMemberDto>>> Handle(GetTeamMembersByFiltersQuery request, CancellationToken cancellationToken)
        {
            Status? status = request.TeamMembersFilterDto.Status;
            var categoryId = request.TeamMembersFilterDto.CategoryId;
            Expression<Func<TeamMember, bool>> filter =
                t => (status == null || t.Status == status) && (categoryId == null || t.Category.Id == categoryId);

            var teamMembers = await _reorderService.GetOrderedPageAsync<TeamMember, long>(
                request.TeamMembersFilterDto.Offset is > 0 ? (int)request.TeamMembersFilterDto.Offset : 0,
                request.TeamMembersFilterDto.Limit is > 0 ? (int)request.TeamMembersFilterDto.Limit : 0,
                filter,
                tm => tm.Include(member => member.Image!));


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
        }*/

    public async Task<Result<PaginationResult<TeamMemberDto>>> Handle(GetTeamMembersByFiltersQuery request, CancellationToken cancellationToken)
    {
        var status = request.TeamMembersFilterDto.Status;
        var categoryId = request.TeamMembersFilterDto.CategoryId;
        var offset = request.TeamMembersFilterDto.Offset is > 0 ? (int)request.TeamMembersFilterDto.Offset : 0;
        var limit = request.TeamMembersFilterDto.Limit is > 0 ? (int)request.TeamMembersFilterDto.Limit : 0;

        Expression<Func<TeamMember, bool>> filter =
            t => (status == null || t.Status == status) && (categoryId == null || t.Category.Id == categoryId);

        var orderedResult = await _reorderService.GetOrderedPageAsync<TeamMember, long>(
            offset,
            limit,
            filter,
            include: tm => tm.Include(member => member.Image!));

        if (orderedResult.IsFailed)
        {
            return Result.Fail(orderedResult.Errors);
        }

        var teamMembersDto = _mapper.Map<List<TeamMemberDto>>(orderedResult.Value.Items);

        return Result.Ok(new PaginationResult<TeamMemberDto>([.. teamMembersDto], orderedResult.Value.TotalItemsCount));
    }
}
