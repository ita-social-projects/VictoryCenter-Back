using AutoMapper;
using FluentValidation;
using Moq;
using VictoryCenter.BLL.Commands.TeamMembers.Update;
using VictoryCenter.BLL.DTOs.TeamMembers;
using VictoryCenter.BLL.Validators.TeamMembers;
using VictoryCenter.DAL.Enums;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.UnitTests.MediatRHandlersTests.TeamMembers;

public class UpdateCategoryTests
{
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<IRepositoryWrapper> _mockRepositoryWrapper;
    private readonly IValidator<UpdateTeamMemberCommand> _validator;

    private readonly TeamMember _testExistingTeamMember = new ()
    {
        Id = 1,
        FirstName = "Test",
        LastName = "Member",
        MiddleName = "Middle",
        CategoryId = 1,
        Priority = 1,
        Status = Status.Published,
        Description = "Test description",
        CreatedAt = new DateTime(2025, 1, 1, 12, 0, 0, DateTimeKind.Local),
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
        FirstName = "Test updated",
        LastName = "Member",
        MiddleName = "Middle",
        CategoryId = 1,
        Priority = 1,
        Status = Status.Published,
        Description = "Test updated description",
        CreatedAt = new DateTime(2025, 1, 1, 12, 0, 0, DateTimeKind.Local),
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

    public UpdateCategoryTests()
    {
        _mockMapper = new Mock<IMapper>();
        _mockRepositoryWrapper = new Mock<IRepositoryWrapper>();
        _validator = new UpdateTeamMemberValidator();
    }

    [Theory]
    [InlineData("Valid Name")]
    [InlineData("Updated Name")]
    [InlineData("A")]
    public async Task Handle_ShouldUpdateEntity(string testDescription)
    {
        _testUpdatedTeamMember.Description = testDescription;
        _testUpdatedTeamMemberDto.Description = testDescription;
        SetupDependencies(_testExistingTeamMember);
        var handler = new UpdateTeamMemberHandler(_mockMapper.Object, _mockRepositoryWrapper.Object, _validator);

        var result = await handler.Handle(
            new UpdateTeamMemberCommand(new UpdateTeamMemberDto
            {
                Id = _testExistingTeamMember.Id,
                FirstName = "Updated Name",
                Description = testDescription,
            }), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(_testUpdatedTeamMemberDto.FirstName, result.Value.FirstName);
        Assert.Equal(_testUpdatedTeamMemberDto.Description, result.Value.Description);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public async Task Handle_ShouldNotUpdateEntity_IncorrectName(string? testName)
    {
        _testUpdatedTeamMemberDto.FirstName = testName ?? string.Empty;
        _testUpdatedTeamMember.FirstName = testName ?? string.Empty;
        SetupDependencies(_testExistingTeamMember);
        var handler = new UpdateTeamMemberHandler(_mockMapper.Object, _mockRepositoryWrapper.Object, _validator);

        var result = await handler.Handle(
            new UpdateTeamMemberCommand(new UpdateTeamMemberDto
            {
                Id = _testExistingTeamMember.Id,
                FirstName = testName ?? string.Empty,
                Description = "Updated Description",
            }), CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Contains("Validation failed", result.Errors[0].Message);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(0)]
    public async Task Handle_ShouldNotUpdateEntity_NotFound(long testId)
    {
        SetupDependencies();
        var handler = new UpdateTeamMemberHandler(_mockMapper.Object, _mockRepositoryWrapper.Object, _validator);

        var result = await handler.Handle(
            new UpdateTeamMemberCommand(new UpdateTeamMemberDto
        {
            Id = testId,
            FirstName = "Updated Name",
            Description = "Updated Description",
        }), CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal("Not found", result.Errors[0].Message);
    }

    [Fact]
    public async Task Handle_ShouldNotUpdateEntity_SaveChangesFails()
    {
        SetupDependencies(_testExistingTeamMember, -1);
        var handler = new UpdateTeamMemberHandler(_mockMapper.Object, _mockRepositoryWrapper.Object, _validator);

        var result = await handler.Handle(
            new UpdateTeamMemberCommand(new UpdateTeamMemberDto
        {
            Id = _testExistingTeamMember.Id,
            FirstName = "Updated Name",
            Description = "Updated Description",
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
        _mockRepositoryWrapper.Setup(x => x.TeamMemberRepository.GetFirstOrDefaultAsync(
                It.IsAny<QueryOptions<TeamMember>>()))
            .ReturnsAsync(teamMemberToReturn);

        _mockRepositoryWrapper.Setup(x => x.SaveChangesAsync())
            .ReturnsAsync(saveResult);
    }
}
