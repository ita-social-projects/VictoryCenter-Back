using Moq;
using VictoryCenter.BLL.Interfaces.BlobStorage;
using VictoryCenter.BLL.Queries.Public.ReportFundsExpenditures.GetPublished;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Entities.Localization;
using VictoryCenter.DAL.Enums;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Interfaces.Localization.Languages;
using VictoryCenter.DAL.Repositories.Interfaces.PublishedReportFundsExpendituresRecords;
using VictoryCenter.DAL.Repositories.Interfaces.PublishedReportFundsExpendituresSnapshot;
using VictoryCenter.DAL.Repositories.Interfaces.PublishedReportProgramExpendituresRecords;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.UnitTests.MediatRHandlersTests.ReportFundsExpenditures;

public class GetPublishedReportFundsExpendituresHandlerTests
{
    private readonly Mock<IRepositoryWrapper> _repositoryWrapperMock;
    private readonly Mock<IBlobService> _blobServiceMock;

    private readonly Mock<IPublishedReportFundsExpendituresSnapshotRepository> _snapshotRepoMock;
    private readonly Mock<IPublishedReportFundsExpendituresRecordsRepository> _fundsRecordsRepoMock;
    private readonly Mock<IPublishedReportProgramExpendituresRecordsRepository> _programRecordsRepoMock;
    private readonly Mock<ILocalizationLanguagesRepository> _languageRepoMock;
    private readonly Mock<IRepositoryBase<CollectedFundsBlock>> _collectedFundsRepoMock;
    private readonly Mock<IRepositoryBase<ChangedLivesBlock>> _changedLivesRepoMock;

    public GetPublishedReportFundsExpendituresHandlerTests()
    {
        _repositoryWrapperMock = new Mock<IRepositoryWrapper>();
        _blobServiceMock = new Mock<IBlobService>();

        _snapshotRepoMock = new Mock<IPublishedReportFundsExpendituresSnapshotRepository>();
        _fundsRecordsRepoMock = new Mock<IPublishedReportFundsExpendituresRecordsRepository>();
        _programRecordsRepoMock = new Mock<IPublishedReportProgramExpendituresRecordsRepository>();
        _languageRepoMock = new Mock<ILocalizationLanguagesRepository>();
        _collectedFundsRepoMock = new Mock<IRepositoryBase<CollectedFundsBlock>>();
        _changedLivesRepoMock = new Mock<IRepositoryBase<ChangedLivesBlock>>();

        _repositoryWrapperMock.Setup(w => w.PublishedReportFundsExpendituresSnapshotRepository).Returns(_snapshotRepoMock.Object);
        _repositoryWrapperMock.Setup(w => w.PublishedReportFundsExpendituresRecordsRepository).Returns(_fundsRecordsRepoMock.Object);
        _repositoryWrapperMock.Setup(w => w.PublishedReportProgramExpendituresRecordsRepository).Returns(_programRecordsRepoMock.Object);
        _repositoryWrapperMock.Setup(w => w.LocalizationLanguagesRepository).Returns(_languageRepoMock.Object);
        _repositoryWrapperMock.Setup(w => w.GetRepository<CollectedFundsBlock>()).Returns(_collectedFundsRepoMock.Object);
        _repositoryWrapperMock.Setup(w => w.GetRepository<ChangedLivesBlock>()).Returns(_changedLivesRepoMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnEmpty_WhenNoSnapshotFound()
    {
        // Arrange
        _snapshotRepoMock.Setup(r => r.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<PublishedReportFundsExpendituresSnapshot>>()))
            .ReturnsAsync((PublishedReportFundsExpendituresSnapshot?)null);

        var handler = new GetPublishedReportFundsExpendituresHandler(_repositoryWrapperMock.Object, _blobServiceMock.Object);

        // Act
        var result = await handler.Handle(new GetPublishedReportFundsExpendituresQuery(null), CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value.Settings);
        Assert.NotNull(result.Value.Funding);
        Assert.NotNull(result.Value.Expenses);
        Assert.NotNull(result.Value.Programs);
        Assert.NotNull(result.Value.MediaSettings);
    }

    [Fact]
    public async Task Handle_ShouldReturnData_WhenSnapshotAndRecordsExist()
    {
        // Arrange
        var snapshot = new PublishedReportFundsExpendituresSnapshot
        {
            DisclaimerTitle = "Title",
            DisclaimerTitleEn = "Title EN",
            ExchangeRate = 38m,
            ProgramExpendituresReportingYear = 2024
        };
        _snapshotRepoMock.Setup(r => r.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<PublishedReportFundsExpendituresSnapshot>>()))
            .ReturnsAsync(snapshot);

        var fundsRecords = new List<PublishedReportFundsExpendituresRecord>
        {
            new() { Type = ReportFundsExpendituresType.Income, AmountUah = 100, CategoryName = "Cat", CategoryNameEn = "Cat EN" },
            new() { Type = ReportFundsExpendituresType.Expense, AmountUah = 50, CategoryName = "Exp", CategoryNameEn = "Exp EN" }
        };
        _fundsRecordsRepoMock.Setup(r => r.GetAllAsync(It.IsAny<QueryOptions<PublishedReportFundsExpendituresRecord>>()))
            .ReturnsAsync(fundsRecords);

        var programRecords = new List<PublishedReportProgramExpendituresRecord>
        {
            new() { AmountUah = 200, CategoryName = "Prog", CategoryNameEn = "Prog EN" }
        };
        _programRecordsRepoMock.Setup(r => r.GetAllAsync(It.IsAny<QueryOptions<PublishedReportProgramExpendituresRecord>>()))
            .ReturnsAsync(programRecords);

        _languageRepoMock.Setup(r => r.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<LocalizationLanguage>>()))
            .ReturnsAsync(new LocalizationLanguage { Code = "en" });

        _collectedFundsRepoMock.Setup(r => r.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<CollectedFundsBlock>>()))
            .ReturnsAsync(new CollectedFundsBlock { Title = "UA", TitleEn = "EN", Image = new Image { BlobName = "a.jpg", MimeType = "image/jpeg" } });

        _changedLivesRepoMock.Setup(r => r.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<ChangedLivesBlock>>()))
            .ReturnsAsync(new ChangedLivesBlock { Title = "UA", TitleEn = "EN" });

        _blobServiceMock.Setup(b => b.GetFileUrl(It.IsAny<string>(), It.IsAny<string>())).Returns("http://url");

        var handler = new GetPublishedReportFundsExpendituresHandler(_repositoryWrapperMock.Object, _blobServiceMock.Object);

        // Act
        var result = await handler.Handle(new GetPublishedReportFundsExpendituresQuery(1), CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal("Title EN", result.Value.Settings.DisclaimerTitle);
        Assert.Single(result.Value.Funding.Items);
        Assert.Equal(100, result.Value.Funding.TotalUah);
        Assert.Equal("Cat EN", result.Value.Funding.Items[0].Label);

        Assert.Single(result.Value.Expenses.Items);
        Assert.Equal(50, result.Value.Expenses.TotalUah);

        Assert.Single(result.Value.Programs.Items);
        Assert.Equal(200, result.Value.Programs.TotalUah);

        Assert.Equal("EN", result.Value.MediaSettings.CollectedFunds.Title);
        Assert.Equal("http://url", result.Value.MediaSettings.CollectedFunds.ImageUrl);
    }
}
