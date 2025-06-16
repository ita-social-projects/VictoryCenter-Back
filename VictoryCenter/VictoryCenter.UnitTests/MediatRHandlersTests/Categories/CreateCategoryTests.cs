using AutoMapper;
using FluentValidation;
using Moq;
using VictoryCenter.BLL.Commands.Categories.Create;
using VictoryCenter.BLL.DTOs.Categories;
using VictoryCenter.BLL.Validators.Categories;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.Base;

namespace VictoryCenter.UnitTests.MediatRHandlersTests.Categories;

public class CreateCategoryTests
{
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<IRepositoryWrapper> _repositoryWrapperMock;
    private readonly IValidator<CreateCategoryCommand> _validator;

    private readonly Category _testEntity = new()
    {
        Id = 1,
        Name = "Test Category",
        Description = "Test Category Description",
        CreatedAt = new DateTime(2025, 1, 1, 12, 0, 0, DateTimeKind.Local),
    };

    private readonly CategoryDto _testCategoryDto = new()
    {
        Name = "Test Category",
        Description = "Test Category Description",
        CreatedAt = new DateTime(2025, 1, 1, 12, 0, 0, DateTimeKind.Local),
    };

    public CreateCategoryTests()
    {
        _mapperMock = new Mock<IMapper>();
        _repositoryWrapperMock = new Mock<IRepositoryWrapper>();
        _validator = new CreateCategoryValidator();
    }

    [Theory]
    [InlineData("Test Category Description")]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public async Task Handle_ShouldCreateCategory(string? description)
    {
        _testEntity.Description = description;
        _testCategoryDto.Description = description;
        SetupDependencies();
        var handler = new CreateCategoryHandler(_mapperMock.Object, _repositoryWrapperMock.Object, _validator);

        var result = await handler.Handle(
            new CreateCategoryCommand(new CreateCategoryDto
        {
            Name = "Test Category",
            Description = description,
        }), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(result.Value.Name, _testCategoryDto.Name);
        Assert.Equal(result.Value.Description, _testCategoryDto.Description);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public async Task Handle_ShouldFail_InvalidName(string? name)
    {
        _testEntity.Name = name;
        _testCategoryDto.Name = name;
        SetupDependencies();
        var handler = new CreateCategoryHandler(_mapperMock.Object, _repositoryWrapperMock.Object, _validator);

        var result = await handler.Handle(
            new CreateCategoryCommand(new CreateCategoryDto
        {
            Name = name,
            Description = "Test Category Description",
        }), CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Contains("Validation failed", result.Errors[0].Message);
    }

    [Fact]
    public async Task Handle_ShouldFail_SaveChangesFails()
    {
        SetupDependencies(-1);
        var handler = new CreateCategoryHandler(_mapperMock.Object, _repositoryWrapperMock.Object, _validator);

        var result = await handler.Handle(
            new CreateCategoryCommand(new CreateCategoryDto
        {
            Name = "Test Category",
            Description = "Test Category Description",
        }), CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal("Failed to create category", result.Errors[0].Message);
    }

    private void SetupDependencies(int saveResult = 1)
    {
        SetupMapper(_testEntity, _testCategoryDto);
        SetupRepositoryWrapper(saveResult);
    }

    private void SetupMapper(Category outputCategoryEntity, CategoryDto outputCategoryDto)
    {
        _mapperMock.Setup(m => m.Map<Category>(It.IsAny<CreateCategoryDto>()))
            .Returns(outputCategoryEntity);
        _mapperMock.Setup(m => m.Map<CategoryDto>(It.IsAny<Category>()))
            .Returns(outputCategoryDto);
    }

    private void SetupRepositoryWrapper(int saveResult)
    {
        _repositoryWrapperMock.Setup(repo => repo.CategoriesRepository.CreateAsync(It.IsAny<Category>()));
        _repositoryWrapperMock.Setup(repo => repo.SaveChangesAsync()).ReturnsAsync(saveResult);
    }
}
