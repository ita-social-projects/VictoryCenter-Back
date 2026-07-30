using AutoMapper;
using FluentResults;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.Partners;
using VictoryCenter.BLL.Exceptions.ReorderExceptions;
using VictoryCenter.BLL.Interfaces.ReorderService;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Enums;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.BLL.Commands.Admin.Partners.UpdateSection;

public class UpdatePartnersSectionHandler : IRequestHandler<UpdatePartnersSectionCommand, Result<PartnersSectionDto>>
{
    private readonly IReorderService _reorderService;
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly IValidator<UpdatePartnersSectionCommand> _validator;
    private readonly IMapper _mapper;

    public UpdatePartnersSectionHandler(
        IRepositoryWrapper repositoryWrapper,
        IMapper mapper,
        IValidator<UpdatePartnersSectionCommand> validator,
        IReorderService reorderService)
    {
        _repositoryWrapper = repositoryWrapper;
        _mapper = mapper;
        _validator = validator;
        _reorderService = reorderService;
    }

    public async Task<Result<PartnersSectionDto>> Handle(UpdatePartnersSectionCommand request, CancellationToken cancellationToken)
    {
        try
        {
            await _validator.ValidateAndThrowAsync(request, cancellationToken);

            // Step 1: Validate that section exists
            var section = await _repositoryWrapper.PartnerSectionsRepository.GetFirstOrDefaultAsync(new()
            {
                Filter = s => s.Id == request.Id,
                Include = q => q.Include(s => s.Partners).ThenInclude(p => p.Localizations)
                    .Include(s => s.Localizations),
                AsNoTracking = false
            });

            if (section is null)
            {
                return Result.Fail<PartnersSectionDto>(ErrorMessagesConstants.NotFound(request.Id, typeof(PartnerSection)));
            }

            // Step 2: Validate that there are no overlapping IDs in update and delete lists
            var overlapValidationResult = ValidateNoDeleteAndUpdateIdsOverlap(request.UpdateDto);
            if (overlapValidationResult.IsFailed)
            {
                return overlapValidationResult;
            }

            // Step 3: Validate that all referenced images exist
            var imageValidationResult = await ValidateImageIdsAsync(request.UpdateDto);
            if (imageValidationResult.IsFailed)
            {
                return imageValidationResult;
            }

            // Step 4: Validate that all partners to be updated or deleted actually exist in this section
            var subEntityValidationResult = ValidateSubEntitiesExist(request.UpdateDto, section);
            if (subEntityValidationResult.IsFailed)
            {
                return subEntityValidationResult;
            }

            // Step 5: Perform updates within a transaction
            using (var scope = _repositoryWrapper.BeginTransaction())
            {
                SetSectionTranslationsToOutdated(request.UpdateDto, section);

                _mapper.Map(request.UpdateDto, section);

                var partnersDict = section.Partners.ToDictionary(p => p.Id);

                HandleDeletions(request.UpdateDto, partnersDict);
                HandleUpdates(request.UpdateDto, partnersDict);
                HandleCreations(request.UpdateDto, section);

                await _repositoryWrapper.SaveChangesAsync();

                await _reorderService.RenumberPriorityAsync<Partner>(
                    groupSelector: p => p.PartnersSectionId == section.Id);

                scope.Complete();
            }

            var updatedSection = await _repositoryWrapper.PartnerSectionsRepository.GetFirstOrDefaultAsync(new()
            {
                Filter = s => s.Id == section.Id,
                Include = q => q.Include(s => s.Partners.OrderBy(p => p.Priority)).ThenInclude(p => p.Image!)
            });

            var resultDto = _mapper.Map<PartnersSectionDto>(updatedSection);
            return Result.Ok(resultDto);
        }
        catch (ValidationException vex)
        {
            return Result.Fail<PartnersSectionDto>(vex.Errors.Select(e => e.ErrorMessage));
        }
        catch (ReorderException ex)
        {
            return Result.Fail(ReorderConstants.ErrorWithReordering(ex.Message));
        }
        catch (DbUpdateException)
        {
            return Result.Fail<PartnersSectionDto>(ErrorMessagesConstants.FailedToUpdateEntityInDatabase(typeof(PartnerSection)));
        }
    }

    private static Result<PartnersSectionDto> ValidateNoDeleteAndUpdateIdsOverlap(UpdatePartnersSectionDto dto)
    {
        var idsToUpdate = dto.PartnersToUpdate.Select(p => p.Id).ToHashSet();
        var idsToDelete = dto.PartnerIdsToDelete.ToHashSet();

        var overlappingIds = idsToUpdate.Intersect(idsToDelete).ToList();

        if (overlappingIds.Count > 0)
        {
            return Result.Fail<PartnersSectionDto>(
                ErrorMessagesConstants.CannotUpdateAndDeleteSameEntity(overlappingIds, typeof(Partner)));
        }

        return Result.Ok();
    }

    private static Result<PartnersSectionDto> ValidateSubEntitiesExist(UpdatePartnersSectionDto dto, PartnerSection section)
    {
        var existingPartnerIds = section.Partners.Select(p => p.Id).ToHashSet();
        var idsToUpdate = dto.PartnersToUpdate.Select(p => p.Id);
        var idsToDelete = dto.PartnerIdsToDelete;
        var allRequestedIds = idsToUpdate.Concat(idsToDelete).Distinct();
        var nonExistingIds = allRequestedIds.Where(id => !existingPartnerIds.Contains(id)).ToList();

        if (nonExistingIds.Count > 0)
        {
            return Result.Fail<PartnersSectionDto>(ErrorMessagesConstants.NotFound(nonExistingIds, typeof(Partner)));
        }

        return Result.Ok();
    }

    private async Task<Result<PartnersSectionDto>> ValidateImageIdsAsync(UpdatePartnersSectionDto dto)
    {
        var imageIds = dto.PartnersToCreate.Select(p => p.ImageId)
            .Concat(dto.PartnersToUpdate.Select(p => p.ImageId))
            .Distinct()
            .ToList();

        if (imageIds.Count == 0)
        {
            return Result.Ok();
        }

        var existingImageIds = (await _repositoryWrapper.ImageRepository
            .GetAllAsync(new QueryOptions<Image> { Filter = i => imageIds.Contains(i.Id) }))
            .Select(i => i.Id)
            .ToHashSet();

        var nonExistingImageIds = imageIds.Where(id => !existingImageIds.Contains(id)).ToList();

        if (nonExistingImageIds.Count > 0)
        {
            return Result.Fail<PartnersSectionDto>(ErrorMessagesConstants.NotFound(nonExistingImageIds, typeof(Image)));
        }

        return Result.Ok();
    }

    private void HandleDeletions(UpdatePartnersSectionDto dto, IDictionary<long, Partner> partnersDict)
    {
        if (dto.PartnerIdsToDelete.Count == 0)
        {
            return;
        }

        var partnersToDelete = dto.PartnerIdsToDelete
            .Where(partnersDict.ContainsKey)
            .Select(id => partnersDict[id])
            .ToList();
        _repositoryWrapper.PartnerRepository.DeleteRange(partnersToDelete);
    }

    private void HandleUpdates(UpdatePartnersSectionDto dto, IDictionary<long, Partner> partnersDict)
    {
        foreach (var partnerDto in dto.PartnersToUpdate)
        {
            if (partnersDict.TryGetValue(partnerDto.Id, out var existingPartner))
            {
                SetPartnerTranslationsToOutdated(partnerDto, existingPartner);
                _mapper.Map(partnerDto, existingPartner);
            }
        }
    }

    private static void SetSectionTranslationsToOutdated(UpdatePartnersSectionDto dto, PartnerSection section)
    {
        if (!string.Equals(dto.Title, section.Title, StringComparison.Ordinal) ||
            !string.Equals(dto.Description, section.Description, StringComparison.Ordinal))
        {
            foreach (var loc in section.Localizations)
            {
                loc.TranslationStatus = TranslationStatus.Outdated;
            }
        }
    }

    private static void SetPartnerTranslationsToOutdated(UpdatePartnerDto dto, Partner partner)
    {
        if (!string.Equals(dto.Description, partner.Description, StringComparison.Ordinal))
        {
            foreach (var loc in partner.Localizations)
            {
                loc.TranslationStatus = TranslationStatus.Outdated;
            }
        }
    }

    private void HandleCreations(UpdatePartnersSectionDto dto, PartnerSection section)
    {
        long nextPriority = section.Partners.Any()
            ? section.Partners.Max(p => p.Priority) + 1
            : 1;

        foreach (var partnerDto in dto.PartnersToCreate)
        {
            var newPartner = _mapper.Map<Partner>(partnerDto);
            newPartner.CreatedAt = DateTimeOffset.UtcNow;
            newPartner.Priority = nextPriority++;
            section.Partners.Add(newPartner);
        }
    }
}
