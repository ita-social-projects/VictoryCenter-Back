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
using MainPageEntity = VictoryCenter.DAL.Entities.MainPage;

namespace VictoryCenter.BLL.Commands.Admin.Localization.MainPage.Create;

public class CreateMainPageLocalizationHandler : IRequestHandler<CreateMainPageLocalizationCommand, Result<MainPageLocalizationDto>>
{
    private readonly IMapper _mapper;
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly IValidator<CreateMainPageLocalizationCommand> _validator;
    private readonly ILocalizationService<MainPageEntity, MainPageLocalization> _mainPageLocalizationService;
    private readonly ILocalizationService<MainAboutUs, MainAboutUsLocalization> _mainAboutUsLocalizationService;
    private readonly ILocalizationService<MainPartners, MainPartnersLocalization> _mainPartnersLocalizationService;
    private readonly ILocalizationService<MainDonations, MainDonationsLocalization> _mainDonationsLocalizationService;

    public CreateMainPageLocalizationHandler(
        IMapper mapper,
        IValidator<CreateMainPageLocalizationCommand> validator,
        IRepositoryWrapper repositoryWrapper,
        ILocalizationService<MainPageEntity, MainPageLocalization> mainPageLocalizationService,
        ILocalizationService<MainAboutUs, MainAboutUsLocalization> mainAboutUsLocalizationService,
        ILocalizationService<MainPartners, MainPartnersLocalization> mainPartnersLocalizationService,
        ILocalizationService<MainDonations, MainDonationsLocalization> mainDonationsLocalizationService)
    {
        _mapper = mapper;
        _validator = validator;
        _repositoryWrapper = repositoryWrapper;
        _mainPageLocalizationService = mainPageLocalizationService;
        _mainAboutUsLocalizationService = mainAboutUsLocalizationService;
        _mainPartnersLocalizationService = mainPartnersLocalizationService;
        _mainDonationsLocalizationService = mainDonationsLocalizationService;
    }

    public async Task<Result<MainPageLocalizationDto>> Handle(CreateMainPageLocalizationCommand request, CancellationToken cancellationToken)
    {
        try
        {
            using var transaction = _repositoryWrapper.BeginTransaction();

            await _validator.ValidateAndThrowAsync(request, cancellationToken);

            var dto = request.Dto;

            var mainPageEntity = _mapper.Map<MainPageLocalization>(dto);
            var createdMainPage = await _mainPageLocalizationService.CreateEntityLocalizationAsync(mainPageEntity);

            MainAboutUsLocalizationDto? mainAboutUsDto = null;
            if (dto.MainAboutUs is not null)
            {
                var mainAboutUsEntity = _mapper.Map<MainAboutUsLocalization>(dto.MainAboutUs);
                mainAboutUsEntity.LanguageId = dto.LanguageId;
                var createdAboutUs = await _mainAboutUsLocalizationService.CreateEntityLocalizationAsync(mainAboutUsEntity);
                mainAboutUsDto = _mapper.Map<MainAboutUsLocalizationDto>(createdAboutUs);
            }

            MainPartnersLocalizationDto? mainPartnersDto = null;
            if (dto.MainPartners is not null)
            {
                var mainPartnersEntity = _mapper.Map<MainPartnersLocalization>(dto.MainPartners);
                mainPartnersEntity.LanguageId = dto.LanguageId;
                var createdPartners = await _mainPartnersLocalizationService.CreateEntityLocalizationAsync(mainPartnersEntity);
                mainPartnersDto = _mapper.Map<MainPartnersLocalizationDto>(createdPartners);
            }

            MainDonationsLocalizationDto? mainDonationsDto = null;
            if (dto.MainDonations is not null)
            {
                var mainDonationsEntity = _mapper.Map<MainDonationsLocalization>(dto.MainDonations);
                mainDonationsEntity.LanguageId = dto.LanguageId;
                var createdDonations = await _mainDonationsLocalizationService.CreateEntityLocalizationAsync(mainDonationsEntity);
                mainDonationsDto = _mapper.Map<MainDonationsLocalizationDto>(createdDonations);
            }

            transaction.Complete();

            var response = _mapper.Map<MainPageLocalizationDto>(createdMainPage) with
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
            return Result.Fail<MainPageLocalizationDto>(ErrorMessagesConstants.FailedToCreateEntity(typeof(MainPageLocalization)));
        }
        catch (ValidationException vex)
        {
            return Result.Fail<MainPageLocalizationDto>(vex.Errors.Select(e => e.ErrorMessage));
        }
        catch (DbUpdateException)
        {
            return Result.Fail<MainPageLocalizationDto>(ErrorMessagesConstants.FailedToCreateEntityInDatabase(typeof(MainPageLocalization)));
        }
    }
}
