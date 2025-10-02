using AutoMapper;
using FluentResults;
using FluentValidation;
using Moq;
using VictoryCenter.BLL.Commands.Admin.HypotherapyProgramCategories.Update;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.HypotherapyProgramCategories;
using VictoryCenter.BLL.Validators.HypotherapyProgramCategories;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.UnitTests.MediatRHandlersTests.HypotherapyProgramCategories;

public class UpdateHypotherapyProgramCategoryTests
{
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<IRepositoryWrapper> _repositoryWrapperMock;
    private readonly IValidator<UpdateHypotherapyProgramCategoryCommand> _validator;

    private readonly UpdateHypotherapyProgramCategoryDto _updateProgramCategoryDto = new()
    {
        Name = "TestName1"
    };

    private readonly HypotherapyProgramCategory _programCategoryEntity = new()
    {
        Id = 1,
        Name = "Test1",
        CreatedAt = DateTimeOffset.UtcNow.AddMinutes(-10)
    };

    private readonly HypotherapyProgramCategoryDto _programCategoryDto = new()
    {
        Id = 1,
        Name = "TestName1",
        CreatedAt = DateTimeOffset.UtcNow.AddMinutes(-10)
    };

    public UpdateHypotherapyProgramCategoryTests()
    {
        _mockMapper = new Mock<IMapper>();
        _repositoryWrapperMock = new Mock<IRepositoryWrapper>();
        _validator = new UpdateHypotherapyProgramCategoryValidator();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public async Task Handle_ShouldFail_InvalidName(string? name)
    {
        var testEntity = new HypotherapyProgramCategory
         {
            Id = 1,
            Name = name!,
            CreatedAt = DateTimeOffset.UtcNow.AddMinutes(-10)
         };
        var testDto = new HypotherapyProgramCategoryDto
        {
            Id = 1,
            Name = name!,
            CreatedAt = DateTimeOffset.UtcNow.AddMinutes(-10)
        };
        SetupDependencies(testEntity, testDto);
        var handler = new UpdateHypotherapyProgramCategoryHandler(_mockMapper.Object, _repositoryWrapperMock.Object, _validator);

        Result<HypotherapyProgramCategoryDto> result = await handler
            .Handle(new UpdateHypotherapyProgramCategoryCommand(new UpdateHypotherapyProgramCategoryDto { Name = name! }, 1), CancellationToken.None);
        Assert.False(result.IsSuccess);
        Assert.NotEmpty(result.Errors);
    }

    [Fact]
    public async Task Handle_ShouldFail_SaveChangesFails()
    {
        SetupDependencies(null, null, -1);
        var handler = new UpdateHypotherapyProgramCategoryHandler(_mockMapper.Object, _repositoryWrapperMock.Object, _validator);
        Result<HypotherapyProgramCategoryDto> result = await handler
            .Handle(new UpdateHypotherapyProgramCategoryCommand(_updateProgramCategoryDto, 1), CancellationToken.None);
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessagesConstants.FailedToUpdateEntity(typeof(HypotherapyProgramCategory)), result.Errors[0].Message);
    }

    [Fact]
    public async Task Handle_ShouldUpdateProgramCategory()
    {
        SetupDependencies();
        var handler = new UpdateHypotherapyProgramCategoryHandler(_mockMapper.Object, _repositoryWrapperMock.Object, _validator);
        Result<HypotherapyProgramCategoryDto> result = await handler.Handle(new UpdateHypotherapyProgramCategoryCommand(_updateProgramCategoryDto, 1), CancellationToken.None);
        Assert.True(result.IsSuccess);
        Assert.Equal(result.Value.Name, _programCategoryDto.Name);
    }

    private void SetupDependencies(HypotherapyProgramCategory? entity = null, HypotherapyProgramCategoryDto? dto = null, int saveResult = 1)
    {
        var programCategoryEntity = entity ?? _programCategoryEntity;
        var programCategoryDto = dto ?? _programCategoryDto;

        SetUpAutomapper(programCategoryEntity, programCategoryDto);
        SetUpRepositoryWrapper(saveResult);
    }

    private void SetUpAutomapper(HypotherapyProgramCategory outputProgramCategoryEntity, HypotherapyProgramCategoryDto outputProgramCategoryDto)
    {
        _mockMapper.Setup(m => m.Map<HypotherapyProgramCategoryDto>(It.IsAny<HypotherapyProgramCategory>()))
            .Returns(outputProgramCategoryDto);
        _mockMapper.Setup(m => m.Map(
                It.IsAny<UpdateHypotherapyProgramCategoryDto>(),
                It.IsAny<HypotherapyProgramCategory>()))
            .Returns(outputProgramCategoryEntity);
    }

    private void SetUpRepositoryWrapper(int saveResult)
    {
        _repositoryWrapperMock.Setup(repo => repo.HypotherapyProgramCategoriesRepository
            .Update(It.IsAny<HypotherapyProgramCategory>()));
        _repositoryWrapperMock.Setup(repo => repo.SaveChangesAsync()).ReturnsAsync(saveResult);
        _repositoryWrapperMock.Setup(repo => repo.HypotherapyProgramCategoriesRepository
                .GetFirstOrDefaultAsync(It.IsAny<QueryOptions<HypotherapyProgramCategory>>()))
                .ReturnsAsync(_programCategoryEntity);
    }
}
