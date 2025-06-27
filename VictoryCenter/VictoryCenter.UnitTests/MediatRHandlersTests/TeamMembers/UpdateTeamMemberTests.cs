using AutoMapper;
using FluentValidation;
using Moq;
using VictoryCenter.BLL.Commands.TeamMembers.Update;
using VictoryCenter.BLL.DTOs.TeamMembers;
using VictoryCenter.DAL.Enums;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;
using VictoryCenter.BLL.Validators.TeamMembers;

namespace VictoryCenter.UnitTests.MediatRHandlersTests.TeamMembers;

public class UpdateTeamMemberTests
{
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<IRepositoryWrapper> _mockRepositoryWrapper;
    private readonly IValidator<UpdateTeamMemberCommand> _validator;

    private readonly TeamMember _testExistingTeamMember = new ()
    {
        Id = 1,
        FirstName = "Test",
        CategoryId = 1,
        Priority = 1,
        Status = Status.Published,
        Description = "Test description",
        CreatedAt = new DateTime(2025, 1, 1, 12, 0, 0, DateTimeKind.Utc),
        Category = new Category
        {
            Id = 1,
            Name = "Test Category",
            Description = "Test category description",
        },
        Email = "test@gmail.com",
        Photo = null,
    };

    private readonly TeamMember _testUpdatedTeamMember = new ()
    {
        Id = 1,
        FirstName = "Updated Name",
        CategoryId = 1,
        Priority = 1,
        Status = Status.Published,
        Description = "Test updated description",
        CreatedAt = new DateTime(2025, 1, 1, 12, 0, 0, DateTimeKind.Utc),
        Category = new Category
        {
            Id = 1,
            Name = "Test Category",
            Description = "Test category description",
        },
        Email = "test@gmail.com",
        Photo = null,
    };

    private readonly TeamMemberDto _testUpdatedTeamMemberDto = new ()
    {
        FirstName = "Updated Name",
        Description = "Updated Description",
    };

    public UpdateTeamMemberTests()
    {
        BaseTeamMembersValidator baseTeamMembersValidator = new BaseTeamMembersValidator();
        _mockMapper = new Mock<IMapper>();
        _mockRepositoryWrapper = new Mock<IRepositoryWrapper>();
        _validator = new UpdateTeamMemberValidator(baseTeamMembersValidator);
    }

    [Theory]
    [InlineData("Valid Name")]
    [InlineData("Updated Name")]
    [InlineData("A")]
    public async Task Handle_ValidRequestWithDifferentDescriptions_ShouldUpdateEntity(string testDescription)
    {
        var testUpdatedTeamMember = new TeamMember
        {
            Id = 1,
            FirstName = "Updated Name",
            CategoryId = 1,
            Priority = 1,
            Status = Status.Published,
            Description = testDescription,
            CreatedAt = new DateTime(2025, 1, 1, 12, 0, 0, DateTimeKind.Utc),
            Category = new Category
            {
                Id = 1,
                Name = "Test Category",
                Description = "Test category description",
            },
            Email = "test@gmail.com",
            Photo = null,
        };

        var testUpdatedTeamMemberDto = new TeamMemberDto
        {
            FirstName = "Updated Name",
            CategoryName = "Test Category",
            Priority = 1,
            Status = Status.Published,
            Description = testDescription,
            Email = "test@gmail.com",
            Photo = null,
            Id = 1
        };

        _mockMapper.Setup(x => x.Map<UpdateTeamMemberDto, TeamMember>(It.IsAny<UpdateTeamMemberDto>()))
            .Returns(testUpdatedTeamMember);

        _mockMapper.Setup(x => x.Map<TeamMember, TeamMemberDto>(It.IsAny<TeamMember>()))
            .Returns(testUpdatedTeamMemberDto);

        SetupRepositoryWrapper(_testExistingTeamMember);
        var handler = new UpdateTeamMemberHandler(_mockMapper.Object, _mockRepositoryWrapper.Object, _validator);

        var result = await handler.Handle(
            new UpdateTeamMemberCommand(new UpdateTeamMemberDto
            {
                Id = _testExistingTeamMember.Id,
                FirstName = "Updated Name",
                CategoryId = _testExistingTeamMember.CategoryId,
                Description = testDescription,
            }), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(testUpdatedTeamMemberDto.CategoryName, result.Value.CategoryName);
        Assert.Equal(testUpdatedTeamMemberDto.Priority, result.Value.Priority);
        Assert.Equal(testUpdatedTeamMemberDto.Status, result.Value.Status);
        Assert.Equal(testUpdatedTeamMemberDto.FirstName, result.Value.FirstName);
        Assert.Equal(testUpdatedTeamMemberDto.Description, result.Value.Description);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public async Task Handle_InvalidFirstName_ShouldReturnValidationError(string? testName)
    {
        _testUpdatedTeamMemberDto.FirstName = testName!;
        _testUpdatedTeamMember.FirstName = testName!;
        SetupDependencies(_testExistingTeamMember);
        var handler = new UpdateTeamMemberHandler(_mockMapper.Object, _mockRepositoryWrapper.Object, _validator);

        var result = await handler.Handle(
            new UpdateTeamMemberCommand(new UpdateTeamMemberDto
            {
                Id = _testExistingTeamMember.Id,
                FirstName = testName!,
                Description = "Updated Description",
            }), CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Contains("FirstName field is required", result.Errors[0].Message);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(0)]
    public async Task Handle_TeamMemberNotFound_ShouldReturnNotFoundError(long testId)
    {
        SetupDependencies();
        var handler = new UpdateTeamMemberHandler(_mockMapper.Object, _mockRepositoryWrapper.Object, _validator);

        var result = await handler.Handle(
            new UpdateTeamMemberCommand(new UpdateTeamMemberDto
        {
            Id = testId,
            FirstName = "Updated Name",
            Description = "Updated Description",
            CategoryId = _testExistingTeamMember.CategoryId,
        }), CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal("Not found", result.Errors[0].Message);
    }

    [Fact]
    public async Task Handle_SaveChangesFails_ShouldReturnFailureError()
    {
        SetupDependencies(_testExistingTeamMember, -1);
        var handler = new UpdateTeamMemberHandler(_mockMapper.Object, _mockRepositoryWrapper.Object, _validator);

        var result = await handler.Handle(
            new UpdateTeamMemberCommand(new UpdateTeamMemberDto
        {
            Id = _testExistingTeamMember.Id,
            FirstName = "Updated Name",
            Description = "Updated Description",
            CategoryId = _testExistingTeamMember.CategoryId,
        }), CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal("Failed to update team member", result.Errors[0].Message);
    }

    private void SetupDependencies(TeamMember? teamMemberToReturn = null, int saveResult = 1)
    {
        SetupMapper();
        SetupRepositoryWrapper(teamMemberToReturn, saveResult);
    }

    private void SetupMapper()
    {
        _mockMapper.Setup(x => x.Map<UpdateTeamMemberDto, TeamMember>(It.IsAny<UpdateTeamMemberDto>()))
            .Returns(_testUpdatedTeamMember);

        _mockMapper.Setup(x => x.Map<TeamMember, TeamMemberDto>(It.IsAny<TeamMember>()))
            .Returns(_testUpdatedTeamMemberDto);
    }

    private void SetupRepositoryWrapper(TeamMember? teamMemberToReturn = null, int saveResult = 1)
    {
        _mockRepositoryWrapper.Setup(x => x.TeamMembersRepository.GetFirstOrDefaultAsync(
                It.IsAny<QueryOptions<TeamMember>>()))
            .ReturnsAsync(teamMemberToReturn);

        _mockRepositoryWrapper.Setup(x => x.SaveChangesAsync())
            .ReturnsAsync(saveResult);
    }
}
