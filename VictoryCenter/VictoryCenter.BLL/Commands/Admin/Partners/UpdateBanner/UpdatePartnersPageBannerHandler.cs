using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using FluentResults;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.Commands.Admin.Partners.Update;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.Partners;
using VictoryCenter.BLL.DTOs.Common;
using VictoryCenter.BLL.Exceptions.BlobStorageExceptions;
using VictoryCenter.BLL.Interfaces.BlobStorage;
using VictoryCenter.BLL.Interfaces.ReorderService;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.Base;

namespace VictoryCenter.BLL.Commands.Admin.Partners.UpdateBanner;

public record UpdatePartnersPageBannerDto
{
    public string Title { get; init; } = null!;
    public string Description { get; init; } = null!;

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
    private readonly IBlobService _blobService;
    private readonly IValidator<UpdatePartnersPageBannerCommand> _validator;

    public UpdatePartnersPageBannerHandler(
        IRepositoryWrapper repositoryWrapper,
        IMapper mapper,
        IBlobService blobService,
        IValidator<UpdatePartnersPageBannerCommand> validator)
    {
        _repositoryWrapper = repositoryWrapper;
        _mapper = mapper;
        _blobService = blobService;
        _validator = validator;
    }

    public async Task<Result<PartnersPageBannerDto>> Handle(UpdatePartnersPageBannerCommand request, CancellationToken cancellationToken)
    {
        try
        {
            await _validator.ValidateAndThrowAsync(request, cancellationToken);

            var bannerEntity = await _repositoryWrapper.PartnersPageBannersRepository
                .GetFirstOrDefaultAsync();

            if (bannerEntity == null)
            {
                var newEntity = _mapper.Map<PartnersPageBanner>(request.Dto);

                newEntity.CreatedAt = DateTimeOffset.UtcNow;

                await _repositoryWrapper.PartnersPageBannersRepository.CreateAsync(newEntity);
                await _repositoryWrapper.SaveChangesAsync();

                var createdDto = _mapper.Map<PartnersPageBannerDto>(newEntity);
                return Result.Ok(createdDto);
            }

            _mapper.Map(request.Dto, bannerEntity);

            _repositoryWrapper.PartnersPageBannersRepository.Update(bannerEntity);
            await _repositoryWrapper.SaveChangesAsync();

            var updatedDto = _mapper.Map<PartnersPageBannerDto>(bannerEntity);
            return Result.Ok(updatedDto);
        }
        catch (ValidationException vex)
        {
            return Result.Fail<PartnersPageBannerDto>(vex.Errors.Select(e => e.ErrorMessage));
        }
        catch (DbUpdateException)
        {
            return Result.Fail<PartnersPageBannerDto>(ErrorMessagesConstants.FailedToCreateEntityInDatabase(typeof(PartnersPageBanner)));
        }
        catch (BlobStorageException e)
        {
            return Result.Fail<PartnersPageBannerDto>(ErrorMessagesConstants.BlobStorageError(e.Message));
        }
    }
}
