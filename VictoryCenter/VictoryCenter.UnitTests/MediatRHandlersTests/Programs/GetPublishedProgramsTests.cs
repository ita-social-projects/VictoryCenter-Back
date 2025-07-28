using Moq;
using AutoMapper;
using VictoryCenter.BLL.DTOs.Images;
using VictoryCenter.BLL.DTOs.Programs;
using VictoryCenter.BLL.Interfaces.BlobStorage;
using VictoryCenter.DAL.Enums;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;
namespace VictoryCenter.UnitTests.MediatRHandlersTests.Programs;

public class GetPublishedProgramsTests
{
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<IRepositoryWrapper> _mockRepositoryWrapper;
    private readonly Mock<IBlobService> _mockBlobService;

    private readonly List<DAL.Entities.Program> _programEntities = new()
    {
        new()
        {
            Id = 1,
            Name = "TestName1",
            Description = "TestDescription1",
            Status = Status.Published,
            ImageId = 1
        },
        new()
        {
            Id = 2,
            Name = "TestName2",
            Description = "TestDescription2",
            Status = Status.Published,
            ImageId = 2,
        }
    };

    private readonly Image _image = new()
    {
        Id = 1,
        BlobName = "BlobName",
        MimeType = "image/png"
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

    private readonly IEnumerable<PublishedProgramDto> _programDto = new List<PublishedProgramDto>()
    {
        new()
        {
            Name = "TestName1",
            Description = "TestDescription1",
            Image = new ImageDTO()
        },
        new()
        {
            Name = "TestName2",
            Description = "TestDescription2",
            Image = new ImageDTO()
        }
    };

    public GetPublishedProgramsTests()
    {
        _mapperMock = new Mock<IMapper>();
        _mockRepositoryWrapper = new Mock<IRepositoryWrapper>();
        _mockBlobService = new Mock<IBlobService>();
    }

    private void SetUpDependencies(DAL.Entities.Program program = null)
    {
        SetUpAutoMapper();
        SetUpRepositoryWrapper(program);
        SetUpBlobService();
    }

    private void SetUpAutoMapper()
    {
        _mapperMock.Setup(x => x.Map<IEnumerable<PublishedProgramDto>>(It.IsAny<IEnumerable<DAL.Entities.Program>>()))
            .Returns(_programDto);
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
