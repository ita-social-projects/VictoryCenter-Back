using AutoMapper;
using FluentResults;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using MediatR;
using VictoryCenter.BLL.DTOs.Admin.EventNews;
using VictoryCenter.BLL.DTOs.Common;
using VictoryCenter.BLL.Interfaces.Search;
using VictoryCenter.BLL.Services.Search.Helpers;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;
using EventNewsEntity = VictoryCenter.DAL.Entities.EventNews;

namespace VictoryCenter.BLL.Queries.Admin.EventNews.Search;

public class SearchEventNewsHandler : IRequestHandler<SearchEventNewsQuery, Result<PaginationResult<EventNewsDto>>>
{
    private readonly IMapper _mapper;
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly IValidator<SearchEventNewsQuery> _validator;
    private readonly ISearchService<EventNewsEntity> _searchService;

    public SearchEventNewsHandler(
        IMapper mapper,
        IRepositoryWrapper repositoryWrapper,
        IValidator<SearchEventNewsQuery> validator,
        ISearchService<EventNewsEntity> searchService)
    {
        _mapper = mapper;
        _repositoryWrapper = repositoryWrapper;
        _validator = validator;
        _searchService = searchService;
    }

    public async Task<Result<PaginationResult<EventNewsDto>>> Handle(
        SearchEventNewsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            await _validator.ValidateAndThrowAsync(request, cancellationToken);
            var dto = request.SearchEventNewsDto;

            var searchByTitleTerm = new SearchTerm<EventNewsEntity>
            {
                TermSelector = eventNews => (eventNews.Title ?? string.Empty).ToLower(),
                TermValue = dto.SearchQuery.ToLower(),
                SearchLogic = SearchLogic.Contains,
            };

            var searchExpression = _searchService.CreateSearchExpression(searchByTitleTerm);

            var eventNews = await _repositoryWrapper.EventNewsRepository.GetAllAsync(
                new QueryOptions<EventNewsEntity>
                {
                    Include = query => query
                        .AsSplitQuery()
                        .Include(entity => entity.PreviewImage)
                        .Include(entity => entity.BackgroundImage)
                        .Include(entity => entity.Categories)
                            .ThenInclude(category => category.Localizations)
                                .ThenInclude(localization => localization.Language)
                        .Include(entity => entity.Localizations)
                            .ThenInclude(localization => localization.Language),
                    Filter = searchExpression,
                    OrderByDESC = eventNewsItem => eventNewsItem.PublishedAt!,
                    Offset = dto.Offset is > 0 ? (int)dto.Offset : 0,
                    Limit = dto.Limit is > 0 ? (int)dto.Limit : 0,
                    AsNoTracking = true,
                });

            var eventNewsDtos = _mapper.Map<List<EventNewsDto>>(eventNews);

            var count = await _repositoryWrapper.EventNewsRepository.CountAsync(
                new QueryOptions<EventNewsEntity>
                {
                    Filter = searchExpression,
                    AsNoTracking = true,
                });

            var paginationResult = new PaginationResult<EventNewsDto>(eventNewsDtos.ToArray(), count);

            return Result.Ok(paginationResult);
        }
        catch (ValidationException vex)
        {
            return Result.Fail(vex.Errors.Select(e => e.ErrorMessage));
        }
    }
}
