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

namespace VictoryCenter.BLL.Commands.Admin.TeamCategories.Update;

public class UpdateTeamCategoryHandler : IRequestHandler<UpdateTeamCategoryCommand, Result<TeamCategoryDto>>
{
    private readonly IMapper _mapper;
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly IValidator<UpdateTeamCategoryCommand> _validator;

    public UpdateTeamCategoryHandler(
        IMapper mapper,
        IRepositoryWrapper repositoryWrapper,
        IValidator<UpdateTeamCategoryCommand> validator)
    {
        _mapper = mapper;
        _repositoryWrapper = repositoryWrapper;
        _validator = validator;
    }

    public async Task<Result<TeamCategoryDto>> Handle(UpdateTeamCategoryCommand request, CancellationToken cancellationToken)
    {
        try
        {
            await _validator.ValidateAndThrowAsync(request, cancellationToken);

            var duplicateCategory =
                await _repositoryWrapper.TeamCategoriesRepository.GetFirstOrDefaultAsync(new QueryOptions<TeamCategory>
                {
                    Filter = entity => entity.Name == request.UpdateTeamCategoryDto.Name && entity.Id != request.Id
                });

            if (duplicateCategory is not null)
            {
                return Result.Fail<TeamCategoryDto>(TeamCategoryConstants.DuplicateCategoryName);
            }

            var categoryEntity =
                await _repositoryWrapper.TeamCategoriesRepository.GetFirstOrDefaultAsync(new QueryOptions<TeamCategory>
                {
                    Filter = entity => entity.Id == request.Id
                });

            if (categoryEntity is null)
            {
                return Result.Fail<TeamCategoryDto>(ErrorMessagesConstants.NotFound(request.Id, typeof(TeamCategory)));
            }

            var entityToUpdate = _mapper.Map<UpdateTeamCategoryDto, TeamCategory>(request.UpdateTeamCategoryDto);
            entityToUpdate.Id = request.Id;
            entityToUpdate.CreatedAt = categoryEntity.CreatedAt;

            _repositoryWrapper.TeamCategoriesRepository.Update(entityToUpdate);

            if (await _repositoryWrapper.SaveChangesAsync() > 0)
            {
                var updatedEntity = await _repositoryWrapper.TeamCategoriesRepository.GetFirstOrDefaultAsync(
                    new QueryOptions<TeamCategory>
                    {
                        Filter = tc => tc.Id == request.Id,
                        Include = tc => tc.Include(tc => tc.TeamMembers)
                    });

                var resultDto = _mapper.Map<TeamCategory, TeamCategoryDto>(updatedEntity!);
                return Result.Ok(resultDto);
            }

            return Result.Fail<TeamCategoryDto>(ErrorMessagesConstants.FailedToUpdateEntity(typeof(TeamCategory)));
        }
        catch (ValidationException ex)
        {
            return Result.Fail<TeamCategoryDto>(ex.Message);
        }
    }
}
