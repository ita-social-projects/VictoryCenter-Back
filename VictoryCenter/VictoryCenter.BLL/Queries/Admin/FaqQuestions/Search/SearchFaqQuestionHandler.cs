using AutoMapper;
using FluentResults;
using FluentValidation;
using MediatR;
using VictoryCenter.BLL.DTOs.Admin.FaqQuestions;
using VictoryCenter.BLL.DTOs.Common;
using VictoryCenter.BLL.Interfaces.Search;
using VictoryCenter.BLL.Services.Search.Helpers;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.BLL.Queries.Admin.FaqQuestions.Search;

public class SearchFaqQuestionHandler : IRequestHandler<SearchFaqQuestionQuery, Result<PaginationResult<FaqQuestionDto>>>
{
    private readonly IMapper _mapper;
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly IValidator<SearchFaqQuestionQuery> _validator;
    private readonly ISearchService<FaqQuestion> _searchService;

    public SearchFaqQuestionHandler(
        IMapper mapper,
        IRepositoryWrapper repositoryWrapper,
        IValidator<SearchFaqQuestionQuery> validator,
        ISearchService<FaqQuestion> searchService)
    {
        _mapper = mapper;
        _repositoryWrapper = repositoryWrapper;
        _validator = validator;
        _searchService = searchService;
    }

    public async Task<Result<PaginationResult<FaqQuestionDto>>> Handle(SearchFaqQuestionQuery request, CancellationToken cancellationToken)
    {
        try
        {
            await _validator.ValidateAndThrowAsync(request, cancellationToken);
            var dto = request.SearchFaqQuestionDto;

            var searchTerm = new SearchTerm<FaqQuestion>
            {
                TermSelector = fq => fq.QuestionText.ToLower(),
                TermValue = dto.SearchQuery.ToLower(),
                SearchLogic = SearchLogic.Contains,
            };

            var searchExpression = _searchService.CreateSearchExpression(searchTerm);

            var faqQuestions = await _repositoryWrapper.FaqQuestionsRepository.GetAllAsync(new QueryOptions<FaqQuestion>
            {
                Filter = searchExpression,
                Offset = dto.Offset is > 0 ? (int)dto.Offset : 0,
                Limit = dto.Limit is > 0 ? (int)dto.Limit : 0,
            });
            var faqQuestionDto = _mapper.Map<List<FaqQuestionDto>>(faqQuestions);

            var count = await _repositoryWrapper.FaqQuestionsRepository.CountAsync(new QueryOptions<FaqQuestion> { Filter = searchExpression });

            var paginationResult = new PaginationResult<FaqQuestionDto>(
                faqQuestionDto.ToArray(),
                count);

            return Result.Ok(paginationResult);
        }
        catch (ValidationException vex)
        {
            return Result.Fail(vex.Errors.Select(e => e.ErrorMessage));
        }
    }
}
