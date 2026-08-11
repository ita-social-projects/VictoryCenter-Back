using AutoMapper;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.DTOs.Admin.HippotherapyProgramCategories;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.BLL.Queries.Admin.HippotherapyProgramCategories;

public class GetHippotherapyProgramCategoriesHandler : IRequestHandler<GetHippotherapyProgramCategoriesQuery, Result<List<HippotherapyProgramCategoryDto>>>
{
    private readonly IMapper _mapper;
    private readonly IRepositoryWrapper _repositoryWrapper;

    public GetHippotherapyProgramCategoriesHandler(IMapper mapper, IRepositoryWrapper repositoryWrapper)
    {
        _mapper = mapper;
        _repositoryWrapper = repositoryWrapper;
    }

    public async Task<Result<List<HippotherapyProgramCategoryDto>>> Handle(GetHippotherapyProgramCategoriesQuery request, CancellationToken cancellationToken)
    {
        IEnumerable<HippotherapyProgramCategory> programCategories = await _repositoryWrapper.HippotherapyProgramCategoriesRepository.GetAllAsync(new QueryOptions<HippotherapyProgramCategory>
        {
            Include = programCategory => programCategory
                .Include(p => p.Programs)
                .Include(p => p.Localizations).ThenInclude(l => l.Language)
        });
        var mapped = _mapper.Map<IEnumerable<HippotherapyProgramCategoryDto>>(programCategories).ToList();

        return Result.Ok(mapped);
    }
}
