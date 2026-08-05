using AutoMapper;
using FluentResults;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.Localization.PartnersPageBanner;
using VictoryCenter.BLL.Interfaces.Localization;
using VictoryCenter.DAL.Entities.Localization;
using PartnersPageBannerEntity = VictoryCenter.DAL.Entities.PartnersPageBanner;

namespace VictoryCenter.BLL.Commands.Admin.Localization.PartnersPageBanner.Update;

public class UpdatePartnersPageBannerLocalizationHandler
    : IRequestHandler<UpdatePartnersPageBannerLocalizationCommand, Result<PartnersPageBannerLocalizationDto>>
{
    private readonly IMapper _mapper;
    private readonly IValidator<UpdatePartnersPageBannerLocalizationCommand> _validator;
    private readonly ILocalizationService<PartnersPageBannerEntity, PartnersPageBannerLocalization> _localizationService;

    public UpdatePartnersPageBannerLocalizationHandler(
        IMapper mapper,
        IValidator<UpdatePartnersPageBannerLocalizationCommand> validator,
        ILocalizationService<PartnersPageBannerEntity, PartnersPageBannerLocalization> localizationService)
    {
        _mapper = mapper;
        _validator = validator;
        _localizationService = localizationService;
    }

    public async Task<Result<PartnersPageBannerLocalizationDto>> Handle(UpdatePartnersPageBannerLocalizationCommand request, CancellationToken cancellationToken)
    {
        try
        {
            await _validator.ValidateAndThrowAsync(request, cancellationToken);

            var dto = request.UpdatePartnersPageBannerLocalizationDto;
            var entity = _mapper.Map<PartnersPageBannerLocalization>(dto);
            entity.EntityId = request.EntityId;
            entity.LanguageId = request.LanguageId;
            var result = await _localizationService.UpdateEntityLocalizationAsync(entity);
            var responseDto = _mapper.Map<PartnersPageBannerLocalizationDto>(result);
            return Result.Ok(responseDto);
        }
        catch (KeyNotFoundException knfex)
        {
            return Result.Fail<PartnersPageBannerLocalizationDto>(knfex.Message);
        }
        catch (InvalidOperationException)
        {
            return Result.Fail<PartnersPageBannerLocalizationDto>(ErrorMessagesConstants.FailedToUpdateEntity(typeof(PartnersPageBannerLocalization)));
        }
        catch (ValidationException vex)
        {
            return Result.Fail<PartnersPageBannerLocalizationDto>(vex.Errors.Select(e => e.ErrorMessage));
        }
        catch (DbUpdateException)
        {
            return Result.Fail<PartnersPageBannerLocalizationDto>(ErrorMessagesConstants.FailedToUpdateEntityInDatabase(typeof(PartnersPageBannerLocalization)));
        }
    }
}
