using AutoMapper;
using Moq;
using VictoryCenter.BLL.DTOs.Admin.ReportMediaSettings;
using VictoryCenter.BLL.Queries.Admin.ReportMediaSettings.GetAll;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.UnitTests.MediatRHandlersTests.ReportMediaSettings;
public class GetReportMediaSettingsHandlerTests
{
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<IRepositoryWrapper> _mockRepositoryWrapper;

    private readonly CollectedFundsBlock _collectedEntity = new()
    {
        Id = 1,
        Title = "Collected",
        ImageId = 10
    };

    private readonly ChangedLivesBlock _changedEntity = new()
    {
        Id = 2,
        Title = "Lives",
        ChangedLivesCount = 50,
        ImageId = 20
    };

    private readonly ReportMediaSettingsDto _resultDto = new()
    {
        CollectedFundsBlock = new(),
        ChangedLivesBlock = new()
    };

    public GetReportMediaSettingsHandlerTests()
    {
        _mockMapper = new Mock<IMapper>();
        _mockRepositoryWrapper = new Mock<IRepositoryWrapper>();
    }

    [Fact]
    public async Task Handle_BlocksExist_ShouldReturnOk()
    {
        // Arrange
        SetupRepositoryWrapper(_collectedEntity, _changedEntity);
        SetupMapper(_resultDto);

        var query = new GetReportMediaSettingsQuery();
        var handler = new GetReportMediaSettingsHandler(
            _mockRepositoryWrapper.Object,
            _mockMapper.Object);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(_resultDto, result.Value);
    }

    [Fact]
    public async Task Handle_BlocksDoNotExist_ShouldReturnOkWithNullDtos()
    {
        // Arrange
        SetupRepositoryWrapper(null, null);

        _mockMapper
            .Setup(m => m.Map<CollectedFundsBlockDto>(null))
            .Returns((CollectedFundsBlockDto?)null);

        _mockMapper
            .Setup(m => m.Map<ChangedLivesBlockDto>(null))
            .Returns((ChangedLivesBlockDto?)null);

        var query = new GetReportMediaSettingsQuery();
        var handler = new GetReportMediaSettingsHandler(
            _mockRepositoryWrapper.Object,
            _mockMapper.Object);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Null(result.Value.CollectedFundsBlock);
        Assert.Null(result.Value.ChangedLivesBlock);
    }

    private void SetupMapper(ReportMediaSettingsDto dtoToReturn)
    {
        _mockMapper
            .Setup(m => m.Map<CollectedFundsBlockDto>(It.IsAny<CollectedFundsBlock>()))
            .Returns(dtoToReturn.CollectedFundsBlock);

        _mockMapper
            .Setup(m => m.Map<ChangedLivesBlockDto>(It.IsAny<ChangedLivesBlock>()))
            .Returns(dtoToReturn.ChangedLivesBlock);
    }

    private void SetupRepositoryWrapper(
        CollectedFundsBlock? collectedBlock,
        ChangedLivesBlock? changedBlock)
    {
        var collectedRepo = new Mock<IRepositoryBase<CollectedFundsBlock>>();
        var changedRepo = new Mock<IRepositoryBase<ChangedLivesBlock>>();

        collectedRepo
            .Setup(r => r.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<CollectedFundsBlock>>()))
            .ReturnsAsync(collectedBlock);

        changedRepo
            .Setup(r => r.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<ChangedLivesBlock>>()))
            .ReturnsAsync(changedBlock);

        _mockRepositoryWrapper
            .Setup(r => r.GetRepository<CollectedFundsBlock>())
            .Returns(collectedRepo.Object);

        _mockRepositoryWrapper
            .Setup(r => r.GetRepository<ChangedLivesBlock>())
            .Returns(changedRepo.Object);
    }
}
