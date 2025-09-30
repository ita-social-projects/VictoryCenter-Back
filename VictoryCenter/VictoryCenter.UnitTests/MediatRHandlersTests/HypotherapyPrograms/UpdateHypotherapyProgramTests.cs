using AutoMapper;
using FluentResults;
using Moq;
using VictoryCenter.BLL.Commands.Admin.HypotherapyPrograms.Update;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.HypotherapyPrograms;
using VictoryCenter.BLL.DTOs.Common;
using VictoryCenter.BLL.Interfaces.BlobStorage;
using VictoryCenter.BLL.Validators.HypotherapyPrograms;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Enums;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.UnitTests.MediatRHandlersTests.HypotherapyPrograms;

public class UpdateHypotherapyProgramTests
{
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<IRepositoryWrapper> _repositoryWrapperMock;
    private readonly Mock<IBlobService> _blobServiceMock;
    private readonly UpdateHypotherapyProgramValidator _validator;

    private readonly HypotherapyUpdateProgramDto _updateProgramDto = new()
    {
        Name = "TestProgramName",
        Description = "TestProgramDescription",
        Status = Status.Published,
        ImageId = 1,
        CategoryIds = [1, 2, 3]
    };

    private readonly DAL.Entities.HypotherapyProgram _programEntity = new()
    {
        Id = 1,
        Name = "TestProgramName",
        Description = "TestProgramDescription",
        Status = Status.Published,
        ImageId = 1,
    };

    private readonly HypotherapyProgramDto _programDto = new()
    {
        Id = 1,
        Name = "TestProgramName",
        Description = "TestProgramDescription",
        Status = Status.Published,
        Image = new ImageDto()
    };

    private readonly IEnumerable<HypotherapyProgramCategory> _programCategories = new List<HypotherapyProgramCategory>
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

    public UpdateHypotherapyProgramTests()
    {
        _mapperMock = new Mock<IMapper>();
        _blobServiceMock = new Mock<IBlobService>();
        _repositoryWrapperMock = new Mock<IRepositoryWrapper>();
        _validator = new UpdateHypotherapyProgramValidator(new BaseHypotherapyProgramValidator());
    }

    [Fact]
    public async Task Handle_ShouldUpdateProgram()
    {
        SetUpDependencies(_programEntity);
        var handler = new UpdateHypotherapyProgramHandler(_mapperMock.Object, _repositoryWrapperMock.Object, _validator, _blobServiceMock.Object);
        Result<HypotherapyProgramDto> result = await handler.Handle(new UpdateHypotherapyProgramCommand(_updateProgramDto, 1), CancellationToken.None);
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
        var handler = new UpdateHypotherapyProgramHandler(_mapperMock.Object, _repositoryWrapperMock.Object, _validator, _blobServiceMock.Object);
        Result<HypotherapyProgramDto> result = await handler.Handle(new UpdateHypotherapyProgramCommand(_updateProgramDto, 1), CancellationToken.None);
        Assert.False(result.IsSuccess);
        Assert.Contains("Validation failed", result.Errors[0].Message);
    }

    [Fact]
    public async Task Handle_ShouldFailUpdate_SaveFail()
    {
        SetUpDependencies(_programEntity, -1);
        var handler = new UpdateHypotherapyProgramHandler(_mapperMock.Object, _repositoryWrapperMock.Object, _validator, _blobServiceMock.Object);
        Result<HypotherapyProgramDto> result = await handler.Handle(new UpdateHypotherapyProgramCommand(_updateProgramDto, 1), CancellationToken.None);
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessagesConstants.FailedToUpdateEntity(typeof(HypotherapyProgram)), result.Errors[0].Message);
    }

    [Fact]
    public async Task Handle_ShouldFailUpdate_NotFoundProgram()
    {
        SetUpDependencies();
        var handler = new UpdateHypotherapyProgramHandler(_mapperMock.Object, _repositoryWrapperMock.Object, _validator, _blobServiceMock.Object);
        Result<HypotherapyProgramDto> result = await handler.Handle(new UpdateHypotherapyProgramCommand(_updateProgramDto, 1), CancellationToken.None);
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessagesConstants.NotFound(1, typeof(DAL.Entities.HypotherapyProgram)), result.Errors[0].Message);
    }

    private void SetUpDependencies(DAL.Entities.HypotherapyProgram programEntity = null!, int saveResult = 1)
    {
        SetUpAutomapper();
        SetUpBlobService();
        SetUpRepositoryWrapper(saveResult, programEntity);
    }

    private void SetUpAutomapper()
    {
        _mapperMock.Setup(m => m.Map(It.IsAny<HypotherapyUpdateProgramDto>(), It.IsAny<DAL.Entities.HypotherapyProgram>()))
            .Returns(_programEntity);
        _mapperMock.Setup(m => m.Map<HypotherapyProgramDto>(It.IsAny<DAL.Entities.HypotherapyProgram>())).Returns(_programDto);
    }

    private void SetUpBlobService()
    {
        _blobServiceMock
            .Setup(x => x.GetFileUrl(It.IsAny<string>(), It.IsAny<string>()))
            .Returns("https://localhost:5000/supersecretimage.png");
    }

    private void SetUpRepositoryWrapper(int saveResult, DAL.Entities.HypotherapyProgram programEntity)
    {
        _repositoryWrapperMock.Setup(r => r.HypotherapyProgramsRepository
            .GetFirstOrDefaultAsync(It.IsAny<QueryOptions<DAL.Entities.HypotherapyProgram>>())).ReturnsAsync(programEntity);
        _repositoryWrapperMock.Setup(r => r.HypotherapyProgramCategoriesRepository
            .GetAllAsync(It.IsAny<QueryOptions<HypotherapyProgramCategory>>())).ReturnsAsync(_programCategories);
        _repositoryWrapperMock.Setup(r => r.ImageRepository
            .GetFirstOrDefaultAsync(It.IsAny<QueryOptions<Image>>())).ReturnsAsync(_image);
        _repositoryWrapperMock.Setup(r => r.HypotherapyProgramsRepository.Update(It.IsAny<DAL.Entities.HypotherapyProgram>()));
        _repositoryWrapperMock.Setup(r => r.SaveChangesAsync()).ReturnsAsync(saveResult);
    }
}
