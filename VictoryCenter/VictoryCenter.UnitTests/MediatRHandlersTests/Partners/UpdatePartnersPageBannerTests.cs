using System.Transactions;
using AutoMapper;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;
using Moq;
using VictoryCenter.BLL.Commands.Admin.Partners.UpdateBanner;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.Partners;
using VictoryCenter.BLL.DTOs.Common;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.UnitTests.MediatRHandlersTests.Partners;

public class UpdatePartnersPageBannerTests
{
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<IRepositoryWrapper> _mockRepositoryWrapper;
    private readonly Mock<IValidator<UpdatePartnersPageBannerCommand>> _mockValidator;

    private readonly UpdatePartnersPageBannerDto _updateDto = new()
    {
        Title = "New Title",
        Description = "New Description",
        ImageId = 10
    };

    private readonly PartnersPageBannerDto _resultDto = new()
    {
        Title = "New Title",
        Description = "New Description",
        Image = new ImageDto { Id = 10, Url = "some-url" }
    };

    private readonly Image _imageEntity = new() { Id = 10, BlobName = "image.png", MimeType = "image/png" };

    private readonly PartnersPageBanner _existingBannerEntity = new()
    {
        Id = 1,
        Title = "Old Title",
        Description = "Old Description",
        ImageId = 5
    };

    public UpdatePartnersPageBannerTests()
    {
        _mockMapper = new Mock<IMapper>();
        _mockRepositoryWrapper = new Mock<IRepositoryWrapper>();
        _mockValidator = new Mock<IValidator<UpdatePartnersPageBannerCommand>>();
    }

    [Fact]
    public async Task Handle_BannerDoesNotExist_ShouldCreateBannerAndReturnOk()
    {
        // Arrange
        var createdEntity = new PartnersPageBanner { Id = 1 };
        SetupValidator(isValid: true);
        SetupRepositoryWrapper(imageToReturn: _imageEntity, bannerToReturn: null, finalBannerToReturn: createdEntity);
        SetupMapper(createdEntity, _resultDto);

        var command = new UpdatePartnersPageBannerCommand(_updateDto);
        var handler = new UpdatePartnersPageBannerHandler(_mockRepositoryWrapper.Object, _mockMapper.Object, _mockValidator.Object);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(_resultDto, result.Value);
        _mockRepositoryWrapper.Verify(r => r.PartnersPageBannersRepository.CreateAsync(It.IsAny<PartnersPageBanner>()), Times.Once);
        _mockRepositoryWrapper.Verify(r => r.PartnersPageBannersRepository.Update(It.IsAny<PartnersPageBanner>()), Times.Never);
    }

    [Fact]
    public async Task Handle_BannerExists_ShouldUpdateBannerAndReturnOk()
    {
        // Arrange
        SetupValidator(isValid: true);
        SetupRepositoryWrapper(imageToReturn: _imageEntity, bannerToReturn: _existingBannerEntity, finalBannerToReturn: _existingBannerEntity);
        SetupMapper(null, _resultDto);

        var command = new UpdatePartnersPageBannerCommand(_updateDto);
        var handler = new UpdatePartnersPageBannerHandler(_mockRepositoryWrapper.Object, _mockMapper.Object, _mockValidator.Object);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(_resultDto, result.Value);
        _mockRepositoryWrapper.Verify(r => r.PartnersPageBannersRepository.CreateAsync(It.IsAny<PartnersPageBanner>()), Times.Never);
        _mockRepositoryWrapper.Verify(r => r.PartnersPageBannersRepository.Update(_existingBannerEntity), Times.Once);
    }

    [Fact]
    public async Task Handle_ImageNotFound_ShouldReturnFailure()
    {
        // Arrange
        SetupValidator(isValid: true);
        SetupRepositoryWrapper(imageToReturn: null, bannerToReturn: null, finalBannerToReturn: null);

        var command = new UpdatePartnersPageBannerCommand(_updateDto);
        var handler = new UpdatePartnersPageBannerHandler(_mockRepositoryWrapper.Object, _mockMapper.Object, _mockValidator.Object);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailed);
        Assert.Equal(ErrorMessagesConstants.NotFound(_updateDto.ImageId, typeof(Image)), result.Errors[0].Message);
    }

    [Fact]
    public async Task Handle_InvalidData_ShouldReturnValidationFailure()
    {
        // Arrange
        var validationError = "Validation failed";
        SetupValidator(isValid: false, errorMessage: validationError);

        var command = new UpdatePartnersPageBannerCommand(_updateDto);
        var handler = new UpdatePartnersPageBannerHandler(_mockRepositoryWrapper.Object, _mockMapper.Object, _mockValidator.Object);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailed);
        Assert.Contains(validationError, result.Errors.Select(e => e.Message));
    }

    [Fact]
    public async Task Handle_DbUpdateException_ShouldReturnFailure()
    {
        // Arrange
        SetupValidator(isValid: true);
        SetupRepositoryWrapper(imageToReturn: _imageEntity, bannerToReturn: _existingBannerEntity, finalBannerToReturn: _existingBannerEntity);
        _mockRepositoryWrapper.Setup(r => r.SaveChangesAsync()).ThrowsAsync(new DbUpdateException());

        var command = new UpdatePartnersPageBannerCommand(_updateDto);
        var handler = new UpdatePartnersPageBannerHandler(_mockRepositoryWrapper.Object, _mockMapper.Object, _mockValidator.Object);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailed);
        Assert.Equal(ErrorMessagesConstants.FailedToCreateEntityInDatabase(typeof(PartnersPageBanner)), result.Errors[0].Message);
    }

    private void SetupValidator(bool isValid, string errorMessage = "Error")
    {
        var validationResult = isValid
            ? new ValidationResult()
            : new ValidationResult([new ValidationFailure("Property", errorMessage)]);

        _mockValidator
            .Setup(v => v.ValidateAsync(It.IsAny<UpdatePartnersPageBannerCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(validationResult);
    }

    private void SetupMapper(PartnersPageBanner? entityToCreate, PartnersPageBannerDto dtoToReturn)
    {
        if (entityToCreate != null)
        {
            _mockMapper.Setup(m => m.Map<PartnersPageBanner>(It.IsAny<UpdatePartnersPageBannerDto>()))
                .Returns(entityToCreate);
        }

        _mockMapper.Setup(m => m.Map(It.IsAny<UpdatePartnersPageBannerDto>(), It.IsAny<PartnersPageBanner>()));

        _mockMapper.Setup(m => m.Map<PartnersPageBannerDto>(It.IsAny<PartnersPageBanner>()))
            .Returns(dtoToReturn);
    }

    private void SetupRepositoryWrapper(Image? imageToReturn, PartnersPageBanner? bannerToReturn, PartnersPageBanner? finalBannerToReturn)
    {
        _mockRepositoryWrapper.Setup(r => r.ImageRepository.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<Image>>()))
            .ReturnsAsync(imageToReturn);

        _mockRepositoryWrapper.SetupSequence(r => r.PartnersPageBannersRepository.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<PartnersPageBanner>>()))
            .ReturnsAsync(bannerToReturn)
            .ReturnsAsync(finalBannerToReturn);

        _mockRepositoryWrapper.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

        _mockRepositoryWrapper.Setup(r => r.BeginTransaction())
            .Returns(new TransactionScope(TransactionScopeAsyncFlowOption.Enabled));
    }
}
