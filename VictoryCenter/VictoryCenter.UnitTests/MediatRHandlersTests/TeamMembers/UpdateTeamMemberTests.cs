using System.Linq.Expressions;
using System.Transactions;
using AutoMapper;
using FluentResults;
using FluentValidation;
using Moq;
using VictoryCenter.BLL.Commands.Admin.TeamMembers.Update;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.TeamMembers;
using VictoryCenter.BLL.Exceptions.ReorderExceptions;
using VictoryCenter.BLL.Interfaces.ReorderService;
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
    private readonly Mock<IReorderService> _mockReorderService;
    private readonly IValidator<UpdateTeamMemberCommand> _validator;

    private readonly TeamCategory _testCategory = new()
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
        CreatedAt = new DateTimeOffset(2025, 1, 1, 12, 0, 0, TimeZoneInfo.Utc.BaseUtcOffset),
        TeamCategory = new TeamCategory
        {
            Id = 1,
            Name = "Test Category",
            Description = "Test category description",
        },
        Email = "test@gmail.com",
        ImageId = null,
        Localizations = [
            new() { TranslationStatus = TranslationStatus.Relevant },
            new() { TranslationStatus = TranslationStatus.Relevant }
        ]
    };

    private readonly TeamMember _testUpdatedTeamMember = new()
    {
        Id = 1,
        FullName = "Updated Name",
        CategoryId = 1,
        Priority = 1,
        Status = Status.Published,
        Description = "Test updated description",
        CreatedAt = new DateTimeOffset(2025, 1, 1, 12, 0, 0, TimeZoneInfo.Utc.BaseUtcOffset),
        TeamCategory = new TeamCategory
        {
            Id = 1,
            Name = "Test Category",
            Description = "Test category description",
        },
        Email = "test@gmail.com",
        ImageId = null,
        Localizations = [
            new() { TranslationStatus = TranslationStatus.Outdated },
            new() { TranslationStatus = TranslationStatus.Outdated }
        ]
    };

    private readonly TeamMemberDto _testUpdatedTeamMemberDto = new()
    {
        FullName = "Updated Name",
        Description = "Updated Description",
    };

    public UpdateTeamMemberTests()
    {
        var baseTeamMembersValidator = new BaseTeamMembersValidator();
        _mockMapper = new Mock<IMapper>();
        _mockRepositoryWrapper = new Mock<IRepositoryWrapper>();
        _mockReorderService = new Mock<IReorderService>();
        _validator = new UpdateTeamMemberValidator(baseTeamMembersValidator);
    }

    [Fact]
    public async Task Handle_ValidRequestWithDifferentDescriptions_ShouldUpdateEntity()
    {
        var validDescription = new string('A', TeamMemberConstants.DescriptionNameMinLength + 5);

        var testUpdatedTeamMemberDto = new TeamMemberDto
        {
            FullName = "Updated Name",
            CategoryId = 1,
            Priority = 1,
            Status = Status.Published,
            Description = validDescription,
            Id = 1
        };

        _mockMapper.Setup(x => x.Map(It.IsAny<UpdateTeamMemberDto>(), It.IsAny<TeamMember>()));

        _mockMapper.Setup(x => x.Map<TeamMember, TeamMemberDto>(It.IsAny<TeamMember>()))
            .Returns(testUpdatedTeamMemberDto);

        SetupRepositoryWrapper(_testExistingTeamMember);
        SetupReorderService();

        var handler = new UpdateTeamMemberHandler(_mockMapper.Object, _mockRepositoryWrapper.Object, _validator, _mockReorderService.Object);

        Result<TeamMemberDto> result = await handler.Handle(
            new UpdateTeamMemberCommand(
                new UpdateTeamMemberDto
                {
                    FullName = "Updated Name",
                    CategoryId = _testExistingTeamMember.CategoryId,
                    Description = validDescription
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

    [Fact]
    public async Task Handle_ValidRequest_ShouldChangeTranslationStatusesToOutdated()
    {
        var testUpdatedTeamMemberDto = new TeamMemberDto
        {
            Id = 1,
            FullName = "Original Name",
            CategoryId = 1,
            Priority = 1,
            Status = Status.Published,
            Description = "Original Description",
            Localizations = [
                new() { TranslationStatus = TranslationStatus.Outdated },
                new() { TranslationStatus = TranslationStatus.Outdated }
            ]
        };

        _mockMapper.Setup(x => x.Map(It.IsAny<UpdateTeamMemberDto>(), It.IsAny<TeamMember>()));

        _mockMapper.Setup(x => x.Map<TeamMember, TeamMemberDto>(It.IsAny<TeamMember>()))
            .Returns(testUpdatedTeamMemberDto);

        SetupRepositoryWrapper(_testExistingTeamMember);

        var handler = new UpdateTeamMemberHandler(_mockMapper.Object, _mockRepositoryWrapper.Object, _validator, _mockReorderService.Object);

        Result<TeamMemberDto> result = await handler.Handle(
            new UpdateTeamMemberCommand(
                new UpdateTeamMemberDto
                {
                    FullName = "Updated Name",
                    CategoryId = _testExistingTeamMember.CategoryId,
                    Description = "Updated Description"
                },
                _testExistingTeamMember.Id), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.All(result.Value.Localizations, l => Assert.Equal(TranslationStatus.Outdated, l.TranslationStatus));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public async Task Handle_InvalidFullName_ShouldReturnValidationError(string? testName)
    {
        _testUpdatedTeamMember.FullName = testName!;
        SetupDependencies(_testExistingTeamMember);

        var handler = new UpdateTeamMemberHandler(_mockMapper.Object, _mockRepositoryWrapper.Object, _validator, _mockReorderService.Object);

        Result<TeamMemberDto> result = await handler.Handle(
            new UpdateTeamMemberCommand(
                new UpdateTeamMemberDto
                {
                    FullName = testName!,
                    Description = "Updated Description"
                }, _testExistingTeamMember.Id), CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.True(
            result.Errors.Any(e => e.Message == "FullName is required") ||
            result.Errors.Any(e => e.Message == "FullName must be in a valid format"),
            "Expected validation error for FullName");
    }

    [Fact]
    public async Task Handle_InvalidCategoryId_ShouldReturnValidationError()
    {
        _testUpdatedTeamMember.CategoryId = 10000;

        _mockRepositoryWrapper.Setup(x =>
                x.TeamMembersRepository.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<TeamMember>>()))
            .ReturnsAsync(_testExistingTeamMember);
        _mockRepositoryWrapper
            .Setup(x => x.TeamCategoriesRepository.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<TeamCategory>>()))
            .ReturnsAsync((TeamCategory?)null);
        SetupMapper();
        SetupReorderService();

        var handler = new UpdateTeamMemberHandler(_mockMapper.Object, _mockRepositoryWrapper.Object, _validator, _mockReorderService.Object);

        Result<TeamMemberDto> result = await handler.Handle(
            new UpdateTeamMemberCommand(
                new UpdateTeamMemberDto
                {
                    FullName = "testOne",
                    CategoryId = 10000,
                    Description = "Updated Description"
                }, _testExistingTeamMember.Id), CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Contains(ErrorMessagesConstants.NotFound(_testUpdatedTeamMember.CategoryId, typeof(TeamCategory)), result.Errors[0].Message);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(0)]
    public async Task Handle_TeamMemberNotFound_ShouldReturnNotFoundError(long testId)
    {
        SetupDependencies();

        var handler = new UpdateTeamMemberHandler(_mockMapper.Object, _mockRepositoryWrapper.Object, _validator, _mockReorderService.Object);

        Result<TeamMemberDto> result = await handler.Handle(
            new UpdateTeamMemberCommand(
                new UpdateTeamMemberDto
                {
                    FullName = "Updated Name",
                    Description = "Updated Description",
                    CategoryId = _testExistingTeamMember.CategoryId
                }, testId), CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessagesConstants.NotFound(testId, typeof(TeamMember)), result.Errors[0].Message);
    }

    [Fact]
    public async Task Handle_SaveChangesFails_ShouldReturnFailureError()
    {
        SetupDependencies(_testExistingTeamMember, -1);

        var handler = new UpdateTeamMemberHandler(_mockMapper.Object, _mockRepositoryWrapper.Object, _validator, _mockReorderService.Object);

        Result<TeamMemberDto> result = await handler.Handle(
            new UpdateTeamMemberCommand(
                new UpdateTeamMemberDto
                {
                    FullName = "Updated Name",
                    Description = "Updated Description",
                    CategoryId = _testExistingTeamMember.CategoryId
                }, _testExistingTeamMember.Id), CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessagesConstants.FailedToUpdateEntity(typeof(TeamMember)), result.Errors[0].Message);
    }

    [Fact]
    public async Task Handle_ReorderServiceThrowsReorderException_ShouldReturnReorderError()
    {
        var reorderExceptionMessage = "Reorder operation failed";
        var testTeamMemberWithDifferentCategory = new TeamMember
        {
            Id = 1,
            FullName = "Test",
            CategoryId = 2,
            Priority = 1,
            Status = Status.Published,
            Description = "Test description",
            Email = "test@gmail.com"
        };

        _mockRepositoryWrapper.Setup(x =>
                x.TeamMembersRepository.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<TeamMember>>()))
            .ReturnsAsync(testTeamMemberWithDifferentCategory);

        _mockRepositoryWrapper.Setup(x => x.TeamCategoriesRepository.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<TeamCategory>>()))
            .ReturnsAsync(_testCategory);

        _mockReorderService.Setup(r => r.GetNextDisplayOrderAsync(It.IsAny<Expression<Func<TeamMember, bool>>>()))
            .ThrowsAsync(new ReorderException(reorderExceptionMessage));

        SetupMapper();

        var handler = new UpdateTeamMemberHandler(_mockMapper.Object, _mockRepositoryWrapper.Object, _validator, _mockReorderService.Object);

        Result<TeamMemberDto> result = await handler.Handle(
            new UpdateTeamMemberCommand(
                new UpdateTeamMemberDto
                {
                    FullName = "Updated Name",
                    CategoryId = 1,
                    Description = "Updated Description"
                }, testTeamMemberWithDifferentCategory.Id), CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(ReorderConstants.ErrorWithReordering(reorderExceptionMessage), result.Errors[0].Message);
    }

    private void SetupDependencies(TeamMember? teamMemberToReturn = null, int saveResult = 1)
    {
        SetupMapper();
        SetupRepositoryWrapper(teamMemberToReturn, saveResult);
        SetupReorderService();
    }

    private void SetupMapper()
    {
        _mockMapper.Setup(x => x.Map(It.IsAny<UpdateTeamMemberDto>(), It.IsAny<TeamMember>()));

        _mockMapper.Setup(x => x.Map<TeamMember, TeamMemberDto>(It.IsAny<TeamMember>()))
            .Returns(_testUpdatedTeamMemberDto);
    }

    private void SetupReorderService()
    {
        _mockReorderService.Setup(r => r.GetNextDisplayOrderAsync<TeamMember>(It.IsAny<Expression<Func<TeamMember, bool>>>()))
            .ReturnsAsync(1L);

        _mockReorderService.Setup(r => r.RenumberPriorityAsync<TeamMember>(It.IsAny<Expression<Func<TeamMember, bool>>>()))
            .Returns(Task.CompletedTask);
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
            .Setup(x => x.TeamCategoriesRepository.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<TeamCategory>>()))
            .ReturnsAsync(_testCategory);
    }
}
