using System.Linq.Expressions;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Moq;
using VictoryCenter.BLL.Commands.Admin.FaqQuestions.Reorder;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.FaqQuestions;
using VictoryCenter.BLL.Exceptions.ReorderExceptions;
using VictoryCenter.BLL.Interfaces.ReorderService;
using VictoryCenter.BLL.Validators.FaqQuestions;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.UnitTests.MediatRHandlersTests.FaqQuestions;

public class ReorderFaqQuestionsTests
{
    private readonly Mock<IRepositoryWrapper> _mockRepoWrapper;
    private readonly Mock<IReorderService> _mockReorderService;
    private readonly IValidator<ReorderFaqQuestionsCommand> _validator;

    public ReorderFaqQuestionsTests()
    {
        _mockRepoWrapper = new Mock<IRepositoryWrapper>();
        _mockReorderService = new Mock<IReorderService>();
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
        SetupRepositoryWrapper(pageIds.Length);
        SetupReorderService();

        var handler = new ReorderFaqQuestionsHandler(_validator, _mockRepoWrapper.Object, _mockReorderService.Object);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.IsSuccess);

        _mockReorderService.Verify(
            service => service.SwapElementsAsync(
            It.Is<List<long>>(ids => ids.SequenceEqual(pageIds)),
            It.IsAny<Expression<Func<FaqPlacement, long>>>(),
            It.IsAny<Expression<Func<FaqPlacement, bool>>>()), Times.Once);
    }

    [Fact]
    public async Task Handle_DtoIsInvalid_ShouldReturnFailure()
    {
        // Arrange
        var command = new ReorderFaqQuestionsCommand(new() { PageId = -10, OrderedIds = [2, 1] });
        SetupRepositoryWrapper(0);
        SetupReorderService();

        var handler = new ReorderFaqQuestionsHandler(_validator, _mockRepoWrapper.Object, _mockReorderService.Object);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.IsFailed);
        Assert.Contains(
                ErrorMessagesConstants.PropertyMustBePositive(nameof(ReorderFaqQuestionsDto.PageId)),
                result.Errors[0].Message,
                StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task Handle_PageNotFoundOrContainsNoFaqQuestions_ShouldReturnFailure()
    {
        // Arrange
        var command = new ReorderFaqQuestionsCommand(new() { PageId = 1000, OrderedIds = [2, 1] });
        SetupRepositoryWrapper(0);
        SetupReorderService();
        var handler = new ReorderFaqQuestionsHandler(_validator, _mockRepoWrapper.Object, _mockReorderService.Object);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.IsFailed);
        Assert.Equal(FaqConstants.PageNotFoundOrContainsNoFaqQuestions, result.Errors[0].Message);
    }

    [Fact]
    public async Task Handle_DbExceptionThrown_ShouldReturnFailure()
    {
        // Arrange
        var command = new ReorderFaqQuestionsCommand(new() { PageId = 1, OrderedIds = [2, 1] });

        SetupRepositoryWrapper(2);

        _mockReorderService.Setup(service => service.SwapElementsAsync<FaqPlacement>(
            It.IsAny<List<long>>(),
            It.IsAny<Expression<Func<FaqPlacement, long>>>(),
            It.IsAny<Expression<Func<FaqPlacement, bool>>>()))
            .ThrowsAsync(new DbUpdateException());

        var handler = new ReorderFaqQuestionsHandler(_validator, _mockRepoWrapper.Object, _mockReorderService.Object);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.IsFailed);
        Assert.Equal(ErrorMessagesConstants.FailedToUpdateEntityInDatabase(typeof(FaqQuestion)), result.Errors[0].Message);
    }

    [Fact]
    public async Task Handle_ReorderExceptionThrown_ShouldReturnFailure()
    {
        // Arrange
        var command = new ReorderFaqQuestionsCommand(new() { PageId = 1, OrderedIds = [2, 1] });
        var reorderErrorMessage = "Test reorder error";

        SetupRepositoryWrapper(2);

        _mockReorderService.Setup(service => service.SwapElementsAsync(
            It.IsAny<List<long>>(),
            It.IsAny<Expression<Func<FaqPlacement, long>>>(),
            It.IsAny<Expression<Func<FaqPlacement, bool>>>()))
            .ThrowsAsync(new ReorderException(reorderErrorMessage));

        var handler = new ReorderFaqQuestionsHandler(_validator, _mockRepoWrapper.Object, _mockReorderService.Object);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.IsFailed);
        Assert.Equal(ReorderConstants.ErrorWithReordering(reorderErrorMessage), result.Errors[0].Message);
    }

    private void SetupReorderService()
    {
        _mockReorderService
            .Setup(service => service.SwapElementsAsync<FaqPlacement>(
                It.IsAny<List<long>>(),
                It.IsAny<Expression<Func<FaqPlacement, long>>>(),
                It.IsAny<Expression<Func<FaqPlacement, bool>>>()))
            .Returns(Task.CompletedTask);
    }

    private void SetupRepositoryWrapper(int countResult)
    {
        _mockRepoWrapper.Setup(
            repositoryWrapper => repositoryWrapper.FaqPlacementsRepository.CountAsync(
                It.IsAny<QueryOptions<FaqPlacement>>())).ReturnsAsync(countResult);
    }
}
