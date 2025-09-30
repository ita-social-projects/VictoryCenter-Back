using AutoMapper;
using FluentResults;
using FluentValidation;
using Moq;
using VictoryCenter.BLL.Commands.Admin.HypotherapyProgramCategories.Create;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.HypotherapyProgramCategories;
using VictoryCenter.BLL.DTOs.Common;
using VictoryCenter.BLL.Validators.HypotherapyProgramCategories;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.Base;

namespace VictoryCenter.UnitTests.MediatRHandlersTests.HypotherapyProgramCategories;

public class CreateHypotherapyProgramCategoryTests
{
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<IRepositoryWrapper> _repositoryWrapperMock;
    private readonly IValidator<CreateHypotherapyProgramCategoryCommand> _validatorMock;

    private readonly HypotherapyProgramCategory _program = new()
    {
        Id = 1,
        Name = "TestCategory",
        CreatedAt = DateTimeOffset.UtcNow.AddMinutes(-10),
        Programs =
        [
            new()
            {
                Image = new Image
                {
                    BlobName = "someBlob.jpg",
                    MimeType = "image/jpeg"
                }
            },
        ]
    };

    private readonly HypotherapyProgramCategoryDto _programCategoryDto = new()
    {
        Id = 1,
        Name = "TestCategory",
        CreatedAt = DateTimeOffset.UtcNow.AddMinutes(-10),
        Programs =
        [
            new()
            {
                Image = new ImageDto
                {
                    BlobName = "someBlob.jpg",
                    MimeType = "image/jpeg"
                }
            }

        ]
    };

    public CreateHypotherapyProgramCategoryTests()
    {
        _mapperMock = new Mock<IMapper>();
        _validatorMock = new CreateHypotherapyProgramCategoryValidator();
        _repositoryWrapperMock = new Mock<IRepositoryWrapper>();
    }

    [Fact]
    public async Task Handle_ShouldCreateProgramCategory()
    {
        SetupDependencies();
        var handler = new CreateHypotherapyProgramCategoryHandler(_mapperMock.Object, _repositoryWrapperMock.Object, _validatorMock);
        Result<HypotherapyProgramCategoryDto> result = await handler
            .Handle(new CreateHypotherapyProgramCategoryCommand(new CreateHypotherapyProgramCategoryDto { Name = "TestCategory" }), CancellationToken.None);
        Assert.True(result.IsSuccess);
        Assert.Equal(result.Value.Name, _programCategoryDto.Name);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public async Task Handle_ShouldFail_InvalidName(string? name)
    {
        _program.Name = name!;
        _programCategoryDto.Name = name!;
        SetupDependencies();
        var handler = new CreateHypotherapyProgramCategoryHandler(_mapperMock.Object, _repositoryWrapperMock.Object, _validatorMock);

        Result<HypotherapyProgramCategoryDto> result = await handler
            .Handle(new CreateHypotherapyProgramCategoryCommand(new CreateHypotherapyProgramCategoryDto { Name = name! }), CancellationToken.None);
        Assert.False(result.IsSuccess);
        Assert.NotEmpty(result.Errors);
    }

    [Fact]
    public async Task Handle_ShouldFail_SaveChangesFails()
    {
        SetupDependencies(-1);
        var handler = new CreateHypotherapyProgramCategoryHandler(_mapperMock.Object, _repositoryWrapperMock.Object, _validatorMock);
        Result<HypotherapyProgramCategoryDto> result = await handler
            .Handle(new CreateHypotherapyProgramCategoryCommand(new CreateHypotherapyProgramCategoryDto { Name = "TestName" }), CancellationToken.None);
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessagesConstants.FailedToCreateEntity(typeof(HypotherapyProgramCategory)), result.Errors[0].Message);
    }

    private void SetupDependencies(int saveResult = 1)
    {
        SetUpAutomapper(_program, _programCategoryDto);
        SetupRepositoryWrapper(saveResult);
    }

    private void SetUpAutomapper(HypotherapyProgramCategory outputProgramCategoryEntity, HypotherapyProgramCategoryDto outputProgramCategoryDto)
    {
        _mapperMock.Setup(m => m.Map<HypotherapyProgramCategory>(It.IsAny<CreateHypotherapyProgramCategoryDto>()))
            .Returns(outputProgramCategoryEntity);
        _mapperMock.Setup(m => m.Map<HypotherapyProgramCategoryDto>(It.IsAny<HypotherapyProgramCategory>()))
            .Returns(outputProgramCategoryDto);
    }

    private void SetupRepositoryWrapper(int saveResult)
    {
        _repositoryWrapperMock.Setup(repo => repo.HypotherapyProgramCategoriesRepository
            .CreateAsync(It.IsAny<HypotherapyProgramCategory>()));
        _repositoryWrapperMock.Setup(repo => repo.SaveChangesAsync()).ReturnsAsync(saveResult);
    }
}
