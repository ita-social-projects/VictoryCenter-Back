using AutoMapper;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.Commands.Base;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.ProgramCategories;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.BLL.Commands.Admin.ProgramCategories.Update;

public class UpdateProgramCategoryHandler : BaseHandler<UpdateProgramCategoryCommand, ProgramCategoryDto>
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

    public override async Task<ProgramCategoryDto> HandleRequest(UpdateProgramCategoryCommand request, CancellationToken cancellationToken)
    {
        await _validator.ValidateAndThrowAsync(request, cancellationToken);

        ProgramCategory? programCategoryEntity = await _repositoryWrapper.ProgramCategoriesRepository
            .GetFirstOrDefaultAsync(new QueryOptions<ProgramCategory>
            {
                Filter = programCategory => programCategory.Id == request.Id
            });

        if (programCategoryEntity is null)
        {
            throw new Exception(ErrorMessagesConstants
                .NotFound(request.Id, typeof(ProgramCategory)));
        }

        ProgramCategory entityToUpdate = _mapper.Map(request.UpdateProgramCategoryDto, programCategoryEntity);
        entityToUpdate.CreatedAt = programCategoryEntity.CreatedAt;

        _repositoryWrapper.ProgramCategoriesRepository.Update(entityToUpdate);

        if (await _repositoryWrapper.SaveChangesAsync() <= 0)
        {
            throw new DbUpdateException(ErrorMessagesConstants.FailedToUpdateEntity(typeof(ProgramCategory)));
        }

        ProgramCategoryDto responseDto = _mapper.Map<ProgramCategoryDto>(entityToUpdate);
        return responseDto;
    }
}
