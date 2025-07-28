using Moq;
using AutoMapper;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Images;
using VictoryCenter.BLL.DTOs.Programs;
using VictoryCenter.BLL.Validators.Programs;
using VictoryCenter.BLL.Interfaces.BlobStorage;
using VictoryCenter.BLL.Commands.Programs.Create;
using VictoryCenter.DAL.Enums;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Options;
using VictoryCenter.DAL.Repositories.Interfaces.Base;

namespace VictoryCenter.UnitTests.MediatRHandlersTests.Programs;

public class CreateProgramTests
{
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<IRepositoryWrapper> _repositoryWrapperMock;
    private readonly CreateProgramValidator _validator;
    private readonly Mock<IBlobService> _blobServiceMock;

    private readonly CreateProgramDto _createProgramDto = new()
    {
        Name = "TestName",
        Description = "TestDescription",
        Status = Status.Draft,
        ImageId = 1,
        CategoriesId = [1, 2]
    };

    private readonly DAL.Entities.Program _programEntity = new()
    {
        Name = "TestName",
        Description = "TestDescription",
        Status = Status.Draft,
        ImageId = 1
    };

    private readonly ProgramDto _programDto = new()
    {
        Name = "TestName",
        Description = "TestDescription",
        Status = Status.Draft,
        Image = new ImageDTO()
    };

    private readonly IEnumerable<ProgramCategory> _programCategories = new List<ProgramCategory>
    {
        new()
        {
            Id = 1,
            Name = "TestCategoryName1"
        },
        new()
        {
            Id = 2,
            Name = "TestCategoryName2"
        }
    };

    private readonly Image _image = new()
    {
        Id = 1,
        BlobName = "BlobName",
        MimeType = "image/png"
    };

    public CreateProgramTests()
    {
        _mapperMock = new Mock<IMapper>();
        _repositoryWrapperMock = new Mock<IRepositoryWrapper>();
        _validator = new CreateProgramValidator();
        _blobServiceMock = new Mock<IBlobService>();
    }

    [Fact]
    public async Task Handle_ShouldCreateProgram()
    {
        SetUpDependencies();
        var handler = new CreateProgramHandler(_mapperMock.Object, _repositoryWrapperMock.Object, _validator, _blobServiceMock.Object);
        var result = await handler.Handle(new CreateProgramCommand(_createProgramDto), CancellationToken.None);
        Assert.True(result.IsSuccess);
        Assert.Equal(result.Value.Name, _programEntity.Name);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public async Task Handle_ShouldFail_InvalidName(string? name)
    {
        _createProgramDto.Name = name;
        _programEntity.Name = name;
        SetUpDependencies();
        var handler = new CreateProgramHandler(_mapperMock.Object, _repositoryWrapperMock.Object, _validator, _blobServiceMock.Object);
        var result = await handler.Handle(new CreateProgramCommand(_createProgramDto), CancellationToken.None);
        Assert.False(result.IsSuccess);
        Assert.Contains("Validation failed", result.Errors[0].Message);
    }

    [Fact]
    public async Task Handle_ShouldFail_SaveChangesFail()
    {
        SetUpDependencies(-1);
        var handler = new CreateProgramHandler(_mapperMock.Object, _repositoryWrapperMock.Object, _validator, _blobServiceMock.Object);
        var result = await handler.Handle(new CreateProgramCommand(_createProgramDto), CancellationToken.None);
        Assert.False(result.IsSuccess);
        Assert.Equal(ProgramConstants.FailedToCreateProgram, result.Errors[0].Message);
    }

    private void SetUpDependencies(int saveResult = 1)
    {
        SetUpAutomapper();
        SetUpBlobService();
        SetUpRepositoryWrapper(saveResult);
    }

    private void SetUpAutomapper()
    {
        _mapperMock.Setup(m => m.Map<DAL.Entities.Program>(It.IsAny<CreateProgramDto>()))
            .Returns(_programEntity);
        _mapperMock.Setup(m => m.Map<ProgramDto>(It.IsAny<DAL.Entities.Program>())).Returns(_programDto);
    }

    private void SetUpBlobService()
    {
        _blobServiceMock
            .Setup(x => x.FindFileInStorageAsBase64Async(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync("mockedBase64");
    }

    private void SetUpRepositoryWrapper(int saveResult)
    {
        _repositoryWrapperMock.Setup(r => r.ProgramCategoriesRepository
            .GetAllAsync(It.IsAny<QueryOptions<ProgramCategory>>())).ReturnsAsync(_programCategories);
        _repositoryWrapperMock.Setup(r => r.ImageRepository
            .GetFirstOrDefaultAsync(It.IsAny<QueryOptions<Image>>())).ReturnsAsync(_image);
        _repositoryWrapperMock.Setup(r => r.ProgramsRepository
            .CreateAsync(It.IsAny<DAL.Entities.Program>()));
        _repositoryWrapperMock.Setup(r => r.SaveChangesAsync()).ReturnsAsync(saveResult);
    }
}
