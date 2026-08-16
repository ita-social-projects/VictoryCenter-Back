using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Moq;
using VictoryCenter.BLL.Commands.Admin.EventNews.Update;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.EventNews;
using VictoryCenter.BLL.Interfaces.SlugService;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Entities.Localization;
using VictoryCenter.DAL.Enums;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;
using VictoryCenter.UnitTests.Utils;
using EventNewsEntity = VictoryCenter.DAL.Entities.EventNews;
using EventNewsPredicate = System.Linq.Expressions.Expression<System.Func<VictoryCenter.DAL.Entities.EventNews, bool>>;

namespace VictoryCenter.UnitTests.MediatRHandlersTests.EventNews;

public class UpdateEventNewsTests
{
    private readonly Mock<IMapper> _mapper = new();
    private readonly Mock<IRepositoryWrapper> _repositoryWrapper = new();
    private readonly Mock<ISlugService> _slugService = new();

    [Fact]
    public async Task Handle_WhenEntityDoesNotExist_ReturnsNotFound()
    {
        var handler = CreateHandler(eventNews: null);

        var result = await handler.Handle(new UpdateEventNewsCommand(10, DraftDto()), CancellationToken.None);

        Assert.True(result.IsFailed);
        Assert.Contains(typeof(EventNewsEntity).Name, result.Errors[0].Message);
        _repositoryWrapper.Verify(wrapper => wrapper.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task Handle_ValidRequest_UpdatesCompleteAggregate()
    {
        var eventNews = ExistingEventNews();
        var originalCreatedAt = eventNews.CreatedAt;
        var originalLocalizationCreatedAt = eventNews.Localizations.Single(item => item.LanguageId == 1).CreatedAt;
        var handler = CreateHandler(eventNews);

        var result = await handler.Handle(
            new UpdateEventNewsCommand(10, PublishedDto()),
            CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal("https://example.com/updated", eventNews.Resource);
        Assert.Equal(Status.Published, eventNews.Status);
        Assert.Equal(3, eventNews.PreviewImageId);
        Assert.Equal(2, eventNews.BackgroundImageId);
        Assert.Equal(originalCreatedAt, eventNews.CreatedAt);
        Assert.Equal("updated-event-title", eventNews.Slug);
        Assert.Equal([2], eventNews.Categories.Select(category => category.Id));
        Assert.Equal([1, 3], eventNews.Localizations.Select(item => item.LanguageId).OrderBy(id => id));

        var updatedLocalization = eventNews.Localizations.Single(item => item.LanguageId == 1);
        Assert.Equal("Updated Event Title", updatedLocalization.Title);
        Assert.Equal("Updated event description", updatedLocalization.Description);
        Assert.Equal(TranslationStatus.Relevant, updatedLocalization.TranslationStatus);
        Assert.Equal(originalLocalizationCreatedAt, updatedLocalization.CreatedAt);

        var newLocalization = eventNews.Localizations.Single(item => item.LanguageId == 3);
        Assert.Equal("German Event Title", newLocalization.Title);
        Assert.NotEqual(default, newLocalization.CreatedAt);
        _repositoryWrapper.Verify(wrapper => wrapper.SaveChangesAsync(), Times.Once);
        _slugService.Verify(
            service => service.GenerateUniqueEventNewsSlugAsync(
                10,
                "Updated Event Title",
                CancellationToken.None),
            Times.Once);
    }

    [Fact]
    public async Task Handle_UnchangedRequest_ReturnsSuccessWithoutSaving()
    {
        var eventNews = ExistingEventNews();
        var handler = CreateHandler(eventNews);

        var result = await handler.Handle(
            new UpdateEventNewsCommand(10, MatchingDto(eventNews)),
            CancellationToken.None);

        Assert.True(result.IsSuccess);
        _repositoryWrapper.Verify(wrapper => wrapper.SaveChangesAsync(), Times.Never);
        _slugService.Verify(
            service => service.GenerateUniqueEventNewsSlugAsync(
                It.IsAny<long>(),
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_WhenLocalizationContentIsUnchanged_PreservesTranslationStatus()
    {
        var eventNews = ExistingEventNews();
        var outdatedLocalization = eventNews.Localizations.Single(item => item.LanguageId == 1);
        var dto = MatchingDto(eventNews) with { Resource = "https://example.com/updated" };
        var handler = CreateHandler(eventNews);

        var result = await handler.Handle(
            new UpdateEventNewsCommand(10, dto),
            CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(TranslationStatus.Outdated, outdatedLocalization.TranslationStatus);
        _repositoryWrapper.Verify(wrapper => wrapper.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task Handle_DraftWithoutContent_RemovesAssociationsAndSlug()
    {
        var eventNews = ExistingEventNews();
        var handler = CreateHandler(eventNews);
        var dto = DraftDto() with
        {
            Localizations = [new CreateEventNewsLocalizationDto { LanguageId = 1 }]
        };

        var result = await handler.Handle(new UpdateEventNewsCommand(10, dto), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Null(eventNews.Slug);
        Assert.Empty(eventNews.Categories);
        Assert.Empty(eventNews.Localizations);
        _repositoryWrapper.Verify(
            wrapper => wrapper.LocalizationLanguagesRepository.GetAllAsync(
                It.IsAny<QueryOptions<LocalizationLanguage>>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_WhenCategoryDoesNotExist_ReturnsNotFoundWithoutSaving()
    {
        var handler = CreateHandler(ExistingEventNews(), categories: [Category(1)]);

        var result = await handler.Handle(
            new UpdateEventNewsCommand(10, PublishedDto()),
            CancellationToken.None);

        Assert.True(result.IsFailed);
        Assert.Contains(nameof(EventNewsCategory), string.Join(" | ", result.Errors.Select(error => error.Message)));
        _repositoryWrapper.Verify(wrapper => wrapper.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task Handle_WhenImageDoesNotExist_ReturnsNotFoundWithoutSaving()
    {
        var handler = CreateHandler(ExistingEventNews(), images: [Image(1), Image(2)]);

        var result = await handler.Handle(
            new UpdateEventNewsCommand(10, PublishedDto()),
            CancellationToken.None);

        Assert.True(result.IsFailed);
        Assert.Contains(nameof(Image), string.Join(" | ", result.Errors.Select(error => error.Message)));
        _repositoryWrapper.Verify(wrapper => wrapper.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task Handle_WhenLanguageDoesNotExist_ReturnsNotFoundWithoutSaving()
    {
        var handler = CreateHandler(ExistingEventNews(), languages: [Language(1), Language(2)]);

        var result = await handler.Handle(
            new UpdateEventNewsCommand(10, PublishedDto()),
            CancellationToken.None);

        Assert.True(result.IsFailed);
        Assert.Contains(
            nameof(LocalizationLanguage),
            string.Join(" | ", result.Errors.Select(error => error.Message)));
        _repositoryWrapper.Verify(wrapper => wrapper.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task Handle_WhenSaveReturnsZero_ReturnsFailedToUpdate()
    {
        var handler = CreateHandler(ExistingEventNews(), saveChanges: 0);

        var result = await handler.Handle(
            new UpdateEventNewsCommand(10, PublishedDto()),
            CancellationToken.None);

        Assert.True(result.IsFailed);
        Assert.Contains(
            ErrorMessagesConstants.FailedToUpdateEntity(typeof(EventNewsEntity)),
            result.Errors[0].Message);
    }

    [Fact]
    public async Task Handle_WhenEntityIsDeletedConcurrently_ReturnsNotFound()
    {
        var handler = CreateHandler(
            ExistingEventNews(),
            saveException: new DbUpdateConcurrencyException());
        _repositoryWrapper.Setup(wrapper => wrapper.EventNewsRepository.ExistsAsync(
                It.IsAny<EventNewsPredicate>()))
            .ReturnsAsync(false);

        var result = await handler.Handle(
            new UpdateEventNewsCommand(10, PublishedDto()),
            CancellationToken.None);

        Assert.True(result.IsFailed);
        Assert.Contains(typeof(EventNewsEntity).Name, result.Errors[0].Message);
    }

    [Fact]
    public async Task Handle_WhenConcurrencyFailureIsNotCausedByEntityDeletion_PropagatesException()
    {
        var exception = new DbUpdateConcurrencyException();
        var handler = CreateHandler(ExistingEventNews(), saveException: exception);
        _repositoryWrapper.Setup(wrapper => wrapper.EventNewsRepository.ExistsAsync(
                It.IsAny<EventNewsPredicate>()))
            .ReturnsAsync(true);

        var actualException = await Assert.ThrowsAsync<DbUpdateConcurrencyException>(() => handler.Handle(
            new UpdateEventNewsCommand(10, PublishedDto()),
            CancellationToken.None));

        Assert.Same(exception, actualException);
    }

    [Fact]
    public async Task Handle_SlugCollision_RegeneratesSlugAndRetriesSave()
    {
        var eventNews = ExistingEventNews();
        var handler = CreateHandler(eventNews);
        var exception = SqlExceptionFactory.CreateDbUpdateException(2601, "Unique constraint violation");
        _repositoryWrapper.SetupSequence(wrapper => wrapper.SaveChangesAsync())
            .ThrowsAsync(exception)
            .ReturnsAsync(1);
        _repositoryWrapper.Setup(wrapper => wrapper.EventNewsRepository.ExistsAsync(
                It.IsAny<EventNewsPredicate>()))
            .ReturnsAsync(true);
        _slugService.SetupSequence(service => service.GenerateUniqueEventNewsSlugAsync(
                10,
                "Updated Event Title",
                CancellationToken.None))
            .ReturnsAsync("updated-event-title")
            .ReturnsAsync("updated-event-title-1");

        var result = await handler.Handle(
            new UpdateEventNewsCommand(10, PublishedDto()),
            CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal("updated-event-title-1", eventNews.Slug);
        _repositoryWrapper.Verify(wrapper => wrapper.SaveChangesAsync(), Times.Exactly(2));
    }

    [Fact]
    public async Task Handle_WhenSlugCollisionsExhaustRetryLimit_PropagatesFinalException()
    {
        var eventNews = ExistingEventNews();
        var exception = SqlExceptionFactory.CreateDbUpdateException(2601, "Unique constraint violation");
        var handler = CreateHandler(eventNews);
        _repositoryWrapper.SetupSequence(wrapper => wrapper.SaveChangesAsync())
            .ThrowsAsync(exception)
            .ThrowsAsync(exception)
            .ThrowsAsync(exception);
        _repositoryWrapper.Setup(wrapper => wrapper.EventNewsRepository.ExistsAsync(
                It.IsAny<EventNewsPredicate>()))
            .ReturnsAsync(true);
        _slugService.SetupSequence(service => service.GenerateUniqueEventNewsSlugAsync(
                10,
                "Updated Event Title",
                CancellationToken.None))
            .ReturnsAsync("updated-event-title")
            .ReturnsAsync("updated-event-title-1")
            .ReturnsAsync("updated-event-title-2");

        var actualException = await Assert.ThrowsAsync<DbUpdateException>(() => handler.Handle(
            new UpdateEventNewsCommand(10, PublishedDto()),
            CancellationToken.None));

        Assert.Same(exception, actualException);
        Assert.Equal("updated-event-title-2", eventNews.Slug);
        _repositoryWrapper.Verify(wrapper => wrapper.SaveChangesAsync(), Times.Exactly(3));
    }

    [Fact]
    public async Task Handle_UniqueConstraintOnNonSlugColumn_PropagatesException()
    {
        var exception = SqlExceptionFactory.CreateDbUpdateException(2601, "Unique constraint violation");
        var handler = CreateHandler(ExistingEventNews(), saveException: exception);
        _repositoryWrapper.Setup(wrapper => wrapper.EventNewsRepository.ExistsAsync(
                It.IsAny<EventNewsPredicate>()))
            .ReturnsAsync(false);

        var actualException = await Assert.ThrowsAsync<DbUpdateException>(() => handler.Handle(
            new UpdateEventNewsCommand(10, PublishedDto()),
            CancellationToken.None));

        Assert.Same(exception, actualException);
        _repositoryWrapper.Verify(wrapper => wrapper.SaveChangesAsync(), Times.Once);
    }

    private UpdateEventNewsHandler CreateHandler(
        EventNewsEntity? eventNews,
        IReadOnlyCollection<EventNewsCategory>? categories = null,
        IReadOnlyCollection<Image>? images = null,
        IReadOnlyCollection<LocalizationLanguage>? languages = null,
        int saveChanges = 1,
        DbUpdateException? saveException = null)
    {
        categories ??= [Category(1), Category(2)];
        images ??= [Image(1), Image(2), Image(3)];
        languages ??= [Language(1), Language(2), Language(3)];

        _repositoryWrapper.Reset();
        _mapper.Reset();
        _slugService.Reset();

        _repositoryWrapper
            .Setup(wrapper => wrapper.EventNewsRepository.GetFirstOrDefaultAsync(
                It.IsAny<QueryOptions<EventNewsEntity>>()))
            .ReturnsAsync(eventNews);
        _repositoryWrapper
            .Setup(wrapper => wrapper.EventNewsCategoryRepository.GetAllAsync(
                It.IsAny<QueryOptions<EventNewsCategory>>()))
            .ReturnsAsync((QueryOptions<EventNewsCategory> options) => ApplyFilter(categories, options));
        _repositoryWrapper
            .Setup(wrapper => wrapper.ImageRepository.GetAllAsync(It.IsAny<QueryOptions<Image>>()))
            .ReturnsAsync((QueryOptions<Image> options) => ApplyFilter(images, options));
        _repositoryWrapper
            .Setup(wrapper => wrapper.LocalizationLanguagesRepository.GetAllAsync(
                It.IsAny<QueryOptions<LocalizationLanguage>>()))
            .ReturnsAsync((QueryOptions<LocalizationLanguage> options) => ApplyFilter(languages, options));

        if (saveException is not null)
        {
            _repositoryWrapper.Setup(wrapper => wrapper.SaveChangesAsync()).ThrowsAsync(saveException);
        }
        else
        {
            _repositoryWrapper.Setup(wrapper => wrapper.SaveChangesAsync()).ReturnsAsync(saveChanges);
        }

        _slugService.Setup(service => service.GenerateUniqueEventNewsSlugAsync(
                It.IsAny<long>(),
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync("updated-event-title");

        _mapper.Setup(mapper => mapper.Map<EventNewsDto>(It.IsAny<EventNewsEntity>()))
            .Returns((EventNewsEntity entity) => new EventNewsDto
            {
                Id = entity.Id,
                Slug = entity.Slug,
                Resource = entity.Resource,
                PublishedAt = entity.PublishedAt,
                Status = entity.Status
            });

        return new UpdateEventNewsHandler(
            _mapper.Object,
            _repositoryWrapper.Object,
            _slugService.Object,
            TimeProvider.System);
    }

    private static IEnumerable<TEntity> ApplyFilter<TEntity>(
        IEnumerable<TEntity> entities,
        QueryOptions<TEntity> options)
        where TEntity : class
    {
        var predicate = options.Filter?.Compile();
        return predicate is null ? entities : entities.Where(predicate);
    }

    private static EventNewsEntity ExistingEventNews()
    {
        var firstLanguage = Language(1);
        var secondLanguage = Language(2);

        return new EventNewsEntity
        {
            Id = 10,
            Slug = "old-event-title",
            Resource = "https://example.com/original",
            PublishedAt = null,
            Status = Status.Draft,
            PreviewImageId = 1,
            PreviewImage = Image(1),
            BackgroundImageId = 2,
            BackgroundImage = Image(2),
            CreatedAt = DateTimeOffset.UtcNow.AddDays(-10),
            Categories = [Category(1)],
            Localizations =
            [
                new EventNewsLocalization
                {
                    EntityId = 10,
                    LanguageId = 1,
                    Language = firstLanguage,
                    Title = "Old Event Title",
                    Description = "Old event description",
                    TranslationStatus = TranslationStatus.Outdated,
                    CreatedAt = DateTimeOffset.UtcNow.AddDays(-5)
                },
                new EventNewsLocalization
                {
                    EntityId = 10,
                    LanguageId = 2,
                    Language = secondLanguage,
                    Title = "Community Workshop",
                    Description = "Community workshop description",
                    TranslationStatus = TranslationStatus.Relevant,
                    CreatedAt = DateTimeOffset.UtcNow.AddDays(-5)
                },
            ]
        };
    }

    private static UpdateEventNewsDto PublishedDto()
    {
        return new UpdateEventNewsDto
        {
            Resource = "https://example.com/updated",
            PublishedAt = DateTimeOffset.UtcNow,
            Status = Status.Published,
            PreviewImageId = 3,
            BackgroundImageId = 2,
            CategoryIds = [2],
            Localizations =
            [
                new CreateEventNewsLocalizationDto
                {
                    LanguageId = 1,
                    Title = "  Updated Event Title  ",
                    Description = "  Updated event description  "
                },
                new CreateEventNewsLocalizationDto
                {
                    LanguageId = 3,
                    Title = "German Event Title",
                    Description = "German event description"
                },
            ]
        };
    }

    private static UpdateEventNewsDto DraftDto()
    {
        return new UpdateEventNewsDto { Status = Status.Draft };
    }

    private static UpdateEventNewsDto MatchingDto(EventNewsEntity eventNews)
    {
        return new UpdateEventNewsDto
        {
            Resource = eventNews.Resource,
            PublishedAt = eventNews.PublishedAt,
            Status = eventNews.Status,
            PreviewImageId = eventNews.PreviewImageId,
            BackgroundImageId = eventNews.BackgroundImageId,
            CategoryIds = [.. eventNews.Categories.Select(category => category.Id)],
            Localizations = [.. eventNews.Localizations.Select(localization => new CreateEventNewsLocalizationDto
            {
                LanguageId = localization.LanguageId,
                Title = localization.Title,
                Description = localization.Description
            })]
        };
    }

    private static EventNewsCategory Category(long id)
    {
        return new EventNewsCategory { Id = id, Name = $"Category {id}" };
    }

    private static Image Image(long id)
    {
        return new Image { Id = id, BlobName = $"image-{id}", MimeType = "image/png" };
    }

    private static LocalizationLanguage Language(long id)
    {
        return new LocalizationLanguage { Id = id, Code = $"l{id}", Name = $"Language {id}" };
    }
}
