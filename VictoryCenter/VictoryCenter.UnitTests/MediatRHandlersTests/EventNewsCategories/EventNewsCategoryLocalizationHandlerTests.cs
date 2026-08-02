using System.Linq.Expressions;
using AutoMapper;
using Moq;
using VictoryCenter.BLL.Commands.Admin.Localization.EventNewsCategories.Create;
using VictoryCenter.BLL.Commands.Admin.Localization.EventNewsCategories.Delete;
using VictoryCenter.BLL.Commands.Admin.Localization.EventNewsCategories.Update;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.Localization.EventNewsCategories;
using VictoryCenter.BLL.Queries.Admin.Localization.EventNewsCategories.GetByEntityId;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Entities.Localization;
using VictoryCenter.DAL.Enums;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Interfaces.EventNewsCategories;
using VictoryCenter.DAL.Repositories.Interfaces.Localization.EventNewsCategories;
using VictoryCenter.DAL.Repositories.Interfaces.Localization.Languages;
using VictoryCenter.DAL.Repositories.Options;
using VictoryCenter.UnitTests.Utils;

namespace VictoryCenter.UnitTests.MediatRHandlersTests.EventNewsCategories;

public class EventNewsCategoryLocalizationHandlerTests
{
    private readonly Mock<IMapper> _mapper = new();
    private readonly Mock<IRepositoryWrapper> _wrapper = new();
    private readonly Mock<IEventNewsCategoryRepository> _categoryRepository = new();
    private readonly Mock<IEventNewsCategoryLocalizationsRepository> _localizationRepository = new();
    private readonly Mock<ILocalizationLanguagesRepository> _languageRepository = new();

    public EventNewsCategoryLocalizationHandlerTests()
    {
        _wrapper.SetupGet(wrapper => wrapper.EventNewsCategoryRepository)
            .Returns(_categoryRepository.Object);
        _wrapper.SetupGet(wrapper => wrapper.EventNewsCategoryLocalizationsRepository)
            .Returns(_localizationRepository.Object);
        _wrapper.SetupGet(wrapper => wrapper.LocalizationLanguagesRepository)
            .Returns(_languageRepository.Object);
    }

    [Fact]
    public async Task CreateLocalization_ShouldTrimNameAndReturnCreatedLocalization()
    {
        EventNewsCategoryLocalization? createdLocalization = null;
        _categoryRepository
            .Setup(repository => repository.ExistsAsync(
                It.IsAny<Expression<Func<EventNewsCategory, bool>>>()))
            .ReturnsAsync(true);
        _languageRepository
            .Setup(repository => repository.GetFirstOrDefaultAsync(
                It.IsAny<QueryOptions<LocalizationLanguage>>()))
            .ReturnsAsync(new LocalizationLanguage { Id = 2, Code = "en", Name = "English" });
        _localizationRepository
            .Setup(repository => repository.ExistsAsync(
                It.IsAny<Expression<Func<EventNewsCategoryLocalization, bool>>>()))
            .ReturnsAsync(false);
        _localizationRepository
            .Setup(repository => repository.CreateAsync(It.IsAny<EventNewsCategoryLocalization>()))
            .Callback<EventNewsCategoryLocalization>(localization => createdLocalization = localization)
            .ReturnsAsync((EventNewsCategoryLocalization localization) => localization);
        _wrapper.Setup(wrapper => wrapper.SaveChangesAsync()).ReturnsAsync(1);
        var handler = new CreateEventNewsCategoryLocalizationHandler(_wrapper.Object);

        var result = await handler.Handle(
            new CreateEventNewsCategoryLocalizationCommand(
                new CreateEventNewsCategoryLocalizationDto
                {
                    EntityId = 1,
                    LanguageId = 2,
                    Name = "  News  "
                }),
            CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.NotNull(createdLocalization);
        Assert.Equal("News", createdLocalization.Name);
        Assert.Equal(TranslationStatus.Relevant, createdLocalization.TranslationStatus);
        Assert.Equal("en", result.Value.Language.Code);
    }

    [Fact]
    public async Task CreateLocalization_ShouldFail_WhenEntityLanguagePairAlreadyExists()
    {
        _categoryRepository
            .Setup(repository => repository.ExistsAsync(
                It.IsAny<Expression<Func<EventNewsCategory, bool>>>()))
            .ReturnsAsync(true);
        _languageRepository
            .Setup(repository => repository.GetFirstOrDefaultAsync(
                It.IsAny<QueryOptions<LocalizationLanguage>>()))
            .ReturnsAsync(new LocalizationLanguage { Id = 2, Code = "en", Name = "English" });
        _localizationRepository
            .Setup(repository => repository.ExistsAsync(
                It.IsAny<Expression<Func<EventNewsCategoryLocalization, bool>>>()))
            .ReturnsAsync(true);
        var handler = new CreateEventNewsCategoryLocalizationHandler(_wrapper.Object);

        var result = await handler.Handle(
            new CreateEventNewsCategoryLocalizationCommand(
                new CreateEventNewsCategoryLocalizationDto
                {
                    EntityId = 1,
                    LanguageId = 2,
                    Name = "News"
                }),
            CancellationToken.None);

        Assert.True(result.IsFailed);
        Assert.Equal(EventNewsCategoryConstants.LocalizationAlreadyExists, result.Errors[0].Message);
        _localizationRepository.Verify(
            repository => repository.CreateAsync(It.IsAny<EventNewsCategoryLocalization>()),
            Times.Never);
    }

    [Fact]
    public async Task CreateLocalization_ShouldFail_WhenLocalizedNameAlreadyExists()
    {
        _categoryRepository
            .Setup(repository => repository.ExistsAsync(
                It.IsAny<Expression<Func<EventNewsCategory, bool>>>()))
            .ReturnsAsync(true);
        _languageRepository
            .Setup(repository => repository.GetFirstOrDefaultAsync(
                It.IsAny<QueryOptions<LocalizationLanguage>>()))
            .ReturnsAsync(new LocalizationLanguage { Id = 2, Code = "en", Name = "English" });
        _localizationRepository
            .SetupSequence(repository => repository.ExistsAsync(
                It.IsAny<Expression<Func<EventNewsCategoryLocalization, bool>>>()))
            .ReturnsAsync(false)
            .ReturnsAsync(true);
        var handler = new CreateEventNewsCategoryLocalizationHandler(_wrapper.Object);

        var result = await handler.Handle(
            new CreateEventNewsCategoryLocalizationCommand(
                new CreateEventNewsCategoryLocalizationDto
                {
                    EntityId = 1,
                    LanguageId = 2,
                    Name = "News"
                }),
            CancellationToken.None);

        Assert.True(result.IsFailed);
        Assert.Equal(EventNewsCategoryConstants.DuplicateLocalizedName, result.Errors[0].Message);
        _localizationRepository.Verify(
            repository => repository.CreateAsync(It.IsAny<EventNewsCategoryLocalization>()),
            Times.Never);
    }

    [Fact]
    public async Task CreateLocalization_ShouldClassifyConcurrentEntityLanguageConflict()
    {
        _categoryRepository
            .Setup(repository => repository.ExistsAsync(
                It.IsAny<Expression<Func<EventNewsCategory, bool>>>()))
            .ReturnsAsync(true);
        _languageRepository
            .Setup(repository => repository.GetFirstOrDefaultAsync(
                It.IsAny<QueryOptions<LocalizationLanguage>>()))
            .ReturnsAsync(new LocalizationLanguage { Id = 2, Code = "en", Name = "English" });
        _localizationRepository
            .SetupSequence(repository => repository.ExistsAsync(
                It.IsAny<Expression<Func<EventNewsCategoryLocalization, bool>>>()))
            .ReturnsAsync(false)
            .ReturnsAsync(false)
            .ReturnsAsync(true);
        _localizationRepository
            .Setup(repository => repository.CreateAsync(It.IsAny<EventNewsCategoryLocalization>()))
            .ReturnsAsync((EventNewsCategoryLocalization localization) => localization);
        _wrapper.Setup(wrapper => wrapper.SaveChangesAsync())
            .ThrowsAsync(SqlExceptionFactory.CreateDbUpdateException(2601, "Unique constraint violation"));
        var handler = new CreateEventNewsCategoryLocalizationHandler(_wrapper.Object);

        var result = await handler.Handle(
            new CreateEventNewsCategoryLocalizationCommand(
                new CreateEventNewsCategoryLocalizationDto
                {
                    EntityId = 1,
                    LanguageId = 2,
                    Name = "News"
                }),
            CancellationToken.None);

        Assert.True(result.IsFailed);
        Assert.Equal(EventNewsCategoryConstants.LocalizationAlreadyExists, result.Errors[0].Message);
    }

    [Fact]
    public async Task UpdateLocalization_ShouldTrimNameAndMarkTranslationRelevant()
    {
        var localization = new EventNewsCategoryLocalization
        {
            EntityId = 1,
            LanguageId = 2,
            Name = "Old",
            TranslationStatus = TranslationStatus.Outdated
        };
        _localizationRepository
            .Setup(repository => repository.GetFirstOrDefaultAsync(
                It.IsAny<QueryOptions<EventNewsCategoryLocalization>>()))
            .ReturnsAsync(localization);
        _localizationRepository
            .Setup(repository => repository.ExistsAsync(
                It.IsAny<Expression<Func<EventNewsCategoryLocalization, bool>>>()))
            .ReturnsAsync(false);
        _wrapper.Setup(wrapper => wrapper.SaveChangesAsync()).ReturnsAsync(1);
        _mapper.Setup(mapper => mapper.Map<AdminEventNewsCategoryLocalizationDto>(localization))
            .Returns(new AdminEventNewsCategoryLocalizationDto
            {
                EntityId = 1,
                Name = "Updated",
                TranslationStatus = TranslationStatus.Relevant
            });
        var handler = new UpdateEventNewsCategoryLocalizationHandler(_mapper.Object, _wrapper.Object);

        var result = await handler.Handle(
            new UpdateEventNewsCategoryLocalizationCommand(
                1,
                2,
                new UpdateEventNewsCategoryLocalizationDto { Name = "  Updated  " }),
            CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal("Updated", localization.Name);
        Assert.Equal(TranslationStatus.Relevant, localization.TranslationStatus);
        _wrapper.Verify(wrapper => wrapper.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task UpdateLocalization_ShouldFail_WhenLocalizedNameAlreadyExists()
    {
        var localization = new EventNewsCategoryLocalization
        {
            EntityId = 1,
            LanguageId = 2,
            Name = "Old"
        };
        _localizationRepository
            .Setup(repository => repository.GetFirstOrDefaultAsync(
                It.IsAny<QueryOptions<EventNewsCategoryLocalization>>()))
            .ReturnsAsync(localization);
        _localizationRepository
            .Setup(repository => repository.ExistsAsync(
                It.IsAny<Expression<Func<EventNewsCategoryLocalization, bool>>>()))
            .ReturnsAsync(true);
        var handler = new UpdateEventNewsCategoryLocalizationHandler(_mapper.Object, _wrapper.Object);

        var result = await handler.Handle(
            new UpdateEventNewsCategoryLocalizationCommand(
                1,
                2,
                new UpdateEventNewsCategoryLocalizationDto { Name = "Existing" }),
            CancellationToken.None);

        Assert.True(result.IsFailed);
        Assert.Equal(EventNewsCategoryConstants.DuplicateLocalizedName, result.Errors[0].Message);
        Assert.Equal("Old", localization.Name);
        _wrapper.Verify(wrapper => wrapper.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task DeleteLocalization_ShouldDeleteExistingLocalization()
    {
        var localization = new EventNewsCategoryLocalization
        {
            EntityId = 1,
            LanguageId = 2,
            Name = "News"
        };
        _localizationRepository
            .Setup(repository => repository.GetFirstOrDefaultAsync(
                It.IsAny<QueryOptions<EventNewsCategoryLocalization>>()))
            .ReturnsAsync(localization);
        _wrapper.Setup(wrapper => wrapper.SaveChangesAsync()).ReturnsAsync(1);
        var handler = new DeleteEventNewsCategoryLocalizationHandler(_wrapper.Object);

        var result = await handler.Handle(
            new DeleteEventNewsCategoryLocalizationCommand(1, 2),
            CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(1, result.Value.EntityId);
        Assert.Equal(2, result.Value.LanguageId);
        _localizationRepository.Verify(repository => repository.Delete(localization), Times.Once);
    }

    [Fact]
    public async Task GetLocalizations_ShouldReturnNotFound_WhenCategoryDoesNotExist()
    {
        _categoryRepository
            .Setup(repository => repository.ExistsAsync(
                It.IsAny<Expression<Func<EventNewsCategory, bool>>>()))
            .ReturnsAsync(false);
        var handler = new GetEventNewsCategoryLocalizationsByEntityIdHandler(
            _mapper.Object,
            _wrapper.Object);

        var result = await handler.Handle(
            new GetEventNewsCategoryLocalizationsByEntityIdQuery(1),
            CancellationToken.None);

        Assert.True(result.IsFailed);
        _localizationRepository.Verify(
            repository => repository.GetAllAsync(
                It.IsAny<QueryOptions<EventNewsCategoryLocalization>>()),
            Times.Never);
    }

    [Fact]
    public async Task GetLocalizations_ShouldUseReadOnlyQuery()
    {
        QueryOptions<EventNewsCategoryLocalization>? capturedOptions = null;
        _categoryRepository
            .Setup(repository => repository.ExistsAsync(
                It.IsAny<Expression<Func<EventNewsCategory, bool>>>()))
            .ReturnsAsync(true);
        _localizationRepository
            .Setup(repository => repository.GetAllAsync(
                It.IsAny<QueryOptions<EventNewsCategoryLocalization>>()))
            .Callback<QueryOptions<EventNewsCategoryLocalization>>(
                options => capturedOptions = options)
            .ReturnsAsync([]);
        _mapper.Setup(mapper => mapper.Map<List<AdminEventNewsCategoryLocalizationDto>>(
                It.IsAny<IEnumerable<EventNewsCategoryLocalization>>()))
            .Returns([]);
        var handler = new GetEventNewsCategoryLocalizationsByEntityIdHandler(
            _mapper.Object,
            _wrapper.Object);

        var result = await handler.Handle(
            new GetEventNewsCategoryLocalizationsByEntityIdQuery(1),
            CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.NotNull(capturedOptions);
        Assert.True(capturedOptions.AsNoTracking);
        Assert.NotNull(capturedOptions.Include);
        Assert.NotNull(capturedOptions.OrderByASC);
    }
}
