using AutoMapper;
using FluentResults;
using Moq;
using VictoryCenter.BLL.Commands.Admin.HippotherapyPrograms.Update;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.HippotherapyPrograms;
using VictoryCenter.BLL.Interfaces.BlobStorage;
using VictoryCenter.BLL.Validators.HippotherapyPrograms;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Enums;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.UnitTests.MediatRHandlersTests.HippotherapyPrograms;

public class UpdateHippotherapyProgramTests
{
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<IRepositoryWrapper> _repositoryWrapperMock;
    private readonly Mock<IBlobService> _blobServiceMock;
    private readonly UpdateHippotherapyProgramValidator _validator;

    private readonly UpdateHippotherapyProgramDto _updateProgramDto = new()
    {
        Name = "TestProgramName",
        Description = "TestProgramDescription",
        Status = Status.Published,
        BackgroundImageId = 1,
        PreviewImageId = 2,
        CategoryIds = [1, 2]
    };

    private readonly HippotherapyProgram _programEntity = new()
    {
        Id = 1,
        Name = "TestProgramName",
        Description = "TestProgramDescription",
        Status = Status.Published,
        BackgroundImageId = 1,
        PreviewImageId = 2,
        Categories = []
    };

    private readonly HippotherapyProgramDto _programDto = new()
    {
        Id = 1,
        Name = "TestProgramName",
        Description = "TestProgramDescription",
        Status = Status.Published
    };

    private readonly IEnumerable<HippotherapyProgramCategory> _programCategories = new List<HippotherapyProgramCategory>
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

    public UpdateHippotherapyProgramTests()
    {
        _mapperMock = new Mock<IMapper>();
        _blobServiceMock = new Mock<IBlobService>();
        _repositoryWrapperMock = new Mock<IRepositoryWrapper>();
        _validator = new UpdateHippotherapyProgramValidator(new BaseHippotherapyProgramValidator());
    }

    [Fact]
    public async Task Handle_ShouldUpdateProgram()
    {
        SetUpDependencies(_programEntity);
        var handler = new UpdateHippotherapyProgramHandler(_mapperMock.Object, _repositoryWrapperMock.Object, _validator);
        Result<HippotherapyProgramDto> result = await handler.Handle(new UpdateHippotherapyProgramCommand(_updateProgramDto, 1), CancellationToken.None);
        Assert.True(result.IsSuccess);
        Assert.Equal(result.Value.Name, _updateProgramDto.Name);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public async Task Handle_ShouldFailUpdate_InvalidName(string? name)
    {
        _updateProgramDto.Name = name!;
        SetUpDependencies(_programEntity);
        var handler = new UpdateHippotherapyProgramHandler(_mapperMock.Object, _repositoryWrapperMock.Object, _validator);
        Result<HippotherapyProgramDto> result = await handler.Handle(new UpdateHippotherapyProgramCommand(_updateProgramDto, 1), CancellationToken.None);
        Assert.False(result.IsSuccess);
        Assert.Contains("Validation failed", result.Errors[0].Message);
    }

    [Fact]
    public async Task Handle_ShouldFailUpdate_SaveFail()
    {
        SetUpDependencies(_programEntity, -1);
        var handler = new UpdateHippotherapyProgramHandler(_mapperMock.Object, _repositoryWrapperMock.Object, _validator);
        Result<HippotherapyProgramDto> result = await handler.Handle(new UpdateHippotherapyProgramCommand(_updateProgramDto, 1), CancellationToken.None);
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessagesConstants.FailedToUpdateEntity(typeof(HippotherapyProgram)), result.Errors[0].Message);
    }

    [Fact]
    public async Task Handle_ShouldFailUpdate_NotFoundProgram()
    {
        SetUpDependencies();
        var handler = new UpdateHippotherapyProgramHandler(_mapperMock.Object, _repositoryWrapperMock.Object, _validator);
        Result<HippotherapyProgramDto> result = await handler.Handle(new UpdateHippotherapyProgramCommand(_updateProgramDto, 1), CancellationToken.None);
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessagesConstants.NotFound(1, typeof(HippotherapyProgram)), result.Errors[0].Message);
    }

    private void SetUpDependencies(HippotherapyProgram programEntity = null!, int saveResult = 1)
    {
        SetUpAutomapper();
        SetUpBlobService();
        SetUpRepositoryWrapper(saveResult, programEntity);
    }

    private void SetUpAutomapper()
    {
        _mapperMock.Setup(m => m.Map(It.IsAny<UpdateHippotherapyProgramDto>(), It.IsAny<HippotherapyProgram>()))
            .Returns(_programEntity);
        _mapperMock.Setup(m => m.Map<HippotherapyProgramDto>(It.IsAny<HippotherapyProgram>())).Returns(_programDto);
    }

    private void SetUpBlobService()
    {
        _blobServiceMock
            .Setup(x => x.GetFileUrl(It.IsAny<string>(), It.IsAny<string>()))
            .Returns("https://localhost:5000/supersecretimage.png");
    }

    private void SetUpRepositoryWrapper(int saveResult, HippotherapyProgram programEntity)
    {
        _repositoryWrapperMock.Setup(r => r.HippotherapyProgramsRepository
            .GetFirstOrDefaultAsync(It.IsAny<QueryOptions<HippotherapyProgram>>())).ReturnsAsync(programEntity);
        _repositoryWrapperMock.Setup(r => r.HippotherapyProgramCategoriesRepository
            .GetAllAsync(It.IsAny<QueryOptions<HippotherapyProgramCategory>>())).ReturnsAsync(_programCategories);
        _repositoryWrapperMock.Setup(r => r.ImageRepository
            .GetFirstOrDefaultAsync(It.IsAny<QueryOptions<Image>>())).ReturnsAsync(_image);
        _repositoryWrapperMock.Setup(r => r.HippotherapyProgramsRepository.Update(It.IsAny<HippotherapyProgram>()));
        _repositoryWrapperMock.Setup(r => r.SaveChangesAsync()).ReturnsAsync(saveResult);
    }
}
