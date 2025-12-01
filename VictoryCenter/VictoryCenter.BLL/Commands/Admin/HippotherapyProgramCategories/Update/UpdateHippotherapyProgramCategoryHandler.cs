using AutoMapper;
using FluentResults;
using FluentValidation;
using MediatR;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.HippotherapyProgramCategories;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.BLL.Commands.Admin.HippotherapyProgramCategories.Update;

public class UpdateHippotherapyProgramCategoryHandler : IRequestHandler<UpdateHippotherapyProgramCategoryCommand, Result<HippotherapyProgramCategoryDto>>
{
    private readonly IMapper _mapper;
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly IValidator<UpdateHippotherapyProgramCategoryCommand> _validator;

    public UpdateHippotherapyProgramCategoryHandler(IMapper mapper, IRepositoryWrapper repositoryWrapper, IValidator<UpdateHippotherapyProgramCategoryCommand> validator)
    {
        _mapper = mapper;
        _repositoryWrapper = repositoryWrapper;
        _validator = validator;
    }

    public async Task<Result<HippotherapyProgramCategoryDto>> Handle(UpdateHippotherapyProgramCategoryCommand request, CancellationToken cancellationToken)
    {
        try
        {
            await _validator.ValidateAndThrowAsync(request, cancellationToken);

            HippotherapyProgramCategory? programCategoryEntity = await _repositoryWrapper.HippotherapyProgramCategoriesRepository
                .GetFirstOrDefaultAsync(new QueryOptions<HippotherapyProgramCategory>
                {
                    Filter = programCategory => programCategory.Id == request.Id
                });

            if (programCategoryEntity is null)
            {
                return Result.Fail<HippotherapyProgramCategoryDto>(ErrorMessagesConstants
                    .NotFound(request.Id, typeof(HippotherapyProgramCategory)));
            }

            HippotherapyProgramCategory entityToUpdate = _mapper.Map(request.UpdateProgramCategoryDto, programCategoryEntity);

            _repositoryWrapper.HippotherapyProgramCategoriesRepository.Update(entityToUpdate);

            if (await _repositoryWrapper.SaveChangesAsync() > 0)
            {
                HippotherapyProgramCategoryDto responseDto = _mapper.Map<HippotherapyProgramCategoryDto>(entityToUpdate);
                return Result.Ok(responseDto);
            }

            return Result.Fail<HippotherapyProgramCategoryDto>(ErrorMessagesConstants.FailedToUpdateEntity(typeof(HippotherapyProgramCategory)));
        }
        catch (ValidationException ex)
        {
            return Result.Fail<HippotherapyProgramCategoryDto>(ex.Errors.Select(e => e.ErrorMessage));
        }
    }
}
