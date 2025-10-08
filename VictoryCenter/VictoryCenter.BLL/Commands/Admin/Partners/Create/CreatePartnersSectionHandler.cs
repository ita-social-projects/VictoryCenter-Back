using AutoMapper;
using FluentResults;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.Partners;
using VictoryCenter.BLL.Interfaces.ReorderService;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.BLL.Commands.Admin.Partners.Create;

public class CreatePartnersSectionHandler : IRequestHandler<CreatePartnersSectionCommand, Result<PartnersSectionDto>>
{
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly IValidator<CreatePartnersSectionCommand> _validator;
    private readonly IMapper _mapper;
    private readonly IReorderService _reorderService;

    public CreatePartnersSectionHandler(
        IRepositoryWrapper repositoryWrapper,
        IMapper mapper,
        IValidator<CreatePartnersSectionCommand> validator,
        IReorderService reorderService)
    {
        _repositoryWrapper = repositoryWrapper;
        _mapper = mapper;
        _validator = validator;
        _reorderService = reorderService;
    }

    public async Task<Result<PartnersSectionDto>> Handle(CreatePartnersSectionCommand request, CancellationToken cancellationToken)
    {
        try
        {
            await _validator.ValidateAndThrowAsync(request, cancellationToken);

            var imageIds = request.CreatePartnersSectionDto.Partners.Select(p => p.ImageId).ToList();

            var nonExistingImageIds = (await _repositoryWrapper.ImageRepository
                .GetAllAsync(new QueryOptions<Image>
                {
                    Filter = i => !imageIds.Contains(i.Id)
                })).ToList();

            if (nonExistingImageIds.Any())
            {
                return Result.Fail<PartnersSectionDto>(ErrorMessagesConstants.NotFound(nonExistingImageIds, typeof(Image)));
            }

            var partnersSectionEntity = _mapper.Map<PartnerSection>(request.CreatePartnersSectionDto);
            partnersSectionEntity.CreatedAt = DateTimeOffset.UtcNow;
            partnersSectionEntity.Priority = await _reorderService.GetNextDisplayOrderAsync<PartnerSection>();

            long currentDisplayOrder = 1;
            foreach (var partnerEntity in partnersSectionEntity.Partners)
            {
                partnerEntity.CreatedAt = DateTimeOffset.UtcNow;
                partnerEntity.Priority = currentDisplayOrder++;
            }

            using (var scope = _repositoryWrapper.BeginTransaction())
            {
                await _repositoryWrapper.PartnerSectionsRepository.CreateAsync(partnersSectionEntity);
                await _repositoryWrapper.SaveChangesAsync();
                scope.Complete();
            }

            var createdSection = await _repositoryWrapper.PartnerSectionsRepository.GetFirstOrDefaultAsync(new()
            {
                Filter = s => s.Id == partnersSectionEntity.Id,
                Include = q => q.Include(s => s.Partners).ThenInclude(p => p.Image!)
            });

            var resultDto = _mapper.Map<PartnersSectionDto>(createdSection);
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
    }
}

/*public class CreatePartnersSectionHandler : IRequestHandler<CreatePartnersSectionCommand, Result<PartnersSectionDto>>
{
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly IValidator<CreatePartnersSectionCommand> _validator;
    private readonly IMapper _mapper;
    private readonly IBlobService _blobService;
    private readonly IReorderService _reorderService;

    public CreatePartnersSectionHandler(
        IRepositoryWrapper repositoryWrapper,
        IMapper mapper,
        IValidator<CreatePartnersSectionCommand> validator,
        IBlobService blobService,
        IReorderService reorderService)
    {
        _repositoryWrapper = repositoryWrapper;
        _mapper = mapper;
        _validator = validator;
        _blobService = blobService;
        _reorderService = reorderService;
    }

    public async Task<Result<PartnersSectionDto>> Handle(CreatePartnersSectionCommand request, CancellationToken cancellationToken)
    {
        try
        {
            await _validator.ValidateAndThrowAsync(request, cancellationToken);

            // Step 1: Prepare entities and images
            var addedImages = new List<(string name, string mimeType, string base64)>();

            var partnersSectionEntity = _mapper.Map<PartnerSection>(request.CreatePartnersSectionDto);
            partnersSectionEntity.CreatedAt = DateTimeOffset.UtcNow;

            var nextPriority = await _reorderService.GetNextDisplayOrderAsync<PartnerSection>();
            partnersSectionEntity.Priority = nextPriority;

            var partnerEntities = new List<Partner>();
            var currentDisplayOrder = 1L;

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

                partnerEntities.Add(partnerEntity);

                addedImages.Add((name: blobName, mimeType: partnerDto.Image.MimeType, base64: partnerDto.Image.Base64));
            }

            partnersSectionEntity.Partners = partnerEntities;

            // Step 2: Save to database within a transaction
            using (var scope = _repositoryWrapper.BeginTransaction())
            {
                await _repositoryWrapper.PartnerSectionsRepository.CreateAsync(partnersSectionEntity);

                if (await _repositoryWrapper.SaveChangesAsync() <= 0)
                {
                    return Result.Fail<PartnersSectionDto>(ErrorMessagesConstants.FailedToCreateEntityInDatabase(typeof(PartnerSection)));
                }

                // Step 3: Save images to blob storage
                await SaveImagesToBlobStorage(addedImages);

                scope.Complete();
            }

            // Finaly: map to DTO and return
            var resultDto = _mapper.Map<PartnersSectionDto>(partnersSectionEntity);
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

    private async Task SaveImagesToBlobStorage(List<(string name, string mimeType, string base64)> addedImages)
    {
        try
        {
            foreach (var (name, mimeType, base64) in addedImages)
            {
                await _blobService.SaveFileInStorageAsync(
                    base64,
                    name,
                    mimeType);
            }
        }
        catch (Exception)
        {
            // If any upload fails, clean up previously uploaded images
            foreach (var (name, mimeType, _) in addedImages)
            {
                try
                {
                    _blobService.DeleteFileInStorage(name, mimeType);
                }
                catch
                {
                    *//* Ignore error *//*
                }
            }

            // Throw the original exception
            throw;
        }
    }
}*/
