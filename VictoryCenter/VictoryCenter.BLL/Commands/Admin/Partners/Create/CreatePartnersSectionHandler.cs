using AutoMapper;
using FluentResults;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.Images;
using VictoryCenter.BLL.DTOs.Common;
using VictoryCenter.BLL.Interfaces.BlobStorage;
using VictoryCenter.BLL.Interfaces.ReorderService;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.BLL.Commands.Admin.Partners.Create;

public record CreatePartnerImageDto
{
    public string Base64 { get; init; } = null!;
    public string MimeType { get; init; } = null!;
}

public record UpdatePartnerImageDto : CreatePartnerImageDto
{
    public long? ImageId { get; init; } = null;
}

public record CreatePartnerDto {
    public string Description { get; init; } = null!;
    public CreatePartnerImageDto Image { get; init; } = null!;
}

public record UpdatePartnerDto
{
    public long? Id { get; init; } = null;
    public string Description { get; init; } = null!;
    public UpdatePartnerImageDto Image { get; init; } = null!;
}

public record CreatePartnersSectionDto
{
    public string Title { get; init; } = null!;
    public string Description { get; init; } = null!;
    public List<CreatePartnerDto> Partners { get; init; } = [];
}

/*public record UpdatePartnersSectionDto
{
    public long? Id { get; init; } = null;
    public string Title { get; init; } = null!;
    public string Description { get; init; } = null!;
    public List<UpdatePartnerDto> Partners { get; init; } = [];
}
*/
public record UpdatePartnersSectionDto
{
    public long? Id { get; init; } = null;
    public string Title { get; init; } = null!;
    public string Description { get; init; } = null!;
    public List<UpdatePartnerDto> Partners { get; init; } = [];
    public List<long> PartnerIdsToDelete { get; init; } = [];
}

public record PartnerDto
{
    public long Id { get; init; }
    public string Description { get; init; }
    public ImageDto Image { get; init; } = null!;
}

public record PartnersSectionDto
{
    public long Id { get; init; }
    public string Title { get; init; } = null!;
    public string Description { get; init; } = null!;
    public List<PartnerDto> Partners { get; init; } = [];
}

// =================================================================================================

public class CreatePartnerImageValidator : AbstractValidator<CreatePartnerImageDto>
{
    public static readonly string[] AllowedMimeTypes = { "image/jpeg", "image/jpg", "image/png", "image/webp" };

    public CreatePartnerImageValidator()
    {
        RuleFor(x => x.Base64)
            .NotEmpty()
            .WithMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(UpdateImageDto.Base64)))
            .Must(IsValidBase64).WithMessage(ImageConstants.Base64ValidationError);

        RuleFor(x => x.MimeType)
            .NotEmpty().WithMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(UpdateImageDto.MimeType)))
            .Must(mimeType => AllowedMimeTypes.Contains(mimeType))
            .WithMessage(ImageConstants.MimeTypeValidationError(AllowedMimeTypes));
    }

    private static bool IsValidBase64(string? base64)
    {
        if (string.IsNullOrWhiteSpace(base64))
        {
            return false;
        }

        Span<byte> buffer = new(new byte[base64.Length]);
        return Convert.TryFromBase64String(base64, buffer, out _);
    }
}

public class UpdatePartnerImageValidator : AbstractValidator<UpdatePartnerImageDto>
{
    public UpdatePartnerImageValidator()
    {
        When(x => !x.ImageId.HasValue, () =>
        {
            RuleFor(x => x).SetValidator(new CreatePartnerImageValidator());
        });
    }
}

// =================================================================================================

public class CreatePartnerValidator : AbstractValidator<CreatePartnerDto>
{
    public static readonly int DescriptionMaxLength = 50;

    public CreatePartnerValidator()
    {
        RuleFor(x => x.Description)
            .NotEmpty()
            .WithMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(CreatePartnerDto.Description)))
            .MaximumLength(DescriptionMaxLength)
            .WithMessage(ErrorMessagesConstants
                .PropertyMustHaveAMaximumLengthOfNCharacters(nameof(CreatePartnerDto.Description), DescriptionMaxLength));

        RuleFor(x => x.Image)
            .SetValidator(new CreatePartnerImageValidator());
    }
}

public class UpdatePartnerValidator : AbstractValidator<UpdatePartnerDto>
{
    public static readonly int DescriptionMaxLength = 50;

    public UpdatePartnerValidator()
    {
        RuleFor(x => x.Description)
            .NotEmpty()
            .WithMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(CreatePartnerDto.Description)))
            .MaximumLength(DescriptionMaxLength)
            .WithMessage(ErrorMessagesConstants
                .PropertyMustHaveAMaximumLengthOfNCharacters(nameof(CreatePartnerDto.Description), DescriptionMaxLength));

        RuleFor(x => x.Image)
            .SetValidator(new UpdatePartnerImageValidator());
    }
}

// =================================================================================================

public class CreatePartnerSectionValidator : AbstractValidator<CreatePartnersSectionDto>
{
    public static readonly int TitleMaxLength = 50;
    public static readonly int DescriptionMaxLength = 100;
    public static readonly int PartnersMaxCount = 50;

    public CreatePartnerSectionValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty()
            .WithMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(CreatePartnersSectionDto.Title)))
            .MaximumLength(TitleMaxLength)
            .WithMessage(ErrorMessagesConstants
                .PropertyMustHaveAMaximumLengthOfNCharacters(nameof(CreatePartnersSectionDto.Title), TitleMaxLength));

        RuleFor(x => x.Description)
            .NotEmpty()
            .WithMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(CreatePartnersSectionDto.Description)))
            .MaximumLength(DescriptionMaxLength)
            .WithMessage(ErrorMessagesConstants
                .PropertyMustHaveAMaximumLengthOfNCharacters(nameof(CreatePartnersSectionDto.Description), DescriptionMaxLength));

        RuleFor(x => x.Partners)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage(ErrorMessagesConstants.CollectionCannotBeEmpty(nameof(CreatePartnersSectionDto.Partners)))
            .Must(partners => partners.Count <= PartnersMaxCount)
            .WithMessage(ErrorMessagesConstants
                .CollectionCannotContainMoreThan(nameof(CreatePartnersSectionDto.Partners), PartnersMaxCount));

        RuleForEach(x => x.Partners)
            .SetValidator(new CreatePartnerValidator());
    }
}

public class UpdatePartnerSectionValidator : AbstractValidator<UpdatePartnersSectionDto>
{
    public static readonly int TitleMaxLength = 50;
    public static readonly int DescriptionMaxLength = 100;
    public static readonly int PartnersMaxCount = 50;

    public UpdatePartnerSectionValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty()
            .WithMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(CreatePartnersSectionDto.Title)))
            .MaximumLength(TitleMaxLength)
            .WithMessage(ErrorMessagesConstants
                .PropertyMustHaveAMaximumLengthOfNCharacters(nameof(CreatePartnersSectionDto.Title), TitleMaxLength));

        RuleFor(x => x.Description)
            .NotEmpty()
            .WithMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(CreatePartnersSectionDto.Description)))
            .MaximumLength(DescriptionMaxLength)
            .WithMessage(ErrorMessagesConstants
                .PropertyMustHaveAMaximumLengthOfNCharacters(nameof(CreatePartnersSectionDto.Description), DescriptionMaxLength));

        RuleFor(x => x.Partners)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage(ErrorMessagesConstants.CollectionCannotBeEmpty(nameof(CreatePartnersSectionDto.Partners)))
            .Must(partners => partners.Count <= PartnersMaxCount)
            .WithMessage(ErrorMessagesConstants
                .CollectionCannotContainMoreThan(nameof(CreatePartnersSectionDto.Partners), PartnersMaxCount));

        RuleForEach(x => x.Partners)
            .SetValidator(new UpdatePartnerValidator());
    }
}

// =================================================================================================

public record CreatePartnersSectionCommand(CreatePartnersSectionDto CreatePartnersSectionDto) : IRequest<Result<PartnersSectionDto>>;

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

            using var scope = _repositoryWrapper.BeginTransaction();

            var partnerEntities = new List<Partner>();
            long currentDisplayOrder = 1;

            foreach (var partnerDto in request.CreatePartnersSectionDto.Partners)
            {
                var blobName = Guid.NewGuid().ToString("N");
                var imageEntity = new Image
                {
                    BlobName = blobName,
                    MimeType = partnerDto.Image.MimeType,
                    CreatedAt = DateTime.UtcNow
                };

                await _repositoryWrapper.ImageRepository.CreateAsync(imageEntity);
                await _repositoryWrapper.SaveChangesAsync(cancellationToken);
                await _blobService.SaveFileInStorageAsync(partnerDto.Image.Base64, blobName, partnerDto.Image.MimeType);

                var partnerEntity = _mapper.Map<Partner>(partnerDto);
                partnerEntity.ImageId = imageEntity.Id;
                partnerEntity.CreatedAt = DateTime.UtcNow;

                partnerEntity.Priority = currentDisplayOrder++;

                partnerEntities.Add(partnerEntity);
            }

            var sectionEntity = _mapper.Map<PartnerSection>(request.CreatePartnersSectionDto);
            sectionEntity.Partners = partnerEntities;
            sectionEntity.CreatedAt = DateTime.UtcNow;

            var maxPriority = await _repositoryWrapper.PartnerSectionsRepository.MaxAsync(s => s.Priority);
            sectionEntity.Priority = (maxPriority ?? 0) + 1;

            await _repositoryWrapper.PartnerSectionsRepository.CreateAsync(sectionEntity);
            await _repositoryWrapper.SaveChangesAsync(cancellationToken);

            scope.Complete();

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
        catch (Exception ex) when (ex.GetType().Name.Contains("BlobStorageException"))
        {
            return Result.Fail<PartnersSectionDto>(ErrorMessagesConstants.BlobStorageError(ex.Message));
        }
    }
}

// =================================================================================================

public record UpdatePartnersSectionCommand(UpdatePartnersSectionDto UpdateDto) : IRequest<Result<PartnersSectionDto>>;

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
        await _validator.ValidateAndThrowAsync(request, cancellationToken);

        using var scope = _repositoryWrapper.BeginTransaction();

        var section = await _repositoryWrapper.PartnerSectionsRepository.GetFirstOrDefaultAsync(new QueryOptions<PartnerSection>
        {
            Filter = s => s.Id == request.UpdateDto.Id,
            Include = q => q.Include(s => s.Partners).ThenInclude(p => p.Image!),
            AsNoTracking = false
        });

        if (section is null)
        {
            return Result.Fail<PartnersSectionDto>(ErrorMessagesConstants.NotFound(request.UpdateDto.Id, typeof(PartnerSection)));
        }

        // 2. Обробка видалення
        if (request.UpdateDto.PartnerIdsToDelete.Any())
        {
            var partnersToDelete = section.Partners
                .Where(p => request.UpdateDto.PartnerIdsToDelete.Contains(p.Id)).ToList();

            if (partnersToDelete.Any())
            {
                var imagesToDelete = partnersToDelete.Select(p => p.Image).Where(img => img != null).ToList();

                _repositoryWrapper.PartnerRepository.DeleteRange(partnersToDelete);

                if (imagesToDelete.Any())
                {
                    _repositoryWrapper.ImageRepository.DeleteRange(imagesToDelete!);
                }

                await _repositoryWrapper.SaveChangesAsync(cancellationToken);

                foreach (var image in imagesToDelete)
                {
                    _blobService.DeleteFileInStorage(image!.BlobName, image.MimeType);
                }

                await _reorderService.RenumberPriorityAsync<Partner>(p => p.PartnersSectionId == section.Id);
            }
        }

        var finalPartnerIdsOrder = new List<long>();

        // 3. Обробка оновлення та створення
        foreach (var partnerDto in request.UpdateDto.Partners)
        {
            // --- ВИПАДОК 1: Створення нового партнера ---
            if (!partnerDto.Id.HasValue)
            {
                var newImage = await CreateImageAsync(partnerDto.Image, cancellationToken);
                var newPartner = _mapper.Map<Partner>(partnerDto);
                newPartner.ImageId = newImage.Id;
                newPartner.CreatedAt = DateTime.UtcNow;
                newPartner.Priority = await _reorderService.GetNextDisplayOrderAsync<Partner>(p => p.PartnersSectionId == section.Id);

                section.Partners.Add(newPartner);
                await _repositoryWrapper.SaveChangesAsync(cancellationToken);

                finalPartnerIdsOrder.Add(newPartner.Id);
                continue;
            }

            // --- ВИПАДОК 2: Оновлення існуючого партнера ---
            var existingPartner = section.Partners.FirstOrDefault(p => p.Id == partnerDto.Id.Value);
            if (existingPartner is not null)
            {
                _mapper.Map(partnerDto, existingPartner);

                if (!string.IsNullOrWhiteSpace(partnerDto.Image.Base64))
                {
                    Image? oldImage = existingPartner.Image;
                    var newImage = await CreateImageAsync(partnerDto.Image, cancellationToken);
                    existingPartner.ImageId = newImage.Id;

                    if (oldImage != null)
                    {
                        _repositoryWrapper.ImageRepository.Delete(oldImage);
                        await _repositoryWrapper.SaveChangesAsync(cancellationToken);
                        _blobService.DeleteFileInStorage(oldImage.BlobName, oldImage.MimeType);
                    }
                }

                finalPartnerIdsOrder.Add(existingPartner.Id);
            }
        }

        // 4. Оновлюємо властивості самої секції
        _mapper.Map(request.UpdateDto, section);
        _repositoryWrapper.PartnerSectionsRepository.Update(section);

        if (finalPartnerIdsOrder.Any())
        {
            await _reorderService.SwapElementsAsync<Partner>(
                finalPartnerIdsOrder,
                p => p.Id,
                p => p.PartnersSectionId == section.Id);
        }

        await _repositoryWrapper.SaveChangesAsync(cancellationToken);
        scope.Complete();

        var resultDto = _mapper.Map<PartnersSectionDto>(section);
        return Result.Ok(resultDto);
    }

    private async Task<Image> CreateImageAsync(CreatePartnerImageDto imageDto, CancellationToken token)
    {
        var blobName = Guid.NewGuid().ToString("N");
        var imageEntity = new Image
        {
            BlobName = blobName,
            MimeType = imageDto.MimeType,
            CreatedAt = DateTime.UtcNow
        };
        await _repositoryWrapper.ImageRepository.CreateAsync(imageEntity);
        await _repositoryWrapper.SaveChangesAsync(token);
        await _blobService.SaveFileInStorageAsync(imageDto.Base64, blobName, imageDto.MimeType);
        return imageEntity;
    }
}

// =================================================================================================

public record DeletePartnersSectionCommand(long Id) : IRequest<Result<long>>;

public class DeletePartnersSectionHandler : IRequestHandler<DeletePartnersSectionCommand, Result<long>>
{
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly IBlobService _blobService;

    public DeletePartnersSectionHandler(IRepositoryWrapper repositoryWrapper, IBlobService blobService)
    {
        _repositoryWrapper = repositoryWrapper;
        _blobService = blobService;
    }

    public async Task<Result<long>> Handle(DeletePartnersSectionCommand request, CancellationToken cancellationToken)
    {
        using var scope = _repositoryWrapper.BeginTransaction();

        var sectionToDelete = await _repositoryWrapper.PartnerSectionsRepository.GetFirstOrDefaultAsync(
            new QueryOptions<PartnerSection>
            {
                Filter = s => s.Id == request.Id,
                Include = q => q.Include(s => s.Partners)
                                .ThenInclude(p => p.Image!),
                AsNoTracking = false
            });

        if (sectionToDelete is null)
        {
            return Result.Fail<long>(ErrorMessagesConstants.NotFound(request.Id, typeof(PartnerSection)));
        }

        var partnersToDelete = sectionToDelete.Partners.ToList();
        var imagesToDelete = partnersToDelete.Select(p => p.Image).Where(img => img != null).ToList();

        try
        {
            if (imagesToDelete.Any())
            {
                _repositoryWrapper.ImageRepository.DeleteRange(imagesToDelete!);
            }

            if (partnersToDelete.Any())
            {
                _repositoryWrapper.PartnerRepository.DeleteRange(partnersToDelete);
            }

            _repositoryWrapper.PartnerSectionsRepository.Delete(sectionToDelete);

            await _repositoryWrapper.SaveChangesAsync(cancellationToken);

            foreach (var image in imagesToDelete)
            {
                _blobService.DeleteFileInStorage(image!.BlobName, image.MimeType);
            }

            scope.Complete();

            return Result.Ok(request.Id);
        }
        catch (DbUpdateException ex)
        {
            return Result.Fail<long>(ErrorMessagesConstants.FailedToDeleteEntity(typeof(PartnerSection)));
        }
    }
}

// =================================================================================================

public record GetAllPartnersSectionsQuery() : IRequest<Result<IEnumerable<PartnersSectionDto>>>;

public class GetAllPartnersSectionsQueryHandler
    : IRequestHandler<GetAllPartnersSectionsQuery, Result<IEnumerable<PartnersSectionDto>>>
{
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly IMapper _mapper;

    public GetAllPartnersSectionsQueryHandler(IRepositoryWrapper repositoryWrapper, IMapper mapper)
    {
        _repositoryWrapper = repositoryWrapper;
        _mapper = mapper;
    }

    public async Task<Result<IEnumerable<PartnersSectionDto>>> Handle(
        GetAllPartnersSectionsQuery request,
        CancellationToken cancellationToken)
    {
        try
        {
            var sections = await _repositoryWrapper.PartnerSectionsRepository.GetAllAsync(
                new QueryOptions<PartnerSection>
                {
                    Include = q => q.Include(s => s.Partners)
                                    .ThenInclude(p => p.Image!),
                    OrderByASC = s => s.Priority,
                    AsNoTracking = true
                });

            var sectionDtos = _mapper.Map<IEnumerable<PartnersSectionDto>>(sections);

            return Result.Ok(sectionDtos);
        }
        catch (Exception)
        {
            return Result.Fail<IEnumerable<PartnersSectionDto>>($"Failed to retrieve partner sections");
        }
    }
}
