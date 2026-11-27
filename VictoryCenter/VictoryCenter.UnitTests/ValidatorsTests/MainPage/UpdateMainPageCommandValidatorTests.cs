using FluentValidation.TestHelper;
using Moq;
using VictoryCenter.BLL.Commands.Admin.MainPage.Update;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.ImpactStatistics;
using VictoryCenter.BLL.DTOs.Admin.ImpactStatistics.Metrics;
using VictoryCenter.BLL.DTOs.Admin.MainPages;
using VictoryCenter.BLL.Validators.MainPage.Commands;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Enums;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Interfaces.MainPage;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.UnitTests.ValidatorsTests.MainPage;

public class UpdateMainPageCommandValidatorTests
{
    private const long ImpactStatisticsId = 13;
    private static readonly long[] ExistingMetricIds = [14, 15, 16, 17];

    private readonly Mock<IRepositoryWrapper> _repositoryWrapperMock = new();
    private readonly Mock<IMainPageRepository> _mainPageRepositoryMock = new();

    public UpdateMainPageCommandValidatorTests()
    {
        _repositoryWrapperMock
            .SetupGet(x => x.MainPageRepository)
            .Returns(_mainPageRepositoryMock.Object);
    }

    [Fact]
    public async Task Validate_ShouldNotHaveErrors_WhenMainPageDoesNotExist()
    {
        // Arrange
        var command = new UpdateMainPageCommand(GetValidDto());
        SetupExistingMainPage(null);

        var validator = CreateValidator();

        // Act
        var result = await validator.TestValidateAsync(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async Task Validate_ShouldHaveError_WhenImpactStatisticIdDoesNotBelongToMainPage()
    {
        // Arrange
        var command = new UpdateMainPageCommand(GetValidDto() with
        {
            ImpactStatistics = GetValidImpactStatisticDto() with { Id = 9999 },
        });
        SetupExistingMainPage(GetExistingMainPage());

        var validator = CreateValidator();

        // Act
        var result = await validator.TestValidateAsync(command);

        // Assert
        AssertHasNestedIdsError(result);
    }

    [Fact]
    public async Task Validate_ShouldHaveError_WhenMetricIdDoesNotBelongToMainPage()
    {
        // Arrange
        var command = new UpdateMainPageCommand(GetValidDto() with
        {
            ImpactStatistics = GetValidImpactStatisticDto(metricIds: [14, 15, 16, 9999]),
        });
        SetupExistingMainPage(GetExistingMainPage());

        var validator = CreateValidator();

        // Act
        var result = await validator.TestValidateAsync(command);

        // Assert
        AssertHasNestedIdsError(result);
    }

    [Fact]
    public async Task Validate_ShouldHaveError_WhenMetricWithoutIdIsSentForExistingImpactStatistics()
    {
        // Arrange
        var command = new UpdateMainPageCommand(GetValidDto() with
        {
            ImpactStatistics = GetValidImpactStatisticDto(metricIds: [14, 15, 16, null]),
        });
        SetupExistingMainPage(GetExistingMainPage());

        var validator = CreateValidator();

        // Act
        var result = await validator.TestValidateAsync(command);

        // Assert
        AssertHasNestedIdsError(result);
    }

    [Fact]
    public async Task Validate_ShouldHaveError_WhenExistingMetricIsOmitted()
    {
        // Arrange
        var existingMainPage = GetExistingMainPage();
        existingMainPage.ImpactStatistics!.Metrics.Add(new Metric { Id = 18, Name = "extra", Kind = MetricKind.Standard });

        var command = new UpdateMainPageCommand(GetValidDto());
        SetupExistingMainPage(existingMainPage);

        var validator = CreateValidator();

        // Act
        var result = await validator.TestValidateAsync(command);

        // Assert
        AssertHasNestedIdsError(result);
    }

    [Fact]
    public async Task Validate_ShouldNotHaveErrors_WhenAllExistingMetricIdsAreSent()
    {
        // Arrange
        var command = new UpdateMainPageCommand(GetValidDto());
        SetupExistingMainPage(GetExistingMainPage());

        var validator = CreateValidator();

        // Act
        var result = await validator.TestValidateAsync(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async Task Validate_ShouldNotHaveErrors_WhenImpactStatisticsDoesNotExistAndMetricsHaveNoIds()
    {
        // Arrange
        var existingMainPage = GetExistingMainPage();
        existingMainPage.ImpactStatistics = null;

        var command = new UpdateMainPageCommand(GetValidDto() with
        {
            ImpactStatistics = GetValidImpactStatisticDto(metricIds: [null, null, null, null]) with { Id = null },
        });
        SetupExistingMainPage(existingMainPage);

        var validator = CreateValidator();

        // Act
        var result = await validator.TestValidateAsync(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData(null)]
    [InlineData(9999L)]
    public async Task Validate_ShouldHaveError_WhenPersistedMetricIsReplaced(long? replacementId)
    {
        // Arrange
        var statDto = GetValidImpactStatisticDto();
        var metrics = statDto.Metrics.ToList();
        metrics[0] = metrics[0] with { Id = replacementId };

        var command = new UpdateMainPageCommand(GetValidDto() with
        {
            ImpactStatistics = statDto with { Metrics = metrics },
        });
        SetupExistingMainPage(GetExistingMainPage());

        var validator = CreateValidator();

        // Act
        var result = await validator.TestValidateAsync(command);

        // Assert
        AssertHasNestedIdsError(result);
    }

    [Fact]
    public async Task Validate_ShouldHaveError_WhenRequestedKindsDifferFromStoredKinds()
    {
        // Arrange
        // Swapping kinds keeps a valid 3 Standard + 1 RaisedFunds composition, so only the stored-kind check can reject it.
        var statDto = GetValidImpactStatisticDto();
        var metrics = statDto.Metrics.ToList();
        metrics[0] = metrics[0] with { Kind = MetricKind.Standard };
        metrics[1] = metrics[1] with { Kind = MetricKind.RaisedFunds };

        var command = new UpdateMainPageCommand(GetValidDto() with
        {
            ImpactStatistics = statDto with { Metrics = metrics },
        });
        SetupExistingMainPage(GetExistingMainPage());

        var validator = CreateValidator();

        // Act
        var result = await validator.TestValidateAsync(command);

        // Assert
        AssertHasNestedIdsError(result);
    }

    [Fact]
    public async Task Validate_ShouldHaveError_WhenMetricIdsAreSentForNewImpactStatistics()
    {
        // Arrange
        var existingMainPage = GetExistingMainPage();
        existingMainPage.ImpactStatistics = null;

        var command = new UpdateMainPageCommand(GetValidDto() with
        {
            ImpactStatistics = GetValidImpactStatisticDto() with { Id = null },
        });
        SetupExistingMainPage(existingMainPage);

        var validator = CreateValidator();

        // Act
        var result = await validator.TestValidateAsync(command);

        // Assert
        AssertHasNestedIdsError(result);
    }

    [Theory]
    [InlineData(0, false)]
    [InlineData(3, false)]
    [InlineData(4, true)]
    public async Task Validate_ShouldRequireFourMetrics_WhenImpactStatisticsIsCreated(int count, bool isValid)
    {
        // Arrange
        var existingMainPage = GetExistingMainPage();
        existingMainPage.ImpactStatistics = null;

        var command = new UpdateMainPageCommand(GetValidDto() with
        {
            ImpactStatistics = GetValidImpactStatisticDto(metricIds: [.. Enumerable.Repeat<long?>(null, count)]) with { Id = null },
        });
        SetupExistingMainPage(existingMainPage);

        var validator = CreateValidator();

        // Act
        var result = await validator.TestValidateAsync(command);

        // Assert
        Assert.Equal(isValid, result.IsValid);
    }

    private static void AssertHasNestedIdsError(TestValidationResult<UpdateMainPageCommand> result)
    {
        result.ShouldHaveValidationErrorFor(x => x)
            .WithErrorMessage(ErrorMessagesConstants.MetricsMustMatchExistingStatistics);
    }

    private void SetupExistingMainPage(DAL.Entities.MainPage? mainPage)
    {
        _mainPageRepositoryMock
            .Setup(x => x.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<DAL.Entities.MainPage>?>()))
            .ReturnsAsync(mainPage);
    }

    private UpdateMainPageCommandValidator CreateValidator() =>
        new(_repositoryWrapperMock.Object);

    private static UpdateMainPageDto GetValidDto() => new()
    {
        Title = "Main page title",
        Description = "Main page description",
        ImageId = 1,
        ImpactStatistics = GetValidImpactStatisticDto(),
    };

    private static UpdateImpactStatisticDto GetValidImpactStatisticDto(IReadOnlyList<long?>? metricIds = null)
    {
        metricIds ??= ExistingMetricIds.Select(id => (long?)id).ToArray();

        return new UpdateImpactStatisticDto
        {
            Id = ImpactStatisticsId,
            Title = "Impact statistic",
            ImageId = 2,
            Metrics = metricIds
                .Select((id, index) => new UpdateMetricDto
                {
                    Id = id,
                    Value = 100 * (index + 1),
                    Name = $"metric-{index}",
                    Kind = index == 0 ? MetricKind.RaisedFunds : MetricKind.Standard,
                })
                .ToList(),
        };
    }

    private static DAL.Entities.MainPage GetExistingMainPage() => new()
    {
        Id = 1,
        Title = "Old title",
        Description = "Old description",
        ImageId = 1,
        MainAboutUs = new MainAboutUs
        {
            Id = 11,
            Title = "Old about us",
            Description = "Old about us desc",
        },
        MainPartners = new MainPartners
        {
            Id = 12,
            Title = "Old partners",
            Description = "Old partners desc",
        },
        ImpactStatistics = new ImpactStatistics
        {
            Id = ImpactStatisticsId,
            Title = "Old stat",
            ImageId = 2,
            Metrics = ExistingMetricIds
                .Select((id, index) => new Metric
                {
                    Id = id,
                    Value = 100,
                    Name = $"old-metric-{index}",
                    Kind = index == 0 ? MetricKind.RaisedFunds : MetricKind.Standard,
                })
                .ToList(),
        },
    };
}
