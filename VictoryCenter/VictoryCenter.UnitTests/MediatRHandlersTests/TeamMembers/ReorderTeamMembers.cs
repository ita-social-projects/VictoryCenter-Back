using System.Transactions;
using FluentValidation;
using Moq;
using VictoryCenter.BLL.Commands.TeamMembers.Reorder;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.TeamMembers;
using VictoryCenter.BLL.Validators.TeamMembers;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.UnitTests.MediatRHandlersTests.TeamMembers;

public class ReorderTeamMembers
{
    private readonly Mock<IRepositoryWrapper> _mockRepositoryWrapper;
    private readonly IValidator<ReorderTeamMembersCommand> _validator;

    private readonly List<TeamMember> _testCategoryMembers =
    [
        new() { Id = 1, CategoryId = 1, Priority = 0 },
        new() { Id = 2, CategoryId = 1, Priority = 1 },
        new() { Id = 3, CategoryId = 1, Priority = 2 },
        new() { Id = 4, CategoryId = 1, Priority = 3 },
        new() { Id = 5, CategoryId = 1, Priority = 4 }
    ];

    private readonly List<TeamMember> _testMixedCategoryMembers =
    [
        new() { Id = 1, CategoryId = 1, Priority = 0 },
        new() { Id = 2, CategoryId = 1, Priority = 1 },
        new() { Id = 3, CategoryId = 2, Priority = 0 },
        new() { Id = 4, CategoryId = 1, Priority = 2 },
        new() { Id = 5, CategoryId = 2, Priority = 1 }
    ];

    private readonly ReorderTeamMembersDto _testValidReorderDto = new()
    {
        CategoryId = 1,
        OrderedIds = [4, 2, 5, 1, 3]
    };

    public ReorderTeamMembers()
    {
        _mockRepositoryWrapper = new Mock<IRepositoryWrapper>();
        _validator = new ReorderTeamMembersValidator();
    }

    [Fact]
    public async Task Handle_ValidRequest_ShouldReorderMembers()
    {
        // Arrange
        SetupDependencies(_testCategoryMembers);
        var handler = new ReorderTeamMembersHandler(_mockRepositoryWrapper.Object, _validator);
        var command = new ReorderTeamMembersCommand(_testValidReorderDto);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);

        // Verify priorities are updated correctly
        var member1 = _testCategoryMembers.First(m => m.Id == 1);
        var member2 = _testCategoryMembers.First(m => m.Id == 2);
        var member3 = _testCategoryMembers.First(m => m.Id == 3);
        var member4 = _testCategoryMembers.First(m => m.Id == 4);
        var member5 = _testCategoryMembers.First(m => m.Id == 5);

        Assert.Equal(0, member4.Priority);
        Assert.Equal(1, member2.Priority);
        Assert.Equal(2, member5.Priority);
        Assert.Equal(3, member1.Priority);
        Assert.Equal(4, member3.Priority);
    }

    [Fact]
    public async Task Handle_PartialReorderIdsProvided_ShouldReorderPartialMembers()
    {
        // Arrange
        var partialReorderDto = new ReorderTeamMembersDto
        {
            CategoryId = 1,
            OrderedIds = [3, 1]
        };

        SetupDependencies(_testCategoryMembers);
        var handler = new ReorderTeamMembersHandler(_mockRepositoryWrapper.Object, _validator);
        var command = new ReorderTeamMembersCommand(partialReorderDto);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);

        // Verify reordered members get new positions
        var member1 = _testCategoryMembers.First(m => m.Id == 1);
        var member3 = _testCategoryMembers.First(m => m.Id == 3);
        Assert.Equal(0, member3.Priority);
        Assert.Equal(1, member1.Priority);

        // Verify unchanged members maintain their relative order but get new positions
        var member2 = _testCategoryMembers.First(m => m.Id == 2);
        var member4 = _testCategoryMembers.First(m => m.Id == 4);
        var member5 = _testCategoryMembers.First(m => m.Id == 5);
        Assert.Equal(2, member2.Priority);
        Assert.Equal(3, member4.Priority);
        Assert.Equal(4, member5.Priority);
    }

    [Fact]
    public async Task Handle_NonExistingCategoryId_ShouldReturnError()
    {
        // Arrange
        SetupDependencies([]);
        var handler = new ReorderTeamMembersHandler(_mockRepositoryWrapper.Object, _validator);
        var command = new ReorderTeamMembersCommand(_testValidReorderDto);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(TeamMemberConstants.CategoryNotFoundOrContainsNoTeamMembers, result.Errors[0].Message);
    }

    [Fact]
    public async Task Handle_NonExistingMemberIds_ShouldReturnError()
    {
        // Arrange
        var invalidReorderDto = new ReorderTeamMembersDto
        {
            CategoryId = 1,
            OrderedIds = [1, 2, 99]
        };

        SetupDependencies(_testCategoryMembers);
        var handler = new ReorderTeamMembersHandler(_mockRepositoryWrapper.Object, _validator);
        var command = new ReorderTeamMembersCommand(invalidReorderDto);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Contains(TeamMemberConstants.InvalidTeamMemberIdsFound([99]), result.Errors[0].Message);
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

        SetupDependencies(_testCategoryMembers);
        var handler = new ReorderTeamMembersHandler(_mockRepositoryWrapper.Object, _validator);
        var command = new ReorderTeamMembersCommand(invalidReorderDto);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Contains(ErrorMessagesConstants.PropertyMustBePositive("CategoryId"), result.Errors[0].Message);
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

        SetupDependencies(_testCategoryMembers);
        var handler = new ReorderTeamMembersHandler(_mockRepositoryWrapper.Object, _validator);
        var command = new ReorderTeamMembersCommand(invalidReorderDto);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Contains(TeamMemberConstants.OrderedIdsCannotBeEmpty, result.Errors[0].Message);
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

        SetupDependencies(_testCategoryMembers);
        var handler = new ReorderTeamMembersHandler(_mockRepositoryWrapper.Object, _validator);
        var command = new ReorderTeamMembersCommand(invalidReorderDto);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Contains(TeamMemberConstants.OrderedIdsMustContainUniqueValues, result.Errors[0].Message);
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

        SetupDependencies(_testCategoryMembers);
        var handler = new ReorderTeamMembersHandler(_mockRepositoryWrapper.Object, _validator);
        var command = new ReorderTeamMembersCommand(invalidReorderDto);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Contains(ErrorMessagesConstants.PropertyMustBeGreaterThan("Each ID in OrderedIDS", 0), result.Errors[0].Message);
    }

    [Fact]
    public async Task Handle_ThreeMembersFromSameCategory_ShouldBeSuccessful()
    {
        // Arrange
        var reorderDto = new ReorderTeamMembersDto
        {
            CategoryId = 1,
            OrderedIds = [4, 1, 2]
        };

        var teamMemberInCategory = _testMixedCategoryMembers.Where(x => x.CategoryId == 1).ToList();

        SetupDependencies(teamMemberInCategory);
        var handler = new ReorderTeamMembersHandler(_mockRepositoryWrapper.Object, _validator);
        var command = new ReorderTeamMembersCommand(reorderDto);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);

        // Verify priorities are updated correctly for reordered members
        var member1 = _testMixedCategoryMembers.First(m => m.Id == 1);
        var member2 = _testMixedCategoryMembers.First(m => m.Id == 2);
        var member4 = _testMixedCategoryMembers.First(m => m.Id == 4);
        Assert.Equal(0, member4.Priority);
        Assert.Equal(1, member1.Priority);
        Assert.Equal(2, member2.Priority);

        // Verify members from other categories are not affected
        var member3 = _testMixedCategoryMembers.First(m => m.Id == 3);
        var member5 = _testMixedCategoryMembers.First(m => m.Id == 5);
        Assert.Equal(0, member3.Priority);
        Assert.Equal(1, member5.Priority);
    }

    [Fact]
    public async Task Handle_TwoMembersFromSameCategory_ShouldUpdatePrioritiesAndPreserveUnchanged()
    {
        // Arrange
        var reorderDto = new ReorderTeamMembersDto
        {
            CategoryId = 1,
            OrderedIds = [2, 4]
        };

        var teamMemberInCategory = _testMixedCategoryMembers.Where(x => x.CategoryId == 1).ToList();

        SetupDependencies(teamMemberInCategory);
        var handler = new ReorderTeamMembersHandler(_mockRepositoryWrapper.Object, _validator);
        var command = new ReorderTeamMembersCommand(reorderDto);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);

        // Verify priorities are updated correctly for reordered members
        var member2 = _testMixedCategoryMembers.First(m => m.Id == 2);
        var member4 = _testMixedCategoryMembers.First(m => m.Id == 4);
        Assert.Equal(0, member2.Priority);
        Assert.Equal(1, member4.Priority);

        // Verify unchanged member from same category gets next position
        var member1 = _testMixedCategoryMembers.First(m => m.Id == 1);
        Assert.Equal(2, member1.Priority);

        // Verify members from other categories are not affected
        var member3 = _testMixedCategoryMembers.First(m => m.Id == 3);
        var member5 = _testMixedCategoryMembers.First(m => m.Id == 5);
        Assert.Equal(0, member3.Priority);
        Assert.Equal(1, member5.Priority);
    }

    [Fact]
    public async Task Handle_OrderedIdsIncludeIdFromAnotherCategory_ShouldReturnError()
    {
        // Arrange
        var reorderDto = new ReorderTeamMembersDto
        {
            CategoryId = 1,
            OrderedIds = [1, 2, 3] // ID 3 belongs to CategoryId = 2, not 1
        };

        var teamMemberInCategory = _testMixedCategoryMembers.Where(x => x.CategoryId == 1).ToList();

        SetupDependencies(teamMemberInCategory);
        var handler = new ReorderTeamMembersHandler(_mockRepositoryWrapper.Object, _validator);
        var command = new ReorderTeamMembersCommand(reorderDto);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Contains(TeamMemberConstants.InvalidTeamMemberIdsFound([3]), result.Errors[0].Message);

        // Verify no priorities were changed since operation failed
        var member1 = _testMixedCategoryMembers.First(m => m.Id == 1);
        var member2 = _testMixedCategoryMembers.First(m => m.Id == 2);
        var member3 = _testMixedCategoryMembers.First(m => m.Id == 3);
        var member4 = _testMixedCategoryMembers.First(m => m.Id == 4);
        var member5 = _testMixedCategoryMembers.First(m => m.Id == 5);

        Assert.Equal(0, member1.Priority);
        Assert.Equal(1, member2.Priority);
        Assert.Equal(0, member3.Priority);
        Assert.Equal(2, member4.Priority);
        Assert.Equal(1, member5.Priority);
    }

    private void SetupDependencies(List<TeamMember> membersToReturn, int saveResult = 1)
    {
        SetupRepositoryWrapper(membersToReturn, saveResult);
    }

    private void SetupRepositoryWrapper(List<TeamMember> membersToReturn, int saveResult = 1)
    {
        _mockRepositoryWrapper.Setup(x => x.TeamMembersRepository.GetAllAsync(
                It.IsAny<QueryOptions<TeamMember>>()))
            .ReturnsAsync(membersToReturn);

        _mockRepositoryWrapper.Setup(x => x.BeginTransaction()).Returns(new TransactionScope(TransactionScopeAsyncFlowOption.Enabled));

        _mockRepositoryWrapper.Setup(x => x.SaveChangesAsync())
            .ReturnsAsync(saveResult);
    }
}
