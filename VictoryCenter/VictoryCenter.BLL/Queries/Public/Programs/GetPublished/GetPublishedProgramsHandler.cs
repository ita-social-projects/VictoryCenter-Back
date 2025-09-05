using AutoMapper;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.DTOs.Public.Programs;
using VictoryCenter.BLL.Interfaces.BlobStorage;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Enums;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.BLL.Queries.Public.Programs.GetPublished;

public class GetPublishedProgramsHandler : IRequestHandler<GetPublishedProgramsQuery, Result<List<PublishedProgramDto>>>
{
    private readonly IMapper _mapper;
    private readonly IRepositoryWrapper _repositoryWrapper;

    public GetPublishedProgramsHandler(IMapper mapper, IRepositoryWrapper repositoryWrapper, IBlobService blobService)
    {
        _mapper = mapper;
        _repositoryWrapper = repositoryWrapper;
    }

    public async Task<Result<List<PublishedProgramDto>>> Handle(GetPublishedProgramsQuery request, CancellationToken cancellationToken)
    {
        var queryOptions = new QueryOptions<Program>
        {
            Filter = program => program.Status == Status.Published,
            Include = program => program
                .Include(program => program.Categories)
                .Include(program => program.Image)!
        };

        IEnumerable<Program> publishedPrograms = await _repositoryWrapper.ProgramsRepository.GetAllAsync(queryOptions);
        var publishedProgramsDto = _mapper.Map<IEnumerable<PublishedProgramDto>>(publishedPrograms).ToList();

        return Result.Ok(publishedProgramsDto);
    }
}
