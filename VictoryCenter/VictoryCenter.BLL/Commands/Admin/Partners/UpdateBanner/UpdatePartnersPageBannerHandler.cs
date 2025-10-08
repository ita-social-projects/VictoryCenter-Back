using AutoMapper;
using FluentResults;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Common;
using VictoryCenter.BLL.Interfaces.BlobStorage;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.BLL.Commands.Admin.Partners.UpdateBanner;

public record UpdatePartnersPageBannerDto
{
    public string Title { get; init; } = null!;
    public string Description { get; init; } = null!;
    public long ImageId { get; init; }

}

public record PartnersPageBannerDto
{
    public long Id { get; init; }
    public string Title { get; init; } = null!;
    public string Description { get; init; } = null!;
    public ImageDto Image { get; init; } = null!;
}

public record UpdatePartnersPageBannerCommand(UpdatePartnersPageBannerDto Dto)
    : IRequest<Result<PartnersPageBannerDto>>;

public class UpdatePartnersPageBannerHandler : IRequestHandler<UpdatePartnersPageBannerCommand, Result<PartnersPageBannerDto>>
{
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly IMapper _mapper;
    private readonly IValidator<UpdatePartnersPageBannerCommand> _validator;
    private readonly IBlobService _blobService;

    public UpdatePartnersPageBannerHandler(
        IRepositoryWrapper repositoryWrapper,
        IMapper mapper,
        IValidator<UpdatePartnersPageBannerCommand> validator,
        IBlobService blobService)
    {
        _repositoryWrapper = repositoryWrapper;
        _mapper = mapper;
        _validator = validator;
        _blobService = blobService;
    }

    public async Task<Result<PartnersPageBannerDto>> Handle(UpdatePartnersPageBannerCommand request, CancellationToken cancellationToken)
    {
        try
        {
            await _validator.ValidateAndThrowAsync(request, cancellationToken);

            var existingImage = await _repositoryWrapper.ImageRepository.GetFirstOrDefaultAsync(new QueryOptions<Image>
            {
                Filter = i => i.Id == request.Dto.ImageId
            });

            if (existingImage == null)
            {
                return Result.Fail<PartnersPageBannerDto>(ErrorMessagesConstants.NotFound(request.Dto.ImageId, typeof(Image)));
            }

            var bannerEntity = await _repositoryWrapper.PartnersPageBannersRepository
                .GetFirstOrDefaultAsync(new()
                {
                    Include = q => q.Include(b => b.Image!)
                });

            Image? oldImageToDelete = null;

            using (var scope = _repositoryWrapper.BeginTransaction())
            {
                if (bannerEntity == null)
                {
                    bannerEntity = _mapper.Map<PartnersPageBanner>(request.Dto);
                    bannerEntity.CreatedAt = DateTimeOffset.UtcNow;
                    await _repositoryWrapper.PartnersPageBannersRepository.CreateAsync(bannerEntity);
                }
                else
                {
                    if (bannerEntity.ImageId != request.Dto.ImageId)
                    {
                        oldImageToDelete = bannerEntity.Image;
                    }

                    _mapper.Map(request.Dto, bannerEntity);
                    _repositoryWrapper.PartnersPageBannersRepository.Update(bannerEntity);
                }

                await _repositoryWrapper.SaveChangesAsync();

                if (oldImageToDelete != null)
                {
                    _repositoryWrapper.ImageRepository.Delete(oldImageToDelete);
                    await _repositoryWrapper.SaveChangesAsync();
                }

                scope.Complete();
            }

            if (oldImageToDelete != null)
            {
                try
                {
                    _blobService.DeleteFileInStorage(oldImageToDelete.BlobName, oldImageToDelete.MimeType);
                }
                catch (Exception)
                {
                    // Ignore exception
                }
            }

            var result = await _repositoryWrapper.PartnersPageBannersRepository.GetFirstOrDefaultAsync(new()
            {
                Filter = b => b.Id == bannerEntity.Id,
                Include = q => q.Include(b => b.Image!)
            });

            var resultDto = _mapper.Map<PartnersPageBannerDto>(result);
            return Result.Ok(resultDto);
        }
        catch (ValidationException vex)
        {
            return Result.Fail<PartnersPageBannerDto>(vex.Errors.Select(e => e.ErrorMessage));
        }
        catch (DbUpdateException)
        {
            return Result.Fail<PartnersPageBannerDto>(ErrorMessagesConstants.FailedToCreateEntityInDatabase(typeof(PartnersPageBanner)));
        }
    }
}
