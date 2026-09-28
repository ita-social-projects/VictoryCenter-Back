using AutoMapper;
using FluentResults;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.DTOs.Admin.FeedbackHistories;
using VictoryCenter.BLL.DTOs.Common;
using VictoryCenter.BLL.Interfaces.Search;
using VictoryCenter.BLL.Services.Search.Helpers;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.BLL.Queries.Admin.FeedbackHistories.Search;

public class SearchFeedbackHistoryHandler
    : IRequestHandler<SearchFeedbackHistoryQuery, Result<PaginationResult<FeedbackHistoryDto>>>
{
    private readonly IMapper _mapper;
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly IValidator<SearchFeedbackHistoryQuery> _validator;
    private readonly ISearchService<FeedbackHistory> _searchService;

    public SearchFeedbackHistoryHandler(
        IMapper mapper,
        IRepositoryWrapper repositoryWrapper,
        IValidator<SearchFeedbackHistoryQuery> validator,
        ISearchService<FeedbackHistory> searchService)
    {
        _mapper = mapper;
        _repositoryWrapper = repositoryWrapper;
        _validator = validator;
        _searchService = searchService;
    }

    public async Task<Result<PaginationResult<FeedbackHistoryDto>>> Handle(
        SearchFeedbackHistoryQuery request, CancellationToken cancellationToken)
    {
        try
        {
            await _validator.ValidateAndThrowAsync(request, cancellationToken);
            var dto = request.SearchDto;
            var query = dto.SearchQuery.ToLower();

            var searchExpression = _searchService.CreateSearchExpression(
                CombineLogic.Or,
                new SearchTerm<FeedbackHistory>
                {
                    TermSelector = history => (history.Title ?? string.Empty).ToLower(),
                    TermValue = query,
                    SearchLogic = SearchLogic.Contains,
                },
                new SearchTerm<FeedbackHistory>
                {
                    TermSelector = history => (history.Story ?? string.Empty).ToLower(),
                    TermValue = query,
                    SearchLogic = SearchLogic.Contains,
                });

            var historiesTask = _repositoryWrapper.FeedbackHistoriesRepository.GetAllAsync(new QueryOptions<FeedbackHistory>
            {
                Include = query => query
                    .Include(history => history.Image!)
                    .Include(history => history.Localizations)
                        .ThenInclude(localization => localization.Language),
                Filter = searchExpression,
                OrderByASC = history => history.Priority,
                Offset = dto.Offset is > 0 ? (int)dto.Offset : 0,
                Limit = dto.Limit is > 0 ? (int)dto.Limit : 0,
            });

            var countTask = _repositoryWrapper.FeedbackHistoriesRepository.CountAsync(
                new QueryOptions<FeedbackHistory> { Filter = searchExpression });

            await Task.WhenAll(historiesTask, countTask);

            var histories = await historiesTask;
            var count = await countTask;

            var historyDtos = _mapper.Map<List<FeedbackHistoryDto>>(histories);

            return Result.Ok(new PaginationResult<FeedbackHistoryDto>(historyDtos.ToArray(), count));
        }
        catch (ValidationException exception)
        {
            return Result.Fail(exception.Errors.Select(error => error.ErrorMessage));
        }
    }
}
