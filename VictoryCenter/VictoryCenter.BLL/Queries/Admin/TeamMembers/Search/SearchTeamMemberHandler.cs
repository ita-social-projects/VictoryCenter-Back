using AutoMapper;
using FluentResults;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.DTOs.Admin.TeamMembers;
using VictoryCenter.BLL.DTOs.Common;
using VictoryCenter.BLL.Interfaces.Search;
using VictoryCenter.BLL.Services.Search.Helpers;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.BLL.Queries.Admin.TeamMembers.Search;

public class SearchTeamMemberHandler : IRequestHandler<SearchTeamMemberQuery, Result<PaginationResult<TeamMemberDto>>>
{
    private readonly IMapper _mapper;
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly IValidator<SearchTeamMemberQuery> _validator;
    private readonly ISearchService<TeamMember> _searchService;

    public SearchTeamMemberHandler(
        IMapper mapper,
        IRepositoryWrapper repositoryWrapper,
        IValidator<SearchTeamMemberQuery> validator,
        ISearchService<TeamMember> searchService)
    {
        _mapper = mapper;
        _repositoryWrapper = repositoryWrapper;
        _validator = validator;
        _searchService = searchService;
    }

    public async Task<Result<PaginationResult<TeamMemberDto>>> Handle(SearchTeamMemberQuery request, CancellationToken cancellationToken)
    {
        try
        {
            await _validator.ValidateAndThrowAsync(request, cancellationToken);
            var dto = request.SearchTeamMemberDto;

            var searchTerm = new SearchTerm<TeamMember>
            {
                TermSelector = tm => tm.FullName.ToLower(),
                TermValue = dto.FullName.ToLower(),
                SearchLogic = SearchLogic.Prefix,
            };

            var searchExpression = _searchService.CreateSearchExpression(searchTerm);

            var teamMembers = await _repositoryWrapper.TeamMembersRepository.GetAllAsync(new QueryOptions<TeamMember>
            {
                Include = q => q
                    .Include(tm => tm.Category)
                    .Include(tm => tm.Image!),
                Filter = searchExpression,
                Offset = dto.Offset is > 0 ? (int)dto.Offset : 0,
                Limit = dto.Limit is > 0 ? (int)dto.Limit : 0,
            });
            var teamMembersDto = _mapper.Map<List<TeamMemberDto>>(teamMembers);

            var count = await _repositoryWrapper.TeamMembersRepository.CountAsync(new QueryOptions<TeamMember> { Filter = searchExpression });

            var paginationResult = new PaginationResult<TeamMemberDto>(
                teamMembersDto.ToArray(),
                count);

            return Result.Ok(paginationResult);
        }
        catch (ValidationException vex)
        {
            return Result.Fail(vex.Errors.Select(e => e.ErrorMessage));
        }
    }
}
