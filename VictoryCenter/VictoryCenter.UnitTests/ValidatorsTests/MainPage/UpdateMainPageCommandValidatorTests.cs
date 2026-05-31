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

        _mainPageRepositoryMock
            .Setup(x => x.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<DAL.Entities.MainPage>?>()))
            .ReturnsAsync((DAL.Entities.MainPage?)null);

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
            ImpactStatistics = new UpdateImpactStatisticDto
            {
                Id = 9999,
                Title = "Updated stat",
                Metrics =
                [
                    new UpdateMetricDto
                    {
                        Id = 14,
                        Value = 123,
                        Name = "kids",
                        Type = MetricType.Raised,
                    },
                ],
            },
        });

        _mainPageRepositoryMock
            .Setup(x => x.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<DAL.Entities.MainPage>?>()))
            .ReturnsAsync(GetExistingMainPage());

        var validator = CreateValidator();

        // Act
        var result = await validator.TestValidateAsync(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x)
            .WithErrorMessage(ErrorMessagesConstants.PropertyIsRequired("ImpactStatistics.Id and ImpactStatistics.Metrics[].Id"));
    }

    [Fact]
    public async Task Validate_ShouldHaveError_WhenMetricIdDoesNotBelongToMainPage()
    {
        // Arrange
        var command = new UpdateMainPageCommand(GetValidDto() with
        {
            ImpactStatistics = new UpdateImpactStatisticDto
            {
                Id = 13,
                Title = "Updated stat",
                Metrics =
                [
                    new UpdateMetricDto
                    {
                        Id = 9999,
                        Value = 123,
                        Name = "kids",
                        Type = MetricType.Raised,
                    },
                ],
            },
        });

        _mainPageRepositoryMock
            .Setup(x => x.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<DAL.Entities.MainPage>?>()))
            .ReturnsAsync(GetExistingMainPage());

        var validator = CreateValidator();

        // Act
        var result = await validator.TestValidateAsync(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x)
            .WithErrorMessage(ErrorMessagesConstants.PropertyIsRequired("ImpactStatistics.Id and ImpactStatistics.Metrics[].Id"));
    }

    [Fact]
    public async Task Validate_ShouldNotHaveErrors_WhenIdsMatchExistingMainPage()
    {
        // Arrange
        var command = new UpdateMainPageCommand(GetValidDto() with
        {
            ImpactStatistics = new UpdateImpactStatisticDto
            {
                Id = 13,
                Title = "Updated existing stat",
                ImageId = 2,
                Metrics =
                [
                    new UpdateMetricDto
                    {
                        Id = 14,
                        Value = 999,
                        Name = "updated-name",
                        Type = MetricType.Raised,
                    },
                    new UpdateMetricDto
                    {
                        Value = 123,
                        Name = "new-metric",
                        Type = MetricType.Partners,
                    },
                ],
            },
        });

        _mainPageRepositoryMock
            .Setup(x => x.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<DAL.Entities.MainPage>?>()))
            .ReturnsAsync(GetExistingMainPage());

        var validator = CreateValidator();

        // Act
        var result = await validator.TestValidateAsync(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    private UpdateMainPageCommandValidator CreateValidator() =>
        new(_repositoryWrapperMock.Object);

    private static UpdateMainPageDto GetValidDto() => new()
    {
        Title = "Main page title",
        Description = "Main page description",
        ImageId = 1,
        ImpactStatistics = new UpdateImpactStatisticDto
        {
            Id = 13,
            Title = "Impact statistic",
            ImageId = 2,
            Metrics =
            [
                new UpdateMetricDto
                {
                    Id = 14,
                    Value = 100,
                    Name = "children",
                    Type = MetricType.Raised,
                },
            ],
        },
    };

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
            Id = 13,
            Title = "Old stat",
            ImageId = 2,
            Metrics =
            [
                new Metric
                {
                    Id = 14,
                    Value = 100,
                    Name = "children",
                    Type = MetricType.Raised,
                },
            ],
        },
    };
}
