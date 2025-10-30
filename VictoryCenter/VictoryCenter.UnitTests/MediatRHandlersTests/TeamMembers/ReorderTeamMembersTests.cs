using System.Linq.Expressions;
using System.Transactions;
using FluentValidation;
using MediatR;
using Moq;
using VictoryCenter.BLL.Commands.Admin.TeamMembers.Reorder;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.TeamMembers;
using VictoryCenter.BLL.Exceptions.ReorderExceptions;
using VictoryCenter.BLL.Interfaces.ReorderService;
using VictoryCenter.BLL.Validators.TeamMembers;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.Base;

namespace VictoryCenter.UnitTests.MediatRHandlersTests.TeamMembers;

public class ReorderTeamMembersTests
{
    private readonly Mock<IRepositoryWrapper> _mockRepositoryWrapper;
    private readonly Mock<IReorderService> _mockReorderService;
    private readonly IValidator<ReorderTeamMembersCommand> _validator;

    private readonly ReorderTeamMembersDto _testValidReorderDto = new()
    {
        CategoryId = 1,
        OrderedIds = [4, 2, 5, 1, 3]
    };

    public ReorderTeamMembersTests()
    {
        _mockRepositoryWrapper = new Mock<IRepositoryWrapper>();
        _mockReorderService = new Mock<IReorderService>();
        _validator = new ReorderTeamMembersValidator();
    }

    [Fact]
    public async Task Handle_ValidRequest_ShouldReorderMembers()
    {
        // Arrange
        SetupDependencies();

        var handler = new ReorderTeamMembersHandler(_mockRepositoryWrapper.Object, _validator, _mockReorderService.Object);
        var command = new ReorderTeamMembersCommand(_testValidReorderDto);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(Unit.Value, result.Value);

        _mockReorderService.Verify(
            x => x.SwapElementsAsync(
            It.Is<List<long>>(ids => ids.SequenceEqual(_testValidReorderDto.OrderedIds)),
            It.IsAny<Expression<Func<TeamMember, long>>>(),
            It.IsAny<Expression<Func<TeamMember, bool>>>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_EmptyOrderedIds_ShouldReturnError()
    {
        // Arrange
        var invalidReorderDto = new ReorderTeamMembersDto
        {
            CategoryId = 1,
            OrderedIds = []
        };

        SetupDependencies();
        var handler = new ReorderTeamMembersHandler(_mockRepositoryWrapper.Object, _validator, _mockReorderService.Object);
        var command = new ReorderTeamMembersCommand(invalidReorderDto);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Contains(ErrorMessagesConstants.CollectionCannotBeEmpty(nameof(ReorderTeamMembersDto.OrderedIds)), result.Errors[0].Message);
    }

    [Fact]
    public async Task Handle_DuplicateIdsInOrderedIds_ShouldReturnError()
    {
        // Arrange
        var invalidReorderDto = new ReorderTeamMembersDto
        {
            CategoryId = 1,
            OrderedIds = [1, 2, 2, 3]
        };

        SetupDependencies();
        var handler = new ReorderTeamMembersHandler(_mockRepositoryWrapper.Object, _validator, _mockReorderService.Object);
        var command = new ReorderTeamMembersCommand(invalidReorderDto);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Contains(ErrorMessagesConstants.CollectionMustContainUniqueValues(nameof(ReorderTeamMembersDto.OrderedIds)), result.Errors[0].Message);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(0)]
    public async Task Handle_InvalidCategoryId_ShouldReturnError(long invalidCategoryId)
    {
        // Arrange
        var invalidReorderDto = new ReorderTeamMembersDto
        {
            CategoryId = invalidCategoryId,
            OrderedIds = [1, 2, 3]
        };

        SetupDependencies();
        var handler = new ReorderTeamMembersHandler(_mockRepositoryWrapper.Object, _validator, _mockReorderService.Object);
        var command = new ReorderTeamMembersCommand(invalidReorderDto);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Contains(ErrorMessagesConstants.PropertyMustBePositive("CategoryId"), result.Errors[0].Message);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(0)]
    public async Task Handle_InvalidIdInOrderedIds_ShouldReturnError(long invalidId)
    {
        // Arrange
        var invalidReorderDto = new ReorderTeamMembersDto
        {
            CategoryId = 1,
            OrderedIds = [1, 2, invalidId]
        };

        SetupDependencies();
        var handler = new ReorderTeamMembersHandler(_mockRepositoryWrapper.Object, _validator, _mockReorderService.Object);
        var command = new ReorderTeamMembersCommand(invalidReorderDto);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Contains(
            ErrorMessagesConstants.PropertyMustBePositive(
            $"Each {nameof(ReorderTeamMembersDto.OrderedIds)} element"),
            result.Errors[0].Message);
    }

    [Fact]
    public async Task Handle_ReorderServiceThrowsReorderException_ShouldReturnReorderError()
    {
        // Arrange
        var reorderExceptionMessage = "Reorder operation failed";

        _mockReorderService.Setup(x => x.SwapElementsAsync<TeamMember>(
                It.IsAny<List<long>>(),
                It.IsAny<Expression<Func<TeamMember, long>>>(),
                It.IsAny<Expression<Func<TeamMember, bool>>>()))
            .ThrowsAsync(new ReorderException(reorderExceptionMessage));

        SetupRepositoryWrapper();
        var handler = new ReorderTeamMembersHandler(_mockRepositoryWrapper.Object, _validator, _mockReorderService.Object);
        var command = new ReorderTeamMembersCommand(_testValidReorderDto);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ReorderConstants.ErrorWithReordering(reorderExceptionMessage), result.Errors[0].Message);
    }

    private void SetupDependencies(int saveResult = 1)
    {
        SetupRepositoryWrapper(saveResult);
        SetupReorderService();
    }

    private void SetupReorderService()
    {
        _mockReorderService.Setup(x => x.SwapElementsAsync<TeamMember>(
                It.IsAny<List<long>>(),
                It.IsAny<Expression<Func<TeamMember, long>>>(),
                It.IsAny<Expression<Func<TeamMember, bool>>>()))
            .Returns(Task.CompletedTask);
    }

    private void SetupRepositoryWrapper(int saveResult = 1)
    {
        _mockRepositoryWrapper.Setup(x => x.BeginTransaction())
            .Returns(new TransactionScope(TransactionScopeAsyncFlowOption.Enabled));

        _mockRepositoryWrapper.Setup(x => x.SaveChangesAsync())
            .ReturnsAsync(saveResult);
    }
}
