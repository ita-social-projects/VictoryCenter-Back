using AutoMapper;
using FluentResults;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.Localization.MainPage;
using VictoryCenter.BLL.Interfaces.Localization;
using VictoryCenter.DAL.Data.BaseEntity;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Entities.Interfaces;
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
    private readonly ILocalizationService<MainDonations, MainDonationsLocalization> _mainDonationsLocalizationService;

    public UpdateMainPageLocalizationHandler(
        IMapper mapper,
        IRepositoryWrapper repositoryWrapper,
        IValidator<UpdateMainPageLocalizationCommand> validator,
        ILocalizationService<MainPageEntity, MainPageLocalization> mainPageLocalizationService,
        ILocalizationService<MainAboutUs, MainAboutUsLocalization> mainAboutUsLocalizationService,
        ILocalizationService<MainPartners, MainPartnersLocalization> mainPartnersLocalizationService,
        ILocalizationService<MainDonations, MainDonationsLocalization> mainDonationsLocalizationService)
    {
        _mapper = mapper;
        _repositoryWrapper = repositoryWrapper;
        _validator = validator;
        _mainPageLocalizationService = mainPageLocalizationService;
        _mainAboutUsLocalizationService = mainAboutUsLocalizationService;
        _mainPartnersLocalizationService = mainPartnersLocalizationService;
        _mainDonationsLocalizationService = mainDonationsLocalizationService;
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
                        .Include(e => e.MainDonations)
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

            if (dto.MainAboutUs is not null && mainPage.MainAboutUs is null)
            {
                return Result.Fail<MainPageLocalizationDto>(ErrorMessagesConstants.NotFound());
            }

            if (dto.MainPartners is not null && mainPage.MainPartners is null)
            {
                return Result.Fail<MainPageLocalizationDto>(ErrorMessagesConstants.NotFound());
            }

            if (dto.MainDonations is not null && mainPage.MainDonations is null)
            {
                return Result.Fail<MainPageLocalizationDto>(ErrorMessagesConstants.NotFound());
            }

            MainAboutUsLocalizationDto? mainAboutUsDto = null;
            if (dto.MainAboutUs is not null)
            {
                var mainAboutUsLocalizationEntity = _mapper.Map<MainAboutUsLocalization>(dto.MainAboutUs);
                mainAboutUsLocalizationEntity.EntityId = mainPage.MainAboutUs!.Id;
                mainAboutUsLocalizationEntity.LanguageId = request.LanguageId;
                var updatedAboutUs = await UpsertLocalizationAsync(mainAboutUsLocalizationEntity, _mainAboutUsLocalizationService);
                mainAboutUsDto = _mapper.Map<MainAboutUsLocalizationDto>(updatedAboutUs);
            }

            MainPartnersLocalizationDto? mainPartnersDto = null;
            if (dto.MainPartners is not null)
            {
                var mainPartnersLocalizationEntity = _mapper.Map<MainPartnersLocalization>(dto.MainPartners);
                mainPartnersLocalizationEntity.EntityId = mainPage.MainPartners!.Id;
                mainPartnersLocalizationEntity.LanguageId = request.LanguageId;
                var updatedPartners = await UpsertLocalizationAsync(mainPartnersLocalizationEntity, _mainPartnersLocalizationService);
                mainPartnersDto = _mapper.Map<MainPartnersLocalizationDto>(updatedPartners);
            }

            MainDonationsLocalizationDto? mainDonationsDto = null;
            if (dto.MainDonations is not null)
            {
                var mainDonationsLocalizationEntity = _mapper.Map<MainDonationsLocalization>(dto.MainDonations);
                mainDonationsLocalizationEntity.EntityId = mainPage.MainDonations!.Id;
                mainDonationsLocalizationEntity.LanguageId = request.LanguageId;
                var updatedDonations = await UpsertLocalizationAsync(mainDonationsLocalizationEntity, _mainDonationsLocalizationService);
                mainDonationsDto = _mapper.Map<MainDonationsLocalizationDto>(updatedDonations);
            }

            transaction.Complete();

            var response = _mapper.Map<MainPageLocalizationDto>(updatedMainPageLocalization) with
            {
                MainAboutUs = mainAboutUsDto,
                MainPartners = mainPartnersDto,
                MainDonations = mainDonationsDto
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

    private async Task<TLocalization> UpsertLocalizationAsync<TEntity, TLocalization>(
        TLocalization entity,
        ILocalizationService<TEntity, TLocalization> service)
        where TEntity : class, ITranslatedEntity<TLocalization>, IEntity
        where TLocalization : LocalizationBase<TEntity>
    {
        var existing = await _repositoryWrapper.GetRepository<TLocalization>()
            .GetFirstOrDefaultAsync(new QueryOptions<TLocalization>
            {
                Filter = l => l.EntityId == entity.EntityId && l.LanguageId == entity.LanguageId,
                AsNoTracking = true
            });

        return existing is null
            ? await service.CreateEntityLocalizationAsync(entity)
            : await service.UpdateEntityLocalizationAsync(entity);
    }
}
