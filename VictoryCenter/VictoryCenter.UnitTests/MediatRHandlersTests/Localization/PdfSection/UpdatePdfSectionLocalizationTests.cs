using AutoMapper;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Moq;
using VictoryCenter.BLL.Commands.Admin.Localization.PdfSection.Update;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.Localization.PdfSection;
using VictoryCenter.BLL.Interfaces.Localization;
using VictoryCenter.BLL.Validators.Localization.PdfSections;
using VictoryCenter.DAL.Entities.Localization;
using VictoryCenter.DAL.Enums;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;
using PdfSectionEntity = VictoryCenter.DAL.Entities.PdfSection;

namespace VictoryCenter.UnitTests.MediatRHandlersTests.Localization.PdfSection;

public class UpdatePdfSectionLocalizationHandlerTests
{
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<ILocalizationService<PdfSectionEntity, PdfSectionLocalization>> _mockLocalizationService;
    private readonly Mock<IRepositoryWrapper> _mockRepositoryWrapper;
    private readonly IValidator<UpdatePdfSectionLocalizationCommand> _validator;

    private readonly PdfSectionEntity _testSection = new()
    {
        Id = 1,
        Title = "Test Title",
        Description = "Test Description"
    };

    private readonly PdfSectionLocalization _testEntity = new()
    {
        EntityId = 1,
        LanguageId = 1,
        Title = "Old Title EN",
        Description = "Old Description EN",
        TranslationStatus = TranslationStatus.Relevant
    };

    private readonly PdfSectionLocalization _updatedEntity = new()
    {
        EntityId = 1,
        LanguageId = 1,
        Title = "New Title EN",
        Description = "New Description EN",
        TranslationStatus = TranslationStatus.Relevant
    };

    private readonly UpdatePdfSectionLocalizationDto _updatedDto = new()
    {
        Title = "New Title EN",
        Description = "New Description EN"
    };

    private readonly PdfSectionLocalizationDto _updatedTestDto = new()
    {
        LanguageId = 1,
        LocalizationInfoDto = new() { Id = 1, Code = "en" },
        Title = "New Title EN",
        Description = "New Description EN"
    };

    private readonly long _languageId = 1;

    public UpdatePdfSectionLocalizationHandlerTests()
    {
        _mockMapper = new Mock<IMapper>();
        _mockLocalizationService = new Mock<ILocalizationService<PdfSectionEntity, PdfSectionLocalization>>();
        _mockRepositoryWrapper = new Mock<IRepositoryWrapper>();
        _validator = new UpdatePdfSectionLocalizationValidator(new BasePdfSectionLocalizationValidator());
    }

    [Fact]
    public async Task Handle_ShouldUpdateLocalization_Successfully()
    {
        // Arrange
        SetupDependencies(_updatedEntity);
        var handler = CreateHandler();
        var command = new UpdatePdfSectionLocalizationCommand(_updatedDto, _languageId);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(_updatedDto.Title, result.Value.Title);
        Assert.Equal(_updatedDto.Description, result.Value.Description);
        _mockMapper.Verify(m => m.Map<PdfSectionLocalization>(It.IsAny<UpdatePdfSectionLocalizationDto>()), Times.Once);
        _mockLocalizationService.Verify(s => s.UpdateEntityLocalizationAsync(It.IsAny<PdfSectionLocalization>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenPdfSectionNotFound()
    {
        // Arrange
        _mockRepositoryWrapper
            .Setup(r => r.PdfSectionRepository.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<PdfSectionEntity>>()))
            .ReturnsAsync((PdfSectionEntity?)null);

        var handler = CreateHandler();
        var command = new UpdatePdfSectionLocalizationCommand(_updatedDto, _languageId);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessagesConstants.NotFound(), result.Errors[0].Message);
        _mockLocalizationService.Verify(s => s.UpdateEntityLocalizationAsync(It.IsAny<PdfSectionLocalization>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenKeyNotFoundExceptionThrown()
    {
        // Arrange
        var notFoundMessage = "Not found";
        SetupRepositoryAndMapper();
        _mockLocalizationService
            .Setup(x => x.UpdateEntityLocalizationAsync(It.IsAny<PdfSectionLocalization>()))
            .ThrowsAsync(new KeyNotFoundException(notFoundMessage));

        var handler = CreateHandler();
        var command = new UpdatePdfSectionLocalizationCommand(_updatedDto, _languageId);

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
        SetupRepositoryAndMapper();
        _mockLocalizationService
            .Setup(x => x.UpdateEntityLocalizationAsync(It.IsAny<PdfSectionLocalization>()))
            .ThrowsAsync(new InvalidOperationException());

        var handler = CreateHandler();
        var command = new UpdatePdfSectionLocalizationCommand(_updatedDto, _languageId);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(
            ErrorMessagesConstants.FailedToUpdateEntity(typeof(PdfSectionLocalization)),
            result.Errors[0].Message);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenDbUpdateExceptionThrown()
    {
        // Arrange
        SetupRepositoryAndMapper();
        _mockLocalizationService
            .Setup(x => x.UpdateEntityLocalizationAsync(It.IsAny<PdfSectionLocalization>()))
            .ThrowsAsync(new DbUpdateException());

        var handler = CreateHandler();
        var command = new UpdatePdfSectionLocalizationCommand(_updatedDto, _languageId);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(
            ErrorMessagesConstants.FailedToUpdateEntityInDatabase(typeof(PdfSectionLocalization)),
            result.Errors[0].Message);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenValidationFails()
    {
        // Arrange
        var invalidDto = new UpdatePdfSectionLocalizationDto
        {
            Title = "",
            Description = "Valid Description"
        };

        var handler = CreateHandler();
        var command = new UpdatePdfSectionLocalizationCommand(invalidDto, _languageId);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Contains(
            ErrorMessagesConstants.PropertyIsRequired(nameof(UpdatePdfSectionLocalizationDto.Title)),
            result.Errors[0].Message);
    }

    [Fact]
    public async Task Handle_ShouldSetEntityIdFromSection()
    {
        // Arrange
        SetupDependencies(_updatedEntity);
        var handler = CreateHandler();
        var command = new UpdatePdfSectionLocalizationCommand(_updatedDto, _languageId);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        _mockLocalizationService.Verify(
            s => s.UpdateEntityLocalizationAsync(
            It.Is<PdfSectionLocalization>(e => e.EntityId == _testSection.Id)), Times.Once);
    }

    private UpdatePdfSectionLocalizationHandler CreateHandler() =>
        new(_mockMapper.Object, _validator, _mockLocalizationService.Object, _mockRepositoryWrapper.Object);

    private void SetupDependencies(PdfSectionLocalization? entityToReturn = null)
    {
        SetupRepositoryAndMapper();
        _mockLocalizationService
            .Setup(s => s.UpdateEntityLocalizationAsync(It.IsAny<PdfSectionLocalization>()))
            .ReturnsAsync(entityToReturn);
    }

    private void SetupRepositoryAndMapper()
    {
        _mockRepositoryWrapper
            .Setup(r => r.PdfSectionRepository.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<PdfSectionEntity>>()))
            .ReturnsAsync(_testSection);
        _mockMapper
            .Setup(m => m.Map<PdfSectionLocalization>(It.IsAny<UpdatePdfSectionLocalizationDto>()))
            .Returns(_updatedEntity);
        _mockMapper
            .Setup(m => m.Map<PdfSectionLocalizationDto>(It.IsAny<PdfSectionLocalization>()))
            .Returns(_updatedTestDto);
    }
}
