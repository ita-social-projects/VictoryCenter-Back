using System.Transactions;
using AutoMapper;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Moq;
using VictoryCenter.BLL.Commands.Admin.Localization.MainPage.Create;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.Localization.MainPage;
using VictoryCenter.BLL.DTOs.Common;
using VictoryCenter.BLL.Interfaces.Localization;
using VictoryCenter.BLL.Validators.Localization.MainPage;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Entities.Localization;
using VictoryCenter.DAL.Enums;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using MainPageEntity = VictoryCenter.DAL.Entities.MainPage;

namespace VictoryCenter.UnitTests.MediatRHandlersTests.Localization.MainPage;

public class CreateMainPageLocalizationHandlerTests
{
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<IRepositoryWrapper> _mockRepositoryWrapper;
    private readonly Mock<ILocalizationService<MainPageEntity, MainPageLocalization>> _mockMainPageService;
    private readonly Mock<ILocalizationService<MainAboutUs, MainAboutUsLocalization>> _mockMainAboutUsService;
    private readonly Mock<ILocalizationService<MainPartners, MainPartnersLocalization>> _mockMainPartnersService;
    private readonly Mock<ILocalizationService<MainDonations, MainDonationsLocalization>> _mockMainDonationsService;
    private readonly IValidator<CreateMainPageLocalizationCommand> _validator;

    private readonly MainPageLocalization _mainPageLocalization = new()
    {
        EntityId = 1,
        LanguageId = 1,
        Title = "Valid title",
        Description = "Valid description here",
        TranslationStatus = TranslationStatus.Relevant
    };

    private readonly MainAboutUsLocalization _mainAboutUsLocalization = new()
    {
        EntityId = 2,
        LanguageId = 1,
        Title = "About Us title",
        TranslationStatus = TranslationStatus.Relevant
    };

    private readonly MainPartnersLocalization _mainPartnersLocalization = new()
    {
        EntityId = 3,
        LanguageId = 1,
        Title = "Partners title",
        TranslationStatus = TranslationStatus.Relevant
    };

    private readonly MainDonationsLocalization _mainDonationsLocalization = new()
    {
        EntityId = 4,
        LanguageId = 1,
        Title = "Donations title",
        TranslationStatus = TranslationStatus.Relevant
    };

    private readonly MainPageLocalizationDto _mainPageLocalizationDto = new()
    {
        EntityId = 1,
        LocalizationInfoDto = new LocalizationInfoDto { Id = 1, Code = "en" },
        Title = "Valid title",
        Description = "Valid description here",
        TranslationStatus = TranslationStatus.Relevant
    };

    private readonly MainAboutUsLocalizationDto _mainAboutUsLocalizationDto = new()
    {
        EntityId = 2,
        LocalizationInfoDto = new LocalizationInfoDto { Id = 1, Code = "en" },
        Title = "About Us title",
        TranslationStatus = TranslationStatus.Relevant
    };

    private readonly MainPartnersLocalizationDto _mainPartnersLocalizationDto = new()
    {
        EntityId = 3,
        LocalizationInfoDto = new LocalizationInfoDto { Id = 1, Code = "en" },
        Title = "Partners title",
        TranslationStatus = TranslationStatus.Relevant
    };

    private readonly MainDonationsLocalizationDto _mainDonationsLocalizationDto = new()
    {
        EntityId = 4,
        LocalizationInfoDto = new LocalizationInfoDto { Id = 1, Code = "en" },
        Title = "Donations title",
        TranslationStatus = TranslationStatus.Relevant
    };

    public CreateMainPageLocalizationHandlerTests()
    {
        _mockMapper = new Mock<IMapper>();
        _mockRepositoryWrapper = new Mock<IRepositoryWrapper>();
        _mockMainPageService = new Mock<ILocalizationService<MainPageEntity, MainPageLocalization>>();
        _mockMainAboutUsService = new Mock<ILocalizationService<MainAboutUs, MainAboutUsLocalization>>();
        _mockMainPartnersService = new Mock<ILocalizationService<MainPartners, MainPartnersLocalization>>();
        _mockMainDonationsService = new Mock<ILocalizationService<MainDonations, MainDonationsLocalization>>();

        var baseValidator = new BaseMainPageLocalizationDtoValidator();
        var mainAboutUsValidator = new CreateMainAboutUsLocalizationDtoValidator(baseValidator);
        var mainPartnersValidator = new CreateMainPartnersLocalizationDtoValidator(baseValidator);
        var mainDonationsValidator = new CreateMainDonationsLocalizationDtoValidator(baseValidator);
        var dtoValidator = new CreateMainPageLocalizationDtoValidator(
            baseValidator,
            mainAboutUsValidator,
            mainPartnersValidator,
            mainDonationsValidator);
        _validator = new CreateMainPageLocalizationCommandValidator(dtoValidator);

        _mockRepositoryWrapper
            .Setup(r => r.BeginTransaction())
            .Returns(new TransactionScope(TransactionScopeAsyncFlowOption.Enabled));
    }

    // ── Happy path ──────────────────────────────────────────────────────────

    [Fact]
    public async Task Handle_ShouldSucceed_WhenAllSectionsProvided()
    {
        var command = BuildCommand(GetValidDto());
        SetupDependencies();

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
        var dto = new CreateMainPageLocalizationDto
        {
            EntityId = 1,
            LanguageId = 1,
            Title = "Valid title",
            Description = "Valid description here",
            MainAboutUs = null,
            MainPartners = null,
            MainDonations = null
        };
        var command = BuildCommand(dto);

        _mockMapper.Setup(m => m.Map<MainPageLocalization>(dto)).Returns(_mainPageLocalization);
        _mockMainPageService.Setup(s => s.CreateEntityLocalizationAsync(_mainPageLocalization))
            .ReturnsAsync(_mainPageLocalization);
        _mockMapper.Setup(m => m.Map<MainPageLocalizationDto>(_mainPageLocalization))
            .Returns(_mainPageLocalizationDto);

        var result = await CreateHandler().Handle(command, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Null(result.Value.MainAboutUs);
        Assert.Null(result.Value.MainPartners);
        Assert.Null(result.Value.MainDonations);

        _mockMainAboutUsService.Verify(s => s.CreateEntityLocalizationAsync(It.IsAny<MainAboutUsLocalization>()), Times.Never);
        _mockMainPartnersService.Verify(s => s.CreateEntityLocalizationAsync(It.IsAny<MainPartnersLocalization>()), Times.Never);
        _mockMainDonationsService.Verify(s => s.CreateEntityLocalizationAsync(It.IsAny<MainDonationsLocalization>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldNotCallMainAboutUsService_WhenMainAboutUsIsNull()
    {
        var dto = GetValidDto() with { MainAboutUs = null };
        var command = BuildCommand(dto);

        _mockMapper.Setup(m => m.Map<MainPageLocalization>(It.IsAny<CreateMainPageLocalizationDto>()))
            .Returns(_mainPageLocalization);
        _mockMapper.Setup(m => m.Map<MainPartnersLocalization>(It.IsAny<CreateMainPartnersLocalizationDto>()))
            .Returns(_mainPartnersLocalization);
        _mockMapper.Setup(m => m.Map<MainDonationsLocalization>(It.IsAny<CreateMainDonationsLocalizationDto>()))
            .Returns(_mainDonationsLocalization);
        _mockMainPageService.Setup(s => s.CreateEntityLocalizationAsync(It.IsAny<MainPageLocalization>()))
            .ReturnsAsync(_mainPageLocalization);
        _mockMainPartnersService.Setup(s => s.CreateEntityLocalizationAsync(It.IsAny<MainPartnersLocalization>()))
            .ReturnsAsync(_mainPartnersLocalization);
        _mockMainDonationsService.Setup(s => s.CreateEntityLocalizationAsync(It.IsAny<MainDonationsLocalization>()))
            .ReturnsAsync(_mainDonationsLocalization);
        _mockMapper.Setup(m => m.Map<MainPageLocalizationDto>(It.IsAny<MainPageLocalization>()))
            .Returns(_mainPageLocalizationDto);
        _mockMapper.Setup(m => m.Map<MainPartnersLocalizationDto>(It.IsAny<MainPartnersLocalization>()))
            .Returns(_mainPartnersLocalizationDto);
        _mockMapper.Setup(m => m.Map<MainDonationsLocalizationDto>(It.IsAny<MainDonationsLocalization>()))
            .Returns(_mainDonationsLocalizationDto);

        await CreateHandler().Handle(command, CancellationToken.None);

        _mockMainAboutUsService.Verify(s => s.CreateEntityLocalizationAsync(It.IsAny<MainAboutUsLocalization>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldNotCallMainPartnersService_WhenMainPartnersIsNull()
    {
        var dto = GetValidDto() with { MainPartners = null };
        var command = BuildCommand(dto);

        _mockMapper.Setup(m => m.Map<MainPageLocalization>(It.IsAny<CreateMainPageLocalizationDto>()))
            .Returns(_mainPageLocalization);
        _mockMapper.Setup(m => m.Map<MainAboutUsLocalization>(It.IsAny<CreateMainAboutUsLocalizationDto>()))
            .Returns(_mainAboutUsLocalization);
        _mockMapper.Setup(m => m.Map<MainDonationsLocalization>(It.IsAny<CreateMainDonationsLocalizationDto>()))
            .Returns(_mainDonationsLocalization);
        _mockMainPageService.Setup(s => s.CreateEntityLocalizationAsync(It.IsAny<MainPageLocalization>()))
            .ReturnsAsync(_mainPageLocalization);
        _mockMainAboutUsService.Setup(s => s.CreateEntityLocalizationAsync(It.IsAny<MainAboutUsLocalization>()))
            .ReturnsAsync(_mainAboutUsLocalization);
        _mockMainDonationsService.Setup(s => s.CreateEntityLocalizationAsync(It.IsAny<MainDonationsLocalization>()))
            .ReturnsAsync(_mainDonationsLocalization);
        _mockMapper.Setup(m => m.Map<MainPageLocalizationDto>(It.IsAny<MainPageLocalization>()))
            .Returns(_mainPageLocalizationDto);
        _mockMapper.Setup(m => m.Map<MainAboutUsLocalizationDto>(It.IsAny<MainAboutUsLocalization>()))
            .Returns(_mainAboutUsLocalizationDto);
        _mockMapper.Setup(m => m.Map<MainDonationsLocalizationDto>(It.IsAny<MainDonationsLocalization>()))
            .Returns(_mainDonationsLocalizationDto);

        await CreateHandler().Handle(command, CancellationToken.None);

        _mockMainPartnersService.Verify(s => s.CreateEntityLocalizationAsync(It.IsAny<MainPartnersLocalization>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldNotCallMainDonationsService_WhenMainDonationsIsNull()
    {
        var dto = GetValidDto() with { MainDonations = null };
        var command = BuildCommand(dto);

        _mockMapper.Setup(m => m.Map<MainPageLocalization>(It.IsAny<CreateMainPageLocalizationDto>()))
            .Returns(_mainPageLocalization);
        _mockMapper.Setup(m => m.Map<MainAboutUsLocalization>(It.IsAny<CreateMainAboutUsLocalizationDto>()))
            .Returns(_mainAboutUsLocalization);
        _mockMapper.Setup(m => m.Map<MainPartnersLocalization>(It.IsAny<CreateMainPartnersLocalizationDto>()))
            .Returns(_mainPartnersLocalization);
        _mockMainPageService.Setup(s => s.CreateEntityLocalizationAsync(It.IsAny<MainPageLocalization>()))
            .ReturnsAsync(_mainPageLocalization);
        _mockMainAboutUsService.Setup(s => s.CreateEntityLocalizationAsync(It.IsAny<MainAboutUsLocalization>()))
            .ReturnsAsync(_mainAboutUsLocalization);
        _mockMainPartnersService.Setup(s => s.CreateEntityLocalizationAsync(It.IsAny<MainPartnersLocalization>()))
            .ReturnsAsync(_mainPartnersLocalization);
        _mockMapper.Setup(m => m.Map<MainPageLocalizationDto>(It.IsAny<MainPageLocalization>()))
            .Returns(_mainPageLocalizationDto);
        _mockMapper.Setup(m => m.Map<MainAboutUsLocalizationDto>(It.IsAny<MainAboutUsLocalization>()))
            .Returns(_mainAboutUsLocalizationDto);
        _mockMapper.Setup(m => m.Map<MainPartnersLocalizationDto>(It.IsAny<MainPartnersLocalization>()))
            .Returns(_mainPartnersLocalizationDto);

        await CreateHandler().Handle(command, CancellationToken.None);

        _mockMainDonationsService.Verify(s => s.CreateEntityLocalizationAsync(It.IsAny<MainDonationsLocalization>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldPropagateLanguageId_ToSubEntities()
    {
        const long languageId = 5;
        var dto = GetValidDto() with { LanguageId = languageId };
        var command = BuildCommand(dto);

        var capturedAboutUs = new MainAboutUsLocalization { EntityId = 2, LanguageId = 0 };
        var capturedPartners = new MainPartnersLocalization { EntityId = 3, LanguageId = 0 };
        var capturedDonations = new MainDonationsLocalization { EntityId = 4, LanguageId = 0 };

        _mockMapper.Setup(m => m.Map<MainPageLocalization>(It.IsAny<CreateMainPageLocalizationDto>()))
            .Returns(_mainPageLocalization);
        _mockMapper.Setup(m => m.Map<MainAboutUsLocalization>(It.IsAny<CreateMainAboutUsLocalizationDto>()))
            .Returns(capturedAboutUs);
        _mockMapper.Setup(m => m.Map<MainPartnersLocalization>(It.IsAny<CreateMainPartnersLocalizationDto>()))
            .Returns(capturedPartners);
        _mockMapper.Setup(m => m.Map<MainDonationsLocalization>(It.IsAny<CreateMainDonationsLocalizationDto>()))
            .Returns(capturedDonations);

        _mockMainPageService.Setup(s => s.CreateEntityLocalizationAsync(It.IsAny<MainPageLocalization>()))
            .ReturnsAsync(_mainPageLocalization);
        _mockMainAboutUsService.Setup(s => s.CreateEntityLocalizationAsync(It.IsAny<MainAboutUsLocalization>()))
            .ReturnsAsync(_mainAboutUsLocalization);
        _mockMainPartnersService.Setup(s => s.CreateEntityLocalizationAsync(It.IsAny<MainPartnersLocalization>()))
            .ReturnsAsync(_mainPartnersLocalization);
        _mockMainDonationsService.Setup(s => s.CreateEntityLocalizationAsync(It.IsAny<MainDonationsLocalization>()))
            .ReturnsAsync(_mainDonationsLocalization);

        _mockMapper.Setup(m => m.Map<MainPageLocalizationDto>(It.IsAny<MainPageLocalization>()))
            .Returns(_mainPageLocalizationDto);
        _mockMapper.Setup(m => m.Map<MainAboutUsLocalizationDto>(It.IsAny<MainAboutUsLocalization>()))
            .Returns(_mainAboutUsLocalizationDto);
        _mockMapper.Setup(m => m.Map<MainPartnersLocalizationDto>(It.IsAny<MainPartnersLocalization>()))
            .Returns(_mainPartnersLocalizationDto);
        _mockMapper.Setup(m => m.Map<MainDonationsLocalizationDto>(It.IsAny<MainDonationsLocalization>()))
            .Returns(_mainDonationsLocalizationDto);

        await CreateHandler().Handle(command, CancellationToken.None);

        Assert.Equal(languageId, capturedAboutUs.LanguageId);
        Assert.Equal(languageId, capturedPartners.LanguageId);
        Assert.Equal(languageId, capturedDonations.LanguageId);
    }

    // ── Failure paths ───────────────────────────────────────────────────────

    [Fact]
    public async Task Handle_ShouldFail_WhenValidationFails()
    {
        var dto = new CreateMainPageLocalizationDto
        {
            EntityId = 0,
            LanguageId = 1
        };
        var command = BuildCommand(dto);

        var result = await CreateHandler().Handle(command, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.NotEmpty(result.Errors);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenKeyNotFoundExceptionThrown()
    {
        const string errorMessage = "Language or entity not found";
        var command = BuildCommand(GetValidDto());

        _mockMapper.Setup(m => m.Map<MainPageLocalization>(It.IsAny<CreateMainPageLocalizationDto>()))
            .Returns(_mainPageLocalization);
        _mockMainPageService.Setup(s => s.CreateEntityLocalizationAsync(It.IsAny<MainPageLocalization>()))
            .ThrowsAsync(new KeyNotFoundException(errorMessage));

        var result = await CreateHandler().Handle(command, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(errorMessage, result.Errors[0].Message);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenInvalidOperationExceptionThrown()
    {
        var command = BuildCommand(GetValidDto());

        _mockMapper.Setup(m => m.Map<MainPageLocalization>(It.IsAny<CreateMainPageLocalizationDto>()))
            .Returns(_mainPageLocalization);
        _mockMainPageService.Setup(s => s.CreateEntityLocalizationAsync(It.IsAny<MainPageLocalization>()))
            .ThrowsAsync(new InvalidOperationException());

        var result = await CreateHandler().Handle(command, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(
            ErrorMessagesConstants.FailedToCreateEntity(typeof(MainPageLocalization)),
            result.Errors[0].Message);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenDbUpdateExceptionThrown()
    {
        var command = BuildCommand(GetValidDto());

        _mockMapper.Setup(m => m.Map<MainPageLocalization>(It.IsAny<CreateMainPageLocalizationDto>()))
            .Returns(_mainPageLocalization);
        _mockMainPageService.Setup(s => s.CreateEntityLocalizationAsync(It.IsAny<MainPageLocalization>()))
            .ThrowsAsync(new DbUpdateException());

        var result = await CreateHandler().Handle(command, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(
            ErrorMessagesConstants.FailedToCreateEntityInDatabase(typeof(MainPageLocalization)),
            result.Errors[0].Message);
    }

    // ── Helpers ──────────────────────────────────────────────────────────────

    private CreateMainPageLocalizationHandler CreateHandler() =>
        new(_mockMapper.Object, _validator, _mockRepositoryWrapper.Object,
            _mockMainPageService.Object, _mockMainAboutUsService.Object, _mockMainPartnersService.Object,
            _mockMainDonationsService.Object);

    private void SetupDependencies()
    {
        _mockMapper.Setup(m => m.Map<MainPageLocalization>(It.IsAny<CreateMainPageLocalizationDto>()))
            .Returns(_mainPageLocalization);
        _mockMapper.Setup(m => m.Map<MainAboutUsLocalization>(It.IsAny<CreateMainAboutUsLocalizationDto>()))
            .Returns(_mainAboutUsLocalization);
        _mockMapper.Setup(m => m.Map<MainPartnersLocalization>(It.IsAny<CreateMainPartnersLocalizationDto>()))
            .Returns(_mainPartnersLocalization);
        _mockMapper.Setup(m => m.Map<MainDonationsLocalization>(It.IsAny<CreateMainDonationsLocalizationDto>()))
            .Returns(_mainDonationsLocalization);

        _mockMainPageService.Setup(s => s.CreateEntityLocalizationAsync(It.IsAny<MainPageLocalization>()))
            .ReturnsAsync(_mainPageLocalization);
        _mockMainAboutUsService.Setup(s => s.CreateEntityLocalizationAsync(It.IsAny<MainAboutUsLocalization>()))
            .ReturnsAsync(_mainAboutUsLocalization);
        _mockMainPartnersService.Setup(s => s.CreateEntityLocalizationAsync(It.IsAny<MainPartnersLocalization>()))
            .ReturnsAsync(_mainPartnersLocalization);
        _mockMainDonationsService.Setup(s => s.CreateEntityLocalizationAsync(It.IsAny<MainDonationsLocalization>()))
            .ReturnsAsync(_mainDonationsLocalization);

        _mockMapper.Setup(m => m.Map<MainPageLocalizationDto>(It.IsAny<MainPageLocalization>()))
            .Returns(_mainPageLocalizationDto);
        _mockMapper.Setup(m => m.Map<MainAboutUsLocalizationDto>(It.IsAny<MainAboutUsLocalization>()))
            .Returns(_mainAboutUsLocalizationDto);
        _mockMapper.Setup(m => m.Map<MainPartnersLocalizationDto>(It.IsAny<MainPartnersLocalization>()))
            .Returns(_mainPartnersLocalizationDto);
        _mockMapper.Setup(m => m.Map<MainDonationsLocalizationDto>(It.IsAny<MainDonationsLocalization>()))
            .Returns(_mainDonationsLocalizationDto);
    }

    private static CreateMainPageLocalizationDto GetValidDto() => new()
    {
        EntityId = 1,
        LanguageId = 1,
        Title = "Valid title",
        Description = "Valid description here",
        MainAboutUs = new CreateMainAboutUsLocalizationDto
        {
            EntityId = 2,
            Title = "About title",
            Description = "About description here",
        },
        MainPartners = new CreateMainPartnersLocalizationDto
        {
            EntityId = 3,
            Title = "Partners title",
            Description = "Partners description here",
        },
        MainDonations = new CreateMainDonationsLocalizationDto
        {
            EntityId = 4,
            Title = "Donations title",
            Description = "Donations description here",
        }
    };

    private static CreateMainPageLocalizationCommand BuildCommand(CreateMainPageLocalizationDto dto) =>
        new(dto);
}
