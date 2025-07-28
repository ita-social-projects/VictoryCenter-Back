using MediatR;
using AutoMapper;
using FluentResults;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.DTOs.ProgramCategories;
using VictoryCenter.BLL.Interfaces.BlobStorage;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.BLL.Queries.ProgramCategories;

public class GetProgramCategoriesHandler : IRequestHandler<GetProgramCategoriesQuery, Result<List<ProgramCategoryDto>>>
{
    private readonly IMapper _mapper;
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly IBlobService _blobService;

    public GetProgramCategoriesHandler(IMapper mapper, IRepositoryWrapper repositoryWrapper, IBlobService blobService)
    {
        _mapper = mapper;
        _repositoryWrapper = repositoryWrapper;
        _blobService = blobService;
    }

    public async Task<Result<List<ProgramCategoryDto>>> Handle(GetProgramCategoriesQuery request, CancellationToken cancellationToken)
    {
        var programCategories = await _repositoryWrapper.ProgramCategoriesRepository.GetAllAsync(new QueryOptions<ProgramCategory>
        {
            Include = programCategory => programCategory
                .Include(p => p.Programs)
                .ThenInclude(p => p.Image)
        });
        var mapped = _mapper.Map<IEnumerable<ProgramCategoryDto>>(programCategories).ToList();
        foreach (var category in mapped)
        {
            foreach (var program in category.Programs)
            {
                if (program.Image != null)
                {
                    program.Image.Base64 = await _blobService.FindFileInStorageAsBase64Async(
                        program.Image.BlobName,
                        program.Image.MimeType);
                }
            }
        }

        return Result.Ok(mapped);
    }
}
