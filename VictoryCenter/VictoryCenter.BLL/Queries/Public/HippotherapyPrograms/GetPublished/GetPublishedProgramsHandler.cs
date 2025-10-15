using AutoMapper;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.DTOs.Public.HippotherapyPrograms;
using VictoryCenter.BLL.Interfaces.BlobStorage;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Enums;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.BLL.Queries.Public.HippotherapyPrograms.GetPublished;

public class GetPublishedProgramsHandler : IRequestHandler<GetPublishedProgramsQuery, Result<List<PublishedHippotherapyProgramDto>>>
{
    private readonly IMapper _mapper;
    private readonly IRepositoryWrapper _repositoryWrapper;

    public GetPublishedProgramsHandler(IMapper mapper, IRepositoryWrapper repositoryWrapper, IBlobService blobService)
    {
        _mapper = mapper;
        _repositoryWrapper = repositoryWrapper;
    }

    public async Task<Result<List<PublishedHippotherapyProgramDto>>> Handle(GetPublishedProgramsQuery request, CancellationToken cancellationToken)
    {
        var queryOptions = new QueryOptions<HippotherapyProgram>
        {
            Filter = program => program.Status == Status.Published,
            Include = program => program
                .Include(program => program.Categories)
                .Include(program => program.Image)!
        };

        IEnumerable<HippotherapyProgram> publishedPrograms = await _repositoryWrapper.HippotherapyProgramsRepository.GetAllAsync(queryOptions);
        var publishedProgramsDto = _mapper.Map<IEnumerable<PublishedHippotherapyProgramDto>>(publishedPrograms).ToList();

        return Result.Ok(publishedProgramsDto);
    }
}
