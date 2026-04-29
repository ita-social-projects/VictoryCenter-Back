using AutoMapper;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Moq;
using VictoryCenter.BLL.Commands.Admin.Localization.PdfSection.Create;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.Localization.PdfSection;
using VictoryCenter.BLL.Interfaces.Localization;
using VictoryCenter.BLL.Validators.Localization.PdfSections;
using VictoryCenter.DAL.Entities.Localization;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;
using PdfSectionEntity = VictoryCenter.DAL.Entities.PdfSection;

namespace VictoryCenter.UnitTests.MediatRHandlersTests.Localization.PdfSection;

public class CreatePdfSectionLocalizationHandlerTests
{
    private readonly Mock<ILocalizationService<PdfSectionEntity, PdfSectionLocalization>> _mockLocalizationService;
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<IRepositoryWrapper> _mockRepositoryWrapper;
    private readonly IValidator<CreatePdfSectionLocalizationCommand> _validator;

    private readonly PdfSectionEntity _testSection = new()
    {
        Id = 1,
        Title = "Test Title",
        Description = "Test Description"
    };

    private readonly CreatePdfSectionLocalizationDto _testCreateDto = new()
    {
        LanguageId = 1,
        Title = "Test Title EN",
        Description = "Test Description EN"
    };

    private readonly PdfSectionLocalization _testEntity = new()
    {
        EntityId = 1,
        LanguageId = 1,
        Title = "Test Title EN",
        Description = "Test Description EN"
    };

    private readonly PdfSectionLocalizationDto _testDto = new()
    {
        LanguageId = 1,
        LocalizationInfoDto = new() { Id = 1, Code = "en" },
        Title = "Test Title EN",
        Description = "Test Description EN"
    };

    public CreatePdfSectionLocalizationHandlerTests()
    {
        _mockLocalizationService = new Mock<ILocalizationService<PdfSectionEntity, PdfSectionLocalization>>();
        _mockMapper = new Mock<IMapper>();
        _mockRepositoryWrapper = new Mock<IRepositoryWrapper>();
        _validator = new CreatePdfSectionLocalizationValidator(new BasePdfSectionLocalizationValidator());
    }

    [Fact]
    public async Task Handle_ShouldCreateLocalization_Successfully()
    {
        // Arrange
        SetupDependencies();
        var handler = CreateHandler();
        var command = new CreatePdfSectionLocalizationCommand(_testCreateDto);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(_testDto.Title, result.Value.Title);
        Assert.Equal(_testDto.Description, result.Value.Description);
        Assert.Equal(_testDto.LocalizationInfoDto.Id, result.Value.LocalizationInfoDto.Id);
        _mockMapper.Verify(m => m.Map<PdfSectionLocalization>(It.IsAny<CreatePdfSectionLocalizationDto>()), Times.Once);
        _mockLocalizationService.Verify(s => s.CreateEntityLocalizationAsync(It.IsAny<PdfSectionLocalization>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenPdfSectionNotFound()
    {
        // Arrange
        _mockRepositoryWrapper
            .Setup(r => r.PdfSectionRepository.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<PdfSectionEntity>>()))
            .ReturnsAsync((PdfSectionEntity?)null);

        var handler = CreateHandler();
        var command = new CreatePdfSectionLocalizationCommand(_testCreateDto);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessagesConstants.NotFound(), result.Errors[0].Message);
        _mockLocalizationService.Verify(s => s.CreateEntityLocalizationAsync(It.IsAny<PdfSectionLocalization>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenKeyNotFoundExceptionThrown()
    {
        // Arrange
        var notFoundMessage = "Not found";
        _mockRepositoryWrapper
            .Setup(r => r.PdfSectionRepository.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<PdfSectionEntity>>()))
            .ReturnsAsync(_testSection);
        _mockMapper
            .Setup(x => x.Map<PdfSectionLocalization>(It.IsAny<CreatePdfSectionLocalizationDto>()))
            .Returns(_testEntity);
        _mockLocalizationService
            .Setup(x => x.CreateEntityLocalizationAsync(It.IsAny<PdfSectionLocalization>()))
            .ThrowsAsync(new KeyNotFoundException(notFoundMessage));

        var handler = CreateHandler();
        var command = new CreatePdfSectionLocalizationCommand(_testCreateDto);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Single(result.Errors);
        Assert.Equal(notFoundMessage, result.Errors[0].Message);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenInvalidOperationExceptionThrown()
    {
        // Arrange
        _mockRepositoryWrapper
            .Setup(r => r.PdfSectionRepository.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<PdfSectionEntity>>()))
            .ReturnsAsync(_testSection);
        _mockMapper
            .Setup(x => x.Map<PdfSectionLocalization>(It.IsAny<CreatePdfSectionLocalizationDto>()))
            .Returns(_testEntity);
        _mockLocalizationService
            .Setup(x => x.CreateEntityLocalizationAsync(It.IsAny<PdfSectionLocalization>()))
            .ThrowsAsync(new InvalidOperationException());

        var handler = CreateHandler();
        var command = new CreatePdfSectionLocalizationCommand(_testCreateDto);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(
            ErrorMessagesConstants.FailedToCreateEntity(typeof(PdfSectionLocalization)),
            result.Errors[0].Message);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenDbUpdateExceptionThrown()
    {
        // Arrange
        _mockRepositoryWrapper
            .Setup(r => r.PdfSectionRepository.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<PdfSectionEntity>>()))
            .ReturnsAsync(_testSection);
        _mockMapper
            .Setup(x => x.Map<PdfSectionLocalization>(It.IsAny<CreatePdfSectionLocalizationDto>()))
            .Returns(_testEntity);
        _mockLocalizationService
            .Setup(x => x.CreateEntityLocalizationAsync(It.IsAny<PdfSectionLocalization>()))
            .ThrowsAsync(new DbUpdateException());

        var handler = CreateHandler();
        var command = new CreatePdfSectionLocalizationCommand(_testCreateDto);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(
            ErrorMessagesConstants.FailedToCreateEntityInDatabase(typeof(PdfSectionLocalization)),
            result.Errors[0].Message);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenValidationFails()
    {
        // Arrange
        var invalidDto = new CreatePdfSectionLocalizationDto
        {
            LanguageId = 0, // invalid
            Title = "Valid Title",
            Description = "Valid Description"
        };

        var handler = CreateHandler();
        var command = new CreatePdfSectionLocalizationCommand(invalidDto);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenTitleIsEmpty()
    {
        // Arrange
        var invalidDto = new CreatePdfSectionLocalizationDto
        {
            LanguageId = 1,
            Title = "",
            Description = "Valid Description"
        };

        var handler = CreateHandler();
        var command = new CreatePdfSectionLocalizationCommand(invalidDto);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Contains(
            ErrorMessagesConstants.PropertyIsRequired(nameof(CreatePdfSectionLocalizationDto.Title)),
            result.Errors[0].Message);
    }

    [Fact]
    public async Task Handle_ShouldSetEntityIdFromSection()
    {
        // Arrange
        SetupDependencies();
        var handler = CreateHandler();
        var command = new CreatePdfSectionLocalizationCommand(_testCreateDto);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        _mockLocalizationService.Verify(
            s => s.CreateEntityLocalizationAsync(
            It.Is<PdfSectionLocalization>(e => e.EntityId == _testSection.Id)), Times.Once);
    }

    private CreatePdfSectionLocalizationHandler CreateHandler() =>
        new(_mockMapper.Object, _validator, _mockLocalizationService.Object, _mockRepositoryWrapper.Object);

    private void SetupDependencies()
    {
        _mockRepositoryWrapper
            .Setup(r => r.PdfSectionRepository.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<PdfSectionEntity>>()))
            .ReturnsAsync(_testSection);
        _mockMapper
            .Setup(x => x.Map<PdfSectionLocalization>(It.IsAny<CreatePdfSectionLocalizationDto>()))
            .Returns(_testEntity);
        _mockMapper
            .Setup(x => x.Map<PdfSectionLocalizationDto>(It.IsAny<PdfSectionLocalization>()))
            .Returns(_testDto);
        _mockLocalizationService
            .Setup(x => x.CreateEntityLocalizationAsync(It.IsAny<PdfSectionLocalization>()))
            .ReturnsAsync(_testEntity);
    }
}
