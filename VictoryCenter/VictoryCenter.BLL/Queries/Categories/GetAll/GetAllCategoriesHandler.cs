using AutoMapper;
using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Categories;
using VictoryCenter.DAL.Repositories.Interfaces.Base;

namespace VictoryCenter.BLL.Queries.Categories.GetAll;

public class GetAllCategoriesHandler : IRequestHandler<GetAllCategoriesQuery, Result<IEnumerable<CategoryDto>>>
{
    private readonly IMapper _mapper;
    private readonly IRepositoryWrapper _repositoryWrapper;

    public GetAllCategoriesHandler(IMapper mapper, IRepositoryWrapper repositoryWrapper)
    {
        _mapper = mapper;
        _repositoryWrapper = repositoryWrapper;
    }

    public async Task<Result<IEnumerable<CategoryDto>>> Handle(GetAllCategoriesQuery request, CancellationToken cancellationToken)
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
