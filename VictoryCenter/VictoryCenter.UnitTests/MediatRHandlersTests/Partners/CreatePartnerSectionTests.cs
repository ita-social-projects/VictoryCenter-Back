using System.Transactions;
using AutoMapper;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;
using Moq;
using VictoryCenter.BLL.Commands.Admin.Partners.Create;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.Partners;
using VictoryCenter.BLL.Exceptions.BlobStorageExceptions;
using VictoryCenter.BLL.Interfaces.BlobStorage;
using VictoryCenter.BLL.Interfaces.ReorderService;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.Base;

namespace VictoryCenter.UnitTests.MediatRHandlersTests.Partners;

public class CreatePartnersSectionTests
{
    private readonly Mock<IRepositoryWrapper> _repositoryWrapperMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<IValidator<CreatePartnersSectionCommand>> _validatorMock;
    private readonly Mock<IBlobService> _blobServiceMock;
    private readonly Mock<IReorderService> _reorderServiceMock;

    // Test Data
    private readonly CreatePartnersSectionDto _createDto = new()
    {
        Title = "Test Section",
        Description = "Test Description",
        Partners =
        [
            new CreatePartnerDto
            {
                Description = "Partner 1",
                Image = new CreatePartnerImageDto { Base64 = "base64_string_1", MimeType = "image/png" }
            },
            new CreatePartnerDto
            {
                Description = "Partner 2",
                Image = new CreatePartnerImageDto { Base64 = "base64_string_2", MimeType = "image/jpeg" }
            }
        ]
    };

    private readonly PartnerSection _sectionEntity;
    private readonly PartnersSectionDto _resultDto;

    public CreatePartnersSectionTests()
    {
        _repositoryWrapperMock = new Mock<IRepositoryWrapper>();
        _mapperMock = new Mock<IMapper>();
        _validatorMock = new Mock<IValidator<CreatePartnersSectionCommand>>();
        _blobServiceMock = new Mock<IBlobService>();
        _reorderServiceMock = new Mock<IReorderService>();

        _sectionEntity = new PartnerSection { Id = 1, Title = "Test Section", Description = "Test Description", Partners = new List<Partner>() };
        _resultDto = new PartnersSectionDto { Id = 1, Title = "Test Section", Description = "Test Description" };
    }

    [Fact]
    public async Task Handle_WhenCreationIsValid_ShouldSucceedAndReturnDto()
    {
        // Arrange
        SetupDependencies();
        var handler = new CreatePartnersSectionHandler(_repositoryWrapperMock.Object, _mapperMock.Object, _validatorMock.Object, _blobServiceMock.Object, _reorderServiceMock.Object);
        var command = new CreatePartnersSectionCommand(_createDto);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(_resultDto, result.Value);
        _blobServiceMock.Verify(b => b.SaveFileInStorageAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Exactly(_createDto.Partners.Count));
        _repositoryWrapperMock.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenValidationFails_ShouldReturnFailure()
    {
        // Arrange
        var validationError = new ValidationFailure("Title", "Title is required");
        _validatorMock.Setup(v => v.ValidateAsync(It.IsAny<CreatePartnersSectionCommand>(), It.IsAny<CancellationToken>()))
                      .ReturnsAsync(new ValidationResult([validationError]));
        var handler = new CreatePartnersSectionHandler(_repositoryWrapperMock.Object, _mapperMock.Object, _validatorMock.Object, _blobServiceMock.Object, _reorderServiceMock.Object);
        var command = new CreatePartnersSectionCommand(_createDto);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailed);
        Assert.Contains(validationError.ErrorMessage, result.Errors.Select(e => e.Message));
        _blobServiceMock.Verify(b => b.SaveFileInStorageAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        _repositoryWrapperMock.Verify(r => r.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task Handle_WhenBlobUploadFails_ShouldCleanupUploadedImagesAndReturnFailure()
    {
        // Arrange
        SetupDependencies();
        var blobException = new BlobStorageException("Upload failed");

        _blobServiceMock.SetupSequence(b => b.SaveFileInStorageAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                        .Returns(Task.FromResult("filename1.png"))
                        .ThrowsAsync(blobException);

        var handler = new CreatePartnersSectionHandler(_repositoryWrapperMock.Object, _mapperMock.Object, _validatorMock.Object, _blobServiceMock.Object, _reorderServiceMock.Object);
        var command = new CreatePartnersSectionCommand(_createDto);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailed);
        Assert.Equal(ErrorMessagesConstants.BlobStorageError(blobException.Message), result.Errors.First().Message);
        _blobServiceMock.Verify(b => b.DeleteFileInStorage(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        _repositoryWrapperMock.Verify(r => r.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task Handle_WhenDbSaveChangesFails_ShouldReturnFailure()
    {
        // Arrange
        SetupDependencies();
        _repositoryWrapperMock.Setup(r => r.SaveChangesAsync())
                              .ThrowsAsync(new DbUpdateException("DB error"));
        var handler = new CreatePartnersSectionHandler(_repositoryWrapperMock.Object, _mapperMock.Object, _validatorMock.Object, _blobServiceMock.Object, _reorderServiceMock.Object);
        var command = new CreatePartnersSectionCommand(_createDto);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailed);
        Assert.Equal(ErrorMessagesConstants.FailedToCreateEntityInDatabase(typeof(PartnerSection)), result.Errors.First().Message);
    }

    // --- NEW TEST 1 ---
    [Fact]
    public async Task Handle_WhenCreationIsValid_ShouldAssignCorrectPriorities()
    {
        // Arrange
        SetupDependencies();
        PartnerSection? capturedSection = null;
        _repositoryWrapperMock.Setup(r => r.PartnerSectionsRepository.CreateAsync(It.IsAny<PartnerSection>()))
            .Callback<PartnerSection>(section => capturedSection = section)
            .ReturnsAsync(_sectionEntity);

        var handler = new CreatePartnersSectionHandler(_repositoryWrapperMock.Object, _mapperMock.Object, _validatorMock.Object, _blobServiceMock.Object, _reorderServiceMock.Object);
        var command = new CreatePartnersSectionCommand(_createDto);

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(capturedSection);
        Assert.Equal(5L, capturedSection.Priority); // Priority from ReorderService
        Assert.Equal(2, capturedSection.Partners.Count);
        Assert.Equal(1L, capturedSection.Partners.ElementAt(0).Priority);
        Assert.Equal(2L, capturedSection.Partners.ElementAt(1).Priority);
    }

    // --- NEW TEST 2 ---
    [Fact]
    public async Task Handle_WhenReorderServiceFails_ShouldNotAttemptBlobOrDbOperations()
    {
        // Arrange
        SetupDependencies();
        var reorderException = new Exception("Reorder service is down");
        _reorderServiceMock.Setup(r => r.GetNextDisplayOrderAsync<PartnerSection>(null))
                           .ThrowsAsync(reorderException);

        var handler = new CreatePartnersSectionHandler(_repositoryWrapperMock.Object, _mapperMock.Object, _validatorMock.Object, _blobServiceMock.Object, _reorderServiceMock.Object);
        var command = new CreatePartnersSectionCommand(_createDto);

        // Act & Assert
        // The handler doesn't catch generic exceptions, so it should propagate up
        var exception = await Assert.ThrowsAsync<Exception>(() => handler.Handle(command, CancellationToken.None));

        Assert.Equal(reorderException.Message, exception.Message);
        _blobServiceMock.Verify(b => b.SaveFileInStorageAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        _repositoryWrapperMock.Verify(r => r.SaveChangesAsync(), Times.Never);
    }

    // --- NEW TEST 3 ---
    [Fact]
    public async Task Handle_WhenBlobUploadFailsAndCleanupAlsoFails_ShouldReturnOriginalBlobError()
    {
        // Arrange
        SetupDependencies();
        var originalBlobException = new BlobStorageException("Upload failed");
        var cleanupException = new Exception("Cleanup failed");

        _blobServiceMock.SetupSequence(b => b.SaveFileInStorageAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                        .Returns(Task.FromResult("filename1.png"))
                        .ThrowsAsync(originalBlobException);

        _blobServiceMock.Setup(b => b.DeleteFileInStorage(It.IsAny<string>(), It.IsAny<string>()))
                        .Throws(cleanupException);

        var handler = new CreatePartnersSectionHandler(_repositoryWrapperMock.Object, _mapperMock.Object, _validatorMock.Object, _blobServiceMock.Object, _reorderServiceMock.Object);
        var command = new CreatePartnersSectionCommand(_createDto);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailed);

        // Crucially, the returned error should be from the ORIGINAL upload failure, not the cleanup failure.
        Assert.Equal(ErrorMessagesConstants.BlobStorageError(originalBlobException.Message), result.Errors.First().Message);
        _blobServiceMock.Verify(b => b.DeleteFileInStorage(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
    }

    private void SetupDependencies(int saveChangesResult = 1)
    {
        _validatorMock.Setup(v => v.ValidateAsync(It.IsAny<CreatePartnersSectionCommand>(), It.IsAny<CancellationToken>()))
                      .ReturnsAsync(new ValidationResult());

        _mapperMock.Setup(m => m.Map<PartnerSection>(It.IsAny<CreatePartnersSectionDto>())).Returns(_sectionEntity);
        _mapperMock.Setup(m => m.Map<Partner>(It.IsAny<CreatePartnerDto>())).Returns((CreatePartnerDto dto) => new Partner { Description = dto.Description });
        _mapperMock.Setup(m => m.Map<PartnersSectionDto>(It.IsAny<PartnerSection>())).Returns(_resultDto);

        _reorderServiceMock.Setup(r => r.GetNextDisplayOrderAsync<PartnerSection>(null)).ReturnsAsync(5L); // Return some priority

        _blobServiceMock.Setup(b => b.SaveFileInStorageAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                        .Returns(Task.FromResult("filename.ext"));

        _repositoryWrapperMock.Setup(r => r.PartnerSectionsRepository.CreateAsync(It.IsAny<PartnerSection>()))
                              .ReturnsAsync(_sectionEntity);
        _repositoryWrapperMock.Setup(r => r.SaveChangesAsync()).ReturnsAsync(saveChangesResult);
        _repositoryWrapperMock.Setup(r => r.BeginTransaction()).Returns(new TransactionScope(TransactionScopeAsyncFlowOption.Enabled));
    }
}
