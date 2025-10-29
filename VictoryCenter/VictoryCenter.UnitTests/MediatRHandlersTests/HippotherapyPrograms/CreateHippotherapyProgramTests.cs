using AutoMapper;
using FluentResults;
using Moq;
using VictoryCenter.BLL.Commands.Admin.HippotherapyPrograms.Create;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.HippotherapyPrograms;
using VictoryCenter.BLL.DTOs.Common;
using VictoryCenter.BLL.Interfaces.BlobStorage;
using VictoryCenter.BLL.Validators.HippotherapyPrograms;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Enums;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.UnitTests.MediatRHandlersTests.HippotherapyPrograms;

public class CreateHippotherapyProgramTests
{
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<IRepositoryWrapper> _repositoryWrapperMock;
    private readonly CreateHippotherapyProgramValidator _validator;
    private readonly Mock<IBlobService> _blobServiceMock;

    private readonly CreateHippotherapyProgramDto _createProgramDto = new()
    {
        Name = "TestName",
        Description = "TestDescription",
        Status = Status.Draft,
        ImageId = 1,
        CategoryIds = [1, 2]
    };

    private readonly DAL.Entities.HippotherapyProgram _programEntity = new()
    {
        Name = "TestName",
        Description = "TestDescription",
        Status = Status.Draft,
        ImageId = 1
    };

    private readonly HippotherapyProgramDto _programDto = new()
    {
        Name = "TestName",
        Description = "TestDescription",
        Status = Status.Draft,
        Image = new ImageDto()
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

    public CreateHippotherapyProgramTests()
    {
        _mapperMock = new Mock<IMapper>();
        _repositoryWrapperMock = new Mock<IRepositoryWrapper>();
        _validator = new CreateHippotherapyProgramValidator(new BaseHippotherapyProgramValidator());
        _blobServiceMock = new Mock<IBlobService>();
    }

    [Fact]
    public async Task Handle_ShouldCreateProgram()
    {
        SetUpDependencies();
        var handler = new CreateHippotherapyProgramHandler(_mapperMock.Object, _repositoryWrapperMock.Object, _validator);
        Result<HippotherapyProgramDto> result = await handler.Handle(new CreateHippotherapyProgramCommand(_createProgramDto), CancellationToken.None);
        Assert.True(result.IsSuccess);
        Assert.Equal(result.Value.Name, _programEntity.Name);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public async Task Handle_ShouldFail_InvalidName(string? name)
    {
        _createProgramDto.Name = name!;
        _programEntity.Name = name!;
        SetUpDependencies();
        var handler = new CreateHippotherapyProgramHandler(_mapperMock.Object, _repositoryWrapperMock.Object, _validator);
        Result<HippotherapyProgramDto> result = await handler.Handle(new CreateHippotherapyProgramCommand(_createProgramDto), CancellationToken.None);
        Assert.False(result.IsSuccess);
        Assert.Contains("Validation failed", result.Errors[0].Message);
    }

    [Fact]
    public async Task Handle_ShouldFail_SaveChangesFail()
    {
        SetUpDependencies(-1);
        var handler = new CreateHippotherapyProgramHandler(_mapperMock.Object, _repositoryWrapperMock.Object, _validator);
        Result<HippotherapyProgramDto> result = await handler.Handle(new CreateHippotherapyProgramCommand(_createProgramDto), CancellationToken.None);
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessagesConstants.FailedToCreateEntity(typeof(HippotherapyProgram)), result.Errors[0].Message);
    }

    private void SetUpDependencies(int saveResult = 1)
    {
        SetUpAutomapper();
        SetUpBlobService();
        SetUpRepositoryWrapper(saveResult);
    }

    private void SetUpAutomapper()
    {
        _mapperMock.Setup(m => m.Map<DAL.Entities.HippotherapyProgram>(It.IsAny<CreateHippotherapyProgramDto>()))
            .Returns(_programEntity);
        _mapperMock.Setup(m => m.Map<HippotherapyProgramDto>(It.IsAny<DAL.Entities.HippotherapyProgram>())).Returns(_programDto);
    }

    private void SetUpBlobService()
    {
        _blobServiceMock
            .Setup(x => x.GetFileUrl(It.IsAny<string>(), It.IsAny<string>()))
            .Returns("https://localhost:5000/supersecretimage.png");
    }

    private void SetUpRepositoryWrapper(int saveResult)
    {
        _repositoryWrapperMock.Setup(r => r.HippotherapyProgramCategoriesRepository
            .GetAllAsync(It.IsAny<QueryOptions<HippotherapyProgramCategory>>())).ReturnsAsync(_programCategories);
        _repositoryWrapperMock.Setup(r => r.ImageRepository
            .GetFirstOrDefaultAsync(It.IsAny<QueryOptions<Image>>())).ReturnsAsync(_image);
        _repositoryWrapperMock.Setup(r => r.HippotherapyProgramsRepository
            .CreateAsync(It.IsAny<DAL.Entities.HippotherapyProgram>()));
        _repositoryWrapperMock.Setup(r => r.SaveChangesAsync()).ReturnsAsync(saveResult);
    }
}
