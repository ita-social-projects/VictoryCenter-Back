using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Moq;
using VictoryCenter.BLL.Commands.Admin.PdfSection.Update;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.PdfSection;
using VictoryCenter.BLL.DTOs.Public.PdfSection;
using VictoryCenter.BLL.Hubs;
using VictoryCenter.BLL.Mapping.Localization.Languages;
using VictoryCenter.BLL.Mapping.PdfSection;
using VictoryCenter.BLL.Validators.PdfSection;
using VictoryCenter.DAL.Entities.Localization;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;
using VictoryCenter.UnitTests.Utils.SignalR;

namespace VictoryCenter.UnitTests.MediatRHandlersTests.PdfSection;

using PdfSection = DAL.Entities.PdfSection;

public class UpdatePdfSectionHandlerTests
{
    private readonly Mock<IRepositoryWrapper> _mockRepo;
    private readonly IValidator<UpdatePdfSectionCommand> _validator;
    private readonly IMapper _mapper;
    private readonly Mock<IHubContext<PdfReportsHub>> _mockHubContext;
    private readonly PdfSection _existingSection;

    public UpdatePdfSectionHandlerTests()
    {
        _mockRepo = new Mock<IRepositoryWrapper>();
        _validator = new UpdatePdfSectionValidator();
        var mapperConfig = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<PublicPdfSectionProfile>();
            cfg.AddProfile<LocalizationsLanguageProfile>();
        });
        _mapper = mapperConfig.CreateMapper();
        _mockHubContext = HubContextMockFactory.Create<PdfReportsHub>();
        _existingSection = new PdfSection
        {
            Id = 1,
            Title = "Стара назва",
            Description = "Старий опис",
            CreatedAt = DateTimeOffset.UtcNow
        };
    }

    [Fact]
    public async Task Handle_ValidRequest_UpdatesSectionAndReturnsUpdatedDto()
    {
        // Arrange
        var updateDto = new PdfSectionDto
        {
            Title = "Нова назва",
            Description = "Новий опис"
        };
        var command = new UpdatePdfSectionCommand(updateDto);

        _mockRepo.Setup(r => r.PdfSectionRepository.CountAsync(It.IsAny<QueryOptions<PdfSection>>()))
                 .ReturnsAsync(1);

        _mockRepo.Setup(r => r.PdfSectionRepository.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<PdfSection>>()))
                 .ReturnsAsync(_existingSection);

        _mockRepo.Setup(r => r.SaveChangesAsync())
                 .ReturnsAsync(1);

        var handler = CreateHandler();

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal("Нова назва", result.Value.Title);
        Assert.Equal("Новий опис", result.Value.Description);

        _mockRepo.Verify(r => r.SaveChangesAsync(), Times.Once);
        Mock.Get(_mockHubContext.Object.Clients.All).Verify(
            p => p.SendCoreAsync(
                "PdfSectionUpdated",
                It.Is<object?[]>(args => IsUpdatedSectionArgs(args, "Нова назва", "Новий опис")),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_NoSectionExists_ReturnsFailResult()
    {
        // Arrange
        var updateDto = new PdfSectionDto
        {
            Title = "Нова назва",
            Description = "Новий опис"
        };
        var command = new UpdatePdfSectionCommand(updateDto);

        _mockRepo.Setup(r => r.PdfSectionRepository.CountAsync(It.IsAny<QueryOptions<PdfSection>>()))
                 .ReturnsAsync(0);

        var handler = CreateHandler();

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Contains(PdfSectionConstants.SectionNotFound, result.Errors.Select(e => e.Message));
    }

    [Fact]
    public async Task Handle_MultipleSectionsExist_ReturnsFailResult()
    {
        // Arrange
        var updateDto = new PdfSectionDto
        {
            Title = "Нова назва",
            Description = "Новий опис"
        };
        var command = new UpdatePdfSectionCommand(updateDto);

        _mockRepo.Setup(r => r.PdfSectionRepository.CountAsync(It.IsAny<QueryOptions<PdfSection>>()))
                 .ReturnsAsync(2);

        var handler = CreateHandler();

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Contains("Multiple", result.Errors.Select(e => e.Message).First());
    }

    [Fact]
    public async Task Handle_SameDataAsExisting_ReturnsSuccessWithoutSaveChanges()
    {
        // Arrange
        var updateDto = new PdfSectionDto
        {
            Title = _existingSection.Title,
            Description = _existingSection.Description
        };
        var command = new UpdatePdfSectionCommand(updateDto);

        _mockRepo.Setup(r => r.PdfSectionRepository.CountAsync(It.IsAny<QueryOptions<PdfSection>>()))
                 .ReturnsAsync(1);

        _mockRepo.Setup(r => r.PdfSectionRepository.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<PdfSection>>()))
                 .ReturnsAsync(_existingSection);

        var handler = CreateHandler();

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(_existingSection.Title, result.Value.Title);
        Assert.Equal(_existingSection.Description, result.Value.Description);

        _mockRepo.Verify(r => r.SaveChangesAsync(), Times.Never);
        Mock.Get(_mockHubContext.Object.Clients.All).Verify(
            p => p.SendCoreAsync("PdfSectionUpdated", It.IsAny<object?[]>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_SaveChangesReturnsZero_ReturnsFailResult()
    {
        // Arrange
        var updateDto = new PdfSectionDto
        {
            Title = "Нова назва",
            Description = "Новий опис"
        };
        var command = new UpdatePdfSectionCommand(updateDto);

        _mockRepo.Setup(r => r.PdfSectionRepository.CountAsync(It.IsAny<QueryOptions<PdfSection>>()))
                 .ReturnsAsync(1);

        _mockRepo.Setup(r => r.PdfSectionRepository.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<PdfSection>>()))
                 .ReturnsAsync(_existingSection);

        _mockRepo.Setup(r => r.SaveChangesAsync())
                 .ReturnsAsync(0);

        var handler = CreateHandler();

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
    }

    [Fact]
    public async Task Handle_ValidationFails_ReturnsFailResult()
    {
        // Arrange
        var updateDto = new PdfSectionDto
        {
            Title = "",
            Description = ""
        };
        var command = new UpdatePdfSectionCommand(updateDto);

        var handler = CreateHandler();

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
    }

    [Fact]
    public async Task Handle_NormalizesTextWithExtraSpaces_UpdatesWithCleanedText()
    {
        // Arrange
        var updateDto = new PdfSectionDto
        {
            Title = "  Назва   з   пробілами  ",
            Description = "  Опис   з   пробілами  "
        };
        var command = new UpdatePdfSectionCommand(updateDto);

        _mockRepo.Setup(r => r.PdfSectionRepository.CountAsync(It.IsAny<QueryOptions<PdfSection>>()))
                 .ReturnsAsync(1);

        _mockRepo.Setup(r => r.PdfSectionRepository.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<PdfSection>>()))
                 .ReturnsAsync(_existingSection);

        _mockRepo.Setup(r => r.SaveChangesAsync())
                 .ReturnsAsync(1);

        var handler = CreateHandler();

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal("Назва з пробілами", result.Value.Title);
        Assert.Equal("Опис з пробілами", result.Value.Description);
    }

    [Fact]
    public async Task Handle_DbUpdateException_ReturnsFailResult()
    {
        // Arrange
        var updateDto = new PdfSectionDto
        {
            Title = "Нова назва",
            Description = "Новий опис"
        };
        var command = new UpdatePdfSectionCommand(updateDto);

        _mockRepo.Setup(r => r.PdfSectionRepository.CountAsync(It.IsAny<QueryOptions<PdfSection>>()))
                 .ReturnsAsync(1);

        _mockRepo.Setup(r => r.PdfSectionRepository.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<PdfSection>>()))
                 .ReturnsAsync(_existingSection);

        _mockRepo.Setup(r => r.SaveChangesAsync())
                 .ThrowsAsync(new DbUpdateException());

        var handler = CreateHandler();

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
    }

    [Fact]
    public async Task Handle_DescriptionLongerThan160Chars_ReturnsFailResult()
    {
        // Arrange
        var longDescription = new string('a', 161);
        var updateDto = new PdfSectionDto
        {
            Title = "Нова назва",
            Description = longDescription
        };
        var command = new UpdatePdfSectionCommand(updateDto);

        var handler = CreateHandler();

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
    }

    [Fact]
    public async Task Handle_DescriptionExactly160Chars_UpdatesSuccessfully()
    {
        // Arrange
        var description160Chars = new string('a', 160);
        var updateDto = new PdfSectionDto
        {
            Title = "Нова назва",
            Description = description160Chars
        };
        var command = new UpdatePdfSectionCommand(updateDto);

        _mockRepo.Setup(r => r.PdfSectionRepository.CountAsync(It.IsAny<QueryOptions<PdfSection>>()))
                 .ReturnsAsync(1);

        _mockRepo.Setup(r => r.PdfSectionRepository.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<PdfSection>>()))
                 .ReturnsAsync(_existingSection);

        _mockRepo.Setup(r => r.SaveChangesAsync())
                 .ReturnsAsync(1);

        var handler = CreateHandler();

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(description160Chars, result.Value.Description);
    }

    [Fact]
    public async Task Handle_SectionWithNullLocalizations_DoesNotThrow()
    {
        // Arrange
        var sectionWithNullLocalizations = new PdfSection
        {
            Id = 1,
            Title = "Стара назва",
            Description = "Старий опис",
            CreatedAt = DateTimeOffset.UtcNow,
            Localizations = null!
        };

        var updateDto = new PdfSectionDto { Title = "Нова назва", Description = "Новий опис" };
        var command = new UpdatePdfSectionCommand(updateDto);

        _mockRepo.Setup(r => r.PdfSectionRepository.CountAsync(It.IsAny<QueryOptions<PdfSection>>()))
                 .ReturnsAsync(1);
        _mockRepo.Setup(r => r.PdfSectionRepository.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<PdfSection>>()))
                 .ReturnsAsync(sectionWithNullLocalizations);
        _mockRepo.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

        var handler = CreateHandler();

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async Task Handle_SectionWithLocalizations_SetsAllLocalizationsToOutdated()
    {
        // Arrange
        var sectionWithLocalizations = new PdfSection
        {
            Id = 1,
            Title = "Стара назва",
            Description = "Старий опис",
            CreatedAt = DateTimeOffset.UtcNow,
            Localizations =
            [
                new()
                {
                    LanguageId = 2,
                    Title = "Old title",
                    Description = "Old description",
                    TranslationStatus = DAL.Enums.TranslationStatus.Relevant,
                    Language = new LocalizationLanguage { Id = 2, Code = "en", Name = "English" },
                },
                new()
                {
                    LanguageId = 3,
                    Title = "Other title",
                    Description = "Other description",
                    TranslationStatus = DAL.Enums.TranslationStatus.Relevant,
                    Language = new LocalizationLanguage { Id = 3, Code = "de", Name = "German" },
                },
            ]
        };

        var updateDto = new PdfSectionDto { Title = "Нова назва", Description = "Новий опис" };
        var command = new UpdatePdfSectionCommand(updateDto);

        _mockRepo.Setup(r => r.PdfSectionRepository.CountAsync(It.IsAny<QueryOptions<PdfSection>>()))
                 .ReturnsAsync(1);
        _mockRepo.Setup(r => r.PdfSectionRepository.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<PdfSection>>()))
                 .ReturnsAsync(sectionWithLocalizations);
        _mockRepo.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

        var handler = CreateHandler();

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.All(
            sectionWithLocalizations.Localizations,
            loc => Assert.Equal(DAL.Enums.TranslationStatus.Outdated, loc.TranslationStatus));
    }

    private static bool IsUpdatedSectionArgs(object?[] args, string expectedTitle, string expectedDescription)
    {
        return args.Length == 1
            && args[0] is PublicPdfSectionDto dto
            && dto.Title == expectedTitle
            && dto.Description == expectedDescription;
    }

    private UpdatePdfSectionHandler CreateHandler() =>
        new(_mockRepo.Object, _validator, _mapper, _mockHubContext.Object);
}
