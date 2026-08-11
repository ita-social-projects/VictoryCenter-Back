using AutoMapper;
using FluentResults;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.TeamCategories;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Enums;
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
            request = request with
            {
                UpdateTeamCategoryDto = request.UpdateTeamCategoryDto with
                {
                    Name = request.UpdateTeamCategoryDto.Name?.Trim()!,
                    Description = request.UpdateTeamCategoryDto.Description?.Trim()!
                }
            };

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

            var entityToUpdate = await _repositoryWrapper.TeamCategoriesRepository.GetFirstOrDefaultAsync(new QueryOptions<TeamCategory>
            {
                Filter = entity => entity.Id == request.Id,
                Include = tc => tc.Include(tc => tc.TeamMembers)
                        .Include(tm => tm.Localizations).ThenInclude(l => l.Language)
            });

            if (entityToUpdate is null)
            {
                return Result.Fail<TeamCategoryDto>(ErrorMessagesConstants.NotFound(request.Id, typeof(TeamCategory)));
            }

            SetTranslationsToOutdated(request, entityToUpdate);

            _mapper.Map(request.UpdateTeamCategoryDto, entityToUpdate);

            _repositoryWrapper.TeamCategoriesRepository.Update(entityToUpdate);

            if (await _repositoryWrapper.SaveChangesAsync() > 0)
            {
                var updatedEntity = await _repositoryWrapper.TeamCategoriesRepository.GetFirstOrDefaultAsync(
                    new QueryOptions<TeamCategory>
                    {
                        Filter = tc => tc.Id == request.Id,
                        Include = tc => tc.Include(tc => tc.TeamMembers)
                        .Include(tm => tm.Localizations).ThenInclude(l => l.Language)
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

    private static void SetTranslationsToOutdated(UpdateTeamCategoryCommand request, TeamCategory entityToUpdate)
    {
        if (!string.Equals(request.UpdateTeamCategoryDto.Name, entityToUpdate.Name) ||
            !string.Equals(request.UpdateTeamCategoryDto.Description, entityToUpdate.Description))
        {
            foreach (var loc in entityToUpdate.Localizations)
            {
                loc.TranslationStatus = TranslationStatus.Outdated;
            }
        }
    }
}
