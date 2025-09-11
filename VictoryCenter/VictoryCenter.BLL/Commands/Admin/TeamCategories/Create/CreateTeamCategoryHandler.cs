using AutoMapper;
using FluentResults;
using FluentValidation;
using MediatR;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.TeamCategories;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.Base;

namespace VictoryCenter.BLL.Commands.Admin.TeamCategories.Create;

public class CreateTeamCategoryHandler : IRequestHandler<CreateTeamCategoryCommand, Result<TeamCategoryDto>>
{
    private readonly IMapper _mapper;
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly IValidator<CreateTeamCategoryCommand> _validator;

    public CreateTeamCategoryHandler(
        IMapper mapper,
        IRepositoryWrapper repositoryWrapper,
        IValidator<CreateTeamCategoryCommand> validator)
    {
        _mapper = mapper;
        _repositoryWrapper = repositoryWrapper;
        _validator = validator;
    }

    public async Task<Result<TeamCategoryDto>> Handle(CreateTeamCategoryCommand request, CancellationToken cancellationToken)
    {
        try
        {
            await _validator.ValidateAndThrowAsync(request, cancellationToken);

            var entity = _mapper.Map<TeamCategory>(request.CreateCategoryDto);
            entity.CreatedAt = DateTime.UtcNow;

            await _repositoryWrapper.TeamCategoriesRepository.CreateAsync(entity);

            if (await _repositoryWrapper.SaveChangesAsync() > 0)
            {
                var resultDto = _mapper.Map<TeamCategoryDto>(entity);
                return Result.Ok(resultDto);
            }

            return Result.Fail<TeamCategoryDto>(ErrorMessagesConstants.FailedToCreateEntity(typeof(TeamCategory)));
        }
        catch (ValidationException ex)
        {
            return Result.Fail<TeamCategoryDto>(ex.Message);
        }
    }
}
