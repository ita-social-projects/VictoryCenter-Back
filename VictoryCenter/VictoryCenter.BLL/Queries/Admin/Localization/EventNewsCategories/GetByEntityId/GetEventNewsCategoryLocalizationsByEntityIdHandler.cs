using AutoMapper;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.Localization.EventNewsCategories;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Entities.Localization;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.BLL.Queries.Admin.Localization.EventNewsCategories.GetByEntityId;

public class GetEventNewsCategoryLocalizationsByEntityIdHandler
    : IRequestHandler<
        GetEventNewsCategoryLocalizationsByEntityIdQuery,
        Result<List<AdminEventNewsCategoryLocalizationDto>>>
{
    private readonly IMapper _mapper;
    private readonly IRepositoryWrapper _repositoryWrapper;

    public GetEventNewsCategoryLocalizationsByEntityIdHandler(IMapper mapper, IRepositoryWrapper repositoryWrapper)
    {
        _mapper = mapper;
        _repositoryWrapper = repositoryWrapper;
    }

    public async Task<Result<List<AdminEventNewsCategoryLocalizationDto>>> Handle(
        GetEventNewsCategoryLocalizationsByEntityIdQuery request,
        CancellationToken cancellationToken)
    {
        if (!await _repositoryWrapper.EventNewsCategoryRepository.ExistsAsync(
                category => category.Id == request.EntityId))
        {
            return Result.Fail<List<AdminEventNewsCategoryLocalizationDto>>(
                ErrorMessagesConstants.NotFound(request.EntityId, typeof(EventNewsCategory)));
        }

        var localizations = await _repositoryWrapper.EventNewsCategoryLocalizationsRepository.GetAllAsync(
            new QueryOptions<EventNewsCategoryLocalization>
            {
                Filter = localization => localization.EntityId == request.EntityId,
                Include = query => query.Include(localization => localization.Language),
                OrderByASC = localization => localization.Language.Code,
                AsNoTracking = true
            });

        return Result.Ok(_mapper.Map<List<AdminEventNewsCategoryLocalizationDto>>(localizations));
    }
}
