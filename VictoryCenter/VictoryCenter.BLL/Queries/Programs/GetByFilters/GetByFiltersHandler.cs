using System.Linq.Expressions;
using MediatR;
using AutoMapper;
using FluentResults;
using VictoryCenter.BLL.Exceptions;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.DTOs.Programs;
using VictoryCenter.BLL.Interfaces.BlobStorage;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Enums;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.BLL.Queries.Programs.GetByFilters;

public class GetByFiltersHandler : IRequestHandler<GetByFiltersQuery, Result<ProgramsFilterResponseDto>>
{
    private readonly IMapper _mapper;
    private readonly IBlobService _blobService;
    private readonly IRepositoryWrapper _repositoryWrapper;

    public GetByFiltersHandler(IMapper mapper, IBlobService blobService, IRepositoryWrapper repositoryWrapper)
    {
        _mapper = mapper;
        _blobService = blobService;
        _repositoryWrapper = repositoryWrapper;
    }

    public async Task<Result<ProgramsFilterResponseDto>> Handle(GetByFiltersQuery request, CancellationToken cancellationToken)
    {
        Status? status = request.RequestDto?.Status;
        List<long>? programCategories = request.RequestDto?.CategoryId;
        Expression<Func<Program, bool>> filter =
            t => (status == null || t.Status == status) &&
                 (programCategories == null || programCategories.Count == 0 ||
                  t.Categories.Any(c => programCategories.Contains(c.Id)));

        var queryOptions = new QueryOptions<Program>
        {
            Offset = request.RequestDto?.Offset is not null and > 0 ? (int)request.RequestDto.Offset : 0,
            Limit = request.RequestDto?.Limit is not null and > 0 ? (int)request.RequestDto.Limit : 0,
            Filter = filter,
            Include = program => program
                .Include(p => p.Image)
                .Include(p => p.Categories)
        };

        var programs = await _repositoryWrapper.ProgramsRepository.GetAllAsync(queryOptions);
        var programDto = _mapper.Map<IEnumerable<ProgramDto>>(programs).ToList();
        IEnumerable<Task> imageLoadTasks = programDto.Where(member => member.Image is not null)
            .Select(async member =>
            {
                try
                {
                    member.Image.Base64 = await _blobService.FindFileInStorageAsBase64Async(member.Image.BlobName, member.Image.MimeType);
                }
                catch (BlobStorageException)
                {
                    member.Image.Base64 = string.Empty;
                }
            });
        await Task.WhenAll(imageLoadTasks);

        ProgramsFilterResponseDto response = new ProgramsFilterResponseDto
        {
            Programs = programDto,
            ProgramCount = programDto.Count()
        };

        return Result.Ok(response);
    }
}
