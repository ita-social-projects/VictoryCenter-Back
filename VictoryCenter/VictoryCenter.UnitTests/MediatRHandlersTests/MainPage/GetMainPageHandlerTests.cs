using AutoMapper;
using Moq;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.MainPages;
using VictoryCenter.BLL.Queries.Public.MainPage.GetMainPage;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Interfaces.MainPage;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.UnitTests.MediatRHandlersTests.MainPage;

public class GetMainPageHandlerTests
{
    private readonly Mock<IRepositoryWrapper> _repositoryWrapperMock = new();
    private readonly Mock<IMainPageRepository> _mainPageRepositoryMock = new();
    private readonly Mock<IMapper> _mapperMock = new();

    public GetMainPageHandlerTests()
    {
        _repositoryWrapperMock
            .SetupGet(x => x.MainPageRepository)
            .Returns(_mainPageRepositoryMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnMainPage_WhenEntityExists()
    {
        // Arrange
        QueryOptions<DAL.Entities.MainPage>? passedQueryOptions = null;

        var entity = new DAL.Entities.MainPage
        {
            Id = 1,
            Title = "Main page",
            Description = "Main page description",
        };

        var dto = new MainPageDto
        {
            Id = 1,
            Title = "Main page",
            Description = "Main page description",
        };

        _mainPageRepositoryMock
            .Setup(x => x.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<DAL.Entities.MainPage>?>()))
            .Callback<QueryOptions<DAL.Entities.MainPage>?>(options => passedQueryOptions = options)
            .ReturnsAsync(entity);

        _mapperMock
            .Setup(x => x.Map<DAL.Entities.MainPage, MainPageDto>(entity))
            .Returns(dto);

        var handler = new GetMainPageHandler(_repositoryWrapperMock.Object, _mapperMock.Object);

        // Act
        var result = await handler.Handle(new GetMainPageQuery(), CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(dto.Id, result.Value.Id);
        Assert.NotNull(passedQueryOptions);
        Assert.True(passedQueryOptions!.AsNoTracking);
        Assert.NotNull(passedQueryOptions.Include);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenEntityNotFound()
    {
        // Arrange
        _mainPageRepositoryMock
            .Setup(x => x.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<DAL.Entities.MainPage>?>()))
            .ReturnsAsync((DAL.Entities.MainPage?)null);

        var handler = new GetMainPageHandler(_repositoryWrapperMock.Object, _mapperMock.Object);

        // Act
        var result = await handler.Handle(new GetMainPageQuery(), CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessagesConstants.NotFound(), result.Errors[0].Message);
    }
}
