using AutoMapper;
using FluentResults;
using FluentValidation;
using Moq;
using VictoryCenter.BLL.Commands.Admin.HippotherapyProgramCategories.Update;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.HippotherapyProgramCategories;
using VictoryCenter.BLL.Validators.HippotherapyProgramCategories;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.UnitTests.MediatRHandlersTests.HippotherapyProgramCategories;

public class UpdateHippotherapyProgramCategoryTests
{
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<IRepositoryWrapper> _repositoryWrapperMock;
    private readonly IValidator<UpdateHippotherapyProgramCategoryCommand> _validator;

    private readonly UpdateHippotherapyProgramCategoryDto _updateProgramCategoryDto = new()
    {
        Name = "TestName1"
    };

    private readonly HippotherapyProgramCategory _programCategoryEntity = new()
    {
        Id = 1,
        Name = "Test1",
        CreatedAt = DateTimeOffset.UtcNow.AddMinutes(-10)
    };

    private readonly HippotherapyProgramCategoryDto _programCategoryDto = new()
    {
        Id = 1,
        Name = "TestName1",
        CreatedAt = DateTimeOffset.UtcNow.AddMinutes(-10)
    };

    public UpdateHippotherapyProgramCategoryTests()
    {
        _mockMapper = new Mock<IMapper>();
        _repositoryWrapperMock = new Mock<IRepositoryWrapper>();
        _validator = new UpdateHippotherapyProgramCategoryValidator();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public async Task Handle_ShouldFail_InvalidName(string? name)
    {
        var testEntity = new HippotherapyProgramCategory
        {
            Id = 1,
            Name = name!,
            CreatedAt = DateTimeOffset.UtcNow.AddMinutes(-10)
        };
        var testDto = new HippotherapyProgramCategoryDto
        {
            Id = 1,
            Name = name!,
            CreatedAt = DateTimeOffset.UtcNow.AddMinutes(-10)
        };
        SetupDependencies(testEntity, testDto);
        var handler = new UpdateHippotherapyProgramCategoryHandler(_mockMapper.Object, _repositoryWrapperMock.Object, _validator);

        Result<HippotherapyProgramCategoryDto> result = await handler
            .Handle(new UpdateHippotherapyProgramCategoryCommand(new UpdateHippotherapyProgramCategoryDto { Name = name! }, 1), CancellationToken.None);
        Assert.False(result.IsSuccess);
        Assert.NotEmpty(result.Errors);
    }

    [Fact]
    public async Task Handle_ShouldFail_SaveChangesFails()
    {
        SetupDependencies(null, null, -1);
        var handler = new UpdateHippotherapyProgramCategoryHandler(_mockMapper.Object, _repositoryWrapperMock.Object, _validator);
        Result<HippotherapyProgramCategoryDto> result = await handler
            .Handle(new UpdateHippotherapyProgramCategoryCommand(_updateProgramCategoryDto, 1), CancellationToken.None);
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessagesConstants.FailedToUpdateEntity(typeof(HippotherapyProgramCategory)), result.Errors[0].Message);
    }

    [Fact]
    public async Task Handle_ShouldUpdateProgramCategory()
    {
        SetupDependencies();
        var handler = new UpdateHippotherapyProgramCategoryHandler(_mockMapper.Object, _repositoryWrapperMock.Object, _validator);
        Result<HippotherapyProgramCategoryDto> result = await handler.Handle(new UpdateHippotherapyProgramCategoryCommand(_updateProgramCategoryDto, 1), CancellationToken.None);
        Assert.True(result.IsSuccess);
        Assert.Equal(result.Value.Name, _programCategoryDto.Name);
    }

    private void SetupDependencies(HippotherapyProgramCategory? entity = null, HippotherapyProgramCategoryDto? dto = null, int saveResult = 1)
    {
        var programCategoryEntity = entity ?? _programCategoryEntity;
        var programCategoryDto = dto ?? _programCategoryDto;

        SetUpAutomapper(programCategoryEntity, programCategoryDto);
        SetUpRepositoryWrapper(saveResult);
    }

    private void SetUpAutomapper(HippotherapyProgramCategory outputProgramCategoryEntity, HippotherapyProgramCategoryDto outputProgramCategoryDto)
    {
        _mockMapper.Setup(m => m.Map<HippotherapyProgramCategoryDto>(It.IsAny<HippotherapyProgramCategory>()))
            .Returns(outputProgramCategoryDto);
        _mockMapper.Setup(m => m.Map(
                It.IsAny<UpdateHippotherapyProgramCategoryDto>(),
                It.IsAny<HippotherapyProgramCategory>()))
            .Returns(outputProgramCategoryEntity);
    }

    private void SetUpRepositoryWrapper(int saveResult)
    {
        _repositoryWrapperMock.Setup(repo => repo.HippotherapyProgramCategoriesRepository
            .Update(It.IsAny<HippotherapyProgramCategory>()));
        _repositoryWrapperMock.Setup(repo => repo.SaveChangesAsync()).ReturnsAsync(saveResult);
        _repositoryWrapperMock.Setup(repo => repo.HippotherapyProgramCategoriesRepository
                .GetFirstOrDefaultAsync(It.IsAny<QueryOptions<HippotherapyProgramCategory>>()))
                .ReturnsAsync(_programCategoryEntity);
    }
}
