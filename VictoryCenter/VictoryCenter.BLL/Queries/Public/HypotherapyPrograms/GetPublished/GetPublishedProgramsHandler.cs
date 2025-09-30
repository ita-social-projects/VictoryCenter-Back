using AutoMapper;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.DTOs.Public.HypotherapyPrograms;
using VictoryCenter.BLL.Interfaces.BlobStorage;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Enums;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.BLL.Queries.Public.HypotherapyPrograms.GetPublished;

public class GetPublishedProgramsHandler : IRequestHandler<GetPublishedProgramsQuery, Result<List<PublishedHypotherapyProgramDto>>>
{
    private readonly IMapper _mapper;
    private readonly IRepositoryWrapper _repositoryWrapper;

    public GetPublishedProgramsHandler(IMapper mapper, IRepositoryWrapper repositoryWrapper, IBlobService blobService)
    {
        _mapper = mapper;
        _repositoryWrapper = repositoryWrapper;
    }

    public async Task<Result<List<PublishedHypotherapyProgramDto>>> Handle(GetPublishedProgramsQuery request, CancellationToken cancellationToken)
    {
        var queryOptions = new QueryOptions<HypotherapyProgram>
        {
            Filter = program => program.Status == Status.Published,
            Include = program => program
                .Include(program => program.Categories)
                .Include(program => program.Image)!
        };

        IEnumerable<HypotherapyProgram> publishedPrograms = await _repositoryWrapper.HypotherapyProgramsRepository.GetAllAsync(queryOptions);
        var publishedProgramsDto = _mapper.Map<IEnumerable<PublishedHypotherapyProgramDto>>(publishedPrograms).ToList();

        return Result.Ok(publishedProgramsDto);
    }
}
