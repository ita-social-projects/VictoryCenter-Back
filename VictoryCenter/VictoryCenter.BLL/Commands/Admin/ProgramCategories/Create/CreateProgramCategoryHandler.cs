using AutoMapper;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.Commands.Base;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.ProgramCategories;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.Base;

namespace VictoryCenter.BLL.Commands.Admin.ProgramCategories.Create;

public class CreateProgramCategoryHandler : BaseHandler<CreateProgramCategoryCommand, ProgramCategoryDto>
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

    public override async Task<ProgramCategoryDto> HandleRequest(CreateProgramCategoryCommand request, CancellationToken cancellationToken)
    {
        await _validator.ValidateAndThrowAsync(request, cancellationToken);

        ProgramCategory entity = _mapper.Map<ProgramCategory>(request.ProgramCategoryDto);
        entity.CreatedAt = DateTimeOffset.UtcNow;
        await _repositoryWrapper.ProgramCategoriesRepository.CreateAsync(entity, cancellationToken);

        if (await _repositoryWrapper.SaveChangesAsync() <= 0)
        {
            throw new DbUpdateException(ErrorMessagesConstants.FailedToCreateEntity(typeof(ProgramCategory)));
        }

        ProgramCategoryDto responseDto = _mapper.Map<ProgramCategoryDto>(entity);
        return responseDto;
    }
}
