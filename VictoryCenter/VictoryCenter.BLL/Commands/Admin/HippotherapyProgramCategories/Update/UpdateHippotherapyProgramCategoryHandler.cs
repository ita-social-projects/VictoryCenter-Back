using AutoMapper;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.Commands.Base;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.HippotherapyProgramCategories;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.BLL.Commands.Admin.HippotherapyProgramCategories.Update;

public class UpdateHippotherapyProgramCategoryHandler : BaseHandler<UpdateHippotherapyProgramCategoryCommand, HippotherapyProgramCategoryDto>
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

    public override async Task<HippotherapyProgramCategoryDto> HandleRequest(UpdateHippotherapyProgramCategoryCommand request, CancellationToken cancellationToken)
    {
        await _validator.ValidateAndThrowAsync(request, cancellationToken);

        HippotherapyProgramCategory? programCategoryEntity = await _repositoryWrapper.HippotherapyProgramCategoriesRepository
            .GetFirstOrDefaultAsync(new QueryOptions<HippotherapyProgramCategory>
            {
                Filter = programCategory => programCategory.Id == request.Id
            });

        if (programCategoryEntity is null)
        {
            throw new Exception(ErrorMessagesConstants
                .NotFound(request.Id, typeof(HippotherapyProgramCategory)));
        }

        HippotherapyProgramCategory entityToUpdate = _mapper.Map(request.UpdateProgramCategoryDto, programCategoryEntity);

        _repositoryWrapper.HippotherapyProgramCategoriesRepository.Update(entityToUpdate);

        if (await _repositoryWrapper.SaveChangesAsync() > 0)
        {
            HippotherapyProgramCategoryDto responseDto = _mapper.Map<HippotherapyProgramCategoryDto>(entityToUpdate);
            return responseDto;
        }

        throw new DbUpdateException(ErrorMessagesConstants.FailedToUpdateEntity(typeof(HippotherapyProgramCategory)));
    }
}
