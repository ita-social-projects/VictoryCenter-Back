using AutoMapper;
using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Categories;
using VictoryCenter.DAL.Repositories.Interfaces.Base;

namespace VictoryCenter.BLL.Queries.Categories.GetCategories;

public class GetCategoriesHandler : IRequestHandler<GetCategoriesQuery, Result<IEnumerable<CategoryDto>>>
{
    private readonly IMapper _mapper;
    private readonly IRepositoryWrapper _repositoryWrapper;

    public GetCategoriesHandler(IMapper mapper, IRepositoryWrapper repositoryWrapper)
    {
        _mapper = mapper;
        _repositoryWrapper = repositoryWrapper;
    }
    
    public async Task<Result<IEnumerable<CategoryDto>>> Handle(GetCategoriesQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var entities = await _repositoryWrapper.CategoriesRepository.GetAllAsync();
            return Result.Ok(_mapper.Map<IEnumerable<CategoryDto>>(entities));
        }
        catch (Exception ex)
        {
            return Result.Fail<IEnumerable<CategoryDto>>(ex.Message);
        }
    }
}
