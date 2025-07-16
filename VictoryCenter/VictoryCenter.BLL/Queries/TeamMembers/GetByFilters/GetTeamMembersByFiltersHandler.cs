using System.Linq.Expressions;
using AutoMapper;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.DTOs.TeamMembers;
using VictoryCenter.BLL.Interfaces.BlobStorage;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.BLL.Queries.TeamMembers.GetByFilters;

public class GetTeamMembersByFiltersHandler : IRequestHandler<GetTeamMembersByFiltersQuery, Result<List<TeamMemberDto>>>
{
    private readonly IMapper _mapper;
    private readonly IRepositoryWrapper _repository;
    private readonly IBlobService _blobService;

    public GetTeamMembersByFiltersHandler(IMapper mapper, IRepositoryWrapper repository, IBlobService blobService)
    {
        _mapper = mapper;
        _repository = repository;
        _blobService = blobService;
    }

    public async Task<Result<List<TeamMemberDto>>> Handle(GetTeamMembersByFiltersQuery request, CancellationToken cancellationToken)
    {
        var status = request.TeamMembersFilter.Status;
        var categoryId = request.TeamMembersFilter.CategoryId;
        Expression<Func<TeamMember, bool>> filter =
            (t) => (status == null || t.Status == status) && (categoryId == null || t.Category.Id == categoryId);

        var queryOptions = new QueryOptions<TeamMember>
        {
            Offset = request.TeamMembersFilter.Offset is not null and > 0 ?
            (int)request.TeamMembersFilter.Offset : 0,
            Limit = request.TeamMembersFilter.Limit is not null and > 0 ?
            (int)request.TeamMembersFilter.Limit : 0,
            Filter = filter,
            Include = t => t.Include(t => t.Image),
            OrderByASC = t => t.Priority
        };

        var teamMembers = await _repository.TeamMembersRepository.GetAllAsync(queryOptions);
        var teamMembersDto = _mapper.Map<List<TeamMemberDto>>(teamMembers);

        foreach (var member in teamMembersDto)
        {
            if (member.Image is not null)
            {
                member.Image.Base64 = await _blobService.FindFileInStorageAsBase64Async(member.Image.BlobName, member.Image.MimeType);
            }
        }

        return Result.Ok(teamMembersDto);
    }
}
