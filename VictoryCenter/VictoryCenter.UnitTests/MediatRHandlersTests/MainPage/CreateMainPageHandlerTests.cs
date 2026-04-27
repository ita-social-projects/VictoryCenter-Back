using System.Transactions;
using AutoMapper;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;
using Moq;
using VictoryCenter.BLL.Commands.Admin.MainPage.Create;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.ImpactStatistics;
using VictoryCenter.BLL.DTOs.Admin.ImpactStatistics.Metrics;
using VictoryCenter.BLL.DTOs.Admin.Localization.MainPage;
using VictoryCenter.BLL.DTOs.Admin.Localization.MainPage.Metrics;
using VictoryCenter.BLL.DTOs.Admin.MainAboutUs;
using VictoryCenter.BLL.DTOs.Admin.MainPages;
using VictoryCenter.BLL.DTOs.Admin.MainPartners;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Entities.Localization;
using VictoryCenter.DAL.Enums;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Interfaces.Localization.MainPage;
using VictoryCenter.DAL.Repositories.Interfaces.MainPage;
using VictoryCenter.DAL.Repositories.Interfaces.Media;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.UnitTests.MediatRHandlersTests.MainPage;

public class CreateMainPageHandlerTests
{
    private readonly Mock<IRepositoryWrapper> _repositoryWrapperMock = new();
    private readonly Mock<IMainPageRepository> _mainPageRepositoryMock = new();
    private readonly Mock<IImageRepository> _imageRepositoryMock = new();
    private readonly Mock<IMapper> _mapperMock = new();
    private readonly Mock<IValidator<CreateMainPageCommand>> _validatorMock = new();

    public CreateMainPageHandlerTests()
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
    public async Task Handle_ShouldCreateMainPage_WhenRequestIsValid()
    {
        // Arrange
        var command = new CreateMainPageCommand(GetValidCreateDto());
        var createdEntity = GetMainPageEntity(10);
        var resultDto = GetMainPageDto(10);

        SetupValidationSuccess();

        _mainPageRepositoryMock
            .Setup(x => x.CountAsync(It.IsAny<QueryOptions<DAL.Entities.MainPage>?>()))
            .ReturnsAsync(0);

        _imageRepositoryMock
            .Setup(x => x.GetAllAsync(It.IsAny<QueryOptions<Image>?>()))
            .ReturnsAsync(new List<Image>
            {
                new() { Id = 5 },
                new() { Id = 6 },
            });

        _mapperMock
            .Setup(x => x.Map<CreateMainPageDto, DAL.Entities.MainPage>(command.CreateMainPageDto))
            .Returns(createdEntity);

        _mainPageRepositoryMock
            .Setup(x => x.CreateAsync(createdEntity))
            .ReturnsAsync(createdEntity);

        _repositoryWrapperMock
            .Setup(x => x.SaveChangesAsync())
            .ReturnsAsync(1);

        _mainPageRepositoryMock
            .Setup(x => x.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<DAL.Entities.MainPage>?>()))
            .ReturnsAsync(createdEntity);

        _mapperMock
            .Setup(x => x.Map<DAL.Entities.MainPage, MainPageDto>(createdEntity))
            .Returns(resultDto);

        var handler = CreateHandler();

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(resultDto.Id, result.Value.Id);

        _mainPageRepositoryMock.Verify(x => x.CreateAsync(createdEntity), Times.Once);
        _repositoryWrapperMock.Verify(x => x.SaveChangesAsync(), Times.Once);
        _imageRepositoryMock.Verify(x => x.GetAllAsync(It.IsAny<QueryOptions<Image>?>()), Times.Once);
        _validatorMock.Verify(
            x => x.ValidateAsync(It.IsAny<ValidationContext<CreateMainPageCommand>>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenValidationFails()
    {
        // Arrange
        var command = new CreateMainPageCommand(GetValidCreateDto());
        SetupValidationFailure("Validation failed");

        var handler = CreateHandler();

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("Validation failed", result.Errors[0].Message);

        _repositoryWrapperMock.Verify(x => x.BeginTransaction(), Times.Never);
        _mainPageRepositoryMock.Verify(x => x.CreateAsync(It.IsAny<DAL.Entities.MainPage>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenMainPageAlreadyExists()
    {
        // Arrange
        var command = new CreateMainPageCommand(GetValidCreateDto());
        SetupValidationSuccess();

        _mainPageRepositoryMock
            .Setup(x => x.CountAsync(It.IsAny<QueryOptions<DAL.Entities.MainPage>?>()))
            .ReturnsAsync(1);

        var handler = CreateHandler();

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(
            ErrorMessagesConstants.OnlyOneEntityOfTypeIsAllowed(nameof(DAL.Entities.MainPage)),
            result.Errors[0].Message);

        _imageRepositoryMock.Verify(x => x.GetAllAsync(It.IsAny<QueryOptions<Image>?>()), Times.Never);
        _mainPageRepositoryMock.Verify(x => x.CreateAsync(It.IsAny<DAL.Entities.MainPage>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenImageDoesNotExist()
    {
        // Arrange
        var command = new CreateMainPageCommand(GetValidCreateDto());
        SetupValidationSuccess();

        _mainPageRepositoryMock
            .Setup(x => x.CountAsync(It.IsAny<QueryOptions<DAL.Entities.MainPage>?>()))
            .ReturnsAsync(0);

        _imageRepositoryMock
            .Setup(x => x.GetAllAsync(It.IsAny<QueryOptions<Image>?>()))
            .ReturnsAsync(new List<Image>
            {
                new() { Id = 5 },
            });

        var handler = CreateHandler();

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(
            ErrorMessagesConstants.NotFound(new[] { 6L }, typeof(Image)),
            result.Errors[0].Message);

        _mainPageRepositoryMock.Verify(x => x.CreateAsync(It.IsAny<DAL.Entities.MainPage>()), Times.Never);
        _repositoryWrapperMock.Verify(x => x.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenCreatedEntityCannotBeLoaded()
    {
        // Arrange
        var dtoWithoutImages = GetValidCreateDto();
        dtoWithoutImages = dtoWithoutImages with
        {
            ImageId = null,
            ImpactStatistics = new CreateImpactStatisticDto
            {
                Title = "Impact statistic title",
                ImageId = null,
                Metrics =
                [
                    new CreateMetricDto { Value = 100, Name = "Partners", Type = MetricType.Partners },
                    new CreateMetricDto { Value = 200, Name = "Programs", Type = MetricType.Programs },
                    new CreateMetricDto { Value = 300, Name = "Raised", Type = MetricType.Raised },
                    new CreateMetricDto { Value = 400, Name = "Therapy", Type = MetricType.TherapyHours },
                ],
            },
        };

        var command = new CreateMainPageCommand(dtoWithoutImages);
        var createdEntity = GetMainPageEntity(99);

        SetupValidationSuccess();

        _mainPageRepositoryMock
            .Setup(x => x.CountAsync(It.IsAny<QueryOptions<DAL.Entities.MainPage>?>()))
            .ReturnsAsync(0);

        _mapperMock
            .Setup(x => x.Map<CreateMainPageDto, DAL.Entities.MainPage>(command.CreateMainPageDto))
            .Returns(createdEntity);

        _mainPageRepositoryMock
            .Setup(x => x.CreateAsync(createdEntity))
            .ReturnsAsync(createdEntity);

        _repositoryWrapperMock
            .Setup(x => x.SaveChangesAsync())
            .ReturnsAsync(1);

        _mainPageRepositoryMock
            .Setup(x => x.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<DAL.Entities.MainPage>?>()))
            .ReturnsAsync((DAL.Entities.MainPage?)null);

        var handler = CreateHandler();

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(
            ErrorMessagesConstants.FailedToCreateEntity(typeof(DAL.Entities.MainPage)),
            result.Errors[0].Message);

        _imageRepositoryMock.Verify(x => x.GetAllAsync(It.IsAny<QueryOptions<Image>?>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenDbUpdateExceptionOccurs()
    {
        // Arrange
        var command = new CreateMainPageCommand(GetValidCreateDto());
        var createdEntity = GetMainPageEntity(77);

        SetupValidationSuccess();

        _mainPageRepositoryMock
            .Setup(x => x.CountAsync(It.IsAny<QueryOptions<DAL.Entities.MainPage>?>()))
            .ReturnsAsync(0);

        _imageRepositoryMock
            .Setup(x => x.GetAllAsync(It.IsAny<QueryOptions<Image>?>()))
            .ReturnsAsync(new List<Image>
            {
                new() { Id = 5 },
                new() { Id = 6 },
            });

        _mapperMock
            .Setup(x => x.Map<CreateMainPageDto, DAL.Entities.MainPage>(command.CreateMainPageDto))
            .Returns(createdEntity);

        _mainPageRepositoryMock
            .Setup(x => x.CreateAsync(createdEntity))
            .ReturnsAsync(createdEntity);

        _repositoryWrapperMock
            .Setup(x => x.SaveChangesAsync())
            .ThrowsAsync(new DbUpdateException());

        var handler = CreateHandler();

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(
            ErrorMessagesConstants.FailedToCreateEntityInDatabase(typeof(DAL.Entities.MainPage)),
            result.Errors[0].Message);
    }

    [Fact]
    public async Task Handle_ShouldCreateMainPage_WhenImpactStatisticsIsNull()
    {
        // Arrange
        var dto = GetValidCreateDto() with { ImageId = null, ImpactStatistics = null };
        var command = new CreateMainPageCommand(dto);
        var entity = GetMainPageEntity(20);
        entity.ImpactStatistics = null;
        var resultDto = GetMainPageDto(20);

        SetupValidationSuccess();

        _mainPageRepositoryMock
            .Setup(x => x.CountAsync(It.IsAny<QueryOptions<DAL.Entities.MainPage>?>()))
            .ReturnsAsync(0);

        _mapperMock
            .Setup(x => x.Map<CreateMainPageDto, DAL.Entities.MainPage>(command.CreateMainPageDto))
            .Returns(entity);

        _mainPageRepositoryMock
            .Setup(x => x.CreateAsync(entity))
            .ReturnsAsync(entity);

        _repositoryWrapperMock
            .Setup(x => x.SaveChangesAsync())
            .ReturnsAsync(1);

        _mainPageRepositoryMock
            .Setup(x => x.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<DAL.Entities.MainPage>?>()))
            .ReturnsAsync(entity);

        _mapperMock
            .Setup(x => x.Map<DAL.Entities.MainPage, MainPageDto>(entity))
            .Returns(resultDto);

        var handler = CreateHandler();

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        _repositoryWrapperMock.Verify(x => x.SaveChangesAsync(), Times.Once);
        _imageRepositoryMock.Verify(x => x.GetAllAsync(It.IsAny<QueryOptions<Image>?>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldSaveLocalizations_WhenLocalizationsAreProvided()
    {
        // Arrange
        var impactStatLocRepoMock = new Mock<IImpactStatisticsLocalizationsRepository>();
        var metricLocRepoMock = new Mock<IMetricLocalizationsRepository>();

        _repositoryWrapperMock
            .SetupGet(x => x.ImpactStatisticsLocalizationsRepository)
            .Returns(impactStatLocRepoMock.Object);

        _repositoryWrapperMock
            .SetupGet(x => x.MetricLocalizationsRepository)
            .Returns(metricLocRepoMock.Object);

        var dto = GetValidCreateDto() with
        {
            ImageId = null,
            ImpactStatistics = new CreateImpactStatisticDto
            {
                Title = "Impact title",
                Localization = new CreateImpactStatisticLocalizationDto { LanguageId = 1, Title = "Вплив" },
                Metrics =
                [
                    new CreateMetricDto { Value = 100, Name = "Partners", Type = MetricType.Partners },
                    new CreateMetricDto { Value = 200, Name = "Programs", Type = MetricType.Programs },
                    new CreateMetricDto
                    {
                        Value = 300, Name = "Raised", Type = MetricType.Raised,
                        Localization = new CreateMetricLocalizationDto { LanguageId = 1, Name = "Зібрано" },
                    },
                    new CreateMetricDto { Value = 400, Name = "Therapy", Type = MetricType.TherapyHours },
                ],
            },
        };

        var command = new CreateMainPageCommand(dto);
        var entity = GetMainPageEntity(30);
        var resultDto = GetMainPageDto(30);

        SetupValidationSuccess();

        _mainPageRepositoryMock
            .Setup(x => x.CountAsync(It.IsAny<QueryOptions<DAL.Entities.MainPage>?>()))
            .ReturnsAsync(0);

        _mapperMock
            .Setup(x => x.Map<CreateMainPageDto, DAL.Entities.MainPage>(command.CreateMainPageDto))
            .Returns(entity);

        _mainPageRepositoryMock
            .Setup(x => x.CreateAsync(entity))
            .ReturnsAsync(entity);

        _repositoryWrapperMock
            .Setup(x => x.SaveChangesAsync())
            .ReturnsAsync(1);

        impactStatLocRepoMock
            .Setup(x => x.CreateAsync(It.IsAny<ImpactStatisticsLocalization>()))
            .ReturnsAsync(new ImpactStatisticsLocalization());

        metricLocRepoMock
            .Setup(x => x.CreateAsync(It.IsAny<MetricLocalization>()))
            .ReturnsAsync(new MetricLocalization());

        _mainPageRepositoryMock
            .Setup(x => x.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<DAL.Entities.MainPage>?>()))
            .ReturnsAsync(entity);

        _mapperMock
            .Setup(x => x.Map<DAL.Entities.MainPage, MainPageDto>(entity))
            .Returns(resultDto);

        var handler = CreateHandler();

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        impactStatLocRepoMock.Verify(x => x.CreateAsync(It.IsAny<ImpactStatisticsLocalization>()), Times.Once);
        metricLocRepoMock.Verify(x => x.CreateAsync(It.IsAny<MetricLocalization>()), Times.Once);
        _repositoryWrapperMock.Verify(x => x.SaveChangesAsync(), Times.Exactly(2));
    }

    private CreateMainPageHandler CreateHandler() => new(
        _repositoryWrapperMock.Object,
        _mapperMock.Object,
        _validatorMock.Object);

    private void SetupValidationSuccess()
    {
        _validatorMock
            .Setup(x => x.ValidateAsync(It.IsAny<ValidationContext<CreateMainPageCommand>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());
    }

    private void SetupValidationFailure(string errorMessage)
    {
        _validatorMock
            .Setup(x => x.ValidateAsync(It.IsAny<ValidationContext<CreateMainPageCommand>>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new ValidationException(new[]
            {
                new ValidationFailure("CreateMainPageDto", errorMessage),
            }));
    }

    private static CreateMainPageDto GetValidCreateDto() => new()
    {
        Title = "Main page title",
        Description = "Main page description that is long enough",
        ImageId = 5,
        MainAboutUs = new CreateMainAboutUsDto
        {
            Title = "About us title",
            Description = "About us description that is long enough",
        },
        MainPartners = new CreateMainPartnersDto
        {
            Title = "Partners title",
            Description = "Partners description that is long enough",
        },
        ImpactStatistics = new CreateImpactStatisticDto
        {
            Title = "Impact statistic title",
            ImageId = 6,
            Metrics =
            [
                new CreateMetricDto { Value = 100, Name = "Partners", Type = MetricType.Partners },
                new CreateMetricDto { Value = 200, Name = "Programs", Type = MetricType.Programs },
                new CreateMetricDto { Value = 300, Name = "Raised", Type = MetricType.Raised },
                new CreateMetricDto { Value = 400, Name = "Therapy", Type = MetricType.TherapyHours },
            ],
        },
    };

    private static DAL.Entities.MainPage GetMainPageEntity(long id) => new()
    {
        Id = id,
        Title = "Main page title",
        Description = "Main page description",
        ImageId = 5,
        MainAboutUs = new MainAboutUs
        {
            Id = 11,
            Title = "About us title",
            Description = "About us description",
        },
        MainPartners = new MainPartners
        {
            Id = 12,
            Title = "Partners title",
            Description = "Partners description",
        },
        ImpactStatistics = new ImpactStatistics
        {
            Id = 13,
            Title = "Impact statistic",
            ImageId = 6,
            Metrics =
            [
                new Metric { Id = 14, Value = 100, Name = "Partners", Type = MetricType.Partners },
                new Metric { Id = 15, Value = 200, Name = "Programs", Type = MetricType.Programs },
                new Metric { Id = 16, Value = 300, Name = "Raised", Type = MetricType.Raised },
                new Metric { Id = 17, Value = 400, Name = "Therapy", Type = MetricType.TherapyHours },
            ],
        },
    };

    private static MainPageDto GetMainPageDto(long id) => new()
    {
        Id = id,
        Title = "Main page title",
        Description = "Main page description",
        MainAboutUs = new MainAboutUsDto
        {
            Id = 11,
            Title = "About us title",
            Description = "About us description",
        },
        MainPartners = new MainPartnersDto
        {
            Id = 12,
            Title = "Partners title",
            Description = "Partners description",
        },
        ImpactStatistics = new ImpactStatisticDto
        {
            Id = 13,
            Title = "Impact statistic",
            Metrics =
            [
                new MetricDto { Id = 14, Value = 100, Name = "Partners", Type = MetricType.Partners },
                new MetricDto { Id = 15, Value = 200, Name = "Programs", Type = MetricType.Programs },
                new MetricDto { Id = 16, Value = 300, Name = "Raised", Type = MetricType.Raised },
                new MetricDto { Id = 17, Value = 400, Name = "Therapy", Type = MetricType.TherapyHours },
            ],
        },
    };
}
