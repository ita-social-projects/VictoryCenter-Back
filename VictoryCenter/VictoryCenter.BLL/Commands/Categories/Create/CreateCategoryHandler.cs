using AutoMapper;
using FluentResults;
using FluentValidation;
using MediatR;
using VictoryCenter.BLL.DTOs.Categories;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.Base;

namespace VictoryCenter.BLL.Commands.Categories.Create;

public class CreateCategoryHandler : IRequestHandler<CreateCategoryCommand, Result<CategoryDto>>
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

    public async Task<Result<CategoryDto>> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
    {
        try
        {
            await _validator.ValidateAndThrowAsync(request, cancellationToken);

            var entity = _mapper.Map<Category>(request.createCategoryDto);
            entity.CreatedAt = DateTime.UtcNow;

            await _repositoryWrapper.CategoriesRepository.CreateAsync(entity);

            if (await _repositoryWrapper.SaveChangesAsync() > 0)
            {
                var resultDto = _mapper.Map<CategoryDto>(entity);
                return Result.Ok(resultDto);
            }

            return Result.Fail<CategoryDto>("Failed to create category");
        }
        catch (ValidationException ex)
        {
            return Result.Fail<CategoryDto>(ex.Message);
        }
    }
}
