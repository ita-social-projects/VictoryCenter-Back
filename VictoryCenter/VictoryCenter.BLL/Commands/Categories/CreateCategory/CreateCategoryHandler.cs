using AutoMapper;
using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Categories;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.Base;

namespace VictoryCenter.BLL.Commands.Categories.CreateCategory;

public class CreateCategoryHandler : IRequestHandler<CreateCategoryCommand, Result<CategoryDto>>
{
    private readonly IMapper _mapper;
    private readonly IRepositoryWrapper _repositoryWrapper;

    public CreateCategoryHandler(IMapper mapper, IRepositoryWrapper repositoryWrapper)
    {
        _mapper = mapper;
        _repositoryWrapper = repositoryWrapper;
    }
    
    public async Task<Result<CategoryDto>> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var entity = _mapper.Map<Category>(request.createCategoryDto);
            entity.CreatedAt = DateTime.Now;
            
            await _repositoryWrapper.CategoriesRepository.CreateAsync(entity);
            await _repositoryWrapper.SaveChangesAsync();
            
            var resultDto = _mapper.Map<CategoryDto>(entity);
            
            return Result.Ok(resultDto);
        }
        catch (Exception ex)
        {
            return Result.Fail<CategoryDto>(ex.Message);
        }
    }
}
