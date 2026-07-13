using System.Transactions;
using FluentValidation;
using FluentValidation.Results;
using MediatR;
using Moq;
using VictoryCenter.BLL.Commands.Admin.ImpactStatistics.UpdateSingleMetric;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.Constants.Localization;
using VictoryCenter.BLL.DTOs.Admin.ImpactStatistics.Metrics;
using VictoryCenter.BLL.Notifications.ReportFunds;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Entities.Localization;
using VictoryCenter.DAL.Enums;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Interfaces.Localization.MainPage;
using VictoryCenter.DAL.Repositories.Interfaces.MainPage;
using VictoryCenter.DAL.Repositories.Options;
using VictoryCenter.BLL.DTOs.Admin.Localization.MainPage.Metrics;

namespace VictoryCenter.UnitTests.MediatRHandlersTests.MainPage;

public class UpdateSingleMetricHandlerTests
{
    private readonly Mock<IRepositoryWrapper> _repositoryWrapperMock = new();
    private readonly Mock<IMetricRepository> _metricRepositoryMock = new();
    private readonly Mock<IMetricLocalizationsRepository> _metricLocalizationsRepositoryMock = new();
    private readonly Mock<IValidator<UpdateSingleMetricCommand>> _validatorMock = new();
    private readonly Mock<IMediator> _mediatorMock = new();

    public UpdateSingleMetricHandlerTests()
    {
        _repositoryWrapperMock.SetupGet(x => x.MetricRepository).Returns(_metricRepositoryMock.Object);
        _repositoryWrapperMock.SetupGet(x => x.MetricLocalizationsRepository).Returns(_metricLocalizationsRepositoryMock.Object);
        _repositoryWrapperMock.Setup(x => x.BeginTransaction()).Returns(new TransactionScope(TransactionScopeAsyncFlowOption.Enabled));
    }

    [Fact]
    public async Task Handle_ShouldUpdateMetric_WhenDataIsValid()
    {
        var metric = new Metric { Id = 1, Value = 10, Name = "old", RowVersion = [1] };
        var command = new UpdateSingleMetricCommand(1, new UpdateSingleMetricDto { Value = 20, ExpectedVersion = [1] });

        SetupValidationSuccess();
        _metricRepositoryMock.Setup(x => x.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<Metric>?>())).ReturnsAsync(metric);
        _repositoryWrapperMock.Setup(x => x.SaveChangesAsync()).ReturnsAsync(1);

        var handler = CreateHandler();
        var result = await handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(20, metric.Value);
        Assert.Contains(nameof(UpdateSingleMetricDto.Value), result.Value.UpdatedFields);
        _repositoryWrapperMock.Verify(x => x.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenValidationFails()
    {
        var command = new UpdateSingleMetricCommand(1, new UpdateSingleMetricDto());
        _validatorMock.Setup(x => x.ValidateAsync(It.IsAny<UpdateSingleMetricCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult(new[] { new ValidationFailure("Prop", "Error") }));

        var handler = CreateHandler();
        var result = await handler.Handle(command, CancellationToken.None);

        Assert.False(result.IsSuccess);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenMetricNotFound()
    {
        var command = new UpdateSingleMetricCommand(1, new UpdateSingleMetricDto());
        SetupValidationSuccess();
        _metricRepositoryMock.Setup(x => x.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<Metric>?>())).ReturnsAsync((Metric?)null);

        var handler = CreateHandler();
        var result = await handler.Handle(command, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessagesConstants.NotFound(1, typeof(Metric)), result.Errors[0].Message);
    }

    [Fact]
    public async Task Handle_ShouldPublishNotification_WhenRaisedMetricIsAutoSynced()
    {
        var metric = new Metric { Id = 1, Type = MetricType.Raised, IsAutoSynced = false, RowVersion = [1] };
        var command = new UpdateSingleMetricCommand(1, new UpdateSingleMetricDto { IsAutoSynced = true, ExpectedVersion = [1] });

        SetupValidationSuccess();
        _metricRepositoryMock.Setup(x => x.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<Metric>?>())).ReturnsAsync(metric);
        _repositoryWrapperMock.Setup(x => x.SaveChangesAsync()).ReturnsAsync(1);
        _mediatorMock.Setup(x => x.Publish(It.IsAny<ReportFundsChangedNotification>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        var handler = CreateHandler();
        var result = await handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.True(metric.IsAutoSynced);
        _mediatorMock.Verify(x => x.Publish(It.IsAny<ReportFundsChangedNotification>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenConcurrencyCheckFails()
    {
        var metric = new Metric { Id = 1, RowVersion = [1] };
        var command = new UpdateSingleMetricCommand(1, new UpdateSingleMetricDto { ExpectedVersion = [2] });

        SetupValidationSuccess();
        _metricRepositoryMock.Setup(x => x.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<Metric>?>())).ReturnsAsync(metric);

        var handler = CreateHandler();
        var result = await handler.Handle(command, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal("Metric was modified by another user. Please refresh and try again.", result.Errors[0].Message);
    }

    [Fact]
    public async Task Handle_ShouldUpdateOtherProperties_AndSetLocalizationsOutdated()
    {
        var metric = new Metric
        {
            Id = 1,
            Name = "old",
            Type = MetricType.Partners,
            Prefix = MetricPrefix.None,
            Localizations = new List<MetricLocalization>
            {
                new() { LanguageId = LocalizationLanguageConstants.PrimaryLanguageId, TranslationStatus = TranslationStatus.Outdated },
                new() { LanguageId = 2, TranslationStatus = TranslationStatus.Relevant }
            }
        };
        var command = new UpdateSingleMetricCommand(1, new UpdateSingleMetricDto
        {
            Name = "new",
            Type = MetricType.Raised,
            Prefix = MetricPrefix.Plus
        });

        SetupValidationSuccess();
        _metricRepositoryMock.Setup(x => x.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<Metric>?>())).ReturnsAsync(metric);
        _repositoryWrapperMock.Setup(x => x.SaveChangesAsync()).ReturnsAsync(1);

        var handler = CreateHandler();
        var result = await handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.True(result.Value.WasModified);
        Assert.Equal("new", metric.Name);
        Assert.Equal(MetricType.Raised, metric.Type);
        Assert.Equal(MetricPrefix.Plus, metric.Prefix);
        Assert.Contains(nameof(UpdateSingleMetricDto.Name), result.Value.UpdatedFields);
        Assert.Contains(nameof(UpdateSingleMetricDto.Type), result.Value.UpdatedFields);
        Assert.Contains(nameof(UpdateSingleMetricDto.Prefix), result.Value.UpdatedFields);

        var primaryLoc = metric.Localizations.First(l => l.LanguageId == LocalizationLanguageConstants.PrimaryLanguageId);
        Assert.Equal(TranslationStatus.Relevant, primaryLoc.TranslationStatus);

        var otherLoc = metric.Localizations.First(l => l.LanguageId == 2);
        Assert.Equal(TranslationStatus.Outdated, otherLoc.TranslationStatus);
    }

    [Fact]
    public async Task Handle_ShouldUpdateLocalization_WhenExisting()
    {
        var metric = new Metric
        {
            Id = 1,
            Localizations = new List<MetricLocalization>
            {
                new() { LanguageId = 2, Name = "oldLocName", Value = "oldLocValue", TranslationStatus = TranslationStatus.Outdated }
            }
        };
        var command = new UpdateSingleMetricCommand(1, new UpdateSingleMetricDto
        {
            Localization = new UpdateMetricLocalizationDto { LanguageId = 2, Name = "newLocName", Value = "newLocValue" }
        });

        SetupValidationSuccess();
        _metricRepositoryMock.Setup(x => x.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<Metric>?>())).ReturnsAsync(metric);
        _repositoryWrapperMock.Setup(x => x.SaveChangesAsync()).ReturnsAsync(1);

        var handler = CreateHandler();
        var result = await handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.True(result.Value.WasModified);
        var loc = metric.Localizations.First();
        Assert.Equal("newLocName", loc.Name);
        Assert.Equal("newLocValue", loc.Value);
        Assert.Equal(TranslationStatus.Relevant, loc.TranslationStatus);
    }

    [Fact]
    public async Task Handle_ShouldCreateLocalization_WhenNotExisting()
    {
        var metric = new Metric { Id = 1, Localizations = new List<MetricLocalization>() };
        var command = new UpdateSingleMetricCommand(1, new UpdateSingleMetricDto
        {
            Localization = new UpdateMetricLocalizationDto { LanguageId = 2, Name = "newLocName", Value = "newLocValue" }
        });

        SetupValidationSuccess();
        _metricRepositoryMock.Setup(x => x.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<Metric>?>())).ReturnsAsync(metric);
        _repositoryWrapperMock.Setup(x => x.SaveChangesAsync()).ReturnsAsync(1);

        var handler = CreateHandler();
        var result = await handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.True(result.Value.WasModified);
        _metricLocalizationsRepositoryMock.Verify(x => x.CreateAsync(It.Is<MetricLocalization>(l => l.LanguageId == 2 && l.Name == "newLocName")), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldReturnOk_WhenNothingModified()
    {
        var metric = new Metric { Id = 1, Value = 10, Name = "name" };
        var command = new UpdateSingleMetricCommand(1, new UpdateSingleMetricDto { Value = 10, Name = "name" });

        SetupValidationSuccess();
        _metricRepositoryMock.Setup(x => x.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<Metric>?>())).ReturnsAsync(metric);

        var handler = CreateHandler();
        var result = await handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.False(result.Value.WasModified);
        _repositoryWrapperMock.Verify(x => x.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldFail_OnGenericException()
    {
        var command = new UpdateSingleMetricCommand(1, new UpdateSingleMetricDto());
        SetupValidationSuccess();
        _metricRepositoryMock.Setup(x => x.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<Metric>?>())).ThrowsAsync(new Exception("DB Error"));

        var handler = CreateHandler();
        var result = await handler.Handle(command, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal("Failed to update metric: DB Error", result.Errors[0].Message);
    }

    private void SetupValidationSuccess()
    {
        _validatorMock.Setup(x => x.ValidateAsync(It.IsAny<UpdateSingleMetricCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());
    }

    private UpdateSingleMetricHandler CreateHandler() => new(
        _repositoryWrapperMock.Object,
        _validatorMock.Object,
        _mediatorMock.Object);
}
