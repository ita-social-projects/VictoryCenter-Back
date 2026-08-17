using AutoMapper;
using FluentResults;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.TeamCategories;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

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
            if (request.CreateTeamCategoryDto is not null)
            {
                request = request with
                {
                    CreateTeamCategoryDto = request.CreateTeamCategoryDto with
                    {
                        Name = request.CreateTeamCategoryDto.Name?.Trim()!,
                        Description = request.CreateTeamCategoryDto.Description?.Trim()!
                    }
                };
            }

            await _validator.ValidateAndThrowAsync(request, cancellationToken);

            var duplicateCategory =
                await _repositoryWrapper.TeamCategoriesRepository.GetFirstOrDefaultAsync(new QueryOptions<TeamCategory>
                {
                    Filter = entity => entity.Name == request.CreateTeamCategoryDto.Name
                });

            if (duplicateCategory is not null)
            {
                return Result.Fail<TeamCategoryDto>(TeamCategoryConstants.DuplicateCategoryName);
            }

            var entity = _mapper.Map<TeamCategory>(request.CreateTeamCategoryDto);
            entity.CreatedAt = DateTimeOffset.UtcNow;

            await _repositoryWrapper.TeamCategoriesRepository.CreateAsync(entity);

            if (await _repositoryWrapper.SaveChangesAsync() > 0)
            {
                var createdEntity = await _repositoryWrapper.TeamCategoriesRepository.GetFirstOrDefaultAsync(
                    new QueryOptions<TeamCategory>
                    {
                        Filter = tc => tc.Id == entity.Id,
                        Include = tc => tc.Include(tc => tc.TeamMembers)
                    });

                var resultDto = _mapper.Map<TeamCategoryDto>(createdEntity);
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
