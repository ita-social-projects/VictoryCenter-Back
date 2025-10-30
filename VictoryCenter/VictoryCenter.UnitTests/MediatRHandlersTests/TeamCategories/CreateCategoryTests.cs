using AutoMapper;
using FluentValidation;
using Moq;
using VictoryCenter.BLL.Commands.Admin.TeamCategories.Create;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.TeamCategories;
using VictoryCenter.BLL.Validators.TeamCategories;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.Base;

namespace VictoryCenter.UnitTests.MediatRHandlersTests.TeamCategories;

public class CreateTeamCategoryTests
{
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<IRepositoryWrapper> _repositoryWrapperMock;
    private readonly IValidator<CreateTeamCategoryCommand> _validator;

    private readonly TeamCategory _testEntity = new()
    {
        Id = 1,
        Name = "Test Category",
        Description = "Test Category Description",
        CreatedAt = new DateTimeOffset(2025, 1, 1, 12, 0, 0, TimeZoneInfo.Local.BaseUtcOffset),
    };

    private TeamCategoryDto _testCategoryDto = new()
    {
        Name = "Test Category",
        Description = "Test Category Description",
    };

    public CreateTeamCategoryTests()
    {
        _mapperMock = new Mock<IMapper>();
        _repositoryWrapperMock = new Mock<IRepositoryWrapper>();
        _validator = new CreateTeamCategoryValidator();
    }

    [Theory]
    [InlineData("Test Category Description")]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public async Task Handle_ShouldCreateCategory(string? description)
    {
        _testEntity.Description = description;
        _testCategoryDto = _testCategoryDto with
        {
            Description = description
        };
        SetupDependencies();
        var handler = new CreateTeamCategoryHandler(_mapperMock.Object, _repositoryWrapperMock.Object, _validator);

        var result = await handler.Handle(
            new CreateTeamCategoryCommand(new CreateTeamCategoryDto
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
        _testEntity.Name = name!;
        _testCategoryDto = _testCategoryDto with
        {
            Name = name!
        };
        SetupDependencies();
        var handler = new CreateTeamCategoryHandler(_mapperMock.Object, _repositoryWrapperMock.Object, _validator);

        var result = await handler.Handle(
            new CreateTeamCategoryCommand(new CreateTeamCategoryDto
            {
                Name = name!,
                Description = "Test Category Description",
            }), CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Contains(ErrorMessagesConstants.PropertyIsRequired(nameof(Category.Name)), result.Errors[0].Message);
    }

    [Fact]
    public async Task Handle_ShouldFail_SaveChangesFails()
    {
        SetupDependencies(-1);
        var handler = new CreateTeamCategoryHandler(_mapperMock.Object, _repositoryWrapperMock.Object, _validator);

        var result = await handler.Handle(
            new CreateTeamCategoryCommand(new CreateTeamCategoryDto
            {
                Name = "Test Category",
                Description = "Test Category Description",
            }), CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessagesConstants.FailedToCreateEntity(typeof(TeamCategory)), result.Errors[0].Message);
    }

    private void SetupDependencies(int saveResult = 1)
    {
        SetupMapper(_testEntity, _testCategoryDto);
        SetupRepositoryWrapper(saveResult);
    }

    private void SetupMapper(TeamCategory outputCategoryEntity, TeamCategoryDto outputCategoryDto)
    {
        _mapperMock.Setup(m => m.Map<TeamCategory>(It.IsAny<CreateTeamCategoryDto>()))
            .Returns(outputCategoryEntity);
        _mapperMock.Setup(m => m.Map<TeamCategoryDto>(It.IsAny<TeamCategory>()))
            .Returns(outputCategoryDto);
    }

    private void SetupRepositoryWrapper(int saveResult)
    {
        _repositoryWrapperMock.Setup(repo => repo.TeamCategoriesRepository.CreateAsync(It.IsAny<TeamCategory>()));
        _repositoryWrapperMock.Setup(repo => repo.SaveChangesAsync()).ReturnsAsync(saveResult);
    }
}
