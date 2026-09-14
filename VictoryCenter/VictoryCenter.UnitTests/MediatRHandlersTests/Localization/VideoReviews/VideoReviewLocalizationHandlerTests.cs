using System.Linq.Expressions;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Moq;
using VictoryCenter.BLL.Commands.Admin.Localization.VideoReviews.Create;
using VictoryCenter.BLL.Commands.Admin.Localization.VideoReviews.Delete;
using VictoryCenter.BLL.Commands.Admin.Localization.VideoReviews.Update;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.Localization.VideoReviews;
using VictoryCenter.BLL.Queries.Admin.Localization.VideoReviews.GetByEntityId;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Entities.Localization;
using VictoryCenter.DAL.Enums;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Interfaces.Localization.Languages;
using VictoryCenter.DAL.Repositories.Interfaces.Localization.VideoReviews;
using VictoryCenter.DAL.Repositories.Interfaces.VideoReviews;
using VictoryCenter.DAL.Repositories.Options;
using VictoryCenter.UnitTests.Utils;

namespace VictoryCenter.UnitTests.MediatRHandlersTests.Localization.VideoReviews;

public class VideoReviewLocalizationHandlerTests
{
    private readonly Mock<IMapper> _mapper = new();
    private readonly Mock<IRepositoryWrapper> _wrapper = new();
    private readonly Mock<IVideoReviewsRepository> _videoReviewsRepository = new();
    private readonly Mock<IVideoReviewLocalizationsRepository> _localizationRepository = new();
    private readonly Mock<ILocalizationLanguagesRepository> _languageRepository = new();

    public VideoReviewLocalizationHandlerTests()
    {
        _wrapper.SetupGet(wrapper => wrapper.VideoReviewsRepository)
            .Returns(_videoReviewsRepository.Object);
        _wrapper.SetupGet(wrapper => wrapper.VideoReviewLocalizationsRepository)
            .Returns(_localizationRepository.Object);
        _wrapper.SetupGet(wrapper => wrapper.LocalizationLanguagesRepository)
            .Returns(_languageRepository.Object);
    }

    [Fact]
    public async Task CreateLocalization_ShouldTrimTitleAndReturnCreatedLocalization()
    {
        VideoReviewLocalization? createdLocalization = null;
        SetupExistingVideoReview();
        SetupExistingLanguage();
        SetupLocalizationDoesNotExist();
        _localizationRepository
            .Setup(repository => repository.CreateAsync(It.IsAny<VideoReviewLocalization>()))
            .Callback<VideoReviewLocalization>(localization => createdLocalization = localization)
            .ReturnsAsync((VideoReviewLocalization localization) => localization);
        _wrapper.Setup(wrapper => wrapper.SaveChangesAsync()).ReturnsAsync(1);
        var handler = new CreateVideoReviewLocalizationHandler(_wrapper.Object);

        var result = await handler.Handle(
            new CreateVideoReviewLocalizationCommand(new CreateVideoReviewLocalizationDto
            {
                EntityId = 1,
                LanguageId = 2,
                Title = "  English video review title  "
            }),
            CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.NotNull(createdLocalization);
        Assert.Equal("English video review title", createdLocalization.Title);
        Assert.Equal(TranslationStatus.Relevant, createdLocalization.TranslationStatus);
        Assert.Equal("en", result.Value.Language.Code);
        Assert.Equal("English video review title", result.Value.Title);
    }

    [Fact]
    public async Task CreateLocalization_ShouldReturnNotFound_WhenVideoReviewDoesNotExist()
    {
        _videoReviewsRepository
            .Setup(repository => repository.ExistsAsync(It.IsAny<Expression<Func<VideoReview, bool>>>()))
            .ReturnsAsync(false);
        var handler = new CreateVideoReviewLocalizationHandler(_wrapper.Object);

        var result = await handler.Handle(
            new CreateVideoReviewLocalizationCommand(new CreateVideoReviewLocalizationDto
            {
                EntityId = 999,
                LanguageId = 2,
                Title = "English video review title"
            }),
            CancellationToken.None);

        Assert.True(result.IsFailed);
        Assert.Equal(
            ErrorMessagesConstants.NotFound(999L, typeof(VideoReview)),
            result.Errors[0].Message);
    }

    [Fact]
    public async Task CreateLocalization_ShouldReturnNotFound_WhenLanguageDoesNotExist()
    {
        SetupExistingVideoReview();
        _languageRepository
            .Setup(repository => repository.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<LocalizationLanguage>>()))
            .ReturnsAsync((LocalizationLanguage?)null);
        var handler = new CreateVideoReviewLocalizationHandler(_wrapper.Object);

        var result = await handler.Handle(
            new CreateVideoReviewLocalizationCommand(new CreateVideoReviewLocalizationDto
            {
                EntityId = 1,
                LanguageId = 999,
                Title = "English video review title"
            }),
            CancellationToken.None);

        Assert.True(result.IsFailed);
        Assert.Equal(
            ErrorMessagesConstants.NotFound(999L, typeof(LocalizationLanguage)),
            result.Errors[0].Message);
    }

    [Fact]
    public async Task CreateLocalization_ShouldFail_WhenLocalizationAlreadyExists()
    {
        SetupExistingVideoReview();
        SetupExistingLanguage();
        _localizationRepository
            .Setup(repository => repository.ExistsAsync(
                It.IsAny<Expression<Func<VideoReviewLocalization, bool>>>()))
            .ReturnsAsync(true);
        var handler = new CreateVideoReviewLocalizationHandler(_wrapper.Object);

        var result = await handler.Handle(
            new CreateVideoReviewLocalizationCommand(new CreateVideoReviewLocalizationDto
            {
                EntityId = 1,
                LanguageId = 2,
                Title = "English video review title"
            }),
            CancellationToken.None);

        Assert.True(result.IsFailed);
        Assert.Equal(VideoReviewConstants.LocalizationAlreadyExists, result.Errors[0].Message);
        _localizationRepository.Verify(
            repository => repository.CreateAsync(It.IsAny<VideoReviewLocalization>()),
            Times.Never);
    }

    [Fact]
    public async Task CreateLocalization_ShouldFail_WhenSaveChangesReturnsZero()
    {
        SetupExistingVideoReview();
        SetupExistingLanguage();
        SetupLocalizationDoesNotExist();
        _localizationRepository
            .Setup(repository => repository.CreateAsync(It.IsAny<VideoReviewLocalization>()))
            .ReturnsAsync((VideoReviewLocalization localization) => localization);
        _wrapper.Setup(wrapper => wrapper.SaveChangesAsync()).ReturnsAsync(0);
        var handler = new CreateVideoReviewLocalizationHandler(_wrapper.Object);

        var result = await handler.Handle(
            new CreateVideoReviewLocalizationCommand(new CreateVideoReviewLocalizationDto
            {
                EntityId = 1,
                LanguageId = 2,
                Title = "English video review title"
            }),
            CancellationToken.None);

        Assert.True(result.IsFailed);
        Assert.Equal(
            ErrorMessagesConstants.FailedToCreateEntity(typeof(VideoReviewLocalization)),
            result.Errors[0].Message);
    }

    [Fact]
    public async Task CreateLocalization_ShouldClassifyConcurrentUniqueConstraintViolation()
    {
        SetupExistingVideoReview();
        SetupExistingLanguage();
        SetupLocalizationDoesNotExist();
        _localizationRepository
            .Setup(repository => repository.CreateAsync(It.IsAny<VideoReviewLocalization>()))
            .ReturnsAsync((VideoReviewLocalization localization) => localization);
        _wrapper.Setup(wrapper => wrapper.SaveChangesAsync())
            .ThrowsAsync(SqlExceptionFactory.CreateDbUpdateException(2627, "Unique constraint violation"));
        var handler = new CreateVideoReviewLocalizationHandler(_wrapper.Object);

        var result = await handler.Handle(
            new CreateVideoReviewLocalizationCommand(new CreateVideoReviewLocalizationDto
            {
                EntityId = 1,
                LanguageId = 2,
                Title = "English video review title"
            }),
            CancellationToken.None);

        Assert.True(result.IsFailed);
        Assert.Equal(VideoReviewConstants.LocalizationAlreadyExists, result.Errors[0].Message);
    }

    [Fact]
    public async Task GetLocalizations_ShouldReturnNotFound_WhenVideoReviewDoesNotExist()
    {
        _videoReviewsRepository
            .Setup(repository => repository.ExistsAsync(It.IsAny<Expression<Func<VideoReview, bool>>>()))
            .ReturnsAsync(false);
        var handler = new GetVideoReviewLocalizationsByEntityIdHandler(_mapper.Object, _wrapper.Object);

        var result = await handler.Handle(
            new GetVideoReviewLocalizationsByEntityIdQuery(1),
            CancellationToken.None);

        Assert.True(result.IsFailed);
        _localizationRepository.Verify(
            repository => repository.GetAllAsync(It.IsAny<QueryOptions<VideoReviewLocalization>>()),
            Times.Never);
    }

    [Fact]
    public async Task GetLocalizations_ShouldUseReadOnlyQueryAndReturnMappedLocalizations()
    {
        QueryOptions<VideoReviewLocalization>? capturedOptions = null;
        SetupExistingVideoReview();
        _localizationRepository
            .Setup(repository => repository.GetAllAsync(It.IsAny<QueryOptions<VideoReviewLocalization>>()))
            .Callback<QueryOptions<VideoReviewLocalization>>(options => capturedOptions = options)
            .ReturnsAsync([]);
        _mapper
            .Setup(mapper => mapper.Map<List<VideoReviewLocalizationDto>>(
                It.IsAny<IEnumerable<VideoReviewLocalization>>()))
            .Returns([]);
        var handler = new GetVideoReviewLocalizationsByEntityIdHandler(_mapper.Object, _wrapper.Object);

        var result = await handler.Handle(
            new GetVideoReviewLocalizationsByEntityIdQuery(1),
            CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.NotNull(capturedOptions);
        Assert.True(capturedOptions.AsNoTracking);
        Assert.NotNull(capturedOptions.Include);
        Assert.NotNull(capturedOptions.OrderByASC);
    }

    [Fact]
    public async Task UpdateLocalization_ShouldTrimTitleAndMarkTranslationRelevant()
    {
        var localization = new VideoReviewLocalization
        {
            EntityId = 1,
            LanguageId = 2,
            Title = "Old title",
            TranslationStatus = TranslationStatus.Outdated
        };
        _localizationRepository
            .Setup(repository => repository.GetFirstOrDefaultAsync(
                It.IsAny<QueryOptions<VideoReviewLocalization>>()))
            .ReturnsAsync(localization);
        _wrapper.Setup(wrapper => wrapper.SaveChangesAsync()).ReturnsAsync(1);
        _mapper
            .Setup(mapper => mapper.Map<VideoReviewLocalizationDto>(localization))
            .Returns(new VideoReviewLocalizationDto
            {
                EntityId = 1,
                Title = "New title",
                TranslationStatus = TranslationStatus.Relevant
            });
        var handler = new UpdateVideoReviewLocalizationHandler(_mapper.Object, _wrapper.Object);

        var result = await handler.Handle(
            new UpdateVideoReviewLocalizationCommand(
                1,
                2,
                new UpdateVideoReviewLocalizationDto { Title = "  New title  " }),
            CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal("New title", localization.Title);
        Assert.Equal(TranslationStatus.Relevant, localization.TranslationStatus);
        _wrapper.Verify(wrapper => wrapper.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task UpdateLocalization_ShouldShortCircuit_WhenNothingChanged()
    {
        var localization = new VideoReviewLocalization
        {
            EntityId = 1,
            LanguageId = 2,
            Title = "Same title",
            TranslationStatus = TranslationStatus.Relevant
        };
        _localizationRepository
            .Setup(repository => repository.GetFirstOrDefaultAsync(
                It.IsAny<QueryOptions<VideoReviewLocalization>>()))
            .ReturnsAsync(localization);
        var handler = new UpdateVideoReviewLocalizationHandler(_mapper.Object, _wrapper.Object);

        var result = await handler.Handle(
            new UpdateVideoReviewLocalizationCommand(
                1,
                2,
                new UpdateVideoReviewLocalizationDto { Title = "Same title" }),
            CancellationToken.None);

        Assert.True(result.IsSuccess);
        _wrapper.Verify(wrapper => wrapper.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task UpdateLocalization_ShouldReturnNotFound_WhenLocalizationDoesNotExist()
    {
        _localizationRepository
            .Setup(repository => repository.GetFirstOrDefaultAsync(
                It.IsAny<QueryOptions<VideoReviewLocalization>>()))
            .ReturnsAsync((VideoReviewLocalization?)null);
        var handler = new UpdateVideoReviewLocalizationHandler(_mapper.Object, _wrapper.Object);

        var result = await handler.Handle(
            new UpdateVideoReviewLocalizationCommand(
                1,
                2,
                new UpdateVideoReviewLocalizationDto { Title = "Title" }),
            CancellationToken.None);

        Assert.True(result.IsFailed);
        Assert.Equal(
            ErrorMessagesConstants.NotFound((1L, 2L), typeof(VideoReviewLocalization)),
            result.Errors[0].Message);
    }

    [Fact]
    public async Task UpdateLocalization_ShouldReturnNotFound_WhenLocalizationIsDeletedConcurrently()
    {
        var localization = new VideoReviewLocalization
        {
            EntityId = 1,
            LanguageId = 2,
            Title = "Old title"
        };
        _localizationRepository
            .Setup(repository => repository.GetFirstOrDefaultAsync(
                It.IsAny<QueryOptions<VideoReviewLocalization>>()))
            .ReturnsAsync(localization);
        _wrapper.Setup(wrapper => wrapper.SaveChangesAsync())
            .ThrowsAsync(new DbUpdateConcurrencyException());
        var handler = new UpdateVideoReviewLocalizationHandler(_mapper.Object, _wrapper.Object);

        var result = await handler.Handle(
            new UpdateVideoReviewLocalizationCommand(
                1,
                2,
                new UpdateVideoReviewLocalizationDto { Title = "New title" }),
            CancellationToken.None);

        Assert.True(result.IsFailed);
        Assert.Equal(
            ErrorMessagesConstants.NotFound((1L, 2L), typeof(VideoReviewLocalization)),
            result.Errors[0].Message);
    }

    [Fact]
    public async Task DeleteLocalization_ShouldDeleteExistingLocalization()
    {
        var localization = new VideoReviewLocalization
        {
            EntityId = 1,
            LanguageId = 2,
            Title = "Title"
        };
        _localizationRepository
            .Setup(repository => repository.GetFirstOrDefaultAsync(
                It.IsAny<QueryOptions<VideoReviewLocalization>>()))
            .ReturnsAsync(localization);
        _wrapper.Setup(wrapper => wrapper.SaveChangesAsync()).ReturnsAsync(1);
        var handler = new DeleteVideoReviewLocalizationHandler(_wrapper.Object);

        var result = await handler.Handle(new DeleteVideoReviewLocalizationCommand(1, 2), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(1, result.Value.EntityId);
        Assert.Equal(2, result.Value.LanguageId);
        _localizationRepository.Verify(repository => repository.Delete(localization), Times.Once);
    }

    [Fact]
    public async Task DeleteLocalization_ShouldReturnNotFound_WhenLocalizationDoesNotExist()
    {
        _localizationRepository
            .Setup(repository => repository.GetFirstOrDefaultAsync(
                It.IsAny<QueryOptions<VideoReviewLocalization>>()))
            .ReturnsAsync((VideoReviewLocalization?)null);
        var handler = new DeleteVideoReviewLocalizationHandler(_wrapper.Object);

        var result = await handler.Handle(new DeleteVideoReviewLocalizationCommand(1, 2), CancellationToken.None);

        Assert.True(result.IsFailed);
        Assert.Equal(
            ErrorMessagesConstants.NotFound((1L, 2L), typeof(VideoReviewLocalization)),
            result.Errors[0].Message);
    }

    [Fact]
    public async Task DeleteLocalization_ShouldReturnNotFound_WhenLocalizationIsDeletedConcurrently()
    {
        var localization = new VideoReviewLocalization
        {
            EntityId = 1,
            LanguageId = 2,
            Title = "Title"
        };
        _localizationRepository
            .Setup(repository => repository.GetFirstOrDefaultAsync(
                It.IsAny<QueryOptions<VideoReviewLocalization>>()))
            .ReturnsAsync(localization);
        _wrapper.Setup(wrapper => wrapper.SaveChangesAsync())
            .ThrowsAsync(new DbUpdateConcurrencyException());
        var handler = new DeleteVideoReviewLocalizationHandler(_wrapper.Object);

        var result = await handler.Handle(new DeleteVideoReviewLocalizationCommand(1, 2), CancellationToken.None);

        Assert.True(result.IsFailed);
        Assert.Equal(
            ErrorMessagesConstants.NotFound((1L, 2L), typeof(VideoReviewLocalization)),
            result.Errors[0].Message);
    }

    private void SetupExistingVideoReview()
    {
        _videoReviewsRepository
            .Setup(repository => repository.ExistsAsync(It.IsAny<Expression<Func<VideoReview, bool>>>()))
            .ReturnsAsync(true);
    }

    private void SetupExistingLanguage()
    {
        _languageRepository
            .Setup(repository => repository.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<LocalizationLanguage>>()))
            .ReturnsAsync(new LocalizationLanguage { Id = 2, Code = "en", Name = "Англійська" });
    }

    private void SetupLocalizationDoesNotExist()
    {
        _localizationRepository
            .Setup(repository => repository.ExistsAsync(
                It.IsAny<Expression<Func<VideoReviewLocalization, bool>>>()))
            .ReturnsAsync(false);
    }
}
