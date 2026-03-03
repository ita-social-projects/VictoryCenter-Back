using AutoMapper;
using FluentResults;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.DTOs.Admin.HippotherapyPrograms;
using VictoryCenter.BLL.DTOs.Common;
using VictoryCenter.BLL.Interfaces.Search;
using VictoryCenter.BLL.Services.Search.Helpers;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.BLL.Queries.Admin.HippotherapyPrograms.Search;

public class SearchHippotherapyProgramsHandler : IRequestHandler<SearchHippotherapyProgramsQuery, Result<PaginationResult<HippotherapyProgramDto>>>
{
    private readonly IMapper _mapper;
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly IValidator<SearchHippotherapyProgramsQuery> _validator;
    private readonly ISearchService<HippotherapyProgram> _searchService;

    public SearchHippotherapyProgramsHandler(
        IMapper mapper,
        IRepositoryWrapper repositoryWrapper,
        IValidator<SearchHippotherapyProgramsQuery> validator,
        ISearchService<HippotherapyProgram> searchService)
    {
        _mapper = mapper;
        _repositoryWrapper = repositoryWrapper;
        _validator = validator;
        _searchService = searchService;
    }

    public async Task<Result<PaginationResult<HippotherapyProgramDto>>> Handle(
        SearchHippotherapyProgramsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            await _validator.ValidateAndThrowAsync(request, cancellationToken);
            var dto = request.SearchHippotherapyProgramDto;

            var searchByNameTerm = new SearchTerm<HippotherapyProgram>
            {
                TermSelector = hp => hp.Name.ToLower(),
                TermValue = dto.SearchQuery.ToLower(),
                SearchLogic = SearchLogic.Prefix,
            };

            var searchExpression = _searchService.CreateSearchExpression(searchByNameTerm);

            var hippotherapyPrograms = await _repositoryWrapper.HippotherapyProgramsRepository.GetAllAsync(
                new QueryOptions<HippotherapyProgram>
                {
                    Include = q => q
                    .Include(tm => tm.Categories)
                    .Include(tm => tm.PreviewImage!)
                    .Include(tm => tm.Localizations)
                        .ThenInclude(l => l.Language),
                    Filter = searchExpression,
                    Offset = dto.Offset is > 0 ? (int)dto.Offset : 0,
                    Limit = dto.Limit is > 0 ? (int)dto.Limit : 0
                });

            var hippotherapyProgramsDto = _mapper.Map<List<HippotherapyProgramDto>>(hippotherapyPrograms);

            var count = await _repositoryWrapper.HippotherapyProgramsRepository.CountAsync(
                new QueryOptions<HippotherapyProgram>
                {
                    Filter = searchExpression
                });

            var paginationResult = new PaginationResult<HippotherapyProgramDto>(
                hippotherapyProgramsDto.ToArray(),
                count);

            return Result.Ok(paginationResult);
        }
        catch (ValidationException vex)
        {
            return Result.Fail(vex.Errors.Select(e => e.ErrorMessage));
        }
    }
}
