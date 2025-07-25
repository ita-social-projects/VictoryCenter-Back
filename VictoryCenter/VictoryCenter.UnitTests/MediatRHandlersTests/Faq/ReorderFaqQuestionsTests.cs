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
    public async Task Handle_DtoIsValid_ShouldReturnOk(params long[] pageIds)
    {
        // Arrange
        var command = new ReorderFaqQuestionsCommand(new() { PageId = 1, OrderedIds = [.. pageIds] });
        var mockPlacements = FilterList(command);
        SetupRepositoryWrapper(mockPlacements, 1);
        var handler = new ReorderFaqQuestionsHandler(_validator, _mockRepoWrapper.Object);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.IsSuccess);

        long[] affected = [..mockPlacements.Where(p => pageIds.Contains(p.QuestionId))
            .OrderBy(p => p.Priority).Select(p => p.QuestionId)];
        Assert.Equal(pageIds, affected);

        var unaffected = mockPlacements.Where(p => !pageIds.Contains(p.QuestionId))
            .OrderBy(p => p.QuestionId).ToArray();

        for (var i = 0; i < unaffected.Length; i++)
        {
            Assert.Equal(unaffected[i].QuestionId, unaffected[i].Priority);
        }
    }

    [Fact]
    public async Task Handle_DtoIsInvalid_ShouldReturnFail()
    {
        // Arrange
        var command = new ReorderFaqQuestionsCommand(new() { PageId = -10, OrderedIds = [2, 1] });
        var mockPlacements = FilterList(command);
        SetupRepositoryWrapper(mockPlacements, 0);
        var handler = new ReorderFaqQuestionsHandler(_validator, _mockRepoWrapper.Object);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.IsFailed);
        Assert.Contains(
                ErrorMessagesConstants.PropertyMustBePositive(nameof(ReorderFaqQuestionsDto.PageId)),
                result.Errors[0].Message);
    }

    [Fact]
    public async Task Handle_PageNotFoundOrContainsNoFaqQuestions_ShouldReturnFail()
    {
        // Arrange
        var command = new ReorderFaqQuestionsCommand(new() { PageId = 1000, OrderedIds = [2, 1] });
        var mockPlacements = FilterList(command);
        SetupRepositoryWrapper(mockPlacements, 0);
        var handler = new ReorderFaqQuestionsHandler(_validator, _mockRepoWrapper.Object);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.IsFailed);
        Assert.Equal(FaqConstants.PageNotFoundOrContainsNoFaqQuestions, result.Errors[0].Message);
    }

    [Fact]
    public async Task Handle_OrderedIdsContainsInvalidId_ShouldReturnFail()
    {
        // Arrange
        var command = new ReorderFaqQuestionsCommand(new() { PageId = 1, OrderedIds = [1000, 1] });
        var mockPlacements = FilterList(command);
        SetupRepositoryWrapper(mockPlacements, 0);
        var handler = new ReorderFaqQuestionsHandler(_validator, _mockRepoWrapper.Object);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.IsFailed);
        Assert.Equal(
            ErrorMessagesConstants.ReorderingContainsInvalidIds(typeof(FaqQuestion), [1000]),
            result.Errors[0].Message);
    }

    [Theory]
    [InlineData(4L, 2L)]
    [InlineData(4L, 2L, 1L)]
    [InlineData(5L, 4L, 2L, 1L)]
    public async Task Handle_OrderedIdsAreNonConsecutive_ShouldReturnFail(params long[] pageIds)
    {
        // Arrange
        var command = new ReorderFaqQuestionsCommand(new() { PageId = 1, OrderedIds = [.. pageIds] });
        var mockPlacements = FilterList(command);
        SetupRepositoryWrapper(mockPlacements, 0);
        var handler = new ReorderFaqQuestionsHandler(_validator, _mockRepoWrapper.Object);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.IsFailed);
        Assert.Equal(FaqConstants.IdsAreNonConsecutive, result.Errors[0].Message);
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
