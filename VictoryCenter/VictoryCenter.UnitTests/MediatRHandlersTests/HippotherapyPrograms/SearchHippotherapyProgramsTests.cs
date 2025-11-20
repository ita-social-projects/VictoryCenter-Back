using AutoMapper;
using FluentValidation;
using Moq;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.HippotherapyPrograms;
using VictoryCenter.BLL.DTOs.Common;
using VictoryCenter.BLL.Interfaces.Search;
using VictoryCenter.BLL.Queries.Admin.HippotherapyPrograms.Search;
using VictoryCenter.BLL.Validators.HippotherapyPrograms;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Enums;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.UnitTests.MediatRHandlersTests.HippotherapyPrograms;

public class SearchHippotherapyProgramsTests
{
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<IRepositoryWrapper> _repositoryWrapperMock;
    private readonly Mock<ISearchService<HippotherapyProgram>> _searchServiceMock;
    private readonly IValidator<SearchHippotherapyProgramsQuery> _validator;

    private readonly List<HippotherapyProgram> _hippotherapyPrograms =
    [
        CreateHippotherapyProgram(1, "TestProgram", Status.Draft)
    ];

    private readonly List<HippotherapyProgramDto> _hippotherapyProgramDtos =
    [
        CreateHippotherapyProgramDto(1, "TestProgram", Status.Draft)
    ];

    public SearchHippotherapyProgramsTests()
    {
        _mapperMock = new Mock<IMapper>();
        _repositoryWrapperMock = new Mock<IRepositoryWrapper>();
        _validator = new SearchHippotherapyProgramValidator();
        _searchServiceMock = new Mock<ISearchService<HippotherapyProgram>>();
    }

    [Fact]
    public async Task Handle_ExistingSearchQuery_ShouldReturnNotEmpty()
    {
        // Arrange
        SetupMapper(_hippotherapyProgramDtos);
        SetupRepositoryWrapper(_hippotherapyPrograms);
        var dto = new SearchHippotherapyProgramDto { SearchQuery = "TestProgram" };
        var query = new SearchHippotherapyProgramsQuery(dto);
        var handler = new SearchHippotherapyProgramsHandler(_mapperMock.Object, _repositoryWrapperMock.Object, _validator, _searchServiceMock.Object);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(_hippotherapyProgramDtos, result.Value.Items);
    }

    [Fact]
    public async Task Handle_NonexistentSearchQuery_ShouldReturnEmpty()
    {
        // Arrange
        SetupMapper([]);
        SetupRepositoryWrapper([]);
        var dto = new SearchHippotherapyProgramDto { SearchQuery = "Nonexistent program" };
        var query = new SearchHippotherapyProgramsQuery(dto);
        var handler = new SearchHippotherapyProgramsHandler(_mapperMock.Object, _repositoryWrapperMock.Object, _validator, _searchServiceMock.Object);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Empty(result.Value.Items);
    }

    [Fact]
    public async Task Handle_InvalidSearchQuery_ShouldReturnValidationError()
    {
        // Arrange
        SetupMapper([]);
        SetupRepositoryWrapper([]);
        var dto = new SearchHippotherapyProgramDto { SearchQuery = "" };
        var query = new SearchHippotherapyProgramsQuery(dto);
        var handler = new SearchHippotherapyProgramsHandler(_mapperMock.Object, _repositoryWrapperMock.Object, _validator, _searchServiceMock.Object);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Contains(ErrorMessagesConstants.PropertyIsRequired(nameof(SearchHippotherapyProgramDto.SearchQuery)), result.Errors[0].Message);
    }

    [Fact]
    public async Task Handle_WithImage_ShouldReturnImage()
    {
        // Arrange
        var image = CreateImage(10, "blob.jpg", "https://cdn.example.com/blob.jpg");
        var program = CreateHippotherapyProgram(2, "With Image", Status.Published, image);
        var imageDto = CreateImageDto(10, "blob.jpg", "https://cdn.example.com/blob.jpg", image.CreatedAt);
        var dto = CreateHippotherapyProgramDto(2, "With Image", Status.Published, imageDto);

        SetupMapper([dto]);
        SetupRepositoryWrapper([program]);

        var searchDto = new SearchHippotherapyProgramDto { SearchQuery = "With Image" };
        var query = new SearchHippotherapyProgramsQuery(searchDto);
        var handler = new SearchHippotherapyProgramsHandler(_mapperMock.Object, _repositoryWrapperMock.Object, _validator, _searchServiceMock.Object);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(Assert.Single(result.Value!.Items).Image);
    }

    [Fact]
    public async Task Handle_WithoutImage_ShouldReturnNullImage()
    {
        // Arrange
        var program = CreateHippotherapyProgram(3, "No Image", Status.Published);
        var dto = CreateHippotherapyProgramDto(3, "No Image", Status.Published);

        SetupMapper([dto]);
        SetupRepositoryWrapper([program]);

        var searchDto = new SearchHippotherapyProgramDto { SearchQuery = "No Image" };
        var query = new SearchHippotherapyProgramsQuery(searchDto);
        var handler = new SearchHippotherapyProgramsHandler(_mapperMock.Object, _repositoryWrapperMock.Object, _validator, _searchServiceMock.Object);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.Null(Assert.Single(result.Value!.Items).Image);
    }

    private static HippotherapyProgram CreateHippotherapyProgram(int id, string name, Status status, Image? image = null)
    {
        return new HippotherapyProgram
        {
            Id = id,
            Name = name,
            Status = status,
            Description = "desc",
            ImageId = image?.Id,
            Image = image,
            Categories = [],
            CreatedAt = DateTimeOffset.UtcNow
        };
    }

    private static HippotherapyProgramDto CreateHippotherapyProgramDto(int id, string name, Status status, ImageDto? image = null)
    {
        return new HippotherapyProgramDto
        {
            Id = id,
            Name = name,
            Status = status,
            Description = "desc",
            Image = image,
            Categories = []
        };
    }

    private static Image CreateImage(int id, string blobName, string url)
    {
        return new Image
        {
            Id = id,
            BlobName = blobName,
            Url = url,
            MimeType = "image/jpeg",
            CreatedAt = DateTimeOffset.UtcNow.AddHours(-1)
        };
    }

    private static ImageDto CreateImageDto(int id, string blobName, string url, DateTimeOffset createdAt)
    {
        return new ImageDto
        {
            Id = id,
            BlobName = blobName,
            Url = url,
            MimeType = "image/jpeg",
            CreatedAt = createdAt
        };
    }

    private void SetupMapper(List<HippotherapyProgramDto> hippotherapyProgramDtos)
    {
        _mapperMock.Setup(mapper => mapper.Map<List<HippotherapyProgramDto>>(It.IsAny<List<HippotherapyProgram>>())).Returns(hippotherapyProgramDtos);
    }

    private void SetupRepositoryWrapper(List<HippotherapyProgram> programsToReturn)
    {
        _repositoryWrapperMock.Setup(x => x.HippotherapyProgramsRepository.GetAllAsync(
            It.IsAny<QueryOptions<HippotherapyProgram>>()))
            .ReturnsAsync(programsToReturn);

        _repositoryWrapperMock.Setup(x => x.HippotherapyProgramsRepository.CountAsync(
            It.IsAny<QueryOptions<HippotherapyProgram>>()))
            .ReturnsAsync(programsToReturn.Count);
    }
}
