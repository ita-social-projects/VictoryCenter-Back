using AutoMapper;
using FluentResults;
using FluentValidation;
using MediatR;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.HippotherapyProgramCategories;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.Base;

namespace VictoryCenter.BLL.Commands.Admin.HippotherapyProgramCategories.Create;

public class CreateHippotherapyProgramCategoryHandler : IRequestHandler<CreateHippotherapyProgramCategoryCommand, Result<HippotherapyProgramCategoryDto>>
{
    private readonly IMapper _mapper;
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly IValidator<CreateHippotherapyProgramCategoryCommand> _validator;

    public CreateHippotherapyProgramCategoryHandler(IMapper mapper, IRepositoryWrapper repositoryWrapper, IValidator<CreateHippotherapyProgramCategoryCommand> validator)
    {
        _mapper = mapper;
        _repositoryWrapper = repositoryWrapper;
        _validator = validator;
    }

    public async Task<Result<HippotherapyProgramCategoryDto>> Handle(CreateHippotherapyProgramCategoryCommand request, CancellationToken cancellationToken)
    {
        try
        {
            await _validator.ValidateAndThrowAsync(request, cancellationToken);

            HippotherapyProgramCategory entity = _mapper.Map<HippotherapyProgramCategory>(request.CreateProgramCategoryDto);
            entity.CreatedAt = DateTimeOffset.UtcNow;
            await _repositoryWrapper.HippotherapyProgramCategoriesRepository.CreateAsync(entity);

            if (await _repositoryWrapper.SaveChangesAsync() > 0)
            {
                HippotherapyProgramCategoryDto responseDto = _mapper.Map<HippotherapyProgramCategoryDto>(entity);
                return Result.Ok(responseDto);
            }

            return Result.Fail<HippotherapyProgramCategoryDto>(ErrorMessagesConstants.FailedToCreateEntity(typeof(HippotherapyProgramCategory)));
        }
        catch (ValidationException ex)
        {
            return Result.Fail<HippotherapyProgramCategoryDto>(ex.Errors.Select(e => e.ErrorMessage));
        }
    }
}
