using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Moq;
using VictoryCenter.BLL.Commands.Admin.PdfSection.Update;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.PdfSection;
using VictoryCenter.BLL.Validators.PdfSection;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.UnitTests.MediatRHandlersTests.PdfSections;

public class UpdatePdfSectionHandlerTests
{
    private readonly Mock<IRepositoryWrapper> _mockRepo;
    private readonly IValidator<UpdatePdfSectionCommand> _validator;
    private readonly PdfSection _existingSection;

    public UpdatePdfSectionHandlerTests()
    {
        _mockRepo = new Mock<IRepositoryWrapper>();
        _validator = new UpdatePdfSectionValidator();
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

        var handler = new UpdatePdfSectionHandler(_mockRepo.Object, _validator);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal("Нова назва", result.Value.Title);
        Assert.Equal("Новий опис", result.Value.Description);

        _mockRepo.Verify(r => r.SaveChangesAsync(), Times.Once);
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

        var handler = new UpdatePdfSectionHandler(_mockRepo.Object, _validator);

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

        var handler = new UpdatePdfSectionHandler(_mockRepo.Object, _validator);

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

        var handler = new UpdatePdfSectionHandler(_mockRepo.Object, _validator);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(_existingSection.Title, result.Value.Title);
        Assert.Equal(_existingSection.Description, result.Value.Description);

        _mockRepo.Verify(r => r.SaveChangesAsync(), Times.Never);
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

        var handler = new UpdatePdfSectionHandler(_mockRepo.Object, _validator);

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

        var handler = new UpdatePdfSectionHandler(_mockRepo.Object, _validator);

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

        var handler = new UpdatePdfSectionHandler(_mockRepo.Object, _validator);

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

        var handler = new UpdatePdfSectionHandler(_mockRepo.Object, _validator);

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

        var handler = new UpdatePdfSectionHandler(_mockRepo.Object, _validator);

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

        var handler = new UpdatePdfSectionHandler(_mockRepo.Object, _validator);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(description160Chars, result.Value.Description);
    }
}
