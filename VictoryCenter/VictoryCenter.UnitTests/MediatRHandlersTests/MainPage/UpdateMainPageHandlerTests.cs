using System.Transactions;
using AutoMapper;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;
using Moq;
using VictoryCenter.BLL.Commands.Admin.MainPage.Update;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.ImpactStatistics;
using VictoryCenter.BLL.DTOs.Admin.ImpactStatistics.Metrics;
using VictoryCenter.BLL.DTOs.Admin.MainAboutUs;
using VictoryCenter.BLL.DTOs.Admin.MainPages;
using VictoryCenter.BLL.DTOs.Admin.MainPartners;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Interfaces.MainPage;
using VictoryCenter.DAL.Repositories.Interfaces.Media;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.UnitTests.MediatRHandlersTests.MainPage;

public class UpdateMainPageHandlerTests
{
    private readonly Mock<IRepositoryWrapper> _repositoryWrapperMock = new();
    private readonly Mock<IMainPageRepository> _mainPageRepositoryMock = new();
    private readonly Mock<IImageRepository> _imageRepositoryMock = new();
    private readonly Mock<IMapper> _mapperMock = new();
    private readonly Mock<IValidator<UpdateMainPageCommand>> _validatorMock = new();

    public UpdateMainPageHandlerTests()
    {
        _repositoryWrapperMock
            .SetupGet(x => x.MainPageRepository)
            .Returns(_mainPageRepositoryMock.Object);

        _repositoryWrapperMock
            .SetupGet(x => x.ImageRepository)
            .Returns(_imageRepositoryMock.Object);

        _repositoryWrapperMock
            .Setup(x => x.BeginTransaction())
            .Returns(new TransactionScope(TransactionScopeAsyncFlowOption.Enabled));
    }

    [Fact]
    public async Task Handle_ShouldUpdateMainPage_WhenRequestIsValid()
    {
        // Arrange
        var existingEntity = GetExistingMainPageEntity(1);

        var command = new UpdateMainPageCommand(GetValidUpdateDto(existingEntity));

        SetupValidationSuccess();

        _mainPageRepositoryMock
            .SetupSequence(x => x.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<DAL.Entities.MainPage>?>()))
            .ReturnsAsync(existingEntity)
            .ReturnsAsync(existingEntity);

        _imageRepositoryMock
            .Setup(x => x.GetAllAsync(It.IsAny<QueryOptions<Image>?>()))
            .ReturnsAsync(new List<Image>
            {
                new() { Id = 5 },
                new() { Id = 6 },
            });

        _mapperMock
            .Setup(x => x.Map(command.UpdateMainPageDto.MainAboutUs, existingEntity.MainAboutUs))
            .Verifiable();
        _mapperMock
            .Setup(x => x.Map(command.UpdateMainPageDto.MainPartners, existingEntity.MainPartners))
            .Verifiable();

        var expectedDto = new MainPageDto
        {
            Id = 1,
            Title = command.UpdateMainPageDto.Title,
            Description = command.UpdateMainPageDto.Description,
        };

        _repositoryWrapperMock
            .Setup(x => x.SaveChangesAsync())
            .ReturnsAsync(1);

        _mapperMock
            .Setup(x => x.Map<DAL.Entities.MainPage, MainPageDto>(existingEntity))
            .Returns(expectedDto);

        var handler = CreateHandler();

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(expectedDto.Id, result.Value.Id);

        var updatedStat = existingEntity.ImpactStatistics.Single();
        Assert.Equal(command.UpdateMainPageDto.ImpactStatistics.Single().Description, updatedStat.Description);

        var updatedMetric = updatedStat.Metrics.Single();
        Assert.Equal(command.UpdateMainPageDto.ImpactStatistics.Single().Metrics.Single().Value, updatedMetric.Value);
        Assert.Equal(command.UpdateMainPageDto.ImpactStatistics.Single().Metrics.Single().Signature, updatedMetric.Signature);

        _repositoryWrapperMock.Verify(x => x.SaveChangesAsync(), Times.Once);
        _imageRepositoryMock.Verify(x => x.GetAllAsync(It.IsAny<QueryOptions<Image>?>()), Times.Once);
        _validatorMock.Verify(
            x => x.ValidateAsync(It.IsAny<ValidationContext<UpdateMainPageCommand>>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenValidationFails()
    {
        // Arrange
        var command = new UpdateMainPageCommand(GetValidUpdateDto());
        SetupValidationFailure("Validation failed");

        var handler = CreateHandler();

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("Validation failed", result.Errors[0].Message);

        _repositoryWrapperMock.Verify(x => x.BeginTransaction(), Times.Never);
        _repositoryWrapperMock.Verify(x => x.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenMainPageNotFound()
    {
        // Arrange
        var command = new UpdateMainPageCommand(GetValidUpdateDto());
        SetupValidationSuccess();

        _mainPageRepositoryMock
            .Setup(x => x.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<DAL.Entities.MainPage>?>()))
            .ReturnsAsync((DAL.Entities.MainPage?)null);

        var handler = CreateHandler();

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessagesConstants.NotFound(), result.Errors[0].Message);

        _repositoryWrapperMock.Verify(x => x.SaveChangesAsync(), Times.Never);
        _imageRepositoryMock.Verify(x => x.GetAllAsync(It.IsAny<QueryOptions<Image>?>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenImageDoesNotExist()
    {
        // Arrange
        var command = new UpdateMainPageCommand(GetValidUpdateDto());
        SetupValidationSuccess();

        var entity = GetExistingMainPageEntity(1);

        _mainPageRepositoryMock
            .Setup(x => x.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<DAL.Entities.MainPage>?>()))
            .ReturnsAsync(entity);

        _imageRepositoryMock
            .Setup(x => x.GetAllAsync(It.IsAny<QueryOptions<Image>?>()))
            .ReturnsAsync(new List<Image> { new() { Id = 5 } });

        var handler = CreateHandler();

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(
            ErrorMessagesConstants.NotFound(new[] { 6L }, typeof(Image)),
            result.Errors[0].Message);

        _repositoryWrapperMock.Verify(x => x.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenUpdatedEntityCannotBeLoaded()
    {
        // Arrange
        var entity = GetExistingMainPageEntity(1);
        var command = new UpdateMainPageCommand(GetValidUpdateDto(entity) with { ImageId = null });

        SetupValidationSuccess();

        _mainPageRepositoryMock
            .SetupSequence(x => x.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<DAL.Entities.MainPage>?>()))
            .ReturnsAsync(entity)
            .ReturnsAsync((DAL.Entities.MainPage?)null);

        _repositoryWrapperMock
            .Setup(x => x.SaveChangesAsync())
            .ReturnsAsync(1);

        _imageRepositoryMock
            .Setup(x => x.GetAllAsync(It.IsAny<QueryOptions<Image>?>()))
            .ReturnsAsync(new List<Image>
            {
                new() { Id = 6 },
            });

        var handler = CreateHandler();

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(
            ErrorMessagesConstants.FailedToUpdateEntityInDatabase(typeof(DAL.Entities.MainPage)),
            result.Errors[0].Message);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenDbUpdateExceptionOccurs()
    {
        // Arrange
        var entity = GetExistingMainPageEntity(1);
        var command = new UpdateMainPageCommand(GetValidUpdateDto(entity) with { ImageId = null });

        SetupValidationSuccess();

        _mainPageRepositoryMock
            .Setup(x => x.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<DAL.Entities.MainPage>?>()))
            .ReturnsAsync(entity);

        _imageRepositoryMock
            .Setup(x => x.GetAllAsync(It.IsAny<QueryOptions<Image>?>()))
            .ReturnsAsync(new List<Image>
            {
                new() { Id = 5 },
                new() { Id = 6 },
            });

        _repositoryWrapperMock
            .Setup(x => x.SaveChangesAsync())
            .ThrowsAsync(new DbUpdateException());

        var handler = CreateHandler();

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(
            ErrorMessagesConstants.FailedToUpdateEntityInDatabase(typeof(DAL.Entities.MainPage)),
            result.Errors[0].Message);
    }

    [Fact]
    public async Task Handle_ShouldUpdateAndCreateNestedItems_WhenPayloadIsMixed()
    {
        // Arrange
        var entity = GetExistingMainPageEntity(1);

        var command = new UpdateMainPageCommand(new UpdateMainPageDto
        {
            Title = "Updated main page title",
            Description = "Updated main page description long enough",
            ImageId = 5,
            MainAboutUs = new UpdateMainAboutUsDto
            {
                Title = "Updated about us title",
                Description = "Updated about us description long enough",
            },
            MainPartners = new UpdateMainPartnersDto
            {
                Title = "Updated partners title",
                Description = "Updated partners description long enough",
            },
            ImpactStatistics =
            [
                new UpdateImpactStatisticDto
                {
                    Id = entity.ImpactStatistics.Single().Id,
                    Description = "Updated existing stat",
                    ImageId = 6,
                    Metrics =
                    [
                        new UpdateMetricDto
                        {
                            Id = entity.ImpactStatistics.Single().Metrics.Single().Id,
                            Value = "999",
                            Signature = "updated-signature",
                        },
                        new UpdateMetricDto
                        {
                            Value = "123",
                            Signature = "new-metric",
                        },
                    ],
                },
                new UpdateImpactStatisticDto
                {
                    Description = "Brand new stat",
                    ImageId = 6,
                    Metrics =
                    [
                        new UpdateMetricDto
                        {
                            Value = "321",
                            Signature = "brand-new-metric",
                        },
                    ],
                },
            ],
        });

        SetupValidationSuccess();

        _mainPageRepositoryMock
            .SetupSequence(x => x.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<DAL.Entities.MainPage>?>()))
            .ReturnsAsync(entity)
            .ReturnsAsync(entity);

        _imageRepositoryMock
            .Setup(x => x.GetAllAsync(It.IsAny<QueryOptions<Image>?>()))
            .ReturnsAsync(new List<Image>
            {
                new() { Id = 5 },
                new() { Id = 6 },
            });

        _mapperMock
            .Setup(x => x.Map(command.UpdateMainPageDto.MainAboutUs, entity.MainAboutUs))
            .Verifiable();
        _mapperMock
            .Setup(x => x.Map(command.UpdateMainPageDto.MainPartners, entity.MainPartners))
            .Verifiable();

        _mapperMock
            .Setup(x => x.Map<Metric>(It.IsAny<UpdateMetricDto>()))
            .Returns((UpdateMetricDto dto) => new Metric
            {
                Value = dto.Value,
                Signature = dto.Signature,
            });

        _mapperMock
            .Setup(x => x.Map<ImpactStatistics>(It.Is<UpdateImpactStatisticDto>(d => !d.Id.HasValue)))
            .Returns((UpdateImpactStatisticDto dto) => new ImpactStatistics
            {
                Description = dto.Description,
                ImageId = dto.ImageId,
                Metrics = dto.Metrics.Select(m => new Metric
                {
                    Value = m.Value,
                    Signature = m.Signature,
                }).ToList(),
            });

        _repositoryWrapperMock
            .Setup(x => x.SaveChangesAsync())
            .ReturnsAsync(1);

        _mapperMock
            .Setup(x => x.Map<DAL.Entities.MainPage, MainPageDto>(entity))
            .Returns(new MainPageDto
            {
                Id = entity.Id,
                Title = command.UpdateMainPageDto.Title,
                Description = command.UpdateMainPageDto.Description,
            });

        var handler = CreateHandler();

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);

        Assert.Contains(entity.ImpactStatistics, s => s.Id == 13 && s.Description == "Updated existing stat");
        Assert.Contains(entity.ImpactStatistics.SelectMany(s => s.Metrics), m => m.Id == 14 && m.Value == "999");

        Assert.True(entity.ImpactStatistics.Count >= 2);
        Assert.Contains(entity.ImpactStatistics, s => s.Description == "Brand new stat");
    }

    private UpdateMainPageHandler CreateHandler() => new(
        _repositoryWrapperMock.Object,
        _mapperMock.Object,
        _validatorMock.Object);

    private void SetupValidationSuccess()
    {
        _validatorMock
            .Setup(x => x.ValidateAsync(It.IsAny<ValidationContext<UpdateMainPageCommand>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());
    }

    private void SetupValidationFailure(string errorMessage)
    {
        _validatorMock
            .Setup(x => x.ValidateAsync(It.IsAny<ValidationContext<UpdateMainPageCommand>>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new ValidationException(new[]
            {
                new ValidationFailure("UpdateMainPageDto", errorMessage),
            }));
    }

    private static UpdateMainPageDto GetValidUpdateDto() => new()
    {
        Title = "Updated main page title",
        Description = "Updated main page description long enough",
        ImageId = 5,
        MainAboutUs = new UpdateMainAboutUsDto
        {
            Title = "Updated about us title",
            Description = "Updated about us description long enough",
        },
        MainPartners = new UpdateMainPartnersDto
        {
            Title = "Updated partners title",
            Description = "Updated partners description long enough",
        },
        ImpactStatistics =
        [
            new UpdateImpactStatisticDto
            {
                Description = "Updated impact statistic description",
                ImageId = 6,
                Metrics =
                [
                    new UpdateMetricDto { Value = "250", Signature = "families" },
                ],
            },
        ],
    };

    private static UpdateMainPageDto GetValidUpdateDto(DAL.Entities.MainPage existing) => new()
    {
        Title = "Updated main page title",
        Description = "Updated main page description long enough",
        ImageId = 5,
        MainAboutUs = new UpdateMainAboutUsDto
        {
            Title = "Updated about us title",
            Description = "Updated about us description long enough",
        },
        MainPartners = new UpdateMainPartnersDto
        {
            Title = "Updated partners title",
            Description = "Updated partners description long enough",
        },
        ImpactStatistics =
        [
            new UpdateImpactStatisticDto
            {
                Id = existing.ImpactStatistics.Single().Id,
                Description = "Updated impact statistic description",
                ImageId = 6,
                Metrics =
                [
                    new UpdateMetricDto
                    {
                        Id = existing.ImpactStatistics.Single().Metrics.Single().Id,
                        Value = "250",
                        Signature = "families",
                    },
                ],
            },
        ],
    };

    private static DAL.Entities.MainPage GetExistingMainPageEntity(long id) => new()
    {
        Id = id,
        Title = "Old title",
        Description = "Old description",
        ImageId = 1,
        MainAboutUs = new MainAboutUs
        {
            Id = 11,
            Title = "Old about us title",
            Description = "Old about us description",
        },
        MainPartners = new MainPartners
        {
            Id = 12,
            Title = "Old partners title",
            Description = "Old partners description",
        },
        ImpactStatistics =
        [
            new ImpactStatistics
            {
                Id = 13,
                Description = "Old impact statistic",
                ImageId = 2,
                Metrics =
                [
                    new Metric
                    {
                        Id = 14,
                        Value = "100",
                        Signature = "children",
                    },
                ],
            },
        ],
    };
}