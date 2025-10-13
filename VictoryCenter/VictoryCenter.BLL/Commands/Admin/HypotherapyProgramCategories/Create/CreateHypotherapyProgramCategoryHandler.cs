using AutoMapper;
using FluentResults;
using FluentValidation;
using MediatR;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.HypotherapyProgramCategories;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.Base;

namespace VictoryCenter.BLL.Commands.Admin.HypotherapyProgramCategories.Create;

public class CreateHypotherapyProgramCategoryHandler : IRequestHandler<CreateHypotherapyProgramCategoryCommand, Result<HypotherapyProgramCategoryDto>>
{
    private readonly IMapper _mapper;
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly IValidator<CreateHypotherapyProgramCategoryCommand> _validator;

    public CreateHypotherapyProgramCategoryHandler(IMapper mapper, IRepositoryWrapper repositoryWrapper, IValidator<CreateHypotherapyProgramCategoryCommand> validator)
    {
        _mapper = mapper;
        _repositoryWrapper = repositoryWrapper;
        _validator = validator;
    }

    public async Task<Result<HypotherapyProgramCategoryDto>> Handle(CreateHypotherapyProgramCategoryCommand request, CancellationToken cancellationToken)
    {
        try
        {
            await _validator.ValidateAndThrowAsync(request, cancellationToken);

            HippotherapyProgramCategory entity = _mapper.Map<HippotherapyProgramCategory>(request.ProgramCategoryDto);
            entity.CreatedAt = DateTimeOffset.UtcNow;
            await _repositoryWrapper.HypotherapyProgramCategoriesRepository.CreateAsync(entity);

            if (await _repositoryWrapper.SaveChangesAsync() > 0)
            {
                HypotherapyProgramCategoryDto responseDto = _mapper.Map<HypotherapyProgramCategoryDto>(entity);
                return Result.Ok(responseDto);
            }

            return Result.Fail<HypotherapyProgramCategoryDto>(ErrorMessagesConstants.FailedToCreateEntity(typeof(HippotherapyProgramCategory)));
        }
        catch (ValidationException ex)
        {
            return Result.Fail<HypotherapyProgramCategoryDto>(ex.Errors.Select(e => e.ErrorMessage));
        }
    }
}
