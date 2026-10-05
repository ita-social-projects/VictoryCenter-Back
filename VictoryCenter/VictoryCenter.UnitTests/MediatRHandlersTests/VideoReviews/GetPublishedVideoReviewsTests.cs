using AutoMapper;
using Moq;
using VictoryCenter.BLL;
using VictoryCenter.BLL.DTOs.Public.VideoReviews;
using VictoryCenter.BLL.Queries.Public.VideoReviews.GetPublished;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Entities.Localization;
using VictoryCenter.DAL.Enums;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Interfaces.VideoReviews;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.UnitTests.MediatRHandlersTests.VideoReviews;

public class GetPublishedVideoReviewsTests
{
    private readonly Mock<IMapper> _mapper = new();
    private readonly Mock<IRepositoryWrapper> _repositoryWrapper = new();
    private readonly Mock<IVideoReviewsRepository> _repository = new();

    public GetPublishedVideoReviewsTests()
    {
        _repositoryWrapper
            .SetupGet(wrapper => wrapper.VideoReviewsRepository)
            .Returns(_repository.Object);
    }

    [Fact]
    public async Task Handle_ShouldQueryOnlyPublishedNonArchivedVideoReviewsOrderedByPriority()
    {
        QueryOptions<VideoReview>? capturedOptions = null;
        _repository
            .Setup(repository => repository.GetAllAsync(It.IsAny<QueryOptions<VideoReview>>()))
            .Callback<QueryOptions<VideoReview>?>(options => capturedOptions = options)
            .ReturnsAsync([]);
        _mapper
            .Setup(mapper => mapper.Map<List<PublishedVideoReviewDto>>(It.IsAny<IEnumerable<VideoReview>>()))
            .Returns([]);

        await CreateHandler().Handle(new GetPublishedVideoReviewsQuery(), CancellationToken.None);

        Assert.NotNull(capturedOptions);
        Assert.NotNull(capturedOptions.Filter);
        var filter = capturedOptions.Filter.Compile();
        Assert.True(filter(new VideoReview { Status = Status.Published, IsArchived = false }));
        Assert.False(filter(new VideoReview { Status = Status.Published, IsArchived = true }));
        Assert.False(filter(new VideoReview { Status = Status.Draft, IsArchived = false }));
        Assert.NotNull(capturedOptions.OrderByASC);
        Assert.NotNull(capturedOptions.Include);
        Assert.True(capturedOptions.AsNoTracking);
    }

    [Fact]
    public async Task Handle_ShouldReturnMappedVideoReviews()
    {
        List<VideoReview> videoReviews = [new() { Id = 1, Title = "Caption", Link = "https://example.com/video", Status = Status.Published }];
        List<PublishedVideoReviewDto> dtos = [new() { Id = 1, Title = "Caption", Link = "https://example.com/video" }];
        _repository
            .Setup(repository => repository.GetAllAsync(It.IsAny<QueryOptions<VideoReview>>()))
            .ReturnsAsync(videoReviews);
        _mapper
            .Setup(mapper => mapper.Map<List<PublishedVideoReviewDto>>(videoReviews))
            .Returns(dtos);

        var result = await CreateHandler().Handle(new GetPublishedVideoReviewsQuery(), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(dtos, result.Value);
    }

    [Fact]
    public async Task Handle_WithRealMapper_ShouldMapLocalizations()
    {
        var language = new LocalizationLanguage { Id = 2, Code = "en", Name = "English" };
        var localization = new VideoReviewLocalization
        {
            EntityId = 1,
            LanguageId = language.Id,
            Language = language,
            Title = "Caption",
            TranslationStatus = TranslationStatus.Relevant
        };
        var entity = new VideoReview
        {
            Id = 1,
            Title = "Підпис",
            Link = "https://example.com/video",
            Status = Status.Published,
            Localizations = [localization]
        };
        _repository
            .Setup(repository => repository.GetAllAsync(It.IsAny<QueryOptions<VideoReview>>()))
            .ReturnsAsync([entity]);
        var mapper = new MapperConfiguration(cfg => cfg.AddMaps(typeof(BllAssemblyMarker).Assembly)).CreateMapper();

        var result = await new GetPublishedVideoReviewsHandler(mapper, _repositoryWrapper.Object)
            .Handle(new GetPublishedVideoReviewsQuery(), CancellationToken.None);

        var videoReview = Assert.Single(result.Value);
        Assert.Equal("Підпис", videoReview.Title);
        var localizationDto = Assert.Single(videoReview.Localizations);
        Assert.Equal("en", localizationDto.Language.Code);
        Assert.Equal("Caption", localizationDto.Title);
        Assert.Equal(TranslationStatus.Relevant, localizationDto.TranslationStatus);
    }

    [Fact]
    public async Task Handle_NoPublishedVideoReviews_ShouldReturnEmptyList()
    {
        _repository
            .Setup(repository => repository.GetAllAsync(It.IsAny<QueryOptions<VideoReview>>()))
            .ReturnsAsync([]);
        _mapper
            .Setup(mapper => mapper.Map<List<PublishedVideoReviewDto>>(It.IsAny<IEnumerable<VideoReview>>()))
            .Returns([]);

        var result = await CreateHandler().Handle(new GetPublishedVideoReviewsQuery(), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Empty(result.Value);
    }

    private GetPublishedVideoReviewsHandler CreateHandler() =>
        new(_mapper.Object, _repositoryWrapper.Object);
}
