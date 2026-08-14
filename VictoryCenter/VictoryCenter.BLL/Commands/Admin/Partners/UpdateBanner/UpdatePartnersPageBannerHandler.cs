using AutoMapper;
using FluentResults;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.Partners;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Enums;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.BLL.Commands.Admin.Partners.UpdateBanner;

public class UpdatePartnersPageBannerHandler : IRequestHandler<UpdatePartnersPageBannerCommand, Result<PartnersPageBannerDto>>
{
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly IMapper _mapper;
    private readonly IValidator<UpdatePartnersPageBannerCommand> _validator;

    public UpdatePartnersPageBannerHandler(
        IRepositoryWrapper repositoryWrapper,
        IMapper mapper,
        IValidator<UpdatePartnersPageBannerCommand> validator)
    {
        _repositoryWrapper = repositoryWrapper;
        _mapper = mapper;
        _validator = validator;
    }

    public async Task<Result<PartnersPageBannerDto>> Handle(UpdatePartnersPageBannerCommand request, CancellationToken cancellationToken)
    {
        try
        {
            await _validator.ValidateAndThrowAsync(request, cancellationToken);

            if (request.Dto.ImageId != null)
            {
                var existingImage = await _repositoryWrapper.ImageRepository.GetFirstOrDefaultAsync(new QueryOptions<Image>
                {
                    Filter = i => i.Id == request.Dto.ImageId,
                    AsNoTracking = true
                });

                if (existingImage == null)
                {
                    return Result.Fail<PartnersPageBannerDto>(ErrorMessagesConstants.NotFound(request.Dto.ImageId, typeof(Image)));
                }
            }

            var bannerEntity = await _repositoryWrapper.PartnersPageBannersRepository
                .GetFirstOrDefaultAsync(new QueryOptions<PartnersPageBanner>
                {
                    Include = q => q.Include(b => b.Localizations),
                    AsNoTracking = false
                });

            if (bannerEntity == null)
            {
                bannerEntity = _mapper.Map<PartnersPageBanner>(request.Dto);
                bannerEntity.CreatedAt = DateTimeOffset.UtcNow;
                await _repositoryWrapper.PartnersPageBannersRepository.CreateAsync(bannerEntity);
            }
            else
            {
                SetTranslationsToOutdated(request.Dto, bannerEntity);
                _mapper.Map(request.Dto, bannerEntity);
                _repositoryWrapper.PartnersPageBannersRepository.Update(bannerEntity);
            }

            if (await _repositoryWrapper.SaveChangesAsync() > 0)
            {
                var result = await _repositoryWrapper.PartnersPageBannersRepository.GetFirstOrDefaultAsync(new()
                {
                    Filter = b => b.Id == bannerEntity.Id,
                    Include = q => q.Include(b => b.Image!)
                                    .Include(b => b.Localizations)
                                        .ThenInclude(l => l.Language)
                });

                return Result.Ok(_mapper.Map<PartnersPageBannerDto>(result));
            }

            return Result.Fail<PartnersPageBannerDto>(ErrorMessagesConstants.FailedToUpdateEntity(typeof(PartnersPageBanner)));
        }
        catch (ValidationException vex)
        {
            return Result.Fail<PartnersPageBannerDto>(vex.Errors.Select(e => e.ErrorMessage));
        }
        catch (DbUpdateException)
        {
            return Result.Fail<PartnersPageBannerDto>(ErrorMessagesConstants.FailedToUpdateEntityInDatabase(typeof(PartnersPageBanner)));
        }
    }

    private static void SetTranslationsToOutdated(UpdatePartnersPageBannerDto dto, PartnersPageBanner entity)
    {
        if (!string.Equals(dto.Title, entity.Title, StringComparison.Ordinal) ||
            !string.Equals(dto.Description, entity.Description, StringComparison.Ordinal))
        {
            foreach (var loc in entity.Localizations)
            {
                loc.TranslationStatus = TranslationStatus.Outdated;
            }
        }
    }
}
