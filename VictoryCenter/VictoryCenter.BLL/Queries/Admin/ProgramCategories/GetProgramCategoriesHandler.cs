using AutoMapper;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.DTOs.Admin.ProgramCategories;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.BLL.Queries.Admin.ProgramCategories;

public class GetProgramCategoriesHandler : IRequestHandler<GetProgramCategoriesQuery, Result<List<ProgramCategoryDto>>>
{
    private readonly IMapper _mapper;
    private readonly IRepositoryWrapper _repositoryWrapper;

    public GetProgramCategoriesHandler(IMapper mapper, IRepositoryWrapper repositoryWrapper)
    {
        _mapper = mapper;
        _repositoryWrapper = repositoryWrapper;
    }

    public async Task<Result<List<ProgramCategoryDto>>> Handle(GetProgramCategoriesQuery request, CancellationToken cancellationToken)
    {
        IEnumerable<ProgramCategory> programCategories = await _repositoryWrapper.ProgramCategoriesRepository.GetAllAsync(new QueryOptions<ProgramCategory>
        {
            Include = programCategory => programCategory
                .Include(p => p.Programs)
                .ThenInclude(p => p.Image)!
        });
        var mapped = _mapper.Map<IEnumerable<ProgramCategoryDto>>(programCategories).ToList();

        return Result.Ok(mapped);
    }
}
