using Moq;
using AutoMapper;
using FluentValidation;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.Commands.ProgramCategories.Create;
using VictoryCenter.BLL.DTOs.ProgramCategories;
using VictoryCenter.BLL.DTOs.Programs;
using VictoryCenter.BLL.DTOs.Images;
using VictoryCenter.BLL.Validators.ProgramCategories;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Entities;

namespace VictoryCenter.UnitTests.MediatRHandlersTests.ProgramCategories;

public class CreateProgramCategoryTests
{
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<IRepositoryWrapper> _repositoryWrapperMock;
    private readonly IValidator<CreateProgramCategoryCommand> _validatorMock;

    private readonly ProgramCategory _program = new()
    {
        Id = 1,
        Name = "TestCategory",
        CreatedAt = DateTime.UtcNow.AddMinutes(-10),
        Programs = new List<DAL.Entities.Program>
        {
            new()
            {
                Image = new Image
                {
                    BlobName = "someblob.jpg",
                    MimeType = "image/jpeg"
                }
            }
        }
    };

    private readonly ProgramCategoryDto _programCategoryDto = new()
    {
        Id = 1,
        Name = "TestCategory",
        CreatedAt = DateTime.UtcNow.AddMinutes(-10),
        Programs = new List<ProgramDto>
        {
            new()
            {
                Image = new ImageDTO
                {
                    BlobName = "someblob.jpg",
                    MimeType = "image/jpeg"
                }
            }
        }
    };

    public CreateProgramCategoryTests()
    {
        _mapperMock = new Mock<IMapper>();
        _validatorMock = new CreateProgramCategoryValidator();
        _repositoryWrapperMock = new Mock<IRepositoryWrapper>();
    }

    [Fact]
    public async Task Handle_ShouldCreateProgramCategory()
    {
        SetupDependencies();
        var handler = new CreateProgramCategoryHandler(_mapperMock.Object, _repositoryWrapperMock.Object, _validatorMock);
        var result = await handler
            .Handle(new CreateProgramCategoryCommand(new CreateProgramCategoryDto { Name = "TestCategory" }), CancellationToken.None);
        Assert.True(result.IsSuccess);
        Assert.Equal(result.Value.Name, _programCategoryDto.Name);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public async Task Handle_ShouldFail_InvalidName(string? name)
    {
        _program.Name = name;
        _programCategoryDto.Name = name;
        SetupDependencies();
        var handler = new CreateProgramCategoryHandler(_mapperMock.Object, _repositoryWrapperMock.Object, _validatorMock);

        var result = await handler
            .Handle(new CreateProgramCategoryCommand(new CreateProgramCategoryDto { Name = name }), CancellationToken.None);
        Assert.False(result.IsSuccess);
        Assert.Contains("Validation failed", result.Errors[0].Message);
    }

    [Fact]
    public async Task Handle_ShouldFail_SaveChangesFails()
    {
        SetupDependencies(-1);
        var handler = new CreateProgramCategoryHandler(_mapperMock.Object, _repositoryWrapperMock.Object, _validatorMock);
        var result = await handler
            .Handle(new CreateProgramCategoryCommand(new CreateProgramCategoryDto { Name = "TestName" }), CancellationToken.None);
        Assert.False(result.IsSuccess);
        Assert.Equal(ProgramCategoryConstants.FailedToCreateCategory, result.Errors[0].Message);
    }

    private void SetupDependencies(int saveResult = 1)
    {
        SetUpAutomapper(_program, _programCategoryDto);
        SetupRepositoryWrapper(saveResult);
    }

    private void SetUpAutomapper(ProgramCategory outputProgramCategoryEntity, ProgramCategoryDto outputProgramCategoryDto)
    {
        _mapperMock.Setup(m => m.Map<ProgramCategory>(It.IsAny<CreateProgramCategoryDto>()))
            .Returns(outputProgramCategoryEntity);
        _mapperMock.Setup(m => m.Map<ProgramCategoryDto>(It.IsAny<ProgramCategory>()))
            .Returns(outputProgramCategoryDto);
    }

    private void SetupRepositoryWrapper(int saveResult)
    {
        _repositoryWrapperMock.Setup(repo => repo.ProgramCategoriesRepository
            .CreateAsync(It.IsAny<ProgramCategory>()));
        _repositoryWrapperMock.Setup(repo => repo.SaveChangesAsync()).ReturnsAsync(saveResult);
    }
}
