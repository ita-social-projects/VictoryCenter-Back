using Moq;
using AutoMapper;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.Commands.Programs.Update;
using VictoryCenter.BLL.DTOs.Images;
using VictoryCenter.BLL.DTOs.Programs;
using VictoryCenter.BLL.Interfaces.BlobStorage;
using VictoryCenter.BLL.Validators.Programs;
using VictoryCenter.DAL.Enums;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.UnitTests.MediatRHandlersTests.Programs;

public class UpdateProgramTests
{
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<IRepositoryWrapper> _repositoryWrapperMock;
    private readonly Mock<IBlobService> _blobServiceMock;
    private readonly UpdateProgramValidator _validator;

    private readonly UpdateProgramDto _updateProgramDto = new()
    {
        Id = 1,
        Name = "TestProgramName",
        Description = "TestProgramDescription",
        Status = Status.Published,
        ImageId = 1,
        CategoriesId = [1, 2, 3]
    };

    private readonly DAL.Entities.Program _programEntity = new()
    {
        Id = 1,
        Name = "TestProgramName",
        Description = "TestProgramDescription",
        Status = Status.Published,
        ImageId = 1,
    };

    private readonly ProgramDto _programDto = new()
    {
        Id = 1,
        Name = "TestProgramName",
        Description = "TestProgramDescription",
        Status = Status.Published,
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

    public UpdateProgramTests()
    {
        _mapperMock = new Mock<IMapper>();
        _blobServiceMock = new Mock<IBlobService>();
        _repositoryWrapperMock = new Mock<IRepositoryWrapper>();
        _validator = new UpdateProgramValidator();
    }

    [Fact]
    public async Task Handle_ShouldUpdateProgram()
    {
        SetUpDependencies(_programEntity);
        var handler = new UpdateProgramHandler(_mapperMock.Object, _repositoryWrapperMock.Object, _validator, _blobServiceMock.Object);
        var result = await handler.Handle(new UpdateProgramCommand(_updateProgramDto), CancellationToken.None);
        Assert.True(result.IsSuccess);
        Assert.Equal(result.Value.Name, _updateProgramDto.Name);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public async Task Handle_ShouldFailUpdate_InvalidName(string? name)
    {
        _updateProgramDto.Name = name;
        SetUpDependencies(_programEntity);
        var handler = new UpdateProgramHandler(_mapperMock.Object, _repositoryWrapperMock.Object, _validator, _blobServiceMock.Object);
        var result = await handler.Handle(new UpdateProgramCommand(_updateProgramDto), CancellationToken.None);
        Assert.False(result.IsSuccess);
        Assert.Contains("Validation failed", result.Errors[0].Message);
    }

    [Fact]
    public async Task Handle_ShouldFailUpdate_SaveFail()
    {
        SetUpDependencies(_programEntity, -1);
        var handler = new UpdateProgramHandler(_mapperMock.Object, _repositoryWrapperMock.Object, _validator, _blobServiceMock.Object);
        var result = await handler.Handle(new UpdateProgramCommand(_updateProgramDto), CancellationToken.None);
        Assert.False(result.IsSuccess);
        Assert.Equal(ProgramConstants.FailedToUpdateProgram, result.Errors[0].Message);
    }

    [Fact]
    public async Task Handle_ShouldFailUpdate_NotFoundProgram()
    {
        SetUpDependencies();
        var handler = new UpdateProgramHandler(_mapperMock.Object, _repositoryWrapperMock.Object, _validator, _blobServiceMock.Object);
        var result = await handler.Handle(new UpdateProgramCommand(_updateProgramDto), CancellationToken.None);
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessagesConstants.NotFound(_updateProgramDto.Id, typeof(DAL.Entities.Program)), result.Errors[0].Message);
    }

    private void SetUpDependencies(DAL.Entities.Program programEntity = null, int saveResult = 1)
    {
        SetUpAutomapper();
        SetUpBlobService();
        SetUpRepositoryWrapper(saveResult, programEntity);
    }

    private void SetUpAutomapper()
    {
        _mapperMock.Setup(m => m.Map(It.IsAny<UpdateProgramDto>(), It.IsAny<DAL.Entities.Program>()))
            .Returns(_programEntity);
        _mapperMock.Setup(m => m.Map<ProgramDto>(It.IsAny<DAL.Entities.Program>())).Returns(_programDto);
    }

    private void SetUpBlobService()
    {
        _blobServiceMock
            .Setup(x => x.FindFileInStorageAsBase64Async(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync("mockedBase64");
    }

    private void SetUpRepositoryWrapper(int saveResult, DAL.Entities.Program programEntity)
    {
        _repositoryWrapperMock.Setup(r => r.ProgramsRepository
            .GetFirstOrDefaultAsync(It.IsAny<QueryOptions<DAL.Entities.Program>>())).ReturnsAsync(programEntity);
        _repositoryWrapperMock.Setup(r => r.ProgramCategoriesRepository
            .GetAllAsync(It.IsAny<QueryOptions<ProgramCategory>>())).ReturnsAsync(_programCategories);
        _repositoryWrapperMock.Setup(r => r.ImageRepository
            .GetFirstOrDefaultAsync(It.IsAny<QueryOptions<Image>>())).ReturnsAsync(_image);
        _repositoryWrapperMock.Setup(r => r.ProgramsRepository.Update(It.IsAny<DAL.Entities.Program>()));
        _repositoryWrapperMock.Setup(r => r.SaveChangesAsync()).ReturnsAsync(saveResult);
    }
}
