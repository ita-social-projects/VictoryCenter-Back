using System.Transactions;
using AutoMapper;
using FluentResults;
using FluentValidation;
using Moq;
using VictoryCenter.BLL.Commands.TeamMembers.Update;
using VictoryCenter.BLL.DTOs.TeamMembers;
using VictoryCenter.BLL.Validators.TeamMembers;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Enums;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.UnitTests.MediatRHandlersTests.TeamMembers;

public class UpdateTeamMemberTests
{
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<IRepositoryWrapper> _mockRepositoryWrapper;

    private readonly Category _testCategory = new()
    {
        Id = 1,
        Name = "Test Category",
        Description = "Sample test category"
    };

    private readonly TeamMember _testExistingTeamMember = new()
    {
        Id = 1,
        FullName = "Test",
        CategoryId = 1,
        Priority = 1,
        Status = Status.Published,
        Description = "Test description",
        CreatedAt = new DateTime(2025, 1, 1, 12, 0, 0, DateTimeKind.Utc),
        Category = new Category
        {
            Id = 1,
            Name = "Test Category",
            Description = "Test category description"
        },
        Email = "test@gmail.com",
        Photo = null
    };

    private readonly TeamMember _testUpdatedTeamMember = new()
    {
        Id = 1,
        FullName = "Updated Name",
        CategoryId = 1,
        Priority = 1,
        Status = Status.Published,
        Description = "Test updated description",
        CreatedAt = new DateTime(2025, 1, 1, 12, 0, 0, DateTimeKind.Utc),
        Category = new Category
        {
            Id = 1,
            Name = "Test Category",
            Description = "Test category description"
        },
        Email = "test@gmail.com",
        Photo = null
    };

    private readonly TeamMemberDto _testUpdatedTeamMemberDto = new()
    {
        FullName = "Updated Name",
        Description = "Updated Description"
    };

    private readonly IValidator<UpdateTeamMemberCommand> _validator;

    public UpdateTeamMemberTests()
    {
        var baseTeamMembersValidator = new BaseTeamMembersValidator();
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
            FullName = "Updated Name",
            CategoryId = 1,
            Priority = 1,
            Status = Status.Published,
            Description = testDescription,
            CreatedAt = new DateTime(2025, 1, 1, 12, 0, 0, DateTimeKind.Utc),
            Category = new Category
            {
                Id = 1,
                Name = "Test Category",
                Description = "Test category description"
            },
            Email = "test@gmail.com",
            Photo = null
        };

        var testUpdatedTeamMemberDto = new TeamMemberDto
        {
            FullName = "Updated Name",
            CategoryId = 1,
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

        Result<TeamMemberDto> result = await handler.Handle(
            new UpdateTeamMemberCommand(
                new UpdateTeamMemberDto
                {
                    FullName = "Updated Name",
                    CategoryId = _testExistingTeamMember.CategoryId,
                    Description = testDescription
                },
                _testExistingTeamMember.Id), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(testUpdatedTeamMemberDto.CategoryId, result.Value.CategoryId);
        Assert.Equal(testUpdatedTeamMemberDto.Priority, result.Value.Priority);
        Assert.Equal(testUpdatedTeamMemberDto.Status, result.Value.Status);
        Assert.Equal(testUpdatedTeamMemberDto.FullName, result.Value.FullName);
        Assert.Equal(testUpdatedTeamMemberDto.Description, result.Value.Description);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public async Task Handle_InvalidFullName_ShouldReturnValidationError(string? testName)
    {
        _testUpdatedTeamMemberDto.FullName = testName!;
        _testUpdatedTeamMember.FullName = testName!;
        SetupDependencies(_testExistingTeamMember);
        var handler = new UpdateTeamMemberHandler(_mockMapper.Object, _mockRepositoryWrapper.Object, _validator);

        Result<TeamMemberDto> result = await handler.Handle(
            new UpdateTeamMemberCommand(
                new UpdateTeamMemberDto
                {
                    FullName = testName!,
                    Description = "Updated Description"
                }, _testExistingTeamMember.Id), CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Contains("FullName field is required", result.Errors[0].Message);
    }

    [Fact]
    public async Task Handle_InvalidCategoryId_ShouldReturnValidationError()
    {
        _testUpdatedTeamMember.CategoryId = 10000;

        _mockRepositoryWrapper.Setup(x =>
                x.TeamMembersRepository.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<TeamMember>>()))
            .ReturnsAsync(_testExistingTeamMember);
        _mockRepositoryWrapper
            .Setup(x => x.CategoriesRepository.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<Category>>()))
            .ReturnsAsync((Category?)null);
        SetupMapper();
        var handler = new UpdateTeamMemberHandler(_mockMapper.Object, _mockRepositoryWrapper.Object, _validator);

        Result<TeamMemberDto> result = await handler.Handle(
            new UpdateTeamMemberCommand(
                new UpdateTeamMemberDto
                {
                    FullName = "test1",
                    CategoryId = 10000,
                    Description = "Updated Description"
                }, _testExistingTeamMember.Id), CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Contains("Category not found", result.Errors[0].Message);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(0)]
    public async Task Handle_TeamMemberNotFound_ShouldReturnNotFoundError(long testId)
    {
        SetupDependencies();
        var handler = new UpdateTeamMemberHandler(_mockMapper.Object, _mockRepositoryWrapper.Object, _validator);

        Result<TeamMemberDto> result = await handler.Handle(
            new UpdateTeamMemberCommand(
                new UpdateTeamMemberDto
                {
                    FullName = "Updated Name",
                    Description = "Updated Description",
                    CategoryId = _testExistingTeamMember.CategoryId
                }, testId), CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal("Not found", result.Errors[0].Message);
    }

    [Fact]
    public async Task Handle_SaveChangesFails_ShouldReturnFailureError()
    {
        SetupDependencies(_testExistingTeamMember, -1);
        var handler = new UpdateTeamMemberHandler(_mockMapper.Object, _mockRepositoryWrapper.Object, _validator);

        Result<TeamMemberDto> result = await handler.Handle(
            new UpdateTeamMemberCommand(
                new UpdateTeamMemberDto
                {
                    FullName = "Updated Name",
                    Description = "Updated Description",
                    CategoryId = _testExistingTeamMember.CategoryId
                }, _testExistingTeamMember.Id), CancellationToken.None);

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
        _mockRepositoryWrapper.Setup(x =>
                x.TeamMembersRepository.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<TeamMember>>()))
            .ReturnsAsync(teamMemberToReturn);

        _mockRepositoryWrapper.Setup(x => x.SaveChangesAsync())
            .ReturnsAsync(saveResult);

        _mockRepositoryWrapper.Setup(x => x.BeginTransaction())
            .Returns(new TransactionScope(TransactionScopeAsyncFlowOption.Enabled));

        _mockRepositoryWrapper
            .Setup(x => x.CategoriesRepository.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<Category>>()))
            .ReturnsAsync(_testCategory);
    }
}
