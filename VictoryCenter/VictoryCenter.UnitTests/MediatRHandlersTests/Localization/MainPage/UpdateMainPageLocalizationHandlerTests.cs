using System.Transactions;
using AutoMapper;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;
using Moq;
using VictoryCenter.BLL.Commands.Admin.Localization.MainPage.Update;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.Localization.MainPage;
using VictoryCenter.BLL.DTOs.Common;
using VictoryCenter.BLL.Interfaces.Localization;
using VictoryCenter.BLL.Services.MainPage;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Entities.Localization;
using VictoryCenter.DAL.Enums;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;
using MainPageEntity = VictoryCenter.DAL.Entities.MainPage;

namespace VictoryCenter.UnitTests.MediatRHandlersTests.Localization.MainPage;

public class UpdateMainPageLocalizationHandlerTests
{
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<IRepositoryWrapper> _mockRepositoryWrapper;
    private readonly Mock<ILocalizationService<MainPageEntity, MainPageLocalization>> _mockMainPageService;
    private readonly Mock<ILocalizationService<MainAboutUs, MainAboutUsLocalization>> _mockMainAboutUsService;
    private readonly Mock<ILocalizationService<MainPartners, MainPartnersLocalization>> _mockMainPartnersService;
    private readonly Mock<ILocalizationService<MainDonations, MainDonationsLocalization>> _mockMainDonationsService;
    private readonly Mock<IValidator<UpdateMainPageLocalizationCommand>> _mockValidator;

    private readonly long _entityId = 1;
    private readonly long _languageId = 2;

    private readonly MainPageEntity _mainPageEntity = new()
    {
        Id = 1,
        MainAboutUs = new MainAboutUs { Id = 101 },
        MainPartners = new MainPartners { Id = 102 },
        MainDonations = new MainDonations { Id = 103 }
    };

    private readonly MainPageLocalization _mainPageLocalization = new()
    {
        EntityId = 1,
        LanguageId = 2,
        Title = "Upd Valid title",
        Description = "Upd Valid description here",
        TranslationStatus = TranslationStatus.Relevant
    };

    private readonly MainAboutUsLocalization _mainAboutUsLocalization = new()
    {
        EntityId = 101,
        LanguageId = 2,
        Title = "Upd About Us title",
        TranslationStatus = TranslationStatus.Relevant
    };

    private readonly MainPartnersLocalization _mainPartnersLocalization = new()
    {
        EntityId = 102,
        LanguageId = 2,
        Title = "Upd Partners title",
        TranslationStatus = TranslationStatus.Relevant
    };

    private readonly MainDonationsLocalization _mainDonationsLocalization = new()
    {
        EntityId = 103,
        LanguageId = 2,
        Title = "Upd Donations title",
        TranslationStatus = TranslationStatus.Relevant
    };

    private readonly MainPageLocalizationDto _mainPageLocalizationDto = new()
    {
        EntityId = 1,
        LocalizationInfoDto = new LocalizationInfoDto { Id = 2, Code = "en" },
        Title = "Upd Valid title",
        Description = "Upd Valid description here",
        TranslationStatus = TranslationStatus.Relevant
    };

    private readonly MainAboutUsLocalizationDto _mainAboutUsLocalizationDto = new()
    {
        EntityId = 101,
        LocalizationInfoDto = new LocalizationInfoDto { Id = 2, Code = "en" },
        Title = "Upd About Us title",
        TranslationStatus = TranslationStatus.Relevant
    };

    private readonly MainPartnersLocalizationDto _mainPartnersLocalizationDto = new()
    {
        EntityId = 102,
        LocalizationInfoDto = new LocalizationInfoDto { Id = 2, Code = "en" },
        Title = "Upd Partners title",
        TranslationStatus = TranslationStatus.Relevant
    };

    private readonly MainDonationsLocalizationDto _mainDonationsLocalizationDto = new()
    {
        EntityId = 103,
        LocalizationInfoDto = new LocalizationInfoDto { Id = 2, Code = "en" },
        Title = "Upd Donations title",
        TranslationStatus = TranslationStatus.Relevant
    };

    public UpdateMainPageLocalizationHandlerTests()
    {
        _mockMapper = new Mock<IMapper>();
        _mockRepositoryWrapper = new Mock<IRepositoryWrapper>();
        _mockMainPageService = new Mock<ILocalizationService<MainPageEntity, MainPageLocalization>>();
        _mockMainAboutUsService = new Mock<ILocalizationService<MainAboutUs, MainAboutUsLocalization>>();
        _mockMainPartnersService = new Mock<ILocalizationService<MainPartners, MainPartnersLocalization>>();
        _mockMainDonationsService = new Mock<ILocalizationService<MainDonations, MainDonationsLocalization>>();
        _mockValidator = new Mock<IValidator<UpdateMainPageLocalizationCommand>>();

        _mockRepositoryWrapper
            .Setup(r => r.BeginTransaction())
            .Returns(new TransactionScope(TransactionScopeAsyncFlowOption.Enabled));

        _mockValidator
            .Setup(v => v.ValidateAsync(It.IsAny<IValidationContext>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        _mockRepositoryWrapper
            .Setup(r => r.MainPageRepository.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<MainPageEntity>>()))
            .ReturnsAsync(_mainPageEntity);
    }

    [Fact]
    public async Task Handle_ShouldSucceed_WhenAllSectionsProvided()
    {
        var command = BuildCommand(GetValidDto());
        SetupDependencies(true);

        var result = await CreateHandler().Handle(command, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(_mainPageLocalizationDto.EntityId, result.Value.EntityId);
        Assert.NotNull(result.Value.MainAboutUs);
        Assert.NotNull(result.Value.MainPartners);
        Assert.NotNull(result.Value.MainDonations);
    }

    [Fact]
    public async Task Handle_ShouldSucceed_WhenSubSectionsAreNull()
    {
        var dto = new UpdateMainPageLocalizationDto
        {
            MainAboutUs = null,
            MainPartners = null,
            MainDonations = null
        };
        var command = BuildCommand(dto);

        _mockMapper.Setup(m => m.Map<MainPageLocalization>(dto)).Returns(_mainPageLocalization);
        _mockMainPageService.Setup(s => s.UpdateEntityLocalizationAsync(_mainPageLocalization))
            .ReturnsAsync(_mainPageLocalization);
        _mockMapper.Setup(m => m.Map<MainPageLocalizationDto>(_mainPageLocalization))
            .Returns(_mainPageLocalizationDto);

        var result = await CreateHandler().Handle(command, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Null(result.Value.MainAboutUs);
        Assert.Null(result.Value.MainPartners);
        Assert.Null(result.Value.MainDonations);

        _mockMainAboutUsService.Verify(s => s.UpdateEntityLocalizationAsync(It.IsAny<MainAboutUsLocalization>()), Times.Never);
        _mockMainPartnersService.Verify(s => s.UpdateEntityLocalizationAsync(It.IsAny<MainPartnersLocalization>()), Times.Never);
        _mockMainDonationsService.Verify(s => s.UpdateEntityLocalizationAsync(It.IsAny<MainDonationsLocalization>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldPropagateLanguageAndEntityIds_Correctly()
    {
        var dto = GetValidDto();
        var command = BuildCommand(dto);
        SetupDependencies(true);

        var capturedMainPage = new MainPageLocalization();
        var capturedAboutUs = new MainAboutUsLocalization();
        var capturedPartners = new MainPartnersLocalization();
        var capturedDonations = new MainDonationsLocalization();

        _mockMapper.Setup(m => m.Map<MainPageLocalization>(dto)).Returns(capturedMainPage);
        _mockMapper.Setup(m => m.Map<MainAboutUsLocalization>(dto.MainAboutUs)).Returns(capturedAboutUs);
        _mockMapper.Setup(m => m.Map<MainPartnersLocalization>(dto.MainPartners)).Returns(capturedPartners);
        _mockMapper.Setup(m => m.Map<MainDonationsLocalization>(dto.MainDonations)).Returns(capturedDonations);

        _mockMainPageService.Setup(s => s.UpdateEntityLocalizationAsync(capturedMainPage)).ReturnsAsync(capturedMainPage);
        _mockMainAboutUsService.Setup(s => s.UpdateEntityLocalizationAsync(capturedAboutUs)).ReturnsAsync(capturedAboutUs);
        _mockMainPartnersService.Setup(s => s.UpdateEntityLocalizationAsync(capturedPartners)).ReturnsAsync(capturedPartners);
        _mockMainDonationsService.Setup(s => s.UpdateEntityLocalizationAsync(capturedDonations)).ReturnsAsync(capturedDonations);

        _mockMapper.Setup(m => m.Map<MainPageLocalizationDto>(It.IsAny<MainPageLocalization>())).Returns(_mainPageLocalizationDto);

        await CreateHandler().Handle(command, CancellationToken.None);

        Assert.Equal(_entityId, capturedMainPage.EntityId);
        Assert.Equal(_languageId, capturedMainPage.LanguageId);

        Assert.Equal(_mainPageEntity.MainAboutUs.Id, capturedAboutUs.EntityId);
        Assert.Equal(_languageId, capturedAboutUs.LanguageId);

        Assert.Equal(_mainPageEntity.MainPartners.Id, capturedPartners.EntityId);
        Assert.Equal(_languageId, capturedPartners.LanguageId);

        Assert.Equal(_mainPageEntity.MainDonations.Id, capturedDonations.EntityId);
        Assert.Equal(_languageId, capturedDonations.LanguageId);
    }

    [Fact]
    public async Task Handle_ShouldCreateLocalizations_WhenTheyDoNotExist()
    {
        var command = BuildCommand(GetValidDto());
        SetupDependencies(false);

        _mockMainAboutUsService.Setup(s => s.CreateEntityLocalizationAsync(It.IsAny<MainAboutUsLocalization>()))
            .ReturnsAsync(_mainAboutUsLocalization);
        _mockMainPartnersService.Setup(s => s.CreateEntityLocalizationAsync(It.IsAny<MainPartnersLocalization>()))
            .ReturnsAsync(_mainPartnersLocalization);
        _mockMainDonationsService.Setup(s => s.CreateEntityLocalizationAsync(It.IsAny<MainDonationsLocalization>()))
            .ReturnsAsync(_mainDonationsLocalization);

        var result = await CreateHandler().Handle(command, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(_mainPageLocalizationDto.EntityId, result.Value.EntityId);

        Assert.NotNull(result.Value.MainAboutUs);
        Assert.Equal(_mainAboutUsLocalizationDto.EntityId, result.Value.MainAboutUs.EntityId);

        Assert.NotNull(result.Value.MainPartners);
        Assert.Equal(_mainPartnersLocalizationDto.EntityId, result.Value.MainPartners.EntityId);

        Assert.NotNull(result.Value.MainDonations);
        Assert.Equal(_mainDonationsLocalizationDto.EntityId, result.Value.MainDonations.EntityId);

        _mockMainAboutUsService.Verify(s => s.CreateEntityLocalizationAsync(It.IsAny<MainAboutUsLocalization>()), Times.Once);
        _mockMainPartnersService.Verify(s => s.CreateEntityLocalizationAsync(It.IsAny<MainPartnersLocalization>()), Times.Once);
        _mockMainDonationsService.Verify(s => s.CreateEntityLocalizationAsync(It.IsAny<MainDonationsLocalization>()), Times.Once);

        _mockMainAboutUsService.Verify(s => s.UpdateEntityLocalizationAsync(It.IsAny<MainAboutUsLocalization>()), Times.Never);
        _mockMainPartnersService.Verify(s => s.UpdateEntityLocalizationAsync(It.IsAny<MainPartnersLocalization>()), Times.Never);
        _mockMainDonationsService.Verify(s => s.UpdateEntityLocalizationAsync(It.IsAny<MainDonationsLocalization>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenMainPageAggregateNotFound()
    {
        _mockRepositoryWrapper
            .Setup(r => r.MainPageRepository.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<MainPageEntity>>()))
            .ReturnsAsync((MainPageEntity?)null);

        var command = BuildCommand(GetValidDto());
        var result = await CreateHandler().Handle(command, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessagesConstants.NotFound(), result.Errors[0].Message);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenMainAboutUsIsNullInDb_ButProvidedInDto()
    {
        SetupDependencies(true);
        var invalidMainPage = new MainPageEntity
        {
            Id = 1,
            MainAboutUs = null,
            MainPartners = new MainPartners(),
            MainDonations = new MainDonations()
        };
        _mockRepositoryWrapper
            .Setup(r => r.MainPageRepository.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<MainPageEntity>>()))
            .ReturnsAsync(invalidMainPage);

        var command = BuildCommand(GetValidDto());
        var result = await CreateHandler().Handle(command, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessagesConstants.NotFound(), result.Errors[0].Message);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenMainPartnersIsNullInDb_ButProvidedInDto()
    {
        SetupDependencies(true);
        var invalidMainPage = new MainPageEntity
        {
            Id = 1,
            MainAboutUs = new MainAboutUs(),
            MainPartners = null,
            MainDonations = new MainDonations()
        };
        _mockRepositoryWrapper
            .Setup(r => r.MainPageRepository.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<MainPageEntity>>()))
            .ReturnsAsync(invalidMainPage);

        var command = BuildCommand(GetValidDto());
        var result = await CreateHandler().Handle(command, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessagesConstants.NotFound(), result.Errors[0].Message);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenMainDonationsIsNullInDb_ButProvidedInDto()
    {
        SetupDependencies(true);
        var invalidMainPage = new MainPageEntity
        {
            Id = 1,
            MainAboutUs = new MainAboutUs(),
            MainPartners = new MainPartners(),
            MainDonations = null
        };
        _mockRepositoryWrapper
            .Setup(r => r.MainPageRepository.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<MainPageEntity>>()))
            .ReturnsAsync(invalidMainPage);

        var command = BuildCommand(GetValidDto());
        var result = await CreateHandler().Handle(command, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessagesConstants.NotFound(), result.Errors[0].Message);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenValidationFails()
    {
        _mockValidator
            .Setup(v => v.ValidateAsync(It.IsAny<IValidationContext>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new ValidationException(new[] { new ValidationFailure("Title", "Error") }));

        var command = BuildCommand(GetValidDto());
        var result = await CreateHandler().Handle(command, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Contains("Error", result.Errors.Select(e => e.Message));
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenKeyNotFoundExceptionThrown()
    {
        const string errorMessage = "Not found exception";
        var command = BuildCommand(GetValidDto());

        SetupDependencies(true);
        _mockMainPageService.Setup(s => s.UpdateEntityLocalizationAsync(It.IsAny<MainPageLocalization>()))
            .ThrowsAsync(new KeyNotFoundException(errorMessage));

        var result = await CreateHandler().Handle(command, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(errorMessage, result.Errors[0].Message);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenInvalidOperationExceptionThrown()
    {
        var command = BuildCommand(GetValidDto());

        SetupDependencies(true);
        _mockMainPageService.Setup(s => s.UpdateEntityLocalizationAsync(It.IsAny<MainPageLocalization>()))
            .ThrowsAsync(new InvalidOperationException());

        var result = await CreateHandler().Handle(command, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessagesConstants.FailedToUpdateEntity(typeof(MainPageLocalization)), result.Errors[0].Message);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenDbUpdateExceptionThrown()
    {
        var command = BuildCommand(GetValidDto());

        SetupDependencies(true);
        _mockMainPageService.Setup(s => s.UpdateEntityLocalizationAsync(It.IsAny<MainPageLocalization>()))
            .ThrowsAsync(new DbUpdateException());

        var result = await CreateHandler().Handle(command, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessagesConstants.FailedToUpdateEntityInDatabase(typeof(MainPageLocalization)), result.Errors[0].Message);
    }

    private UpdateMainPageLocalizationHandler CreateHandler()
    {
        var blocksUpdater = new MainPageBlocksLocalizationUpdater(
            _mockMapper.Object,
            _mockRepositoryWrapper.Object,
            _mockMainAboutUsService.Object,
            _mockMainPartnersService.Object,
            _mockMainDonationsService.Object);

        return new(_mockMapper.Object, _mockRepositoryWrapper.Object, _mockValidator.Object, _mockMainPageService.Object, blocksUpdater);
    }

    private void SetupDependencies(bool localizationsExist)
    {
        _mockMapper.Setup(m => m.Map<MainPageLocalization>(It.IsAny<UpdateMainPageLocalizationDto>()))
            .Returns(_mainPageLocalization);
        _mockMapper.Setup(m => m.Map<MainAboutUsLocalization>(It.IsAny<UpdateMainAboutUsLocalizationDto>()))
            .Returns(_mainAboutUsLocalization);
        _mockMapper.Setup(m => m.Map<MainPartnersLocalization>(It.IsAny<UpdateMainPartnersLocalizationDto>()))
            .Returns(_mainPartnersLocalization);
        _mockMapper.Setup(m => m.Map<MainDonationsLocalization>(It.IsAny<UpdateMainDonationsLocalizationDto>()))
            .Returns(_mainDonationsLocalization);

        _mockMainPageService.Setup(s => s.UpdateEntityLocalizationAsync(It.IsAny<MainPageLocalization>()))
            .ReturnsAsync(_mainPageLocalization);
        _mockMainAboutUsService.Setup(s => s.UpdateEntityLocalizationAsync(It.IsAny<MainAboutUsLocalization>()))
            .ReturnsAsync(_mainAboutUsLocalization);
        _mockMainPartnersService.Setup(s => s.UpdateEntityLocalizationAsync(It.IsAny<MainPartnersLocalization>()))
            .ReturnsAsync(_mainPartnersLocalization);
        _mockMainDonationsService.Setup(s => s.UpdateEntityLocalizationAsync(It.IsAny<MainDonationsLocalization>()))
            .ReturnsAsync(_mainDonationsLocalization);

        var mockAboutUsRepo = new Mock<IRepositoryBase<MainAboutUsLocalization>>();
        mockAboutUsRepo.Setup(r => r.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<MainAboutUsLocalization>>()))
            .ReturnsAsync(localizationsExist ? _mainAboutUsLocalization : null);
        _mockRepositoryWrapper.Setup(r => r.GetRepository<MainAboutUsLocalization>()).Returns(mockAboutUsRepo.Object);

        var mockPartnersRepo = new Mock<IRepositoryBase<MainPartnersLocalization>>();
        mockPartnersRepo.Setup(r => r.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<MainPartnersLocalization>>()))
            .ReturnsAsync(localizationsExist ? _mainPartnersLocalization : null);
        _mockRepositoryWrapper.Setup(r => r.GetRepository<MainPartnersLocalization>()).Returns(mockPartnersRepo.Object);

        var mockDonationsRepo = new Mock<IRepositoryBase<MainDonationsLocalization>>();
        mockDonationsRepo.Setup(r => r.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<MainDonationsLocalization>>()))
            .ReturnsAsync(localizationsExist ? _mainDonationsLocalization : null);
        _mockRepositoryWrapper.Setup(r => r.GetRepository<MainDonationsLocalization>()).Returns(mockDonationsRepo.Object);

        _mockMapper.Setup(m => m.Map<MainPageLocalizationDto>(It.IsAny<MainPageLocalization>()))
            .Returns(_mainPageLocalizationDto);
        _mockMapper.Setup(m => m.Map<MainAboutUsLocalizationDto>(It.IsAny<MainAboutUsLocalization>()))
            .Returns(_mainAboutUsLocalizationDto);
        _mockMapper.Setup(m => m.Map<MainPartnersLocalizationDto>(It.IsAny<MainPartnersLocalization>()))
            .Returns(_mainPartnersLocalizationDto);
        _mockMapper.Setup(m => m.Map<MainDonationsLocalizationDto>(It.IsAny<MainDonationsLocalization>()))
            .Returns(_mainDonationsLocalizationDto);
    }

    private static UpdateMainPageLocalizationDto GetValidDto() => new()
    {
        MainAboutUs = new UpdateMainAboutUsLocalizationDto(),
        MainPartners = new UpdateMainPartnersLocalizationDto(),
        MainDonations = new UpdateMainDonationsLocalizationDto()
    };

    private UpdateMainPageLocalizationCommand BuildCommand(UpdateMainPageLocalizationDto dto) =>
        new(dto, _entityId, _languageId);
}
