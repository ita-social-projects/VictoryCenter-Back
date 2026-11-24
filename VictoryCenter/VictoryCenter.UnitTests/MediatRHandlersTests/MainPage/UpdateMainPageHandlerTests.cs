using System.Transactions;
using AutoMapper;
using FluentValidation;
using FluentValidation.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Moq;
using VictoryCenter.BLL.Commands.Admin.MainPage.Update;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.ImpactStatistics;
using VictoryCenter.BLL.DTOs.Admin.ImpactStatistics.Metrics;
using VictoryCenter.BLL.DTOs.Admin.Localization.MainPage;
using VictoryCenter.BLL.DTOs.Admin.Localization.MainPage.Metrics;
using VictoryCenter.BLL.DTOs.Admin.MainAboutUs;
using VictoryCenter.BLL.DTOs.Admin.MainDonations;
using VictoryCenter.BLL.DTOs.Admin.MainPages;
using VictoryCenter.BLL.DTOs.Admin.MainPartners;
using VictoryCenter.BLL.Notifications.ReportFunds;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Entities.Localization;
using VictoryCenter.DAL.Enums;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Interfaces.Localization.MainPage;
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
    private readonly Mock<IMediator> _mediatorMock = new();
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
                new() { Id = 7 },
            });

        _mapperMock
            .Setup(x => x.Map(command.UpdateMainPageDto.MainAboutUs, existingEntity.MainAboutUs))
            .Verifiable();
        _mapperMock
            .Setup(x => x.Map(command.UpdateMainPageDto.MainPartners, existingEntity.MainPartners))
            .Verifiable();
        _mapperMock
            .Setup(x => x.Map(command.UpdateMainPageDto.MainDonations, existingEntity.MainDonations))
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

        var updatedStat = existingEntity.ImpactStatistics!;
        Assert.Equal(command.UpdateMainPageDto.ImpactStatistics!.Title, updatedStat.Title);

        var updatedMetric = updatedStat.Metrics.Single();
        Assert.Equal(command.UpdateMainPageDto.ImpactStatistics!.Metrics.Single().Value, updatedMetric.Value);
        Assert.Equal(command.UpdateMainPageDto.ImpactStatistics!.Metrics.Single().Name, updatedMetric.Name);

        _mapperMock.Verify(x => x.Map(command.UpdateMainPageDto.MainDonations, existingEntity.MainDonations), Times.Once);
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

        _repositoryWrapperMock.Verify(x => x.BeginTransaction(), Times.Once);
        _repositoryWrapperMock.Verify(x => x.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldCreateMainDonations_WhenMainDonationsDoesNotExist()
    {
        // Arrange
        var existingEntity = GetExistingMainPageEntity(1);
        existingEntity.MainDonations = null;

        var command = new UpdateMainPageCommand(GetValidUpdateDto(existingEntity));
        var mappedDonations = new MainDonations
        {
            Title = command.UpdateMainPageDto.MainDonations!.Title,
            Description = command.UpdateMainPageDto.MainDonations.Description,
            ImageId = command.UpdateMainPageDto.MainDonations.ImageId,
        };

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
                new() { Id = 7 },
            });

        _mapperMock
            .Setup(x => x.Map(command.UpdateMainPageDto.MainAboutUs, existingEntity.MainAboutUs))
            .Verifiable();
        _mapperMock
            .Setup(x => x.Map(command.UpdateMainPageDto.MainPartners, existingEntity.MainPartners))
            .Verifiable();
        _mapperMock
            .Setup(x => x.Map<MainDonations>(command.UpdateMainPageDto.MainDonations))
            .Returns(mappedDonations);

        _repositoryWrapperMock
            .Setup(x => x.SaveChangesAsync())
            .ReturnsAsync(1);

        _mapperMock
            .Setup(x => x.Map<DAL.Entities.MainPage, MainPageDto>(existingEntity))
            .Returns(new MainPageDto { Id = existingEntity.Id });

        var handler = CreateHandler();

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(existingEntity.MainDonations);
        Assert.Equal(command.UpdateMainPageDto.MainDonations!.Title, existingEntity.MainDonations!.Title);
        _repositoryWrapperMock.Verify(x => x.SaveChangesAsync(), Times.Once);
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
            ErrorMessagesConstants.NotFound(new[] { 6L, 7L }, typeof(Image)),
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
                new() { Id = 7 },
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
                new() { Id = 7 },
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
    public async Task Handle_ShouldOnlyUpdateExistingMetricsAndKeepTheirKinds_WhenRequestContainsUnknownMetrics()
    {
        // Arrange
        var entity = GetExistingMainPageEntity(1);
        var existingMetric = entity.ImpactStatistics!.Metrics.Single();

        var command = new UpdateMainPageCommand(GetValidUpdateDto(entity) with
        {
            ImpactStatistics = GetValidUpdateDto(entity).ImpactStatistics! with
            {
                Metrics =
                [
                    new UpdateMetricDto
                    {
                        Id = existingMetric.Id,
                        Value = 999,
                        Name = "renamed-metric",
                        Kind = MetricKind.Standard,
                    },
                    new UpdateMetricDto
                    {
                        Value = 123,
                        Name = "new-metric",
                        Kind = MetricKind.Standard,
                    },
                ],
            },
        });

        SetupSuccessfulUpdate(entity);

        var handler = CreateHandler();

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);

        var metric = Assert.Single(entity.ImpactStatistics.Metrics);
        Assert.Same(existingMetric, metric);
        Assert.Equal(999, metric.Value);
        Assert.Equal("renamed-metric", metric.Name);
        Assert.Equal(MetricKind.RaisedFunds, metric.Kind);
        _mapperMock.Verify(x => x.Map<Metric>(It.IsAny<UpdateMetricDto>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldNotPublishReportFundsChangedNotification_WhenRequestClaimsRaisedFundsKindForStandardMetric()
    {
        // Arrange
        var entity = GetExistingMainPageEntity(1);
        var existingMetric = entity.ImpactStatistics!.Metrics.Single();
        existingMetric.Kind = MetricKind.Standard;

        var command = new UpdateMainPageCommand(GetValidUpdateDto(entity) with
        {
            ImpactStatistics = GetValidUpdateDto(entity).ImpactStatistics! with
            {
                Metrics =
                [
                    new UpdateMetricDto
                    {
                        Id = existingMetric.Id,
                        Value = 500,
                        Name = "partners",
                        Kind = MetricKind.RaisedFunds,
                        IsAutoSynced = true,
                    },
                ],
            },
        });

        SetupSuccessfulUpdate(entity);

        var handler = CreateHandler();

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(MetricKind.Standard, existingMetric.Kind);
        _mediatorMock.Verify(
            mediator => mediator.Publish(
                It.IsAny<ReportFundsChangedNotification>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldCreateMetricLocalizationsInRequestOrder_WhenImpactStatisticsIsCreated()
    {
        // Arrange
        var entity = GetExistingMainPageEntity(1);
        entity.ImpactStatistics = null;

        var metricDtos = Enumerable.Range(0, 4)
            .Select(index => new UpdateMetricDto
            {
                Value = 100 * (index + 1),
                Name = $"metric-{index}",
                Kind = index == 2 ? MetricKind.RaisedFunds : MetricKind.Standard,
                Localization = new UpdateMetricLocalizationDto { LanguageId = 2, Name = $"localized-{index}" },
            })
            .ToList();

        var command = new UpdateMainPageCommand(GetValidUpdateDto() with
        {
            ImpactStatistics = GetValidUpdateDto().ImpactStatistics! with { Metrics = metricDtos },
        });

        var createdImpactStatistics = new ImpactStatistics
        {
            Id = 13,
            Title = "Updated impact statistic title",
            Metrics = metricDtos
                .Select((dto, index) => new Metric { Id = 40 + index, Value = dto.Value, Name = dto.Name, Kind = dto.Kind })
                .ToList(),
        };

        _mapperMock
            .Setup(x => x.Map<ImpactStatistics>(command.UpdateMainPageDto.ImpactStatistics))
            .Returns(createdImpactStatistics);

        var metricLocalizationsRepositoryMock = new Mock<IMetricLocalizationsRepository>();
        var createdLocalizations = new List<MetricLocalization>();
        metricLocalizationsRepositoryMock
            .Setup(x => x.CreateAsync(It.IsAny<MetricLocalization>()))
            .Callback<MetricLocalization>(createdLocalizations.Add)
            .ReturnsAsync((MetricLocalization localization) => localization);
        _repositoryWrapperMock
            .SetupGet(x => x.MetricLocalizationsRepository)
            .Returns(metricLocalizationsRepositoryMock.Object);

        SetupSuccessfulUpdate(entity);

        var handler = CreateHandler();

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(new long[] { 40, 41, 42, 43 }, createdLocalizations.Select(l => l.EntityId));
        Assert.Equal(
            new[] { "localized-0", "localized-1", "localized-2", "localized-3" },
            createdLocalizations.Select(l => l.Name));
    }

    [Fact]
    public async Task Handle_ShouldPublishReportFundsChangedNotification_WhenRaisedMetricIsAutoSynced()
    {
        // Arrange
        var entity = GetExistingMainPageEntity(1);
        var updateDto = GetValidUpdateDto(entity) with
        {
            ImpactStatistics = GetValidUpdateDto(entity).ImpactStatistics! with
            {
                Metrics =
                [
                    new UpdateMetricDto
                    {
                        Id = entity.ImpactStatistics!.Metrics.First().Id,
                        Value = 500,
                        Name = "raised",
                        Kind = MetricKind.RaisedFunds,
                        IsAutoSynced = true,
                    },
                ],
            },
        };

        var command = new UpdateMainPageCommand(updateDto);

        SetupValidationSuccess();

        _mainPageRepositoryMock
            .SetupSequence(x => x.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<DAL.Entities.MainPage>?>()))
            .ReturnsAsync(entity)
            .ReturnsAsync(entity);

        _imageRepositoryMock
            .Setup(x => x.GetAllAsync(It.IsAny<QueryOptions<Image>?>()))
            .ReturnsAsync([new Image { Id = 5 }, new Image { Id = 6 }, new Image { Id = 7 }]);

        _mapperMock
            .Setup(x => x.Map(command.UpdateMainPageDto.MainAboutUs, entity.MainAboutUs))
            .Verifiable();
        _mapperMock
            .Setup(x => x.Map(command.UpdateMainPageDto.MainPartners, entity.MainPartners))
            .Verifiable();
        _mapperMock
            .Setup(x => x.Map(command.UpdateMainPageDto.MainDonations, entity.MainDonations))
            .Verifiable();

        _repositoryWrapperMock
            .Setup(x => x.SaveChangesAsync())
            .ReturnsAsync(1);

        _mediatorMock
            .Setup(mediator => mediator.Publish(
                It.IsAny<ReportFundsChangedNotification>(),
                It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _mapperMock
            .Setup(x => x.Map<DAL.Entities.MainPage, MainPageDto>(entity))
            .Returns(new MainPageDto { Id = entity.Id });

        var handler = CreateHandler();

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.True(entity.ImpactStatistics!.Metrics.Single().IsAutoSynced);
        _mediatorMock.Verify(
            mediator => mediator.Publish(
                It.IsAny<ReportFundsChangedNotification>(),
                It.Is<CancellationToken>(token => token == CancellationToken.None)),
            Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldNotPublishReportFundsChangedNotification_WhenRaisedMetricIsNotAutoSynced()
    {
        // Arrange
        var entity = GetExistingMainPageEntity(1);
        var updateDto = GetValidUpdateDto(entity) with
        {
            ImpactStatistics = GetValidUpdateDto(entity).ImpactStatistics! with
            {
                Metrics =
                [
                    new UpdateMetricDto
                    {
                        Id = entity.ImpactStatistics!.Metrics.First().Id,
                        Value = 500,
                        Name = "raised",
                        Kind = MetricKind.RaisedFunds,
                        IsAutoSynced = false,
                    },
                ],
            },
        };

        var command = new UpdateMainPageCommand(updateDto);

        SetupValidationSuccess();

        _mainPageRepositoryMock
            .SetupSequence(x => x.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<DAL.Entities.MainPage>?>()))
            .ReturnsAsync(entity)
            .ReturnsAsync(entity);

        _imageRepositoryMock
            .Setup(x => x.GetAllAsync(It.IsAny<QueryOptions<Image>?>()))
            .ReturnsAsync([new Image { Id = 5 }, new Image { Id = 6 }, new Image { Id = 7 }]);

        _mapperMock
            .Setup(x => x.Map(command.UpdateMainPageDto.MainAboutUs, entity.MainAboutUs))
            .Verifiable();
        _mapperMock
            .Setup(x => x.Map(command.UpdateMainPageDto.MainPartners, entity.MainPartners))
            .Verifiable();
        _mapperMock
            .Setup(x => x.Map(command.UpdateMainPageDto.MainDonations, entity.MainDonations))
            .Verifiable();

        _repositoryWrapperMock
            .Setup(x => x.SaveChangesAsync())
            .ReturnsAsync(1);

        _mapperMock
            .Setup(x => x.Map<DAL.Entities.MainPage, MainPageDto>(entity))
            .Returns(new MainPageDto { Id = entity.Id });

        var handler = CreateHandler();

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.False(entity.ImpactStatistics!.Metrics.Single().IsAutoSynced);
        _mediatorMock.Verify(
            mediator => mediator.Publish(
                It.IsAny<ReportFundsChangedNotification>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldMarkOnlyTitleBlockTranslationsOutdated_WhenTitleBlockSourceChanges()
    {
        // Arrange
        var entity = GetExistingMainPageEntity(1);
        entity.Localizations =
        [
            new MainPageLocalization { EntityId = entity.Id, LanguageId = 2, TranslationStatus = TranslationStatus.Relevant },
        ];
        entity.MainAboutUs!.Localizations =
        [
            new MainAboutUsLocalization { EntityId = entity.MainAboutUs.Id, LanguageId = 2, TranslationStatus = TranslationStatus.Relevant },
        ];

        var updateDto = GetSourceMatchingUpdateDto(entity) with
        {
            Title = "Changed source title",
        };
        var command = new UpdateMainPageCommand(updateDto);

        SetupSuccessfulUpdate(entity);

        var handler = CreateHandler();

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(TranslationStatus.Outdated, entity.Localizations.Single().TranslationStatus);
        Assert.Equal(TranslationStatus.Relevant, entity.MainAboutUs.Localizations.Single().TranslationStatus);
    }

    [Fact]
    public async Task Handle_ShouldMarkOnlyChangedMetricTranslationsOutdated_WhenMetricSourceChanges()
    {
        // Arrange
        var entity = GetExistingMainPageEntity(1);
        var metric = entity.ImpactStatistics!.Metrics.Single();
        entity.ImpactStatistics.Localizations =
        [
            new ImpactStatisticsLocalization
            {
                EntityId = entity.ImpactStatistics.Id,
                LanguageId = 2,
                TranslationStatus = TranslationStatus.Relevant,
            },
        ];
        metric.Localizations =
        [
            new MetricLocalization { EntityId = metric.Id, LanguageId = 2, TranslationStatus = TranslationStatus.Relevant },
        ];

        var updateDto = GetSourceMatchingUpdateDto(entity) with
        {
            ImpactStatistics = GetSourceMatchingUpdateDto(entity).ImpactStatistics! with
            {
                Metrics =
                [
                    new UpdateMetricDto
                    {
                        Id = metric.Id,
                        Value = metric.Value + 1,
                        Name = metric.Name,
                        Kind = metric.Kind,
                    },
                ],
            },
        };
        var command = new UpdateMainPageCommand(updateDto);

        SetupSuccessfulUpdate(entity);

        var handler = CreateHandler();

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(TranslationStatus.Relevant, entity.ImpactStatistics.Localizations.Single().TranslationStatus);
        Assert.Equal(TranslationStatus.Outdated, metric.Localizations.Single().TranslationStatus);
    }

    [Fact]
    public async Task Handle_ShouldCreateImpactStatisticsLocalization_WhenLanguageHasNoTranslation()
    {
        // Arrange
        var entity = GetExistingMainPageEntity(1);
        var sourceDto = GetSourceMatchingUpdateDto(entity);
        var updateDto = sourceDto with
        {
            ImpactStatistics = sourceDto.ImpactStatistics! with
            {
                Localization = new UpdateImpactStatisticLocalizationDto { LanguageId = 2, Title = "Localized impact statistic" },
            },
        };

        SetupSuccessfulUpdate(entity);

        var localizationsRepositoryMock = new Mock<IImpactStatisticsLocalizationsRepository>();
        var createdLocalizations = new List<ImpactStatisticsLocalization>();
        localizationsRepositoryMock
            .Setup(x => x.CreateAsync(It.IsAny<ImpactStatisticsLocalization>()))
            .Callback<ImpactStatisticsLocalization>(createdLocalizations.Add)
            .ReturnsAsync((ImpactStatisticsLocalization localization) => localization);
        _repositoryWrapperMock
            .SetupGet(x => x.ImpactStatisticsLocalizationsRepository)
            .Returns(localizationsRepositoryMock.Object);

        var handler = CreateHandler();

        // Act
        var result = await handler.Handle(new UpdateMainPageCommand(updateDto), CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        var created = Assert.Single(createdLocalizations);
        Assert.Equal(entity.ImpactStatistics!.Id, created.EntityId);
        Assert.Equal(2L, created.LanguageId);
        Assert.Equal("Localized impact statistic", created.Title);
        _repositoryWrapperMock.Verify(x => x.SaveChangesAsync(), Times.Exactly(2));
    }

    [Theory]
    [InlineData("Old localized title", "New localized title", TranslationStatus.Relevant)]
    [InlineData("Same localized title", "Same localized title", TranslationStatus.Outdated)]
    public async Task Handle_ShouldUpdateExistingImpactStatisticsLocalization_AndMarkRelevantOnlyWhenTitleChanges(
        string currentTitle,
        string requestedTitle,
        TranslationStatus expectedStatus)
    {
        // Arrange
        var entity = GetExistingMainPageEntity(1);
        var localization = new ImpactStatisticsLocalization
        {
            EntityId = entity.ImpactStatistics!.Id,
            LanguageId = 2,
            Title = currentTitle,
            TranslationStatus = TranslationStatus.Outdated,
        };
        entity.ImpactStatistics.Localizations = [localization];

        var sourceDto = GetSourceMatchingUpdateDto(entity);
        var updateDto = sourceDto with
        {
            ImpactStatistics = sourceDto.ImpactStatistics! with
            {
                Localization = new UpdateImpactStatisticLocalizationDto { LanguageId = 2, Title = requestedTitle },
            },
        };

        SetupSuccessfulUpdate(entity);

        var handler = CreateHandler();

        // Act
        var result = await handler.Handle(new UpdateMainPageCommand(updateDto), CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(requestedTitle, localization.Title);
        Assert.Equal(expectedStatus, localization.TranslationStatus);
    }

    [Theory]
    [InlineData("old name", "100", "new name", "100", TranslationStatus.Relevant)]
    [InlineData("same name", "100", "same name", "250", TranslationStatus.Relevant)]
    [InlineData("same name", "100", "same name", "100", TranslationStatus.Outdated)]
    public async Task Handle_ShouldUpdateExistingMetricLocalization_AndMarkRelevantOnlyWhenNameOrValueChanges(
        string currentName,
        string currentValue,
        string requestedName,
        string requestedValue,
        TranslationStatus expectedStatus)
    {
        // Arrange
        var entity = GetExistingMainPageEntity(1);
        var metric = entity.ImpactStatistics!.Metrics.Single();
        var localization = new MetricLocalization
        {
            EntityId = metric.Id,
            LanguageId = 2,
            Name = currentName,
            Value = currentValue,
            TranslationStatus = TranslationStatus.Outdated,
        };
        metric.Localizations = [localization];

        var sourceDto = GetSourceMatchingUpdateDto(entity);
        var updateDto = sourceDto with
        {
            ImpactStatistics = sourceDto.ImpactStatistics! with
            {
                Metrics =
                [
                    sourceDto.ImpactStatistics.Metrics.Single() with
                    {
                        Localization = new UpdateMetricLocalizationDto { LanguageId = 2, Name = requestedName, Value = requestedValue },
                    },
                ],
            },
        };

        SetupSuccessfulUpdate(entity);

        var handler = CreateHandler();

        // Act
        var result = await handler.Handle(new UpdateMainPageCommand(updateDto), CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(requestedName, localization.Name);
        Assert.Equal(requestedValue, localization.Value);
        Assert.Equal(expectedStatus, localization.TranslationStatus);
    }

    private UpdateMainPageHandler CreateHandler() => new(
        _repositoryWrapperMock.Object,
        _mapperMock.Object,
        _mediatorMock.Object,
        _validatorMock.Object);

    private void SetupSuccessfulUpdate(DAL.Entities.MainPage entity)
    {
        SetupValidationSuccess();

        _mainPageRepositoryMock
            .SetupSequence(x => x.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<DAL.Entities.MainPage>?>()))
            .ReturnsAsync(entity)
            .ReturnsAsync(entity);

        _imageRepositoryMock
            .Setup(x => x.GetAllAsync(It.IsAny<QueryOptions<Image>?>()))
            .ReturnsAsync(
            [
                new Image { Id = 1 },
                new Image { Id = 2 },
                new Image { Id = 3 },
                new Image { Id = 5 },
                new Image { Id = 6 },
                new Image { Id = 7 },
            ]);

        _repositoryWrapperMock
            .Setup(x => x.SaveChangesAsync())
            .ReturnsAsync(1);

        _mapperMock
            .Setup(x => x.Map<DAL.Entities.MainPage, MainPageDto>(entity))
            .Returns(new MainPageDto { Id = entity.Id });
    }

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
        MainDonations = new UpdateMainDonationsDto
        {
            Title = "Updated donations title",
            Description = "Updated donations description long enough",
            ImageId = 7,
        },
        ImpactStatistics = new UpdateImpactStatisticDto
        {
            Title = "Updated impact statistic title",
            ImageId = 6,
            Metrics =
            [
                new UpdateMetricDto { Value = 250, Name = "families", Kind = MetricKind.Standard },
            ],
        },
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
        MainDonations = new UpdateMainDonationsDto
        {
            Title = "Updated donations title",
            Description = "Updated donations description long enough",
            ImageId = 7,
        },
        ImpactStatistics = new UpdateImpactStatisticDto
        {
            Id = existing.ImpactStatistics!.Id,
            Title = "Updated impact statistic title",
            ImageId = 6,
            Metrics =
            [
                new UpdateMetricDto
                {
                    Id = existing.ImpactStatistics!.Metrics.First().Id,
                    Value = 250,
                    Name = "families",
                    Kind = MetricKind.Standard,
                },
            ],
        },
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
        MainDonations = new MainDonations
        {
            Id = 15,
            Title = "Old donations title",
            Description = "Old donations description",
            ImageId = 3,
        },
        ImpactStatistics = new ImpactStatistics
        {
            Id = 13,
            Title = "Old impact statistic",
            ImageId = 2,
            Metrics =
            [
                new Metric
                {
                    Id = 14,
                    Value = 100,
                    Name = "children",
                    Kind = MetricKind.RaisedFunds,
                },
            ],
        },
    };

    private static UpdateMainPageDto GetSourceMatchingUpdateDto(DAL.Entities.MainPage existing) => new()
    {
        Title = existing.Title,
        Description = existing.Description,
        ImageId = existing.ImageId,
        MainAboutUs = new UpdateMainAboutUsDto
        {
            Title = existing.MainAboutUs!.Title,
            Description = existing.MainAboutUs.Description,
        },
        MainPartners = new UpdateMainPartnersDto
        {
            Title = existing.MainPartners!.Title,
            Description = existing.MainPartners.Description,
        },
        MainDonations = new UpdateMainDonationsDto
        {
            Title = existing.MainDonations!.Title,
            Description = existing.MainDonations.Description,
            ImageId = existing.MainDonations.ImageId,
        },
        ImpactStatistics = new UpdateImpactStatisticDto
        {
            Id = existing.ImpactStatistics!.Id,
            Title = existing.ImpactStatistics.Title,
            ImageId = existing.ImpactStatistics.ImageId,
            Metrics = existing.ImpactStatistics.Metrics
                .Select(metric => new UpdateMetricDto
                {
                    Id = metric.Id,
                    Value = metric.Value,
                    Name = metric.Name,
                    Kind = metric.Kind,
                    Prefix = metric.Prefix,
                    IsAutoSynced = metric.IsAutoSynced,
                })
                .ToList(),
        },
    };
}
