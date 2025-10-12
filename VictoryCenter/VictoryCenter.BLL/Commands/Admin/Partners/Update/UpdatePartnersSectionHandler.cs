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
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.BLL.Commands.Admin.Partners.Update;

public class UpdatePartnersSectionHandler : IRequestHandler<UpdatePartnersSectionCommand, Result<PartnersSectionDto>>
{
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly IValidator<UpdatePartnersSectionCommand> _validator;
    private readonly IMapper _mapper;
    private readonly IReorderService _reorderService;

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

            var section = await _repositoryWrapper.PartnerSectionsRepository.GetFirstOrDefaultAsync(new()
            {
                Filter = s => s.Id == request.Id,
                Include = q => q.Include(s => s.Partners),
                AsNoTracking = false
            });

            if (section is null)
            {
                return Result.Fail<PartnersSectionDto>(ErrorMessagesConstants.NotFound(request.Id, typeof(PartnerSection)));
            }

            // Step 1: Validate all referenced images exist
            var imageIds = request.UpdateDto.PartnersToUpdate
                .Select(p => p.ImageId)
                .Distinct()
                .ToList();

            if (imageIds.Any())
            {
                var existingImageIds = (await _repositoryWrapper.ImageRepository
                    .GetAllAsync(new QueryOptions<Image>
                    {
                        Filter = i => imageIds.Contains(i.Id)
                    }))
                    .Select(i => i.Id)
                    .ToHashSet();

                var nonExistingImageIds = imageIds.Where(id => !existingImageIds.Contains(id)).ToList();

                if (nonExistingImageIds.Any())
                {
                    return Result.Fail<PartnersSectionDto>(
                        ErrorMessagesConstants.NotFound(nonExistingImageIds, typeof(Image)));
                }
            }

            // Apply all changes in a single transaction
            using (var scope = _repositoryWrapper.BeginTransaction())
            {
                // Update section properties
                _mapper.Map(request.UpdateDto, section);

                // Step 3: Delete partners marked for deletion
                if (request.UpdateDto.PartnerIdsToDelete.Any())
                {
                    var partnersToDelete = section.Partners
                        .Where(p => request.UpdateDto.PartnerIdsToDelete.Contains(p.Id))
                        .ToList();
                    _repositoryWrapper.PartnerRepository.DeleteRange(partnersToDelete);
                }

                var dbPartnersDict = section.Partners.ToDictionary(p => p.Id);
                var displayOrderList = new List<Partner>();

                // Step 4: Update existing partners and add new ones
                foreach (var partnerDto in request.UpdateDto.PartnersToUpdate)
                {
                    if (partnerDto.Id.HasValue && dbPartnersDict.TryGetValue(partnerDto.Id.Value, out var existingPartner))
                    {
                        _mapper.Map(partnerDto, existingPartner);

                        // Preserve specified order
                        displayOrderList.Add(existingPartner);
                    }
                    else
                    {
                        var newPartner = _mapper.Map<Partner>(partnerDto);
                        newPartner.CreatedAt = DateTimeOffset.UtcNow;
                        newPartner.Priority = await _reorderService.GetNextDisplayOrderAsync<Partner>(p => p.PartnersSectionId == section.Id);
                        section.Partners.Add(newPartner);

                        // Preserve specified order
                        displayOrderList.Add(newPartner);
                    }
                }

                await _repositoryWrapper.SaveChangesAsync();

                await _reorderService.RenumberPriorityAsync<Partner>(p => p.PartnersSectionId == section.Id);

                await _repositoryWrapper.SaveChangesAsync();

                var finalOrderIds = displayOrderList.Select(x => x.Id).ToList();

                await _reorderService.SwapElementsAsync<Partner>(
                    finalOrderIds,
                    p => p.Id,
                    p => p.PartnersSectionId == section.Id);

                await _repositoryWrapper.SaveChangesAsync();

                scope.Complete();
            }

            var resultDto = _mapper.Map<PartnersSectionDto>(section);
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
}
