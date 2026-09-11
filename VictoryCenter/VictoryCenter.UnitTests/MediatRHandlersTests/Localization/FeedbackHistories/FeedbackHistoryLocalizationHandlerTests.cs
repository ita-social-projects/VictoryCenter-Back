using System.Linq.Expressions;
using AutoMapper;
using Moq;
using VictoryCenter.BLL.Commands.Admin.Localization.FeedbackHistories.Create;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.Localization.FeedbackHistories;
using VictoryCenter.BLL.Queries.Admin.Localization.FeedbackHistories.GetByEntityId;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Entities.Localization;
using VictoryCenter.DAL.Enums;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Interfaces.FeedbackHistories;
using VictoryCenter.DAL.Repositories.Interfaces.Localization.FeedbackHistories;
using VictoryCenter.DAL.Repositories.Interfaces.Localization.Languages;
using VictoryCenter.DAL.Repositories.Options;
using VictoryCenter.UnitTests.Utils;

namespace VictoryCenter.UnitTests.MediatRHandlersTests.Localization.FeedbackHistories;

public class FeedbackHistoryLocalizationHandlerTests
{
    private readonly Mock<IMapper> _mapper = new();
    private readonly Mock<IRepositoryWrapper> _wrapper = new();
    private readonly Mock<IFeedbackHistoriesRepository> _feedbackHistoriesRepository = new();
    private readonly Mock<IFeedbackHistoryLocalizationsRepository> _localizationRepository = new();
    private readonly Mock<ILocalizationLanguagesRepository> _languageRepository = new();

    public FeedbackHistoryLocalizationHandlerTests()
    {
        _wrapper.SetupGet(wrapper => wrapper.FeedbackHistoriesRepository)
            .Returns(_feedbackHistoriesRepository.Object);
        _wrapper.SetupGet(wrapper => wrapper.FeedbackHistoryLocalizationsRepository)
            .Returns(_localizationRepository.Object);
        _wrapper.SetupGet(wrapper => wrapper.LocalizationLanguagesRepository)
            .Returns(_languageRepository.Object);
    }

    [Fact]
    public async Task CreateLocalization_ShouldTrimFieldsAndReturnCreatedLocalization()
    {
        FeedbackHistoryLocalization? createdLocalization = null;
        SetupExistingFeedbackHistory();
        SetupExistingLanguage();
        SetupLocalizationDoesNotExist();
        _localizationRepository
            .Setup(repository => repository.CreateAsync(It.IsAny<FeedbackHistoryLocalization>()))
            .Callback<FeedbackHistoryLocalization>(localization => createdLocalization = localization)
            .ReturnsAsync((FeedbackHistoryLocalization localization) => localization);
        _wrapper.Setup(wrapper => wrapper.SaveChangesAsync()).ReturnsAsync(1);
        var handler = new CreateFeedbackHistoryLocalizationHandler(_wrapper.Object);

        var result = await handler.Handle(
            new CreateFeedbackHistoryLocalizationCommand(new CreateFeedbackHistoryLocalizationDto
            {
                EntityId = 1,
                LanguageId = 2,
                Title = "  Recovery journey  ",
                Story = "  A detailed English translation of the story.  "
            }),
            CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.NotNull(createdLocalization);
        Assert.Equal("Recovery journey", createdLocalization.Title);
        Assert.Equal("A detailed English translation of the story.", createdLocalization.Story);
        Assert.Equal(TranslationStatus.Relevant, createdLocalization.TranslationStatus);
        Assert.Equal("en", result.Value.Language.Code);
        Assert.Equal("Recovery journey", result.Value.Title);
    }

    [Fact]
    public async Task CreateLocalization_ShouldReturnNotFound_WhenFeedbackHistoryDoesNotExist()
    {
        _feedbackHistoriesRepository
            .Setup(repository => repository.ExistsAsync(It.IsAny<Expression<Func<FeedbackHistory, bool>>>()))
            .ReturnsAsync(false);
        var handler = new CreateFeedbackHistoryLocalizationHandler(_wrapper.Object);

        var result = await handler.Handle(
            new CreateFeedbackHistoryLocalizationCommand(new CreateFeedbackHistoryLocalizationDto
            {
                EntityId = 999,
                LanguageId = 2,
                Title = "Recovery journey",
                Story = "A detailed English translation of the story."
            }),
            CancellationToken.None);

        Assert.True(result.IsFailed);
        Assert.Equal(
            ErrorMessagesConstants.NotFound(999L, typeof(FeedbackHistory)),
            result.Errors[0].Message);
    }

    [Fact]
    public async Task CreateLocalization_ShouldReturnNotFound_WhenLanguageDoesNotExist()
    {
        SetupExistingFeedbackHistory();
        _languageRepository
            .Setup(repository => repository.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<LocalizationLanguage>>()))
            .ReturnsAsync((LocalizationLanguage?)null);
        var handler = new CreateFeedbackHistoryLocalizationHandler(_wrapper.Object);

        var result = await handler.Handle(
            new CreateFeedbackHistoryLocalizationCommand(new CreateFeedbackHistoryLocalizationDto
            {
                EntityId = 1,
                LanguageId = 999,
                Title = "Recovery journey",
                Story = "A detailed English translation of the story."
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
        SetupExistingFeedbackHistory();
        SetupExistingLanguage();
        _localizationRepository
            .Setup(repository => repository.ExistsAsync(
                It.IsAny<Expression<Func<FeedbackHistoryLocalization, bool>>>()))
            .ReturnsAsync(true);
        var handler = new CreateFeedbackHistoryLocalizationHandler(_wrapper.Object);

        var result = await handler.Handle(
            new CreateFeedbackHistoryLocalizationCommand(new CreateFeedbackHistoryLocalizationDto
            {
                EntityId = 1,
                LanguageId = 2,
                Title = "Recovery journey",
                Story = "A detailed English translation of the story."
            }),
            CancellationToken.None);

        Assert.True(result.IsFailed);
        Assert.Equal(FeedbackHistoryConstants.LocalizationAlreadyExists, result.Errors[0].Message);
        _localizationRepository.Verify(
            repository => repository.CreateAsync(It.IsAny<FeedbackHistoryLocalization>()),
            Times.Never);
    }

    [Fact]
    public async Task CreateLocalization_ShouldFail_WhenSaveChangesReturnsZero()
    {
        SetupExistingFeedbackHistory();
        SetupExistingLanguage();
        SetupLocalizationDoesNotExist();
        _localizationRepository
            .Setup(repository => repository.CreateAsync(It.IsAny<FeedbackHistoryLocalization>()))
            .ReturnsAsync((FeedbackHistoryLocalization localization) => localization);
        _wrapper.Setup(wrapper => wrapper.SaveChangesAsync()).ReturnsAsync(0);
        var handler = new CreateFeedbackHistoryLocalizationHandler(_wrapper.Object);

        var result = await handler.Handle(
            new CreateFeedbackHistoryLocalizationCommand(new CreateFeedbackHistoryLocalizationDto
            {
                EntityId = 1,
                LanguageId = 2,
                Title = "Recovery journey",
                Story = "A detailed English translation of the story."
            }),
            CancellationToken.None);

        Assert.True(result.IsFailed);
        Assert.Equal(
            ErrorMessagesConstants.FailedToCreateEntity(typeof(FeedbackHistoryLocalization)),
            result.Errors[0].Message);
    }

    [Fact]
    public async Task CreateLocalization_ShouldClassifyConcurrentUniqueConstraintViolation()
    {
        SetupExistingFeedbackHistory();
        SetupExistingLanguage();
        SetupLocalizationDoesNotExist();
        _localizationRepository
            .Setup(repository => repository.CreateAsync(It.IsAny<FeedbackHistoryLocalization>()))
            .ReturnsAsync((FeedbackHistoryLocalization localization) => localization);
        _wrapper.Setup(wrapper => wrapper.SaveChangesAsync())
            .ThrowsAsync(SqlExceptionFactory.CreateDbUpdateException(2627, "Unique constraint violation"));
        var handler = new CreateFeedbackHistoryLocalizationHandler(_wrapper.Object);

        var result = await handler.Handle(
            new CreateFeedbackHistoryLocalizationCommand(new CreateFeedbackHistoryLocalizationDto
            {
                EntityId = 1,
                LanguageId = 2,
                Title = "Recovery journey",
                Story = "A detailed English translation of the story."
            }),
            CancellationToken.None);

        Assert.True(result.IsFailed);
        Assert.Equal(FeedbackHistoryConstants.LocalizationAlreadyExists, result.Errors[0].Message);
    }

    [Fact]
    public async Task GetLocalizations_ShouldReturnNotFound_WhenFeedbackHistoryDoesNotExist()
    {
        _feedbackHistoriesRepository
            .Setup(repository => repository.ExistsAsync(It.IsAny<Expression<Func<FeedbackHistory, bool>>>()))
            .ReturnsAsync(false);
        var handler = new GetFeedbackHistoryLocalizationsByEntityIdHandler(_mapper.Object, _wrapper.Object);

        var result = await handler.Handle(
            new GetFeedbackHistoryLocalizationsByEntityIdQuery(1),
            CancellationToken.None);

        Assert.True(result.IsFailed);
        _localizationRepository.Verify(
            repository => repository.GetAllAsync(It.IsAny<QueryOptions<FeedbackHistoryLocalization>>()),
            Times.Never);
    }

    [Fact]
    public async Task GetLocalizations_ShouldUseReadOnlyQueryAndReturnMappedLocalizations()
    {
        QueryOptions<FeedbackHistoryLocalization>? capturedOptions = null;
        SetupExistingFeedbackHistory();
        _localizationRepository
            .Setup(repository => repository.GetAllAsync(It.IsAny<QueryOptions<FeedbackHistoryLocalization>>()))
            .Callback<QueryOptions<FeedbackHistoryLocalization>>(options => capturedOptions = options)
            .ReturnsAsync([]);
        _mapper
            .Setup(mapper => mapper.Map<List<FeedbackHistoryLocalizationDto>>(
                It.IsAny<IEnumerable<FeedbackHistoryLocalization>>()))
            .Returns([]);
        var handler = new GetFeedbackHistoryLocalizationsByEntityIdHandler(_mapper.Object, _wrapper.Object);

        var result = await handler.Handle(
            new GetFeedbackHistoryLocalizationsByEntityIdQuery(1),
            CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.NotNull(capturedOptions);
        Assert.True(capturedOptions.AsNoTracking);
        Assert.NotNull(capturedOptions.Include);
        Assert.NotNull(capturedOptions.OrderByASC);
    }

    private void SetupExistingFeedbackHistory()
    {
        _feedbackHistoriesRepository
            .Setup(repository => repository.ExistsAsync(It.IsAny<Expression<Func<FeedbackHistory, bool>>>()))
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
                It.IsAny<Expression<Func<FeedbackHistoryLocalization, bool>>>()))
            .ReturnsAsync(false);
    }
}
