using MediatR;
using AutoMapper;
using FluentResults;
using FluentValidation;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.ProgramCategories;
using VictoryCenter.DAL.Repositories.Options;
using VictoryCenter.DAL.Repositories.Interfaces.Base;

namespace VictoryCenter.BLL.Commands.ProgramCategories.Update;

public class UpdateProgramCategoryHandler : IRequestHandler<UpdateProgramCategoryCommand, Result<ProgramCategoryDto>>
{
    private readonly IMapper _mapper;
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly IValidator<UpdateProgramCategoryCommand> _validator;

    public UpdateProgramCategoryHandler(IMapper mapper, IRepositoryWrapper repositoryWrapper, IValidator<UpdateProgramCategoryCommand> validator)
    {
        _mapper = mapper;
        _repositoryWrapper = repositoryWrapper;
        _validator = validator;
    }

    public async Task<Result<ProgramCategoryDto>> Handle(UpdateProgramCategoryCommand request, CancellationToken cancellationToken)
    {
        try
        {
            await _validator.ValidateAndThrowAsync(request, cancellationToken);

            var programCategoryEntity = await _repositoryWrapper.ProgramCategoriesRepository
                .GetFirstOrDefaultAsync(new QueryOptions<DAL.Entities.ProgramCategory>
                {
                    Filter = programCategory => programCategory.Id == request.updateProgramCategoryDto.Id
                });

            if (programCategoryEntity is null)
            {
                return Result.Fail<ProgramCategoryDto>(ErrorMessagesConstants
                    .NotFound(request.updateProgramCategoryDto.Id, typeof(DAL.Entities.ProgramCategory)));
            }

            var entityToUpdate = _mapper.Map(request.updateProgramCategoryDto, programCategoryEntity);
            entityToUpdate.CreatedAt = programCategoryEntity.CreatedAt;

            _repositoryWrapper.ProgramCategoriesRepository.Update(entityToUpdate);

            if (await _repositoryWrapper.SaveChangesAsync() > 0)
            {
                var responseDto = _mapper.Map<ProgramCategoryDto>(entityToUpdate);
                return Result.Ok(responseDto);
            }

            return Result.Fail<ProgramCategoryDto>(ProgramCategoryConstants.FailedToUpdateCategory);
        }
        catch (ValidationException ex)
        {
            return Result.Fail<ProgramCategoryDto>(ex.Message);
        }
    }
}
