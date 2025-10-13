using AutoMapper;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.DTOs.Admin.HypotherapyProgramCategories;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.BLL.Queries.Admin.HypotherapyProgramCategories;

public class GetHypotherapyProgramCategoriesHandler : IRequestHandler<GetHypotherapyProgramCategoriesQuery, Result<List<HypotherapyProgramCategoryDto>>>
{
    private readonly IMapper _mapper;
    private readonly IRepositoryWrapper _repositoryWrapper;

    public GetHypotherapyProgramCategoriesHandler(IMapper mapper, IRepositoryWrapper repositoryWrapper)
    {
        _mapper = mapper;
        _repositoryWrapper = repositoryWrapper;
    }

    public async Task<Result<List<HypotherapyProgramCategoryDto>>> Handle(GetHypotherapyProgramCategoriesQuery request, CancellationToken cancellationToken)
    {
        IEnumerable<HippotherapyProgramCategory> programCategories = await _repositoryWrapper.HypotherapyProgramCategoriesRepository.GetAllAsync(new QueryOptions<HippotherapyProgramCategory>
        {
            Include = programCategory => programCategory
                .Include(p => p.Programs)
                .ThenInclude(p => p.Image)!
        });
        var mapped = _mapper.Map<IEnumerable<HypotherapyProgramCategoryDto>>(programCategories).ToList();

        return Result.Ok(mapped);
    }
}
