using AutoMapper;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.TeamMembers;
using VictoryCenter.BLL.Exceptions;
using VictoryCenter.BLL.Interfaces.BlobStorage;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.BLL.Queries.Admin.TeamMembers.GetById;

public class GetTeamMemberByIdHandler : IRequestHandler<GetTeamMemberByIdQuery, Result<TeamMemberDto>>
{
    private readonly IMapper _mapper;
    private readonly IRepositoryWrapper _repository;
    private readonly IBlobService _blobService;

    public GetTeamMemberByIdHandler(IMapper mapper, IRepositoryWrapper repository, IBlobService blobService)
    {
        _mapper = mapper;
        _repository = repository;
        _blobService = blobService;
    }

    public async Task<Result<TeamMemberDto>> Handle(GetTeamMemberByIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var queryOptions = new QueryOptions<TeamMember>
            {
                Filter = tm => tm.Id == request.Id,
                Include = t => t.Include(t => t.Image!)
            };

            TeamMember? teamMember = await _repository.TeamMembersRepository.GetFirstOrDefaultAsync(queryOptions);

            if (teamMember == null)
            {
                return Result.Fail<TeamMemberDto>(ErrorMessagesConstants.NotFound(request.Id, typeof(TeamMember)));
            }

            if (teamMember.Image is not null)
            {
                teamMember.Image.Base64 =
                    await _blobService.FindFileInStorageAsBase64Async(teamMember.Image.BlobName, teamMember.Image.MimeType);
            }

            var result = _mapper.Map<TeamMemberDto>(teamMember);

            return Result.Ok(result);
        }
        catch (BlobStorageException e)
        {
            return Result.Fail<TeamMemberDto>(ErrorMessagesConstants.BlobStorageError(e.Message));
        }
    }
}
