using Moq;
using AutoMapper;
using FluentResults;
using FluentValidation;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.ProgramCategories;
using VictoryCenter.BLL.Commands.ProgramCategories.Update;
using VictoryCenter.BLL.Validators.ProgramCategories;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Options;
using VictoryCenter.DAL.Repositories.Interfaces.Base;

namespace VictoryCenter.UnitTests.MediatRHandlersTests.ProgramCategories;

public class UpdateProgramCategoryTests
{
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<IRepositoryWrapper> _repositoryWrapperMock;
    private readonly IValidator<UpdateProgramCategoryCommand> _validator;

    private readonly UpdateProgramCategoryDto _updateProgramCategoryDto = new()
    {
        Id = 1,
        Name = "TestName1"
    };

    private readonly ProgramCategory _programCategoryEntity = new()
    {
        Id = 1,
        Name = "Test1",
        CreatedAt = DateTime.UtcNow.AddMinutes(-10)
    };

    private readonly ProgramCategoryDto _programCategoryDto = new()
    {
        Id = 1,
        Name = "TestName1",
        CreatedAt = DateTime.UtcNow.AddMinutes(-10)
    };

    public UpdateProgramCategoryTests()
    {
        _mockMapper = new Mock<IMapper>();
        _repositoryWrapperMock = new Mock<IRepositoryWrapper>();
        _validator = new UpdateProgramCategoryValidator();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public async Task Handle_ShouldFail_InvalidName(string? name)
    {
        _programCategoryEntity.Name = name!;
        _programCategoryDto.Name = name!;
        SetupDependencies();
        var handler = new UpdateProgramCategoryHandler(_mockMapper.Object, _repositoryWrapperMock.Object, _validator);

        Result<ProgramCategoryDto> result = await handler
            .Handle(new UpdateProgramCategoryCommand(new UpdateProgramCategoryDto { Name = name! }), CancellationToken.None);
        Assert.False(result.IsSuccess);
        Assert.Contains("Validation failed", result.Errors[0].Message);
    }

    [Fact]
    public async Task Handle_ShouldFail_SaveChangesFails()
    {
        SetupDependencies(-1);
        var handler = new UpdateProgramCategoryHandler(_mockMapper.Object, _repositoryWrapperMock.Object, _validator);
        Result<ProgramCategoryDto> result = await handler
            .Handle(new UpdateProgramCategoryCommand(_updateProgramCategoryDto), CancellationToken.None);
        Assert.False(result.IsSuccess);
        Assert.Equal(ProgramCategoryConstants.FailedToUpdateCategory, result.Errors[0].Message);
    }

    [Fact]
    public async Task Handle_ShouldUpdateProgramCategory()
    {
        SetupDependencies();
        var handler = new UpdateProgramCategoryHandler(_mockMapper.Object, _repositoryWrapperMock.Object, _validator);
        Result<ProgramCategoryDto> result = await handler.Handle(new UpdateProgramCategoryCommand(_updateProgramCategoryDto), CancellationToken.None);
        Assert.True(result.IsSuccess);
        Assert.Equal(result.Value.Name, _programCategoryDto.Name);
    }

    private void SetupDependencies(int saveResult = 1)
    {
        SetUpAutomapper(_programCategoryEntity, _programCategoryDto);
        SetUpRepositoryWrapper(saveResult);
    }

    private void SetUpAutomapper(ProgramCategory outputProgramCategoryEntity, ProgramCategoryDto outputProgramCategoryDto)
    {
        _mockMapper.Setup(m => m.Map<ProgramCategoryDto>(It.IsAny<ProgramCategory>()))
            .Returns(outputProgramCategoryDto);
        _mockMapper.Setup(m => m.Map(
                It.IsAny<UpdateProgramCategoryDto>(),
                It.IsAny<ProgramCategory>()))
            .Returns(outputProgramCategoryEntity);
    }

    private void SetUpRepositoryWrapper(int saveResult)
    {
        _repositoryWrapperMock.Setup(repo => repo.ProgramCategoriesRepository
            .Update(It.IsAny<ProgramCategory>()));
        _repositoryWrapperMock.Setup(repo => repo.SaveChangesAsync()).ReturnsAsync(saveResult);
        _repositoryWrapperMock.Setup(repo => repo.ProgramCategoriesRepository
                .GetFirstOrDefaultAsync(It.IsAny<QueryOptions<ProgramCategory>>()))
                .ReturnsAsync(_programCategoryEntity);
    }
}
