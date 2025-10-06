using AutoMapper;
using FluentResults;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.Partners;
using VictoryCenter.BLL.Exceptions.BlobStorageExceptions;
using VictoryCenter.BLL.Interfaces.BlobStorage;
using VictoryCenter.BLL.Interfaces.ReorderService;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.BLL.Commands.Admin.Partners.Update;

public class UpdatePartnersSectionHandler : IRequestHandler<UpdatePartnersSectionCommand, Result<PartnersSectionDto>>
{
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly IMapper _mapper;
    private readonly IBlobService _blobService;
    private readonly IReorderService _reorderService;
    private readonly IValidator<UpdatePartnersSectionCommand> _validator;

    public UpdatePartnersSectionHandler(
        IRepositoryWrapper repositoryWrapper,
        IMapper mapper,
        IBlobService blobService,
        IReorderService reorderService,
        IValidator<UpdatePartnersSectionCommand> validator)
    {
        _repositoryWrapper = repositoryWrapper;
        _mapper = mapper;
        _blobService = blobService;
        _reorderService = reorderService;
        _validator = validator;
    }

    public async Task<Result<PartnersSectionDto>> Handle(UpdatePartnersSectionCommand request, CancellationToken cancellationToken)
    {
        try
        {
            await _validator.ValidateAndThrowAsync(request, cancellationToken);

            var section = await _repositoryWrapper.PartnerSectionsRepository.GetFirstOrDefaultAsync(new QueryOptions<PartnerSection>
            {
                Filter = s => s.Id == request.Id,
                Include = q => q.Include(s => s.Partners).ThenInclude(p => p.Image!),
                AsNoTracking = false
            });

            if (section is null)
            {
                return Result.Fail<PartnersSectionDto>(ErrorMessagesConstants.NotFound(request.Id, typeof(PartnerSection)));
            }

            var imagesToUpload = new List<(string base64, string name, string mimeType)>();
            var imagesToDelete = new List<(string name, string mimeType)>();
            var finalPartnerIdsOrder = new List<long>();

            // Step 1: Prepare all changes in memory

            // Handle deletions
            var partnersToDelete = section.Partners
                .Where(p => request.UpdateDto.PartnerIdsToDelete.Contains(p.Id)).ToList();

            if (partnersToDelete.Any())
            {
                foreach (var partner in partnersToDelete)
                {
                    if (partner.Image != null)
                    {
                        imagesToDelete.Add((partner.Image.BlobName, partner.Image.MimeType));
                    }

                    section.Partners.Remove(partner);
                }
            }

            // Handle updates and creations
            foreach (var partnerDto in request.UpdateDto.Partners)
            {
                // Case 1: Create new partner
                if (!partnerDto.Id.HasValue)
                {
                    var blobName = Guid.NewGuid().ToString("N");
                    var newImage = new Image { BlobName = blobName, MimeType = partnerDto.Image.MimeType, CreatedAt = DateTime.UtcNow };
                    var newPartner = _mapper.Map<Partner>(partnerDto);
                    newPartner.Image = newImage;
                    newPartner.CreatedAt = DateTimeOffset.UtcNow;
                    newPartner.Priority = await _reorderService.GetNextDisplayOrderAsync<Partner>(p => p.PartnersSectionId == section.Id);

                    section.Partners.Add(newPartner);
                    imagesToUpload.Add((partnerDto.Image.Base64, blobName, partnerDto.Image.MimeType));
                }

                // Case 2: Update existing partner
                else
                {
                    var existingPartner = section.Partners.FirstOrDefault(p => p.Id == partnerDto.Id.Value);
                    if (existingPartner is not null)
                    {
                        _mapper.Map(partnerDto, existingPartner);
                        if (!string.IsNullOrWhiteSpace(partnerDto.Image.Base64))
                        {
                            // Prepare old image for deletion
                            if (existingPartner.Image != null)
                            {
                                imagesToDelete.Add((existingPartner.Image.BlobName, existingPartner.Image.MimeType));
                            }

                            // Prepare new image for upload
                            var newBlobName = Guid.NewGuid().ToString("N");
                            existingPartner.Image = new Image { BlobName = newBlobName, MimeType = partnerDto.Image.MimeType, CreatedAt = DateTime.UtcNow };
                            imagesToUpload.Add((partnerDto.Image.Base64, newBlobName, partnerDto.Image.MimeType));
                        }
                    }
                }
            }

            // Step 2: Perform blob storage operations
            var uploadedImages = new List<(string name, string mimeType)>();
            try
            {
                // Deletions first
                foreach (var (name, mimeType) in imagesToDelete)
                {
                    _blobService.DeleteFileInStorage(name, mimeType);
                }

                // Then uploads
                foreach (var (base64, name, mimeType) in imagesToUpload)
                {
                    await _blobService.SaveFileInStorageAsync(base64, name, mimeType);
                    uploadedImages.Add((name, mimeType));
                }
            }
            catch (Exception)
            {
                // If any upload fails, clean up previously uploaded images from this operation
                foreach (var (name, mimeType) in uploadedImages)
                {
                    try
                    {
                        _blobService.DeleteFileInStorage(name, mimeType);
                    }
                    catch
                    {
                        /* Ignore error */
                    }
                }

                // NOTE: We cannot safely roll back blob deletions, which is a limitation of this pattern.
                // The operation should be designed to be idempotent if possible.
                throw;
            }

            // Step 3: Save all entities in a single transaction
            using (var scope = _repositoryWrapper.BeginTransaction())
            {
                // Apply deletions to the database
                if (partnersToDelete.Any())
                {
                    _repositoryWrapper.PartnerRepository.DeleteRange(partnersToDelete);
                }

                _mapper.Map(request.UpdateDto, section); // Update section properties
                await _repositoryWrapper.SaveChangesAsync();

                // Final reordering
                finalPartnerIdsOrder = request.UpdateDto.Partners.Select(p => p.Id)
                    .Zip(section.Partners, (dtoId, entity) => dtoId ?? entity.Id)
                    .ToList();

                if (finalPartnerIdsOrder.Any())
                {
                    await _reorderService.SwapElementsAsync<Partner>(
                       finalPartnerIdsOrder,
                       p => p.Id,
                       p => p.PartnersSectionId == section.Id);
                }

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
        catch (DbUpdateException)
        {
            return Result.Fail<PartnersSectionDto>(ErrorMessagesConstants.FailedToCreateEntityInDatabase(typeof(PartnerSection)));
        }
        catch (BlobStorageException e)
        {
            return Result.Fail<PartnersSectionDto>(ErrorMessagesConstants.BlobStorageError(e.Message));
        }
    }
}
