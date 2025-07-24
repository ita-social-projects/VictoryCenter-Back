using AutoMapper;
using Moq;
using VictoryCenter.BLL.DTOs.Admin.VisitorPages;
using VictoryCenter.BLL.Queries.Admin.VisitorPages.GetAll;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.UnitTests.MediatRHandlersTests.VisitorPages;

public class GetAllVisitorPagesTests
{
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<IRepositoryWrapper> _mockRepoWrapper;

    private readonly List<VisitorPage> _testPageEntities = new()
    {
        new() { Id = 1, Title = "Програми", Slug = "program-page" },
        new() { Id = 2, Title = "Донати", Slug = "donate-page" }
    };
    private readonly List<VisitorPageDto> _testPageDtos = new()
    {
        new() { Id = 1, Title = "Програми", Slug = "program-page" },
        new() { Id = 2, Title = "Донати", Slug = "donate-page" }
    };

    public GetAllVisitorPagesTests()
    {
        _mockMapper = new Mock<IMapper>();
        _mockRepoWrapper = new Mock<IRepositoryWrapper>();
    }

    [Fact]
    public async Task Handle_ShouldReturnAllVisitorPages()
    {
        // Arrange
        _mockRepoWrapper.Setup(
            repoWrapper => repoWrapper.VisitorPagesRepository.GetAllAsync(
                It.IsAny<QueryOptions<VisitorPage>>())).ReturnsAsync(_testPageEntities);
        _mockMapper.Setup(
            mapper => mapper.Map<List<VisitorPageDto>>(It.IsAny<List<VisitorPage>>())).Returns(_testPageDtos);
        var handler = new GetAllVisitorPagesHandler(_mockMapper.Object, _mockRepoWrapper.Object);

        // Act
        var result = await handler.Handle(new GetAllVisitorPagesQuery(), CancellationToken.None);

        // Assert
        Assert.Multiple(
            () => Assert.NotNull(result),
            () => Assert.NotNull(result.Value),
            () => Assert.NotEmpty(result.Value),
            () => Assert.Equal(_testPageDtos, result.Value));
    }
}
