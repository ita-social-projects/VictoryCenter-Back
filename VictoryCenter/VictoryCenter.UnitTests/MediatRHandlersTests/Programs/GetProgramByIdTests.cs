using AutoMapper;
using Moq;
using VictoryCenter.BLL.DTOs.Programs;
using VictoryCenter.BLL.DTOs.Images;
using VictoryCenter.BLL.Interfaces.BlobStorage;
using VictoryCenter.BLL.Queries.Programs.GetById;
using VictoryCenter.BLL.Constants;
using VictoryCenter.DAL.Enums;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.UnitTests.MediatRHandlersTests.Programs;

public class GetProgramByIdTests
{
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<IRepositoryWrapper> _mockRepositoryWrapper;
    private readonly Mock<IBlobService> _mockBlobService;

    private readonly DAL.Entities.Program _programEntity = new()
    {
        Id = 1,
        Name = "TestName",
        Description = "TestDescription",
        Status = Status.Draft,
        ImageId = 1,
    };

    private readonly ProgramDto _programDto = new()
    {
        Name = "TestName",
        Description = "TestDescription",
        Status = Status.Draft,
        Image = new ImageDTO()
    };

    public GetProgramByIdTests()
    {
        _mapperMock = new Mock<IMapper>();
        _mockRepositoryWrapper = new Mock<IRepositoryWrapper>();
        _mockBlobService = new Mock<IBlobService>();
    }

    [Fact]
    public async Task Handle_ShouldFindProgram()
    {
        SetUpDependencies(_programEntity);
        var handler =
            new GetProgramByIdHandler(_mapperMock.Object, _mockRepositoryWrapper.Object, _mockBlobService.Object);
        var result = await handler.Handle(new GetProgramByIdQuery(_programEntity.Id), CancellationToken.None);
        Assert.True(result.IsSuccess);
        Assert.NotNull(result);
    }

    [Fact]
    public async Task Handle_ShouldFailFindProgram()
    {
        SetUpDependencies();
        var handler =
            new GetProgramByIdHandler(_mapperMock.Object, _mockRepositoryWrapper.Object, _mockBlobService.Object);
        var result = await handler.Handle(new GetProgramByIdQuery(_programEntity.Id), CancellationToken.None);
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessagesConstants.NotFound(_programEntity.Id, typeof(Program)), result.Errors[0].Message);
    }

    private void SetUpDependencies(DAL.Entities.Program program = null)
    {
        SetUpAutoMapper();
        SetUpRepositoryWrapper(program);
        SetUpBlobService();
    }

    private void SetUpAutoMapper()
    {
        _mapperMock.Setup(x => x.Map<ProgramDto>(It.IsAny<DAL.Entities.Program>())).Returns(_programDto);
    }

    private void SetUpRepositoryWrapper(DAL.Entities.Program program)
    {
        _mockRepositoryWrapper.Setup(x => x.ProgramsRepository
            .GetFirstOrDefaultAsync(It.IsAny<QueryOptions<DAL.Entities.Program>>())).ReturnsAsync(program);
    }

    private void SetUpBlobService()
    {
        _mockBlobService
            .Setup(x => x.FindFileInStorageAsBase64Async(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync("mockedBase64");
    }
}
