using System.Linq.Expressions;
using AutoMapper;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.DTOs.Admin.TeamMembers;
using VictoryCenter.BLL.DTOs.Common;
using VictoryCenter.BLL.Exceptions;
using VictoryCenter.BLL.Interfaces.BlobStorage;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Enums;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.BLL.Queries.Admin.TeamMembers.GetByFilters;

public class GetTeamMembersByFiltersHandler : IRequestHandler<GetTeamMembersByFiltersQuery, Result<PaginationResult<TeamMemberDto>>>
{
    private readonly IBlobService _blobService;
    private readonly IMapper _mapper;
    private readonly IRepositoryWrapper _repository;

    public GetTeamMembersByFiltersHandler(IMapper mapper, IRepositoryWrapper repository, IBlobService blobService)
    {
        _mapper = mapper;
        _repository = repository;
        _blobService = blobService;
    }

    public async Task<Result<PaginationResult<TeamMemberDto>>> Handle(GetTeamMembersByFiltersQuery request, CancellationToken cancellationToken)
    {
        Status? status = request.TeamMembersFilterDto.Status;
        var categoryId = request.TeamMembersFilterDto.CategoryId;
        Expression<Func<TeamMember, bool>> filter =
            t => (status == null || t.Status == status) && (categoryId == null || t.Category.Id == categoryId);

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
        var itemsTotalCount = await _repository.TeamMembersRepository.CountAsync(queryOptions.Filter);

        IEnumerable<Task> imageLoadTasks = teamMembersDto.Where(member => member.Image is not null)
            .Select(async member =>
            {
                try
                {
                    member.Image!.Base64 = await _blobService.FindFileInStorageAsBase64Async(member.Image.BlobName, member.Image.MimeType);
                }
                catch (BlobStorageException)
                {
                    member.Image!.Base64 = string.Empty;
                }
            });
        await Task.WhenAll(imageLoadTasks);

        return Result.Ok(new PaginationResult<TeamMemberDto>([.. teamMembersDto], itemsTotalCount));
    }
}
