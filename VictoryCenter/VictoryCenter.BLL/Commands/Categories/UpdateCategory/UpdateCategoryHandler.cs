using AutoMapper;
using FluentResults;
using FluentValidation;
using MediatR;
using VictoryCenter.BLL.DTOs.Categories;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.Base;

namespace VictoryCenter.BLL.Commands.Categories.UpdateCategory;

public class UpdateCategoryHandler : IRequestHandler<UpdateCategoryCommand, Result<CategoryDto>>
{
    private readonly IMapper _mapper;
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly IValidator<UpdateCategoryCommand> _validator;

    public UpdateCategoryHandler(IMapper mapper, IRepositoryWrapper repositoryWrapper, IValidator<UpdateCategoryCommand> validator)
    {
        _mapper = mapper;
        _repositoryWrapper = repositoryWrapper;
        _validator = validator;
    }
    
    public async Task<Result<CategoryDto>> Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
    {
        try
        {
            await _validator.ValidateAndThrowAsync(request, cancellationToken);
            
            var categoryEntity =
                await _repositoryWrapper.CategoriesRepository.GetFirstOrDefaultAsync(entity =>
                    entity.Id == request.updateCategoryDto.Id);

            if (categoryEntity is null)
            {
                return Result.Fail("Entity not found");
            }
            
            var entityToUpdate = _mapper.Map<UpdateCategoryDto, Category>(request.updateCategoryDto);
            
            _repositoryWrapper.CategoriesRepository.Update(entityToUpdate);
            
            if (await _repositoryWrapper.SaveChangesAsync() > 0)
            {
                var resultDto = _mapper.Map<Category, CategoryDto>(categoryEntity);
                return Result.Ok(resultDto);
            }
            return Result.Fail<CategoryDto>("Failed to update category");
        }
        catch (Exception ex)
        {
            return Result.Fail<CategoryDto>(ex.Message);
        }
    }
}
