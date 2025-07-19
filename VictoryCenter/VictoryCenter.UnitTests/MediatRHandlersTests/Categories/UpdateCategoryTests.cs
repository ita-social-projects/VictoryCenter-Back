using AutoMapper;
using FluentValidation;
using Moq;
using VictoryCenter.BLL.Commands.Admin.Categories.Update;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.Categories;
using VictoryCenter.BLL.Validators.Categories;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.UnitTests.MediatRHandlersTests.Categories;

public class UpdateCategoryTests
{
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<IRepositoryWrapper> _mockRepositoryWrapper;
    private readonly IValidator<UpdateCategoryCommand> _validator;

    private readonly Category _testExistingCategory = new ()
    {
        Id = 1,
        Name = "Test",
        Description = "Test description",
    };

    private readonly Category _testUpdatedCategory = new ()
    {
        Id = 1,
        Name = "Updated Name",
        Description = "Updated Description",
    };

    private readonly CategoryDto _testUpdatedCategoryDto = new ()
    {
        Name = "Updated Name",
        Description = "Updated Description",
    };

    public UpdateCategoryTests()
    {
        _mockMapper = new Mock<IMapper>();
        _mockRepositoryWrapper = new Mock<IRepositoryWrapper>();
        _validator = new UpdateCategoryValidator();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("Updated description")]
    public async Task Handle_ShouldUpdateEntity(string? testDescription)
    {
        _testUpdatedCategory.Description = testDescription;
        _testUpdatedCategoryDto.Description = testDescription;
        SetupDependencies(_testExistingCategory);
        var handler = new UpdateCategoryHandler(_mockMapper.Object, _mockRepositoryWrapper.Object, _validator);

        var result = await handler.Handle(
            new UpdateCategoryCommand(new UpdateCategoryDto
        {
            Id = _testExistingCategory.Id,
            Name = "Updated Name",
            Description = testDescription,
        }), CancellationToken.None);

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
        _testUpdatedCategoryDto.Name = testName;
        _testUpdatedCategory.Name = testName;
        SetupDependencies(_testExistingCategory);
        var handler = new UpdateCategoryHandler(_mockMapper.Object, _mockRepositoryWrapper.Object, _validator);

        var result = await handler.Handle(
            new UpdateCategoryCommand(new UpdateCategoryDto
        {
            Id = _testExistingCategory.Id,
            Name = testName,
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
        var handler = new UpdateCategoryHandler(_mockMapper.Object, _mockRepositoryWrapper.Object, _validator);

        var result = await handler.Handle(
            new UpdateCategoryCommand(new UpdateCategoryDto
        {
            Id = testId,
            Name = "Updated Name",
            Description = "Updated Description",
        }), CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessagesConstants.NotFound(testId, typeof(Category)), result.Errors[0].Message);
    }

    [Fact]
    public async Task Handle_ShouldNotUpdateEntity_SaveChangesFails()
    {
        SetupDependencies(_testExistingCategory, -1);
        var handler = new UpdateCategoryHandler(_mockMapper.Object, _mockRepositoryWrapper.Object, _validator);

        var result = await handler.Handle(
            new UpdateCategoryCommand(new UpdateCategoryDto
        {
            Id = _testExistingCategory.Id,
            Name = "Updated Name",
            Description = "Updated Description",
        }), CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessagesConstants.FailedToUpdateEntity(typeof(Category)), result.Errors[0].Message);
    }

    private void SetupDependencies(Category? categoryToReturn = null, int saveResult = 1)
    {
        SetupMapper();
        SetupRepositoryWrapper(categoryToReturn, saveResult);
    }

    private void SetupMapper()
    {
        _mockMapper.Setup(x => x.Map<UpdateCategoryDto, Category>(It.IsAny<UpdateCategoryDto>()))
            .Returns(_testUpdatedCategory);

        _mockMapper.Setup(x => x.Map<Category, CategoryDto>(It.IsAny<Category>()))
            .Returns(_testUpdatedCategoryDto);
    }

    private void SetupRepositoryWrapper(Category? categoryToReturn = null, int saveResult = 1)
    {
        _mockRepositoryWrapper.Setup(x => x.CategoriesRepository.GetFirstOrDefaultAsync(
                It.IsAny<QueryOptions<Category>>()))
            .ReturnsAsync(categoryToReturn);

        _mockRepositoryWrapper.Setup(x => x.SaveChangesAsync())
            .ReturnsAsync(saveResult);
    }
}
