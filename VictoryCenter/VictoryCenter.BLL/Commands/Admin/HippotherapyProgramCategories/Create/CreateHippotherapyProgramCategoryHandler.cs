using AutoMapper;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.Commands.Base;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.HippotherapyProgramCategories;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.Base;

namespace VictoryCenter.BLL.Commands.Admin.HippotherapyProgramCategories.Create;

public class CreateHippotherapyProgramCategoryHandler : BaseHandler<CreateHippotherapyProgramCategoryCommand, HippotherapyProgramCategoryDto>
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

    public override async Task<HippotherapyProgramCategoryDto> HandleRequest(CreateHippotherapyProgramCategoryCommand request, CancellationToken cancellationToken)
    {
        await _validator.ValidateAndThrowAsync(request, cancellationToken);

        HippotherapyProgramCategory entity = _mapper.Map<HippotherapyProgramCategory>(request.CreateProgramCategoryDto);
        entity.CreatedAt = DateTimeOffset.UtcNow;
        await _repositoryWrapper.HippotherapyProgramCategoriesRepository.CreateAsync(entity, cancellationToken);

        if (await _repositoryWrapper.SaveChangesAsync() > 0)
        {
            HippotherapyProgramCategoryDto responseDto = _mapper.Map<HippotherapyProgramCategoryDto>(entity);
            return responseDto;
        }

        throw new DbUpdateException(ErrorMessagesConstants.FailedToCreateEntity(typeof(HippotherapyProgramCategory)));
    }
}
