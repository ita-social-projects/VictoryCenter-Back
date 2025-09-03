using AutoMapper;
using FluentResults;
using FluentValidation;
using MediatR;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.ProgramCategories;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.Base;

namespace VictoryCenter.BLL.Commands.Admin.ProgramCategories.Create;

public class CreateProgramCategoryHandler : IRequestHandler<CreateProgramCategoryCommand, Result<ProgramCategoryDto>>
{
    private readonly IMapper _mapper;
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly IValidator<CreateProgramCategoryCommand> _validator;

    public CreateProgramCategoryHandler(IMapper mapper, IRepositoryWrapper repositoryWrapper, IValidator<CreateProgramCategoryCommand> validator)
    {
        _mapper = mapper;
        _repositoryWrapper = repositoryWrapper;
        _validator = validator;
    }

    public async Task<Result<ProgramCategoryDto>> Handle(CreateProgramCategoryCommand request, CancellationToken cancellationToken)
    {
        try
        {
            await _validator.ValidateAndThrowAsync(request, cancellationToken);

            ProgramCategory entity = _mapper.Map<ProgramCategory>(request.programCategoryDto);
            entity.CreatedAt = DateTime.UtcNow;
            await _repositoryWrapper.ProgramCategoriesRepository.CreateAsync(entity);

            if (await _repositoryWrapper.SaveChangesAsync() > 0)
            {
                ProgramCategoryDto responseDto = _mapper.Map<ProgramCategoryDto>(entity);
                return Result.Ok(responseDto);
            }

            return Result.Fail<ProgramCategoryDto>(ErrorMessagesConstants.FailedToCreateEntity(typeof(ProgramCategory)));
        }
        catch (ValidationException ex)
        {
            return Result.Fail<ProgramCategoryDto>(ex.Errors.Select(e => e.ErrorMessage));
        }
    }
}
