using System.Linq.Expressions;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Moq;
using VictoryCenter.BLL.Commands.Admin.Partners.ReorderSections;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.Partners;
using VictoryCenter.BLL.Exceptions.ReorderExceptions;
using VictoryCenter.BLL.Interfaces.ReorderService;
using VictoryCenter.BLL.Validators.Partners.Commands;
using VictoryCenter.DAL.Entities;

namespace VictoryCenter.UnitTests.MediatRHandlersTests.Partners;

public class ReorderPartnersSectionsTests
{
    private readonly Mock<IReorderService> _mockReorderService;
    private readonly IValidator<ReorderPartnersSectionsCommand> _validator;

    public ReorderPartnersSectionsTests()
    {
        _mockReorderService = new Mock<IReorderService>();
        _validator = new ReorderPartnersSectionsCommandValidator();
    }

    [Theory]
    [InlineData(2L, 1L)]
    [InlineData(3L, 2L, 1L)]
    public async Task Handle_ValidDto_ShouldReturnOk(params long[] orderedIds)
    {
        // Arrange
        var reorderDto = new ReorderPartnersSectionsDto { OrderedIds = [.. orderedIds] };
        var command = new ReorderPartnersSectionsCommand(reorderDto);
        SetupReorderService();

        var handler = new ReorderPartnersSectionsHandler(_validator, _mockReorderService.Object);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.IsSuccess);

        _mockReorderService.Verify(
            service => service.SwapElementsAsync(
                It.Is<List<long>>(ids => ids.SequenceEqual(orderedIds)),
                It.IsAny<Expression<Func<PartnerSection, long>>>(),
                It.IsAny<Expression<Func<PartnerSection, bool>>>()), Times.Once);
    }

    [Fact]
    public async Task Handle_InvalidDto_ShouldReturnValidationFailure()
    {
        // Arrange
        // This command will fail the real validator because OrderedIds is empty.
        var reorderDto = new ReorderPartnersSectionsDto { OrderedIds = [] };
        var command = new ReorderPartnersSectionsCommand(reorderDto);

        var handler = new ReorderPartnersSectionsHandler(_validator, _mockReorderService.Object);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.IsFailed);
    }

    [Fact]
    public async Task Handle_DbUpdateExceptionThrown_ShouldReturnFailure()
    {
        // Arrange
        var reorderDto = new ReorderPartnersSectionsDto { OrderedIds = [2, 1] };
        var command = new ReorderPartnersSectionsCommand(reorderDto);

        _mockReorderService.Setup(service => service.SwapElementsAsync<PartnerSection>(
            It.IsAny<List<long>>(),
            It.IsAny<Expression<Func<PartnerSection, long>>>(),
            It.IsAny<Expression<Func<PartnerSection, bool>>>()))
            .ThrowsAsync(new DbUpdateException());

        var handler = new ReorderPartnersSectionsHandler(_validator, _mockReorderService.Object);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.IsFailed);
        Assert.Equal(ErrorMessagesConstants.FailedToUpdateEntityInDatabase(typeof(PartnerSection)), result.Errors[0].Message);
    }

    [Fact]
    public async Task Handle_ReorderExceptionThrown_ShouldReturnFailure()
    {
        // Arrange
        var reorderDto = new ReorderPartnersSectionsDto { OrderedIds = [2, 1] };
        var command = new ReorderPartnersSectionsCommand(reorderDto);
        var reorderErrorMessage = "Test reorder error";

        _mockReorderService.Setup(service => service.SwapElementsAsync<PartnerSection>(
            It.IsAny<List<long>>(),
            It.IsAny<Expression<Func<PartnerSection, long>>>(),
            It.IsAny<Expression<Func<PartnerSection, bool>>>()))
            .ThrowsAsync(new ReorderException(reorderErrorMessage));

        var handler = new ReorderPartnersSectionsHandler(_validator, _mockReorderService.Object);

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
            .Setup(service => service.SwapElementsAsync<PartnerSection>(
                It.IsAny<List<long>>(),
                It.IsAny<Expression<Func<PartnerSection, long>>>(),
                It.IsAny<Expression<Func<PartnerSection, bool>>>()))
            .Returns(Task.CompletedTask);
    }
}
