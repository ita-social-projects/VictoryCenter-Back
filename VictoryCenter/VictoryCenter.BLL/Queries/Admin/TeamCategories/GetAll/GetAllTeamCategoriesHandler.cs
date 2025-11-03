using AutoMapper;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.DTOs.Admin.TeamCategories;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.BLL.Queries.Admin.TeamCategories.GetAll;

public class GetAllTeamCategoriesHandler : IRequestHandler<GetAllTeamCategoriesQuery, Result<IEnumerable<TeamCategoryDto>>>
{
    private readonly IMapper _mapper;
    private readonly IRepositoryWrapper _repositoryWrapper;

    public GetAllTeamCategoriesHandler(IMapper mapper, IRepositoryWrapper repositoryWrapper)
    {
        _mapper = mapper;
        _repositoryWrapper = repositoryWrapper;
    }

    public async Task<Result<IEnumerable<TeamCategoryDto>>> Handle(
        GetAllTeamCategoriesQuery request,
        CancellationToken cancellationToken)
    {
        var entities = await _repositoryWrapper.TeamCategoriesRepository.GetAllAsync(
            new QueryOptions<TeamCategory>
            {
                Include = tc => tc.Include(tc => tc.TeamMembers)
            });
        return Result.Ok(_mapper.Map<IEnumerable<TeamCategoryDto>>(entities));
    }
}
