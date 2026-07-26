using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Moq;
using VictoryCenter.BLL.Commands.Admin.EventNews.Create;
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

namespace VictoryCenter.UnitTests.MediatRHandlersTests.EventNews;

public class CreateEventNewsTests
{
    private readonly Mock<IMapper> _mapper = new();
    private readonly Mock<IRepositoryWrapper> _repo = new();
    private readonly Mock<ISlugService> _slugService = new();

    private static readonly List<EventNewsCategory> Categories =
    [
        new() { Id = 1, Name = "News" },
        new() { Id = 2, Name = "Events" }
    ];

    private static readonly List<Image> Images =
    [
        new() { Id = 1, BlobName = "image-1", MimeType = "image/png" }
    ];

    private static readonly List<LocalizationLanguage> Languages =
    [
        new() { Id = 1, Code = "uk", Name = "Ukrainian" },
        new() { Id = 2, Code = "en", Name = "English" }
    ];

    [Fact]
    public async Task Handle_ValidPublishedRequest_ReturnsSuccessAndSetsSlug()
    {
        var (sut, entity) = CreateSut(saveChanges: 1);

        var result = await sut.Handle(Command(Dto(Status.Published)), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal("event-news-title", entity.Slug);
        Assert.Single(entity.Categories);
        Assert.Single(entity.Localizations);
    }

    [Fact]
    public async Task Handle_ValidPublishedRequest_PassesDefaultEntityIdToSlugService()
    {
        var (sut, _) = CreateSut(saveChanges: 1);

        await sut.Handle(Command(Dto(Status.Published)), CancellationToken.None);

        _slugService.Verify(
            service => service.GenerateUniqueEventNewsSlugAsync(
                0,
                "Event News Title",
                CancellationToken.None),
            Times.Once);
    }

    [Fact]
    public async Task Handle_ValidDraftWithoutTitle_ReturnsSuccessWithoutSlug()
    {
        var (sut, entity) = CreateSut(saveChanges: 1);

        var result = await sut.Handle(
            Command(new CreateEventNewsDto
            {
                Status = Status.Draft
            }),
            CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Null(entity.Slug);
        Assert.Empty(entity.Localizations);
    }

    [Fact]
    public async Task Handle_DraftLocalizationWithoutContent_IgnoresLocalization()
    {
        var (sut, entity) = CreateSut(saveChanges: 1);

        var result = await sut.Handle(
            Command(new CreateEventNewsDto
            {
                Status = Status.Draft,
                Localizations = [new CreateEventNewsLocalizationDto { LanguageId = 1 }]
            }),
            CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Empty(entity.Localizations);
        _repo.Verify(
            repository => repository.LocalizationLanguagesRepository.GetAllAsync(
                It.IsAny<QueryOptions<LocalizationLanguage>>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_NullDraftCollections_ReturnsSuccess()
    {
        var (sut, entity) = CreateSut(saveChanges: 1);

        var result = await sut.Handle(
            Command(new CreateEventNewsDto
            {
                Status = Status.Draft,
                CategoryIds = null!,
                Localizations = null!
            }),
            CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Empty(entity.Categories);
        Assert.Empty(entity.Localizations);
    }

    [Fact]
    public async Task Handle_MissingCategory_ReturnsNotFoundError()
    {
        var (sut, _) = CreateSut(saveChanges: 1, categories: [Categories[0]]);

        var result = await sut.Handle(Command(Dto(Status.Published, categoryIds: [1, 2])), CancellationToken.None);

        Assert.Contains(
            nameof(EventNewsCategory),
            string.Join(" | ", result.Errors.Select(error => error.Message)));
    }

    [Fact]
    public async Task Handle_MissingImage_ReturnsNotFoundError()
    {
        var (sut, _) = CreateSut(saveChanges: 1, images: []);

        var result = await sut.Handle(Command(Dto(Status.Published)), CancellationToken.None);

        Assert.Contains(
            nameof(Image),
            string.Join(" | ", result.Errors.Select(error => error.Message)));
    }

    [Fact]
    public async Task Handle_MissingLocalizationLanguage_ReturnsNotFoundError()
    {
        var (sut, _) = CreateSut(saveChanges: 1, languages: []);

        var result = await sut.Handle(Command(Dto(Status.Published)), CancellationToken.None);

        Assert.Contains(
            nameof(LocalizationLanguage),
            string.Join(" | ", result.Errors.Select(error => error.Message)));
    }

    [Fact]
    public async Task Handle_SaveChangesReturnsZero_ReturnsFailedToCreateEntity()
    {
        var (sut, _) = CreateSut(saveChanges: 0);

        var result = await sut.Handle(Command(Dto(Status.Published)), CancellationToken.None);

        Assert.Contains(
            ErrorMessagesConstants.FailedToCreateEntity(typeof(EventNewsEntity)),
            string.Join(" | ", result.Errors.Select(error => error.Message)));
    }

    [Fact]
    public async Task Handle_SaveChangesThrowsDbUpdateException_PropagatesException()
    {
        var (sut, _) = CreateSut(saveChanges: 1, throwOnSave: true);

        await Assert.ThrowsAsync<DbUpdateException>(
            () => sut.Handle(Command(Dto(Status.Published)), CancellationToken.None));
    }

    [Fact]
    public async Task Handle_SlugUniqueConstraintViolation_RegeneratesSlugAndRetriesSave()
    {
        var (sut, entity) = CreateSut(saveChanges: 1);
        var exception = SqlExceptionFactory.CreateDbUpdateException(2601, "Unique constraint violation");
        _repo.SetupSequence(repository => repository.SaveChangesAsync())
            .ThrowsAsync(exception)
            .ReturnsAsync(1);
        _repo.Setup(repository => repository.EventNewsRepository.ExistsAsync(
                It.IsAny<System.Linq.Expressions.Expression<Func<EventNewsEntity, bool>>>()))
            .ReturnsAsync(true);
        _slugService.SetupSequence(service => service.GenerateUniqueEventNewsSlugAsync(
                It.IsAny<long>(),
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync("event-news-title")
            .ReturnsAsync("event-news-title-1");

        var result = await sut.Handle(Command(Dto(Status.Published)), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal("event-news-title-1", entity.Slug);
        _repo.Verify(repository => repository.SaveChangesAsync(), Times.Exactly(2));
        _slugService.Verify(
            service => service.GenerateUniqueEventNewsSlugAsync(
                0,
                "Event News Title",
                CancellationToken.None),
            Times.Exactly(2));
    }

    [Fact]
    public async Task Handle_NonSlugUniqueConstraintViolation_PropagatesWithoutRetry()
    {
        var (sut, _) = CreateSut(saveChanges: 1);
        var exception = SqlExceptionFactory.CreateDbUpdateException(2601, "Unique constraint violation");
        _repo.Setup(repository => repository.SaveChangesAsync()).ThrowsAsync(exception);
        _repo.Setup(repository => repository.EventNewsRepository.ExistsAsync(
                It.IsAny<System.Linq.Expressions.Expression<Func<EventNewsEntity, bool>>>()))
            .ReturnsAsync(false);

        var actualException = await Assert.ThrowsAsync<DbUpdateException>(
            () => sut.Handle(Command(Dto(Status.Published)), CancellationToken.None));

        Assert.Same(exception, actualException);
        _repo.Verify(repository => repository.SaveChangesAsync(), Times.Once);
        _slugService.Verify(
            service => service.GenerateUniqueEventNewsSlugAsync(
                It.IsAny<long>(),
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    private (CreateEventNewsHandler sut, EventNewsEntity entity) CreateSut(
        int saveChanges,
        List<EventNewsCategory>? categories = null,
        List<Image>? images = null,
        List<LocalizationLanguage>? languages = null,
        bool throwOnSave = false)
    {
        var entity = new EventNewsEntity
        {
            Categories = [],
            Localizations = []
        };

        SetUpMapper(entity);
        SetUpRepositories(saveChanges, categories ?? Categories, images ?? Images, languages ?? Languages, throwOnSave);
        SetUpSlugService();

        return (new CreateEventNewsHandler(_mapper.Object, _repo.Object, _slugService.Object), entity);
    }

    private void SetUpMapper(EventNewsEntity entity)
    {
        _mapper.Reset();

        _mapper
            .Setup(mapper => mapper.Map<EventNewsEntity>(It.IsAny<CreateEventNewsDto>()))
            .Returns((CreateEventNewsDto dto) =>
            {
                entity.Resource = dto.Resource;
                entity.PublishedAt = dto.PublishedAt;
                entity.Status = dto.Status;
                entity.PreviewImageId = dto.PreviewImageId;
                entity.BackgroundImageId = dto.BackgroundImageId;
                entity.Categories = [];
                entity.Localizations = [];
                return entity;
            });

        _mapper
            .Setup(mapper => mapper.Map<EventNewsDto>(It.IsAny<EventNewsEntity>()))
            .Returns((EventNewsEntity eventNews) => new EventNewsDto
            {
                Id = eventNews.Id,
                Slug = eventNews.Slug,
                Resource = eventNews.Resource,
                PublishedAt = eventNews.PublishedAt,
                Status = eventNews.Status,
                Categories = [],
                Localizations = []
            });
    }

    private void SetUpRepositories(
        int saveChanges,
        List<EventNewsCategory> categories,
        List<Image> images,
        List<LocalizationLanguage> languages,
        bool throwOnSave)
    {
        _repo.Reset();

        _repo
            .Setup(repo => repo.EventNewsCategoryRepository.GetAllAsync(It.IsAny<QueryOptions<EventNewsCategory>>()))
            .ReturnsAsync((QueryOptions<EventNewsCategory> options) =>
            {
                var predicate = options.Filter?.Compile();
                return predicate is null ? categories : [.. categories.Where(predicate)];
            });

        _repo
            .Setup(repo => repo.ImageRepository.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<Image>>()))
            .ReturnsAsync((QueryOptions<Image> options) =>
            {
                var predicate = options.Filter?.Compile();
                return predicate is null ? images.FirstOrDefault() : images.FirstOrDefault(predicate);
            });

        _repo
            .Setup(repo => repo.LocalizationLanguagesRepository.GetAllAsync(It.IsAny<QueryOptions<LocalizationLanguage>>()))
            .ReturnsAsync((QueryOptions<LocalizationLanguage> options) =>
            {
                var predicate = options.Filter?.Compile();
                return predicate is null ? languages : [.. languages.Where(predicate)];
            });

        _repo
            .Setup(repo => repo.EventNewsRepository.CreateAsync(It.IsAny<EventNewsEntity>()));

        if (throwOnSave)
        {
            _repo.Setup(repo => repo.SaveChangesAsync()).ThrowsAsync(new DbUpdateException());
            return;
        }

        _repo.Setup(repo => repo.SaveChangesAsync()).ReturnsAsync(saveChanges);
    }

    private void SetUpSlugService()
    {
        _slugService.Reset();

        _slugService
            .Setup(service => service.GenerateUniqueEventNewsSlugAsync(It.IsAny<long>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync("event-news-title");
    }

    private static CreateEventNewsCommand Command(CreateEventNewsDto dto) => new(dto);

    private static CreateEventNewsDto Dto(Status status, List<long>? categoryIds = null)
    {
        return new CreateEventNewsDto
        {
            Status = status,
            PublishedAt = DateTimeOffset.UtcNow,
            PreviewImageId = 1,
            CategoryIds = categoryIds ?? [1],
            Localizations =
            [
                new CreateEventNewsLocalizationDto
                {
                    LanguageId = 1,
                    Title = "Event News Title",
                    Description = "Valid event description"
                },
            ]
        };
    }
}
