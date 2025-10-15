using AutoMapper;
using FluentValidation;
using Moq;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.TeamMembers;
using VictoryCenter.BLL.DTOs.Common;
using VictoryCenter.BLL.Interfaces.Search;
using VictoryCenter.BLL.Queries.Admin.TeamMembers.Search;
using VictoryCenter.BLL.Validators.TeamMembers;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Enums;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.UnitTests.MediatRHandlersTests.TeamMembers;

public class SearchTeamMemberTests
{
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<IRepositoryWrapper> _repositoryWrapperMock;
    private readonly Mock<ISearchService<TeamMember>> _searchServiceMock;
    private readonly IValidator<SearchTeamMemberQuery> _validator;

    private readonly List<TeamMember> _teamMembers =
    [
        CreateTeamMember(1, "TestName", Status.Draft, "Test@gmail.com")
    ];

    private readonly List<TeamMemberDto> _teamMemberDtos =
    [
        CreateTeamMemberDto(1, "TestName", Status.Draft, "Test@gmail.com")
    ];

    public SearchTeamMemberTests()
    {
        _mapperMock = new Mock<IMapper>();
        _repositoryWrapperMock = new Mock<IRepositoryWrapper>();
        _validator = new SearchTeamMemberValidator();
        _searchServiceMock = new Mock<ISearchService<TeamMember>>();
    }

    [Fact]
    public async Task Handle_ExistingFullName_ShouldReturnNotEmpty()
    {
        // Arrange
        SetupMapper(_teamMemberDtos);
        SetupRepositoryWrapper(_teamMembers);
        var dto = new SearchTeamMemberDto { FullName = "TestName" };
        var query = new SearchTeamMemberQuery(dto);
        var handler = new SearchTeamMemberHandler(_mapperMock.Object, _repositoryWrapperMock.Object, _validator, _searchServiceMock.Object);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(_teamMemberDtos, result.Value.Items);
    }

    [Fact]
    public async Task Handle_NonexistentFullName_ShouldReturnEmpty()
    {
        // Arrange
        SetupMapper([]);
        SetupRepositoryWrapper([]);
        var dto = new SearchTeamMemberDto { FullName = "Nonexistent fullname" };
        var query = new SearchTeamMemberQuery(dto);
        var handler = new SearchTeamMemberHandler(_mapperMock.Object, _repositoryWrapperMock.Object, _validator, _searchServiceMock.Object);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Empty(result.Value.Items);
    }

    [Fact]
    public async Task Handle_InvalidFullName_ShouldReturnValidationError()
    {
        // Arrange
        SetupMapper([]);
        SetupRepositoryWrapper([]);
        var dto = new SearchTeamMemberDto { FullName = "" };
        var query = new SearchTeamMemberQuery(dto);
        var handler = new SearchTeamMemberHandler(_mapperMock.Object, _repositoryWrapperMock.Object, _validator, _searchServiceMock.Object);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Contains(ErrorMessagesConstants.PropertyIsRequired(nameof(SearchTeamMemberDto.FullName)), result.Errors[0].Message);
    }

    [Fact]
    public async Task Handle_WithImage_ShouldReturnImage()
    {
        // Arrange
        var image = CreateImage(10, "blob.jpg", "https://cdn.example.com/blob.jpg");
        var member = CreateTeamMember(2, "With Image", Status.Published, "with@img.com", image);
        var imageDto = CreateImageDto(10, "blob.jpg", "https://cdn.example.com/blob.jpg", image.CreatedAt);
        var dto = CreateTeamMemberDto(2, "With Image", Status.Published, "with@img.com", imageDto);

        SetupMapper([dto]);
        SetupRepositoryWrapper([member]);

        var searchDto = new SearchTeamMemberDto { FullName = "With Image" };
        var query = new SearchTeamMemberQuery(searchDto);
        var handler = new SearchTeamMemberHandler(_mapperMock.Object, _repositoryWrapperMock.Object, _validator, _searchServiceMock.Object);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(Assert.Single(result.Value!.Items).Image);
    }

    [Fact]
    public async Task Handle_WithoutImage_ShouldReturnNullImage()
    {
        // Arrange
        var member = CreateTeamMember(3, "No Image", Status.Published, "no@img.com");
        var dto = CreateTeamMemberDto(3, "No Image", Status.Published, "no@img.com");

        SetupMapper([dto]);
        SetupRepositoryWrapper([member]);

        var searchDto = new SearchTeamMemberDto { FullName = "No Image" };
        var query = new SearchTeamMemberQuery(searchDto);
        var handler = new SearchTeamMemberHandler(_mapperMock.Object, _repositoryWrapperMock.Object, _validator, _searchServiceMock.Object);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.Null(Assert.Single(result.Value!.Items).Image);
    }

    private static TeamMember CreateTeamMember(int id, string fullName, Status status, string email, Image? image = null)
    {
        return new TeamMember
        {
            Id = id,
            FullName = fullName,
            Priority = 1,
            CategoryId = 1,
            Status = status,
            Description = "desc",
            Email = email,
            Image = image,
            CreatedAt = DateTimeOffset.UtcNow
        };
    }

    private static TeamMemberDto CreateTeamMemberDto(int id, string fullName, Status status, string email, ImageDto? image = null)
    {
        return new TeamMemberDto
        {
            Id = id,
            FullName = fullName,
            Priority = 1,
            Status = status,
            Description = "desc",
            Email = email,
            Image = image
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

    private void SetupMapper(List<TeamMemberDto> teamMemberDtos)
    {
        _mapperMock.Setup(mapper => mapper.Map<List<TeamMemberDto>>(It.IsAny<List<TeamMember>>())).Returns(teamMemberDtos);
    }

    private void SetupRepositoryWrapper(List<TeamMember> membersToReturn)
    {
        _repositoryWrapperMock.Setup(x => x.TeamMembersRepository.GetAllAsync(
                It.IsAny<QueryOptions<TeamMember>>()))
            .ReturnsAsync(membersToReturn);
    }
}
