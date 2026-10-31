using AutoMapper;
using Moq;
using VictoryCenter.BLL;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Public.FeedbackHistories;
using VictoryCenter.BLL.Queries.Public.FeedbackHistories.GetLatestPublished;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Entities.Localization;
using VictoryCenter.DAL.Enums;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Interfaces.FeedbackHistories;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.UnitTests.MediatRHandlersTests.FeedbackHistories;

public class GetLatestPublishedFeedbackHistoriesTests
{
    private readonly Mock<IMapper> _mapper = new();
    private readonly Mock<IRepositoryWrapper> _repositoryWrapper = new();
    private readonly Mock<IFeedbackHistoriesRepository> _repository = new();

    public GetLatestPublishedFeedbackHistoriesTests()
    {
        _repositoryWrapper
            .SetupGet(wrapper => wrapper.FeedbackHistoriesRepository)
            .Returns(_repository.Object);
    }

    [Fact]
    public async Task Handle_ShouldQueryOnlyPublishedWithPublishedAtOrderedDescAndLimitedToLatestCount()
    {
        QueryOptions<FeedbackHistory>? capturedOptions = null;
        _repository
            .Setup(repository => repository.GetAllAsync(It.IsAny<QueryOptions<FeedbackHistory>>()))
            .Callback<QueryOptions<FeedbackHistory>?>(options => capturedOptions = options)
            .ReturnsAsync([]);
        _mapper
            .Setup(mapper => mapper.Map<List<PublishedFeedbackHistoryDto>>(It.IsAny<IEnumerable<FeedbackHistory>>()))
            .Returns([]);

        await CreateHandler().Handle(new GetLatestPublishedFeedbackHistoriesQuery(), CancellationToken.None);

        Assert.NotNull(capturedOptions);
        Assert.NotNull(capturedOptions.Filter);
        var filter = capturedOptions.Filter.Compile();

        Assert.True(filter(new FeedbackHistory { Status = Status.Published, PublishedAt = DateTimeOffset.UtcNow }));
        Assert.False(filter(new FeedbackHistory { Status = Status.Published, PublishedAt = null }));
        Assert.False(filter(new FeedbackHistory { Status = Status.Draft, PublishedAt = DateTimeOffset.UtcNow }));

        Assert.NotNull(capturedOptions.OrderByDESC);
        Assert.NotNull(capturedOptions.ThenByDESC);
        Assert.Equal(FeedbackHistoryConstants.LatestPublishedFeedbackHistoryCount, capturedOptions.Limit);
        Assert.NotNull(capturedOptions.Include);
        Assert.True(capturedOptions.AsNoTracking);
    }

    [Fact]
    public async Task Handle_ShouldReturnMappedHistories()
    {
        List<FeedbackHistory> histories =
        [
            new()
            {
                Id = 1,
                Title = "Title",
                Story = "Story",
                Status = Status.Published,
                PublishedAt = DateTimeOffset.UtcNow
            }

        ];

        List<PublishedFeedbackHistoryDto> dtos = [new() { Id = 1, Title = "Title", Story = "Story" }];

        _repository
            .Setup(repository => repository.GetAllAsync(It.IsAny<QueryOptions<FeedbackHistory>>()))
            .ReturnsAsync(histories);
        _mapper
            .Setup(mapper => mapper.Map<List<PublishedFeedbackHistoryDto>>(histories))
            .Returns(dtos);

        var result = await CreateHandler().Handle(new GetLatestPublishedFeedbackHistoriesQuery(), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(dtos, result.Value);
    }

    [Fact]
    public async Task Handle_WithRealMapper_ShouldMapLocalizations()
    {
        var language = new LocalizationLanguage { Id = 2, Code = "en", Name = "English" };
        var localization = new FeedbackHistoryLocalization
        {
            EntityId = 1,
            LanguageId = language.Id,
            Language = language,
            Title = "Title",
            Story = "Story",
            TranslationStatus = TranslationStatus.Outdated
        };

        var entity = new FeedbackHistory
        {
            Id = 1,
            Title = "Заголовок",
            Story = "Історія",
            Status = Status.Published,
            PublishedAt = DateTimeOffset.UtcNow,
            Localizations = [localization]
        };

        _repository
            .Setup(repository => repository.GetAllAsync(It.IsAny<QueryOptions<FeedbackHistory>>()))
            .ReturnsAsync([entity]);

        var mapper = new MapperConfiguration(cfg => cfg.AddMaps(typeof(BllAssemblyMarker).Assembly)).CreateMapper();

        var result = await new GetLatestPublishedFeedbackHistoriesHandler(mapper, _repositoryWrapper.Object)
            .Handle(new GetLatestPublishedFeedbackHistoriesQuery(), CancellationToken.None);

        var history = Assert.Single(result.Value);
        Assert.Equal("Заголовок", history.Title);
        var localizationDto = Assert.Single(history.Localizations);
        Assert.Equal("en", localizationDto.Language.Code);
        Assert.Equal("Title", localizationDto.Title);
        Assert.Equal("Story", localizationDto.Story);
        Assert.Equal(TranslationStatus.Outdated, localizationDto.TranslationStatus);
    }

    private GetLatestPublishedFeedbackHistoriesHandler CreateHandler() =>
        new(_mapper.Object, _repositoryWrapper.Object);
}
