using AutoMapper;
using FluentResults;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.Partners;
using VictoryCenter.BLL.Exceptions.BlobStorageExceptions;
using VictoryCenter.BLL.Interfaces.BlobStorage;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.Base;

namespace VictoryCenter.BLL.Commands.Admin.Partners.Create;

public class CreatePartnersSectionHandler : IRequestHandler<CreatePartnersSectionCommand, Result<PartnersSectionDto>>
{
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly IValidator<CreatePartnersSectionCommand> _validator;
    private readonly IMapper _mapper;
    private readonly IBlobService _blobService;

    public CreatePartnersSectionHandler(
        IRepositoryWrapper repositoryWrapper,
        IMapper mapper,
        IValidator<CreatePartnersSectionCommand> validator,
        IBlobService blobService)
    {
        _repositoryWrapper = repositoryWrapper;
        _mapper = mapper;
        _validator = validator;
        _blobService = blobService;
    }

    public async Task<Result<PartnersSectionDto>> Handle(CreatePartnersSectionCommand request, CancellationToken cancellationToken)
    {
        try
        {
            await _validator.ValidateAndThrowAsync(request, cancellationToken);

            var addedImages = new List<(string name, string mimeType)>();

            var sectionEntity = _mapper.Map<PartnerSection>(request.CreatePartnersSectionDto);
            sectionEntity.CreatedAt = DateTimeOffset.UtcNow;

            var maxPriority = await _repositoryWrapper.PartnerSectionsRepository.MaxAsync(s => s.Priority);
            sectionEntity.Priority = (maxPriority ?? 0) + 1;

            var partnerEntities = new List<Partner>();
            var imageEntities = new List<Image>();
            var currentDisplayOrder = 1L;

            // Step 1: Prepare entities and image metadata
            foreach (var partnerDto in request.CreatePartnersSectionDto.Partners)
            {
                var blobName = Guid.NewGuid().ToString("N");
                var imageEntity = new Image
                {
                    BlobName = blobName,
                    MimeType = partnerDto.Image.MimeType,
                    CreatedAt = DateTime.UtcNow
                };

                var partnerEntity = _mapper.Map<Partner>(partnerDto);
                partnerEntity.CreatedAt = DateTimeOffset.UtcNow;
                partnerEntity.Priority = currentDisplayOrder++;
                partnerEntity.Image = imageEntity;

                imageEntities.Add(imageEntity);
                partnerEntities.Add(partnerEntity);

                addedImages.Add((blobName, partnerDto.Image.MimeType));
            }

            sectionEntity.Partners = partnerEntities;

            // Step 2: Upload images to blob storage
            try
            {
                for (int i = 0; i < addedImages.Count; i++)
                {
                    await _blobService.SaveFileInStorageAsync(
                        request.CreatePartnersSectionDto.Partners[i].Image.Base64,
                        addedImages[i].name,
                        addedImages[i].mimeType);
                }
            }
            catch (Exception)
            {
                // If any upload fails, clean up previously uploaded images
                foreach (var (name, mimeType) in addedImages)
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

                // Throw the original exception
                throw;
            }

            // Step 3: Save all entities in a single transaction
            using (var scope = _repositoryWrapper.BeginTransaction())
            {
                await _repositoryWrapper.PartnerSectionsRepository.CreateAsync(sectionEntity);
                await _repositoryWrapper.SaveChangesAsync(cancellationToken);
                scope.Complete();
            }

            var resultDto = _mapper.Map<PartnersSectionDto>(sectionEntity);
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
