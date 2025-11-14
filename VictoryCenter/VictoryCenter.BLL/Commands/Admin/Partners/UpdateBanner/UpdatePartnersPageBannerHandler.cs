using AutoMapper;
using FluentResults;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.Partners;
using VictoryCenter.DAL.Entities;
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
                .GetFirstOrDefaultAsync();

            if (bannerEntity == null)
            {
                bannerEntity = _mapper.Map<PartnersPageBanner>(request.Dto);
                bannerEntity.CreatedAt = DateTimeOffset.UtcNow;
                await _repositoryWrapper.PartnersPageBannersRepository.CreateAsync(bannerEntity);
            }
            else
            {
                _mapper.Map(request.Dto, bannerEntity);
                _repositoryWrapper.PartnersPageBannersRepository.Update(bannerEntity);
            }

            await _repositoryWrapper.SaveChangesAsync();

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
