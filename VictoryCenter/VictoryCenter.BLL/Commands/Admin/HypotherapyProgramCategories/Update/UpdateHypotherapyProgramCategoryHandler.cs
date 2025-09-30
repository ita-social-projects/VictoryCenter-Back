using AutoMapper;
using FluentResults;
using FluentValidation;
using MediatR;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.HypotherapyProgramCategories;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.BLL.Commands.Admin.HypotherapyProgramCategories.Update;

public class UpdateHypotherapyProgramCategoryHandler : IRequestHandler<UpdateHypotherapyProgramCategoryCommand, Result<HypotherapyProgramCategoryDto>>
{
    private readonly IMapper _mapper;
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly IValidator<UpdateHypotherapyProgramCategoryCommand> _validator;

    public UpdateHypotherapyProgramCategoryHandler(IMapper mapper, IRepositoryWrapper repositoryWrapper, IValidator<UpdateHypotherapyProgramCategoryCommand> validator)
    {
        _mapper = mapper;
        _repositoryWrapper = repositoryWrapper;
        _validator = validator;
    }

    public async Task<Result<HypotherapyProgramCategoryDto>> Handle(UpdateHypotherapyProgramCategoryCommand request, CancellationToken cancellationToken)
    {
        try
        {
            await _validator.ValidateAndThrowAsync(request, cancellationToken);

            HypotherapyProgramCategory? programCategoryEntity = await _repositoryWrapper.HypotherapyProgramCategoriesRepository
                .GetFirstOrDefaultAsync(new QueryOptions<HypotherapyProgramCategory>
                {
                    Filter = programCategory => programCategory.Id == request.Id
                });

            if (programCategoryEntity is null)
            {
                return Result.Fail<HypotherapyProgramCategoryDto>(ErrorMessagesConstants
                    .NotFound(request.Id, typeof(HypotherapyProgramCategory)));
            }

            HypotherapyProgramCategory entityToUpdate = _mapper.Map(request.UpdateProgramCategoryDto, programCategoryEntity);
            entityToUpdate.CreatedAt = programCategoryEntity.CreatedAt;

            _repositoryWrapper.HypotherapyProgramCategoriesRepository.Update(entityToUpdate);

            if (await _repositoryWrapper.SaveChangesAsync() > 0)
            {
                HypotherapyProgramCategoryDto responseDto = _mapper.Map<HypotherapyProgramCategoryDto>(entityToUpdate);
                return Result.Ok(responseDto);
            }

            return Result.Fail<HypotherapyProgramCategoryDto>(ErrorMessagesConstants.FailedToUpdateEntity(typeof(HypotherapyProgramCategory)));
        }
        catch (ValidationException ex)
        {
            return Result.Fail<HypotherapyProgramCategoryDto>(ex.Errors.Select(e => e.ErrorMessage));
        }
    }
}
