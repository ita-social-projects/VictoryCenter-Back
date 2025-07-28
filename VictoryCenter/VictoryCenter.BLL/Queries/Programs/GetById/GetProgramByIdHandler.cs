using MediatR;
using AutoMapper;
using FluentResults;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Programs;
using VictoryCenter.BLL.Interfaces.BlobStorage;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;
using VictoryCenter.DAL.Entities;

namespace VictoryCenter.BLL.Queries.Programs.GetById;

public class GetProgramByIdHandler : IRequestHandler<GetProgramByIdQuery, Result<ProgramDto>>
{
    private readonly IMapper _mapper;
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly IBlobService _blobService;

    public GetProgramByIdHandler(IMapper mapper, IRepositoryWrapper repositoryWrapper, IBlobService blobService)
    {
        _mapper = mapper;
        _repositoryWrapper = repositoryWrapper;
        _blobService = blobService;
    }

    public async Task<Result<ProgramDto>> Handle(GetProgramByIdQuery request, CancellationToken cancellationToken)
    {
        var queryOptions = new QueryOptions<Program>
        {
            Filter = program => program.Id == request.id,
            Include = program => program
                .Include(p => p.Categories)
                .Include(p => p.Image)
        };

        Program? program = await _repositoryWrapper.ProgramsRepository.GetFirstOrDefaultAsync(queryOptions);

        if (program is null)
        {
            return Result.Fail<ProgramDto>(ErrorMessagesConstants.NotFound(request.id, typeof(Program)));
        }

        if (program.Image is not null)
        {
            program.Image.Base64 = await _blobService.FindFileInStorageAsBase64Async(
                program.Image.BlobName,
                program.Image.MimeType);
        }

        var responseDto = _mapper.Map<ProgramDto>(program);
        return Result.Ok(responseDto);
    }
}
