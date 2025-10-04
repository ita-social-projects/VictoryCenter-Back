using AutoMapper;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.Commands.Base;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.Categories;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.Base;

namespace VictoryCenter.BLL.Commands.Admin.Categories.Create;

public class CreateCategoryHandler : BaseHandler<CreateCategoryCommand, CategoryDto>
{
    private readonly IMapper _mapper;
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly IValidator<CreateCategoryCommand> _validator;

    public CreateCategoryHandler(
        IMapper mapper,
        IRepositoryWrapper repositoryWrapper,
        IValidator<CreateCategoryCommand> validator)
    {
        _mapper = mapper;
        _repositoryWrapper = repositoryWrapper;
        _validator = validator;
    }

    public override async Task<CategoryDto> HandleRequest(CreateCategoryCommand request, CancellationToken cancellationToken)
    {
        await _validator.ValidateAndThrowAsync(request, cancellationToken);

        var entity = _mapper.Map<Category>(request.CreateCategoryDto);
        entity.CreatedAt = DateTimeOffset.UtcNow;

        await _repositoryWrapper.CategoriesRepository.CreateAsync(entity);

        if (await _repositoryWrapper.SaveChangesAsync() <= 0)
        {
            throw new DbUpdateException(ErrorMessagesConstants.FailedToCreateEntity(typeof(Category)));
        }

        var resultDto = _mapper.Map<CategoryDto>(entity);
        return resultDto;
    }
}
