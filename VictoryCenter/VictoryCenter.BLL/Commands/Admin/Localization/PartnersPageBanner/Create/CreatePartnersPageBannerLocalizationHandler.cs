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

namespace VictoryCenter.BLL.Commands.Admin.Localization.PartnersPageBanner.Create;

public class CreatePartnersPageBannerLocalizationHandler
    : IRequestHandler<CreatePartnersPageBannerLocalizationCommand, Result<PartnersPageBannerLocalizationDto>>
{
    private readonly IMapper _mapper;
    private readonly IValidator<CreatePartnersPageBannerLocalizationCommand> _validator;
    private readonly ILocalizationService<PartnersPageBannerEntity, PartnersPageBannerLocalization> _localizationService;

    public CreatePartnersPageBannerLocalizationHandler(
        IMapper mapper,
        IValidator<CreatePartnersPageBannerLocalizationCommand> validator,
        ILocalizationService<PartnersPageBannerEntity, PartnersPageBannerLocalization> localizationService)
    {
        _mapper = mapper;
        _validator = validator;
        _localizationService = localizationService;
    }

    public async Task<Result<PartnersPageBannerLocalizationDto>> Handle(CreatePartnersPageBannerLocalizationCommand request, CancellationToken cancellationToken)
    {
        try
        {
            await _validator.ValidateAndThrowAsync(request, cancellationToken);
            var entity = _mapper.Map<PartnersPageBannerLocalization>(request.CreatePartnersPageBannerLocalizationDto);
            var result = await _localizationService.CreateEntityLocalizationAsync(entity);
            var responseDto = _mapper.Map<PartnersPageBannerLocalizationDto>(result);
            return Result.Ok(responseDto);
        }
        catch (KeyNotFoundException knfex)
        {
            return Result.Fail<PartnersPageBannerLocalizationDto>(knfex.Message);
        }
        catch (InvalidOperationException)
        {
            return Result.Fail<PartnersPageBannerLocalizationDto>(ErrorMessagesConstants.FailedToCreateEntity(typeof(PartnersPageBannerLocalization)));
        }
        catch (ValidationException vex)
        {
            return Result.Fail<PartnersPageBannerLocalizationDto>(vex.Errors.Select(e => e.ErrorMessage));
        }
        catch (DbUpdateException)
        {
            return Result.Fail<PartnersPageBannerLocalizationDto>(ErrorMessagesConstants.FailedToCreateEntityInDatabase(typeof(PartnersPageBannerLocalization)));
        }
    }
}
