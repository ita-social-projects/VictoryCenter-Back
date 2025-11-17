using AutoMapper;
using FluentValidation;
using Moq;
using VictoryCenter.BLL.Commands.Admin.TeamCategories.Update;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.TeamCategories;
using VictoryCenter.BLL.Validators.TeamCategories;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.UnitTests.MediatRHandlersTests.TeamCategories;

public class UpdateTeamCategoryTests
{
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<IRepositoryWrapper> _mockRepositoryWrapper;
    private readonly IValidator<UpdateTeamCategoryCommand> _validator;

    private readonly TeamCategory _testExistingCategory = new()
    {
        Id = 1,
        Name = "Test",
        Description = "Test description",
    };

    private readonly TeamCategory _testUpdatedCategory = new()
    {
        Id = 1,
        Name = "Updated Name",
        Description = "Updated Description",
    };

    private TeamCategoryDto _testUpdatedCategoryDto = new()
    {
        Name = "Updated Name",
        Description = "Updated Description",
    };

    public UpdateTeamCategoryTests()
    {
        _mockMapper = new Mock<IMapper>();
        _mockRepositoryWrapper = new Mock<IRepositoryWrapper>();
        _validator = new UpdateTeamCategoryValidator(new BaseTeamCategoryValidator());
    }

    [Fact]
    public async Task Handle_ShouldUpdateEntity()
    {
        SetupDependencies(_testExistingCategory);
        var handler = new UpdateTeamCategoryHandler(_mockMapper.Object, _mockRepositoryWrapper.Object, _validator);

        var result = await handler.Handle(
            new UpdateTeamCategoryCommand(
                new UpdateTeamCategoryDto
                {
                    Name = "Updated Name",
                    Description = "Updated Description",
                },
                _testExistingCategory.Id), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(_testUpdatedCategoryDto.Name, result.Value.Name);
        Assert.Equal(_testUpdatedCategoryDto.Description, result.Value.Description);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public async Task Handle_ShouldNotUpdateEntity_IncorrectName(string? testName)
    {
        _testUpdatedCategoryDto = _testUpdatedCategoryDto with
        {
            Name = testName!
        };
        _testUpdatedCategory.Name = testName!;
        SetupDependencies(_testExistingCategory);
        var handler = new UpdateTeamCategoryHandler(_mockMapper.Object, _mockRepositoryWrapper.Object, _validator);

        var result = await handler.Handle(
            new UpdateTeamCategoryCommand(
                new UpdateTeamCategoryDto
                {
                    Name = testName!,
                    Description = "Updated Description",
                }, _testExistingCategory.Id), CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Contains("Validation failed", result.Errors[0].Message);
    }

    [Fact]
    public async Task Handle_ShouldNotUpdateEntity_DuplicateName()
    {
        var duplicateCategory = new TeamCategory
        {
            Id = 2,
            Name = "Updated Name",
            Description = "Some other category"
        };

        SetupDependencies(_testExistingCategory, duplicateCategory: duplicateCategory);
        var handler = new UpdateTeamCategoryHandler(_mockMapper.Object, _mockRepositoryWrapper.Object, _validator);

        var result = await handler.Handle(
            new UpdateTeamCategoryCommand(
                new UpdateTeamCategoryDto
                {
                    Name = "Updated Name",
                    Description = "Updated Description",
                }, _testExistingCategory.Id), CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(TeamCategoryConstants.DuplicateCategoryName, result.Errors[0].Message);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(0)]
    public async Task Handle_ShouldNotUpdateEntity_NotFound(long testId)
    {
        SetupDependencies();
        var handler = new UpdateTeamCategoryHandler(_mockMapper.Object, _mockRepositoryWrapper.Object, _validator);

        var result = await handler.Handle(
            new UpdateTeamCategoryCommand(
                new UpdateTeamCategoryDto
                {
                    Name = "Updated Name",
                    Description = "Updated Description",
                }, testId), CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessagesConstants.NotFound(testId, typeof(TeamCategory)), result.Errors[0].Message);
    }

    [Fact]
    public async Task Handle_ShouldNotUpdateEntity_SaveChangesFails()
    {
        SetupDependencies(_testExistingCategory, -1);
        var handler = new UpdateTeamCategoryHandler(_mockMapper.Object, _mockRepositoryWrapper.Object, _validator);

        var result = await handler.Handle(
            new UpdateTeamCategoryCommand(
                new UpdateTeamCategoryDto
                {
                    Name = "Updated Name",
                    Description = "Updated Description",
                }, _testExistingCategory.Id), CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessagesConstants.FailedToUpdateEntity(typeof(TeamCategory)), result.Errors[0].Message);
    }

    private void SetupDependencies(TeamCategory? categoryToReturn = null, int saveResult = 1, TeamCategory? duplicateCategory = null)
    {
        SetupMapper();
        SetupRepositoryWrapper(categoryToReturn, saveResult, duplicateCategory);
    }

    private void SetupMapper()
    {
        _mockMapper.Setup(x => x.Map<UpdateTeamCategoryDto, TeamCategory>(It.IsAny<UpdateTeamCategoryDto>()))
            .Returns(_testUpdatedCategory);

        _mockMapper.Setup(x => x.Map<TeamCategory, TeamCategoryDto>(It.IsAny<TeamCategory>()))
            .Returns(_testUpdatedCategoryDto);
    }

    private void SetupRepositoryWrapper(TeamCategory? categoryToReturn = null, int saveResult = 1, TeamCategory? duplicateCategory = null)
    {
        var entityWithSameNameDifferentId = new TeamCategory
        {
            Id = 999,
            Name = "Updated Name",
            Description = "Some other description"
        };

        var entityWithSameId = new TeamCategory
        {
            Id = _testExistingCategory.Id,
            Name = "Different Name",
            Description = "Different description"
        };

        _mockRepositoryWrapper.Setup(x => x.TeamCategoriesRepository.GetFirstOrDefaultAsync(
                It.Is<QueryOptions<TeamCategory>>(q =>
                    q.Filter != null &&
                    q.Include == null &&
                    q.Filter.Compile()(entityWithSameNameDifferentId) &&
                    !q.Filter.Compile()(entityWithSameId))))
            .ReturnsAsync(duplicateCategory);

        _mockRepositoryWrapper.Setup(x => x.TeamCategoriesRepository.GetFirstOrDefaultAsync(
                It.Is<QueryOptions<TeamCategory>>(q =>
                    q.Filter != null &&
                    q.Include == null &&
                    q.Filter.Compile()(entityWithSameId) &&
                    !q.Filter.Compile()(entityWithSameNameDifferentId))))
            .ReturnsAsync(categoryToReturn);

        _mockRepositoryWrapper.Setup(x => x.TeamCategoriesRepository.GetFirstOrDefaultAsync(
                It.Is<QueryOptions<TeamCategory>>(q => q.Include != null)))
            .ReturnsAsync(categoryToReturn ?? _testUpdatedCategory);

        _mockRepositoryWrapper.Setup(x => x.SaveChangesAsync())
            .ReturnsAsync(saveResult);
    }
}
