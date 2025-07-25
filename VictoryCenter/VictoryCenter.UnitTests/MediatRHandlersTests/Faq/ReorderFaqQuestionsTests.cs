using System.Transactions;
using FluentValidation;
using Moq;
using VictoryCenter.BLL.Commands.Admin.FaqQuestions.Reorder;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.FaqQuestions;
using VictoryCenter.BLL.Validators.FaqQuestions;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.UnitTests.MediatRHandlersTests.Faq;

public class ReorderFaqQuestionsTests
{
    private readonly List<FaqPlacement> _mockPlacements = [..Enumerable.Range(1, 5).Select(i =>
            new FaqPlacement { PageId = 1, QuestionId = i, Priority = i })];
    private readonly Mock<IRepositoryWrapper> _mockRepoWrapper;
    private readonly IValidator<ReorderFaqQuestionsCommand> _validator;

    public ReorderFaqQuestionsTests()
    {
        _mockRepoWrapper = new Mock<IRepositoryWrapper>();
        _validator = new ReorderFaqQuestionsValidator();
    }

    [Theory]
    [InlineData(2L, 1L)]
    [InlineData(3L, 2L, 1L)]
    [InlineData(1L, 2L, 4L, 3L)]
    [InlineData(4L, 1L, 2L, 3L, 5L)]
    public async Task Handle_ShouldReturnOk_WhenDtoIsValid(params long[] pageIds)
    {
        // Arrange
        var command = new ReorderFaqQuestionsCommand(new() { PageId = 1, OrderedIds = [.. pageIds] });
        var mockPlacements = FilterList(command);
        SetupRepositoryWrapper(mockPlacements, 1);
        var handler = new ReorderFaqQuestionsHandler(_validator, _mockRepoWrapper.Object);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Multiple(
            () => Assert.True(result.IsSuccess),
            () =>
        {
            long[] affected = [..mockPlacements
                .Where(p => pageIds.Contains(p.QuestionId))
                .OrderBy(p => p.Priority).Select(p => p.QuestionId)];

            Assert.Equal(pageIds, affected);

            var unaffected = mockPlacements
                .Where(p => !pageIds.Contains(p.QuestionId))
                .OrderBy(p => p.QuestionId)
                .ToArray();

            for (var i = 0; i < unaffected.Length; i++)
            {
                Assert.Equal(unaffected[i].QuestionId, unaffected[i].Priority);
            }
        });
    }

    [Fact]
    public async Task Handle_ShouldReturnFail_WhenDtoIsInvalid()
    {
        // Arrange
        var command = new ReorderFaqQuestionsCommand(new() { PageId = -10, OrderedIds = [2, 1] });
        var mockPlacements = FilterList(command);
        SetupRepositoryWrapper(mockPlacements, 0);
        var handler = new ReorderFaqQuestionsHandler(_validator, _mockRepoWrapper.Object);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Multiple(
            () => Assert.NotNull(result),
            () => Assert.True(result.IsFailed),
            () => Assert.Contains(
                ErrorMessagesConstants.PropertyMustBePositive(nameof(ReorderFaqQuestionsDto.PageId)),
                result.Errors[0].Message));
    }

    [Fact]
    public async Task Handle_ShouldReturnFail_WhenPageNotFoundOrContainsNoFaqQuestions()
    {
        // Arrange
        var command = new ReorderFaqQuestionsCommand(new() { PageId = 1000, OrderedIds = [2, 1] });
        var mockPlacements = FilterList(command);
        SetupRepositoryWrapper(mockPlacements, 0);
        var handler = new ReorderFaqQuestionsHandler(_validator, _mockRepoWrapper.Object);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Multiple(
            () => Assert.NotNull(result),
            () => Assert.True(result.IsFailed),
            () => Assert.Equal(FaqConstants.PageNotFoundOrContainsNoFaqQuestions, result.Errors[0].Message));
    }

    [Fact]
    public async Task Handle_ShouldReturnFail_WhenOrderedIdsContainsInvalidId()
    {
        // Arrange
        var command = new ReorderFaqQuestionsCommand(new() { PageId = 1, OrderedIds = [1000, 1] });
        var mockPlacements = FilterList(command);
        SetupRepositoryWrapper(mockPlacements, 0);
        var handler = new ReorderFaqQuestionsHandler(_validator, _mockRepoWrapper.Object);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Multiple(
            () => Assert.NotNull(result),
            () => Assert.True(result.IsFailed),
            () => Assert.Equal(
                ErrorMessagesConstants.ReorderingContainsInvalidIds(typeof(FaqQuestion), [1000]),
                result.Errors[0].Message));
    }

    [Theory]
    [InlineData(4L, 2L)]
    [InlineData(4L, 2L, 1L)]
    [InlineData(5L, 4L, 2L, 1L)]
    public async Task Handle_ShouldReturnFail_WhenOrderedIdsAreNonConsecutive(params long[] pageIds)
    {
        // Arrange
        var command = new ReorderFaqQuestionsCommand(new() { PageId = 1, OrderedIds = [.. pageIds] });
        var mockPlacements = FilterList(command);
        SetupRepositoryWrapper(mockPlacements, 0);
        var handler = new ReorderFaqQuestionsHandler(_validator, _mockRepoWrapper.Object);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Multiple(
            () => Assert.NotNull(result),
            () => Assert.True(result.IsFailed),
            () => Assert.Equal(FaqConstants.IdsAreNonConsecutive, result.Errors[0].Message));
    }

    private List<FaqPlacement> FilterList(ReorderFaqQuestionsCommand command)
    {
        return [.. _mockPlacements
            .Where(e => e.PageId == command.ReorderFaqQuestionsDto.PageId
                && command.ReorderFaqQuestionsDto.OrderedIds.Contains(e.QuestionId))
            .OrderBy(e => e.Priority)];
    }

    private void SetupRepositoryWrapper(List<FaqPlacement> placements, int saveResult = 1)
    {
        _mockRepoWrapper.Setup(
            repositoryWrapper => repositoryWrapper.FaqPlacementsRepository.GetAllAsync(
                It.IsAny<QueryOptions<FaqPlacement>>())).ReturnsAsync(placements);

        _mockRepoWrapper.Setup(x => x.SaveChangesAsync())
            .ReturnsAsync(saveResult);

        _mockRepoWrapper.Setup(x => x.BeginTransaction())
            .Returns(new TransactionScope(TransactionScopeAsyncFlowOption.Enabled));
    }
}

/*
public class UpdateFaqQuestionTests
{
    private readonly FaqQuestion _existingFaqQuestion = new()
    {
        Id = 1,
        QuestionText = new('Q', 15),
        AnswerText = new('A', 60),
        Status = Status.Draft,
        Placements = [
                    new FaqPlacement { PageId = 1, QuestionId = 1, Priority = 1 },
                    new FaqPlacement { PageId = 2, QuestionId = 1, Priority = 2 },
                    ],
        CreatedAt = DateTime.UtcNow.AddMinutes(-20)
    };

    private readonly FaqQuestion _updateFaqQuestion = new()
    {
        Id = 1,
        QuestionText = new('G', 20),
        AnswerText = new('R', 80),
        Status = Status.Published,
        Placements = [
                    new FaqPlacement { PageId = 1, QuestionId = 1, Priority = 2 },
                    new FaqPlacement { PageId = 3, QuestionId = 1, Priority = 2 },
                    ],
        CreatedAt = DateTime.UtcNow.AddMinutes(-20)
    };

    private readonly FaqQuestionDto _faqQuestionDto = new()
    {
        Id = 1,
        QuestionText = new('G', 20),
        AnswerText = new('R', 80),
        Status = Status.Published,
        PageIds = [1, 3],
    };

    private readonly UpdateFaqQuestionDto _updateFaqQuestionDto = new()
    {
        QuestionText = new('G', 20),
        AnswerText = new('R', 80),
        Status = Status.Published,
        PageIds = [1, 3],
    };

    private readonly List<FaqPlacement> _faqPlacements = [
        new FaqPlacement { PageId = 1, QuestionId = 1, Priority = 1 },
        new FaqPlacement { PageId = 2, QuestionId = 1, Priority = 1 },
    ];

    private readonly List<VisitorPage> _visitorPages = [
        new VisitorPage { Id = 1, Title = "Page 1" },
        new VisitorPage { Id = 2, Title = "Page 2" },
        new VisitorPage { Id = 3, Title = "Page 3" },
    ];

    [Theory]
    [InlineData(1L)]
    [InlineData(3L)]
    [InlineData(2L, 3L)]
    [InlineData(1L, 2L, 3L)]
    public async Task Handle_ValidRequestWithDifferentPageIds_ShouldUpdateEntity(params long[] pageIds)
    {
        var existingFaqQuestion = _existingFaqQuestion;

        var updateFaqQuestion = _updateFaqQuestion;
        updateFaqQuestion.Placements =
            pageIds.Select(id => new FaqPlacement { PageId = id, QuestionId = updateFaqQuestion.Id, Priority = 1 }).ToList();

        var validUpdateFaqQuestionDto = _updateFaqQuestionDto with
        { PageIds = pageIds.ToList() };

        var validResultFaqQuestionDto = _faqQuestionDto with
        { PageIds = pageIds.ToList() };

        SetupRepositoryWrapper(existingFaqQuestion, updateFaqQuestion);
        SetupMapper(updateFaqQuestion, validResultFaqQuestionDto);
        var handler = new UpdateFaqQuestionHandler(_mockMapper.Object, _mockRepoWrapper.Object, _validator);

        Result<FaqQuestionDto> result = await handler.Handle(
            new UpdateFaqQuestionCommand(validUpdateFaqQuestionDto, _existingFaqQuestion.Id), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(validResultFaqQuestionDto, result.Value);
    }

    [Theory]
    [InlineData(1L, 2L, 3L, 4L)]
    public async Task Handle_InvalidRequestWithInvalidPageIds_ShouldReturnNotFound(params long[] pageIds)
    {
        var existingFaqQuestion = _existingFaqQuestion;

        var updateFaqQuestion = _updateFaqQuestion;
        updateFaqQuestion.Placements =
            pageIds.Select(id => new FaqPlacement { PageId = id, QuestionId = updateFaqQuestion.Id, Priority = 1 }).ToList();

        var validUpdateFaqQuestionDto = _updateFaqQuestionDto with
        { PageIds = pageIds.ToList() };

        var validResultFaqQuestionDto = _faqQuestionDto with
        { PageIds = pageIds.ToList() };

        SetupRepositoryWrapper(existingFaqQuestion, updateFaqQuestion);
        SetupMapper(updateFaqQuestion, validResultFaqQuestionDto);
        var handler = new UpdateFaqQuestionHandler(_mockMapper.Object, _mockRepoWrapper.Object, _validator);

        Result<FaqQuestionDto> result = await handler.Handle(
            new UpdateFaqQuestionCommand(validUpdateFaqQuestionDto, _existingFaqQuestion.Id), CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(FaqConstants.SomePagesNotFound, result.Errors[0].Message);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public async Task Handle_InvalidData_ShouldReturnValidationError(string? questionText)
    {
        var invalidUpdateFaqQuestionDto = _updateFaqQuestionDto with
        { QuestionText = questionText! };
        SetupRepositoryWrapper(null, null);

        var handler = new UpdateFaqQuestionHandler(_mockMapper.Object, _mockRepoWrapper.Object, _validator);

        Result<FaqQuestionDto> result = await handler.Handle(
            new UpdateFaqQuestionCommand(invalidUpdateFaqQuestionDto, _existingFaqQuestion.Id), CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Contains(ErrorMessagesConstants.PropertyIsRequired(nameof(UpdateFaqQuestionDto.QuestionText)), result.Errors[0].Message);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(0)]
    public async Task Handle_FaqQuestionNotFound_ShouldReturnNotFoundError(long testId)
    {
        SetupRepositoryWrapper(null, null);
        var handler = new UpdateFaqQuestionHandler(_mockMapper.Object, _mockRepoWrapper.Object, _validator);

        Result<FaqQuestionDto> result = await handler.Handle(
            new UpdateFaqQuestionCommand(_updateFaqQuestionDto, testId), CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessagesConstants.NotFound(testId, typeof(FaqQuestion)), result.Errors[0].Message);
    }

    [Fact]
    public async Task Handle_SaveChangesFails_ShouldReturnFailureError()
    {
        SetupRepositoryWrapper(_existingFaqQuestion, _existingFaqQuestion, -1);
        SetupMapper(_existingFaqQuestion, _faqQuestionDto);
        var handler = new UpdateFaqQuestionHandler(_mockMapper.Object, _mockRepoWrapper.Object, _validator);

        Result<FaqQuestionDto> result = await handler.Handle(
            new UpdateFaqQuestionCommand(_updateFaqQuestionDto, _existingFaqQuestion.Id), CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessagesConstants.FailedToUpdateEntity(typeof(FaqQuestion)), result.Errors[0].Message);
    }
}
*/
