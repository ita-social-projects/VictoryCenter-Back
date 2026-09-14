using System.Linq.Expressions;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Moq;
using VictoryCenter.BLL.Commands.Admin.Localization.FeedbackReviews.Create;
using VictoryCenter.BLL.Commands.Admin.Localization.FeedbackReviews.Delete;
using VictoryCenter.BLL.Commands.Admin.Localization.FeedbackReviews.Update;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.Localization.FeedbackReviews;
using VictoryCenter.BLL.Queries.Admin.Localization.FeedbackReviews.GetByEntityId;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Entities.Localization;
using VictoryCenter.DAL.Enums;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Interfaces.FeedbackReviews;
using VictoryCenter.DAL.Repositories.Interfaces.Localization.FeedbackReviews;
using VictoryCenter.DAL.Repositories.Interfaces.Localization.Languages;
using VictoryCenter.DAL.Repositories.Options;
using VictoryCenter.UnitTests.Utils;

namespace VictoryCenter.UnitTests.MediatRHandlersTests.Localization.FeedbackReviews;

public class FeedbackReviewLocalizationHandlerTests
{
    private readonly Mock<IMapper> _mapper = new();
    private readonly Mock<IRepositoryWrapper> _wrapper = new();
    private readonly Mock<IFeedbackReviewsRepository> _feedbackReviewsRepository = new();
    private readonly Mock<IFeedbackReviewLocalizationsRepository> _localizationRepository = new();
    private readonly Mock<ILocalizationLanguagesRepository> _languageRepository = new();

    public FeedbackReviewLocalizationHandlerTests()
    {
        _wrapper.SetupGet(wrapper => wrapper.FeedbackReviewsRepository)
            .Returns(_feedbackReviewsRepository.Object);
        _wrapper.SetupGet(wrapper => wrapper.FeedbackReviewLocalizationsRepository)
            .Returns(_localizationRepository.Object);
        _wrapper.SetupGet(wrapper => wrapper.LocalizationLanguagesRepository)
            .Returns(_languageRepository.Object);
    }

    [Fact]
    public async Task CreateLocalization_ShouldTrimFieldsAndReturnCreatedLocalization()
    {
        FeedbackReviewLocalization? createdLocalization = null;
        SetupExistingFeedbackReview();
        SetupExistingLanguage();
        SetupLocalizationDoesNotExist();
        _localizationRepository
            .Setup(repository => repository.CreateAsync(It.IsAny<FeedbackReviewLocalization>()))
            .Callback<FeedbackReviewLocalization>(localization => createdLocalization = localization)
            .ReturnsAsync((FeedbackReviewLocalization localization) => localization);
        _wrapper.Setup(wrapper => wrapper.SaveChangesAsync()).ReturnsAsync(1);
        var handler = new CreateFeedbackReviewLocalizationHandler(_wrapper.Object);

        var result = await handler.Handle(
            new CreateFeedbackReviewLocalizationCommand(new CreateFeedbackReviewLocalizationDto
            {
                EntityId = 1,
                LanguageId = 2,
                AuthorName = "  John Doe  ",
                Text = "  An English translation of the participant review.  "
            }),
            CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.NotNull(createdLocalization);
        Assert.Equal("John Doe", createdLocalization.AuthorName);
        Assert.Equal("An English translation of the participant review.", createdLocalization.Text);
        Assert.Equal(TranslationStatus.Relevant, createdLocalization.TranslationStatus);
        Assert.Equal("en", result.Value.Language.Code);
        Assert.Equal("John Doe", result.Value.AuthorName);
    }

    [Fact]
    public async Task CreateLocalization_ShouldReturnNotFound_WhenFeedbackReviewDoesNotExist()
    {
        _feedbackReviewsRepository
            .Setup(repository => repository.ExistsAsync(It.IsAny<Expression<Func<FeedbackReview, bool>>>()))
            .ReturnsAsync(false);
        var handler = new CreateFeedbackReviewLocalizationHandler(_wrapper.Object);

        var result = await handler.Handle(
            new CreateFeedbackReviewLocalizationCommand(new CreateFeedbackReviewLocalizationDto
            {
                EntityId = 999,
                LanguageId = 2,
                AuthorName = "John Doe",
                Text = "An English translation of the participant review."
            }),
            CancellationToken.None);

        Assert.True(result.IsFailed);
        Assert.Equal(
            ErrorMessagesConstants.NotFound(999L, typeof(FeedbackReview)),
            result.Errors[0].Message);
    }

    [Fact]
    public async Task CreateLocalization_ShouldReturnNotFound_WhenLanguageDoesNotExist()
    {
        SetupExistingFeedbackReview();
        _languageRepository
            .Setup(repository => repository.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<LocalizationLanguage>>()))
            .ReturnsAsync((LocalizationLanguage?)null);
        var handler = new CreateFeedbackReviewLocalizationHandler(_wrapper.Object);

        var result = await handler.Handle(
            new CreateFeedbackReviewLocalizationCommand(new CreateFeedbackReviewLocalizationDto
            {
                EntityId = 1,
                LanguageId = 999,
                AuthorName = "John Doe",
                Text = "An English translation of the participant review."
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
        SetupExistingFeedbackReview();
        SetupExistingLanguage();
        _localizationRepository
            .Setup(repository => repository.ExistsAsync(
                It.IsAny<Expression<Func<FeedbackReviewLocalization, bool>>>()))
            .ReturnsAsync(true);
        var handler = new CreateFeedbackReviewLocalizationHandler(_wrapper.Object);

        var result = await handler.Handle(
            new CreateFeedbackReviewLocalizationCommand(new CreateFeedbackReviewLocalizationDto
            {
                EntityId = 1,
                LanguageId = 2,
                AuthorName = "John Doe",
                Text = "An English translation of the participant review."
            }),
            CancellationToken.None);

        Assert.True(result.IsFailed);
        Assert.Equal(FeedbackReviewConstants.LocalizationAlreadyExists, result.Errors[0].Message);
        _localizationRepository.Verify(
            repository => repository.CreateAsync(It.IsAny<FeedbackReviewLocalization>()),
            Times.Never);
    }

    [Fact]
    public async Task CreateLocalization_ShouldFail_WhenSaveChangesReturnsZero()
    {
        SetupExistingFeedbackReview();
        SetupExistingLanguage();
        SetupLocalizationDoesNotExist();
        _localizationRepository
            .Setup(repository => repository.CreateAsync(It.IsAny<FeedbackReviewLocalization>()))
            .ReturnsAsync((FeedbackReviewLocalization localization) => localization);
        _wrapper.Setup(wrapper => wrapper.SaveChangesAsync()).ReturnsAsync(0);
        var handler = new CreateFeedbackReviewLocalizationHandler(_wrapper.Object);

        var result = await handler.Handle(
            new CreateFeedbackReviewLocalizationCommand(new CreateFeedbackReviewLocalizationDto
            {
                EntityId = 1,
                LanguageId = 2,
                AuthorName = "John Doe",
                Text = "An English translation of the participant review."
            }),
            CancellationToken.None);

        Assert.True(result.IsFailed);
        Assert.Equal(
            ErrorMessagesConstants.FailedToCreateEntity(typeof(FeedbackReviewLocalization)),
            result.Errors[0].Message);
    }

    [Fact]
    public async Task CreateLocalization_ShouldClassifyConcurrentUniqueConstraintViolation()
    {
        SetupExistingFeedbackReview();
        SetupExistingLanguage();
        SetupLocalizationDoesNotExist();
        _localizationRepository
            .Setup(repository => repository.CreateAsync(It.IsAny<FeedbackReviewLocalization>()))
            .ReturnsAsync((FeedbackReviewLocalization localization) => localization);
        _wrapper.Setup(wrapper => wrapper.SaveChangesAsync())
            .ThrowsAsync(SqlExceptionFactory.CreateDbUpdateException(2601, "Unique constraint violation"));
        var handler = new CreateFeedbackReviewLocalizationHandler(_wrapper.Object);

        var result = await handler.Handle(
            new CreateFeedbackReviewLocalizationCommand(new CreateFeedbackReviewLocalizationDto
            {
                EntityId = 1,
                LanguageId = 2,
                AuthorName = "John Doe",
                Text = "An English translation of the participant review."
            }),
            CancellationToken.None);

        Assert.True(result.IsFailed);
        Assert.Equal(FeedbackReviewConstants.LocalizationAlreadyExists, result.Errors[0].Message);
    }

    [Fact]
    public async Task GetLocalizations_ShouldReturnNotFound_WhenFeedbackReviewDoesNotExist()
    {
        _feedbackReviewsRepository
            .Setup(repository => repository.ExistsAsync(It.IsAny<Expression<Func<FeedbackReview, bool>>>()))
            .ReturnsAsync(false);
        var handler = new GetFeedbackReviewLocalizationsByEntityIdHandler(_mapper.Object, _wrapper.Object);

        var result = await handler.Handle(
            new GetFeedbackReviewLocalizationsByEntityIdQuery(1),
            CancellationToken.None);

        Assert.True(result.IsFailed);
        _localizationRepository.Verify(
            repository => repository.GetAllAsync(It.IsAny<QueryOptions<FeedbackReviewLocalization>>()),
            Times.Never);
    }

    [Fact]
    public async Task GetLocalizations_ShouldUseReadOnlyQueryAndReturnMappedLocalizations()
    {
        QueryOptions<FeedbackReviewLocalization>? capturedOptions = null;
        SetupExistingFeedbackReview();
        _localizationRepository
            .Setup(repository => repository.GetAllAsync(It.IsAny<QueryOptions<FeedbackReviewLocalization>>()))
            .Callback<QueryOptions<FeedbackReviewLocalization>>(options => capturedOptions = options)
            .ReturnsAsync([]);
        _mapper
            .Setup(mapper => mapper.Map<List<FeedbackReviewLocalizationDto>>(
                It.IsAny<IEnumerable<FeedbackReviewLocalization>>()))
            .Returns([]);
        var handler = new GetFeedbackReviewLocalizationsByEntityIdHandler(_mapper.Object, _wrapper.Object);

        var result = await handler.Handle(
            new GetFeedbackReviewLocalizationsByEntityIdQuery(1),
            CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.NotNull(capturedOptions);
        Assert.True(capturedOptions.AsNoTracking);
        Assert.NotNull(capturedOptions.Include);
        Assert.NotNull(capturedOptions.OrderByASC);
    }

    [Fact]
    public async Task UpdateLocalization_ShouldTrimFieldsAndMarkTranslationRelevant()
    {
        var localization = new FeedbackReviewLocalization
        {
            EntityId = 1,
            LanguageId = 2,
            AuthorName = "Old Name",
            Text = "Old text",
            TranslationStatus = TranslationStatus.Outdated
        };
        _localizationRepository
            .Setup(repository => repository.GetFirstOrDefaultAsync(
                It.IsAny<QueryOptions<FeedbackReviewLocalization>>()))
            .ReturnsAsync(localization);
        _wrapper.Setup(wrapper => wrapper.SaveChangesAsync()).ReturnsAsync(1);
        _mapper
            .Setup(mapper => mapper.Map<FeedbackReviewLocalizationDto>(localization))
            .Returns(new FeedbackReviewLocalizationDto
            {
                EntityId = 1,
                AuthorName = "New Name",
                Text = "New text",
                TranslationStatus = TranslationStatus.Relevant
            });
        var handler = new UpdateFeedbackReviewLocalizationHandler(_mapper.Object, _wrapper.Object);

        var result = await handler.Handle(
            new UpdateFeedbackReviewLocalizationCommand(
                1,
                2,
                new UpdateFeedbackReviewLocalizationDto { AuthorName = "  New Name  ", Text = "  New text  " }),
            CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal("New Name", localization.AuthorName);
        Assert.Equal("New text", localization.Text);
        Assert.Equal(TranslationStatus.Relevant, localization.TranslationStatus);
        _wrapper.Verify(wrapper => wrapper.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task UpdateLocalization_ShouldShortCircuit_WhenNothingChanged()
    {
        var localization = new FeedbackReviewLocalization
        {
            EntityId = 1,
            LanguageId = 2,
            AuthorName = "Same Name",
            Text = "Same text",
            TranslationStatus = TranslationStatus.Relevant
        };
        _localizationRepository
            .Setup(repository => repository.GetFirstOrDefaultAsync(
                It.IsAny<QueryOptions<FeedbackReviewLocalization>>()))
            .ReturnsAsync(localization);
        var handler = new UpdateFeedbackReviewLocalizationHandler(_mapper.Object, _wrapper.Object);

        var result = await handler.Handle(
            new UpdateFeedbackReviewLocalizationCommand(
                1,
                2,
                new UpdateFeedbackReviewLocalizationDto { AuthorName = "Same Name", Text = "Same text" }),
            CancellationToken.None);

        Assert.True(result.IsSuccess);
        _wrapper.Verify(wrapper => wrapper.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task UpdateLocalization_ShouldReturnNotFound_WhenLocalizationDoesNotExist()
    {
        _localizationRepository
            .Setup(repository => repository.GetFirstOrDefaultAsync(
                It.IsAny<QueryOptions<FeedbackReviewLocalization>>()))
            .ReturnsAsync((FeedbackReviewLocalization?)null);
        var handler = new UpdateFeedbackReviewLocalizationHandler(_mapper.Object, _wrapper.Object);

        var result = await handler.Handle(
            new UpdateFeedbackReviewLocalizationCommand(
                1,
                2,
                new UpdateFeedbackReviewLocalizationDto { AuthorName = "Name", Text = "Text" }),
            CancellationToken.None);

        Assert.True(result.IsFailed);
        Assert.Equal(
            ErrorMessagesConstants.NotFound((1L, 2L), typeof(FeedbackReviewLocalization)),
            result.Errors[0].Message);
    }

    [Fact]
    public async Task UpdateLocalization_ShouldReturnNotFound_WhenLocalizationIsDeletedConcurrently()
    {
        var localization = new FeedbackReviewLocalization
        {
            EntityId = 1,
            LanguageId = 2,
            AuthorName = "Old Name",
            Text = "Old text"
        };
        _localizationRepository
            .Setup(repository => repository.GetFirstOrDefaultAsync(
                It.IsAny<QueryOptions<FeedbackReviewLocalization>>()))
            .ReturnsAsync(localization);
        _wrapper.Setup(wrapper => wrapper.SaveChangesAsync())
            .ThrowsAsync(new DbUpdateConcurrencyException());
        var handler = new UpdateFeedbackReviewLocalizationHandler(_mapper.Object, _wrapper.Object);

        var result = await handler.Handle(
            new UpdateFeedbackReviewLocalizationCommand(
                1,
                2,
                new UpdateFeedbackReviewLocalizationDto { AuthorName = "New Name", Text = "New text" }),
            CancellationToken.None);

        Assert.True(result.IsFailed);
        Assert.Equal(
            ErrorMessagesConstants.NotFound((1L, 2L), typeof(FeedbackReviewLocalization)),
            result.Errors[0].Message);
    }

    [Fact]
    public async Task DeleteLocalization_ShouldDeleteExistingLocalization()
    {
        var localization = new FeedbackReviewLocalization
        {
            EntityId = 1,
            LanguageId = 2,
            AuthorName = "Name",
            Text = "Text"
        };
        _localizationRepository
            .Setup(repository => repository.GetFirstOrDefaultAsync(
                It.IsAny<QueryOptions<FeedbackReviewLocalization>>()))
            .ReturnsAsync(localization);
        _wrapper.Setup(wrapper => wrapper.SaveChangesAsync()).ReturnsAsync(1);
        var handler = new DeleteFeedbackReviewLocalizationHandler(_wrapper.Object);

        var result = await handler.Handle(new DeleteFeedbackReviewLocalizationCommand(1, 2), CancellationToken.None);

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
                It.IsAny<QueryOptions<FeedbackReviewLocalization>>()))
            .ReturnsAsync((FeedbackReviewLocalization?)null);
        var handler = new DeleteFeedbackReviewLocalizationHandler(_wrapper.Object);

        var result = await handler.Handle(new DeleteFeedbackReviewLocalizationCommand(1, 2), CancellationToken.None);

        Assert.True(result.IsFailed);
        Assert.Equal(
            ErrorMessagesConstants.NotFound((1L, 2L), typeof(FeedbackReviewLocalization)),
            result.Errors[0].Message);
    }

    [Fact]
    public async Task DeleteLocalization_ShouldReturnNotFound_WhenLocalizationIsDeletedConcurrently()
    {
        var localization = new FeedbackReviewLocalization
        {
            EntityId = 1,
            LanguageId = 2,
            AuthorName = "Name",
            Text = "Text"
        };
        _localizationRepository
            .Setup(repository => repository.GetFirstOrDefaultAsync(
                It.IsAny<QueryOptions<FeedbackReviewLocalization>>()))
            .ReturnsAsync(localization);
        _wrapper.Setup(wrapper => wrapper.SaveChangesAsync())
            .ThrowsAsync(new DbUpdateConcurrencyException());
        var handler = new DeleteFeedbackReviewLocalizationHandler(_wrapper.Object);

        var result = await handler.Handle(new DeleteFeedbackReviewLocalizationCommand(1, 2), CancellationToken.None);

        Assert.True(result.IsFailed);
        Assert.Equal(
            ErrorMessagesConstants.NotFound((1L, 2L), typeof(FeedbackReviewLocalization)),
            result.Errors[0].Message);
    }

    private void SetupExistingFeedbackReview()
    {
        _feedbackReviewsRepository
            .Setup(repository => repository.ExistsAsync(It.IsAny<Expression<Func<FeedbackReview, bool>>>()))
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
                It.IsAny<Expression<Func<FeedbackReviewLocalization, bool>>>()))
            .ReturnsAsync(false);
    }
}
