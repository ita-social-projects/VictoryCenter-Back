using AutoMapper;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.Commands.Base;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.Categories;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.BLL.Commands.Admin.Categories.Update;

public class UpdateCategoryHandler : BaseHandler<UpdateCategoryCommand, CategoryDto>
{
    private readonly IMapper _mapper;
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly IValidator<UpdateCategoryCommand> _validator;

    public UpdateCategoryHandler(
        IMapper mapper,
        IRepositoryWrapper repositoryWrapper,
        IValidator<UpdateCategoryCommand> validator)
    {
        _mapper = mapper;
        _repositoryWrapper = repositoryWrapper;
        _validator = validator;
    }

    public override async Task<CategoryDto> HandleRequest(UpdateCategoryCommand request, CancellationToken cancellationToken)
    {
        await _validator.ValidateAndThrowAsync(request, cancellationToken);

        var categoryEntity =
            await _repositoryWrapper.CategoriesRepository.GetFirstOrDefaultAsync(new QueryOptions<Category>
            {
                Filter = entity => entity.Id == request.Id
            });

        if (categoryEntity is null)
        {
            throw new Exception(ErrorMessagesConstants.NotFound(request.Id, typeof(Category)));
        }

        var entityToUpdate = _mapper.Map<UpdateCategoryDto, Category>(request.UpdateCategoryDto);
        entityToUpdate.Id = request.Id;
        entityToUpdate.CreatedAt = categoryEntity.CreatedAt;

        _repositoryWrapper.CategoriesRepository.Update(entityToUpdate);

        if (await _repositoryWrapper.SaveChangesAsync() <= 0)
        {
            throw new DbUpdateException(ErrorMessagesConstants.FailedToUpdateEntity(typeof(Category)));
        }

        var resultDto = _mapper.Map<Category, CategoryDto>(entityToUpdate);
        return resultDto;
    }
}
