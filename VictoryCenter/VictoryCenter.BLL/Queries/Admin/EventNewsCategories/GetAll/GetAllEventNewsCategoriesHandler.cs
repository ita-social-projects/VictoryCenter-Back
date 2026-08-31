using AutoMapper;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.DTOs.Admin.EventNewsCategories;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.BLL.Queries.Admin.EventNewsCategories.GetAll;

public class GetAllEventNewsCategoriesHandler
    : IRequestHandler<GetAllEventNewsCategoriesQuery, Result<List<AdminEventNewsCategoryDto>>>
{
    private readonly IMapper _mapper;
    private readonly IRepositoryWrapper _repositoryWrapper;

    public GetAllEventNewsCategoriesHandler(IMapper mapper, IRepositoryWrapper repositoryWrapper)
    {
        _mapper = mapper;
        _repositoryWrapper = repositoryWrapper;
    }

    public async Task<Result<List<AdminEventNewsCategoryDto>>> Handle(
        GetAllEventNewsCategoriesQuery request,
        CancellationToken cancellationToken)
    {
        var categories = await _repositoryWrapper.EventNewsCategoryRepository.GetAllAsync(
            new QueryOptions<EventNewsCategory>
            {
                Include = query => query
                 .Include(category => category.Localizations)
                 .ThenInclude(localization => localization.Language)
                 .Include(category => category.EventsNews),
                OrderByASC = category => category.Name,
                AsNoTracking = true
            });

        return Result.Ok(_mapper.Map<List<AdminEventNewsCategoryDto>>(categories));
    }
}
