using AutoMapper;
using FluentResults;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.Localization.MainPage;
using VictoryCenter.BLL.Interfaces.Localization;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Entities.Localization;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;
using MainPageEntity = VictoryCenter.DAL.Entities.MainPage;

namespace VictoryCenter.BLL.Commands.Admin.Localization.MainPage.Update;

public class UpdateMainPageLocalizationHandler : IRequestHandler<UpdateMainPageLocalizationCommand, Result<MainPageLocalizationDto>>
{
    private readonly IMapper _mapper;
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly IValidator<UpdateMainPageLocalizationCommand> _validator;
    private readonly ILocalizationService<MainPageEntity, MainPageLocalization> _mainPageLocalizationService;
    private readonly ILocalizationService<MainAboutUs, MainAboutUsLocalization> _mainAboutUsLocalizationService;
    private readonly ILocalizationService<MainPartners, MainPartnersLocalization> _mainPartnersLocalizationService;

    public UpdateMainPageLocalizationHandler(
        IMapper mapper,
        IRepositoryWrapper repositoryWrapper,
        IValidator<UpdateMainPageLocalizationCommand> validator,
        ILocalizationService<MainPageEntity, MainPageLocalization> mainPageLocalizationService,
        ILocalizationService<MainAboutUs, MainAboutUsLocalization> mainAboutUsLocalizationService,
        ILocalizationService<MainPartners, MainPartnersLocalization> mainPartnersLocalizationService)
    {
        _mapper = mapper;
        _repositoryWrapper = repositoryWrapper;
        _validator = validator;
        _mainPageLocalizationService = mainPageLocalizationService;
        _mainAboutUsLocalizationService = mainAboutUsLocalizationService;
        _mainPartnersLocalizationService = mainPartnersLocalizationService;
    }

    public async Task<Result<MainPageLocalizationDto>> Handle(UpdateMainPageLocalizationCommand request, CancellationToken cancellationToken)
    {
        try
        {
            await _validator.ValidateAndThrowAsync(request, cancellationToken);

            using var transaction = _repositoryWrapper.BeginTransaction();

            var mainPage = await _repositoryWrapper.MainPageRepository
                .GetFirstOrDefaultAsync(new QueryOptions<MainPageEntity>
                {
                    Filter = e => e.Id == request.EntityId,
                    Include = query => query
                        .Include(e => e.MainAboutUs)
                        .Include(e => e.MainPartners)
                });

            if (mainPage is null)
            {
                return Result.Fail<MainPageLocalizationDto>(ErrorMessagesConstants.NotFound());
            }

            var dto = request.Dto;

            MainPageLocalization mainPageLocalizationEntity = _mapper.Map<MainPageLocalization>(dto);
            mainPageLocalizationEntity.EntityId = request.EntityId;
            mainPageLocalizationEntity.LanguageId = request.LanguageId;
            var updatedMainPageLocalization = await _mainPageLocalizationService.UpdateEntityLocalizationAsync(mainPageLocalizationEntity);

            MainAboutUsLocalizationDto? mainAboutUsDto = null;
            if (dto.MainAboutUs is not null)
            {
                if (mainPage.MainAboutUs is null)
                {
                    return Result.Fail<MainPageLocalizationDto>(ErrorMessagesConstants.NotFound());
                }

                var mainAboutUsLocalizationEntity = _mapper.Map<MainAboutUsLocalization>(dto.MainAboutUs);
                mainAboutUsLocalizationEntity.EntityId = mainPage.MainAboutUs.Id;
                mainAboutUsLocalizationEntity.LanguageId = request.LanguageId;
                var updatedAboutUs = await _mainAboutUsLocalizationService.UpdateEntityLocalizationAsync(mainAboutUsLocalizationEntity);
                mainAboutUsDto = _mapper.Map<MainAboutUsLocalizationDto>(updatedAboutUs);
            }

            MainPartnersLocalizationDto? mainPartnersDto = null;
            if (dto.MainPartners is not null)
            {
                if (mainPage.MainPartners is null)
                {
                    return Result.Fail<MainPageLocalizationDto>(ErrorMessagesConstants.NotFound());
                }

                var mainPartnersLocalizationEntity = _mapper.Map<MainPartnersLocalization>(dto.MainPartners);
                mainPartnersLocalizationEntity.EntityId = mainPage.MainPartners.Id;
                mainPartnersLocalizationEntity.LanguageId = request.LanguageId;
                var updatedPartners = await _mainPartnersLocalizationService.UpdateEntityLocalizationAsync(mainPartnersLocalizationEntity);
                mainPartnersDto = _mapper.Map<MainPartnersLocalizationDto>(updatedPartners);
            }

            transaction.Complete();

            var response = _mapper.Map<MainPageLocalizationDto>(updatedMainPageLocalization) with
            {
                MainAboutUs = mainAboutUsDto,
                MainPartners = mainPartnersDto
            };

            return Result.Ok(response);
        }
        catch (KeyNotFoundException knfex)
        {
            return Result.Fail<MainPageLocalizationDto>(knfex.Message);
        }
        catch (InvalidOperationException)
        {
            return Result.Fail<MainPageLocalizationDto>(ErrorMessagesConstants.FailedToUpdateEntity(typeof(MainPageLocalization)));
        }
        catch (ValidationException vex)
        {
            return Result.Fail<MainPageLocalizationDto>(vex.Errors.Select(e => e.ErrorMessage));
        }
        catch (DbUpdateException)
        {
            return Result.Fail<MainPageLocalizationDto>(ErrorMessagesConstants.FailedToUpdateEntityInDatabase(typeof(MainPageLocalization)));
        }
    }
}
