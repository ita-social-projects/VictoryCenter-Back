using AutoMapper;
using FluentResults;
using FluentValidation;
using Moq;
using VictoryCenter.BLL.Commands.Admin.HippotherapyProgramCategories.Create;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.HippotherapyProgramCategories;
using VictoryCenter.BLL.DTOs.Common;
using VictoryCenter.BLL.Validators.HippotherapyProgramCategories;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.Base;

namespace VictoryCenter.UnitTests.MediatRHandlersTests.HippotherapyProgramCategories;

public class CreateHippotherapyProgramCategoryTests
{
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<IRepositoryWrapper> _repositoryWrapperMock;
    private readonly IValidator<CreateHippotherapyProgramCategoryCommand> _validatorMock;

    private readonly HippotherapyProgramCategory _program = new()
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

    private readonly HippotherapyProgramCategoryDto _programCategoryDto = new()
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

    public CreateHippotherapyProgramCategoryTests()
    {
        _mapperMock = new Mock<IMapper>();
        _validatorMock = new CreateHippotherapyProgramCategoryValidator();
        _repositoryWrapperMock = new Mock<IRepositoryWrapper>();
    }

    [Fact]
    public async Task Handle_ShouldCreateProgramCategory()
    {
        SetupDependencies();
        var handler = new CreateHippotherapyProgramCategoryHandler(_mapperMock.Object, _repositoryWrapperMock.Object, _validatorMock);
        Result<HippotherapyProgramCategoryDto> result = await handler
            .Handle(new CreateHippotherapyProgramCategoryCommand(new CreateHippotherapyProgramCategoryDto { Name = "TestCategory" }), CancellationToken.None);
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
        var handler = new CreateHippotherapyProgramCategoryHandler(_mapperMock.Object, _repositoryWrapperMock.Object, _validatorMock);

        Result<HippotherapyProgramCategoryDto> result = await handler
            .Handle(new CreateHippotherapyProgramCategoryCommand(new CreateHippotherapyProgramCategoryDto { Name = name! }), CancellationToken.None);
        Assert.False(result.IsSuccess);
        Assert.NotEmpty(result.Errors);
    }

    [Fact]
    public async Task Handle_ShouldFail_SaveChangesFails()
    {
        SetupDependencies(-1);
        var handler = new CreateHippotherapyProgramCategoryHandler(_mapperMock.Object, _repositoryWrapperMock.Object, _validatorMock);
        Result<HippotherapyProgramCategoryDto> result = await handler
            .Handle(new CreateHippotherapyProgramCategoryCommand(new CreateHippotherapyProgramCategoryDto { Name = "TestName" }), CancellationToken.None);
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessagesConstants.FailedToCreateEntity(typeof(HippotherapyProgramCategory)), result.Errors[0].Message);
    }

    private void SetupDependencies(int saveResult = 1)
    {
        SetUpAutomapper(_program, _programCategoryDto);
        SetupRepositoryWrapper(saveResult, CancellationToken.None);
    }

    private void SetUpAutomapper(HippotherapyProgramCategory outputProgramCategoryEntity, HippotherapyProgramCategoryDto outputProgramCategoryDto)
    {
        _mapperMock.Setup(m => m.Map<HippotherapyProgramCategory>(It.IsAny<CreateHippotherapyProgramCategoryDto>()))
            .Returns(outputProgramCategoryEntity);
        _mapperMock.Setup(m => m.Map<HippotherapyProgramCategoryDto>(It.IsAny<HippotherapyProgramCategory>()))
            .Returns(outputProgramCategoryDto);
    }

    private void SetupRepositoryWrapper(int saveResult, CancellationToken cancellationToken)
    {
        _repositoryWrapperMock.Setup(repo => repo.HippotherapyProgramCategoriesRepository
            .CreateAsync(It.IsAny<HippotherapyProgramCategory>(), cancellationToken));
        _repositoryWrapperMock.Setup(repo => repo.SaveChangesAsync()).ReturnsAsync(saveResult);
    }
}
