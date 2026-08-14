using AutoMapper;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.DTOs.Admin.Localization.HippotherapyProgramCategories;
using VictoryCenter.DAL.Entities.Localization;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.BLL.Queries.Admin.Localization.HippotherapyProgramCategories.GetByEntityId;

public class GetHippotherapyProgramCategoryLocalizationByEntityIdHandler
    : IRequestHandler<GetHippotherapyProgramCategoryLocalizationByEntityIdQuery, Result<List<HippotherapyProgramCategoryLocalizationDto>>>
{
    private readonly IMapper _mapper;
    private readonly IRepositoryWrapper _repositoryWrapper;

    public GetHippotherapyProgramCategoryLocalizationByEntityIdHandler(IMapper mapper, IRepositoryWrapper repositoryWrapper)
    {
        _mapper = mapper;
        _repositoryWrapper = repositoryWrapper;
    }

    public async Task<Result<List<HippotherapyProgramCategoryLocalizationDto>>> Handle(
        GetHippotherapyProgramCategoryLocalizationByEntityIdQuery request,
        CancellationToken cancellationToken)
    {
        var queryOptions = new QueryOptions<HippotherapyProgramCategoryLocalization>
        {
            Filter = l => l.EntityId == request.Id,
            Include = l => l.Include(loc => loc.Language),
            AsNoTracking = true,
        };

        var entities = await _repositoryWrapper.HippotherapyProgramCategoryLocalizationsRepository.GetAllAsync(queryOptions);
        var responseDto = _mapper.Map<List<HippotherapyProgramCategoryLocalizationDto>>(entities);
        return Result.Ok(responseDto);
    }
}
