using Microsoft.EntityFrameworkCore;
using Moq;
using VictoryCenter.BLL.Commands.Admin.EventNews.Delete;
using VictoryCenter.BLL.Constants;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Interfaces.EventNews;
using VictoryCenter.DAL.Repositories.Options;
using EventNewsEntity = VictoryCenter.DAL.Entities.EventNews;
using EventNewsPredicate = System.Linq.Expressions.Expression<System.Func<VictoryCenter.DAL.Entities.EventNews, bool>>;

namespace VictoryCenter.UnitTests.MediatRHandlersTests.EventNews;

public class DeleteEventNewsTests
{
    private readonly Mock<IEventNewsRepository> _eventNewsRepository = new();
    private readonly Mock<IRepositoryWrapper> _repositoryWrapper = new();

    public DeleteEventNewsTests()
    {
        _repositoryWrapper
            .SetupGet(wrapper => wrapper.EventNewsRepository)
            .Returns(_eventNewsRepository.Object);
    }

    [Fact]
    public async Task Handle_ShouldDeleteEventNewsAndReturnId()
    {
        var eventNews = EventNews(10);
        SetupEventNews(eventNews);
        _repositoryWrapper.Setup(wrapper => wrapper.SaveChangesAsync()).ReturnsAsync(1);
        var handler = new DeleteEventNewsHandler(_repositoryWrapper.Object);

        var result = await handler.Handle(new DeleteEventNewsCommand(eventNews.Id), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(eventNews.Id, result.Value);
        _eventNewsRepository.Verify(repository => repository.Delete(eventNews), Times.Once);
        _repositoryWrapper.Verify(wrapper => wrapper.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldUseTrackedAggregateLookupForRequestedId()
    {
        const long eventNewsId = 10;
        var eventNews = EventNews(eventNewsId);
        SetupEventNews(eventNews);
        _repositoryWrapper.Setup(wrapper => wrapper.SaveChangesAsync()).ReturnsAsync(1);
        var handler = new DeleteEventNewsHandler(_repositoryWrapper.Object);

        await handler.Handle(new DeleteEventNewsCommand(eventNewsId), CancellationToken.None);

        _eventNewsRepository.Verify(
            repository => repository.GetFirstOrDefaultAsync(
                It.Is<QueryOptions<EventNewsEntity>>(options =>
                    !options.AsNoTracking
                    && options.AsSplitQuery
                    && options.Include != null
                    && options.Filter != null
                    && options.Filter.Compile()(eventNews))),
            Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldReturnNotFound_WhenEventNewsDoesNotExist()
    {
        const long eventNewsId = 10;
        SetupEventNews(null);
        var handler = new DeleteEventNewsHandler(_repositoryWrapper.Object);

        var result = await handler.Handle(new DeleteEventNewsCommand(eventNewsId), CancellationToken.None);

        Assert.True(result.IsFailed);
        Assert.Equal(
            ErrorMessagesConstants.NotFound(eventNewsId, typeof(EventNewsEntity)),
            result.Errors[0].Message);
        _eventNewsRepository.Verify(repository => repository.Delete(It.IsAny<EventNewsEntity>()), Times.Never);
        _repositoryWrapper.Verify(wrapper => wrapper.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenSaveDoesNotDeleteRow()
    {
        var eventNews = EventNews(10);
        SetupEventNews(eventNews);
        _repositoryWrapper.Setup(wrapper => wrapper.SaveChangesAsync()).ReturnsAsync(0);
        var handler = new DeleteEventNewsHandler(_repositoryWrapper.Object);

        var result = await handler.Handle(new DeleteEventNewsCommand(eventNews.Id), CancellationToken.None);

        Assert.True(result.IsFailed);
        Assert.Equal(
            ErrorMessagesConstants.FailedToDeleteEntity(typeof(EventNewsEntity)),
            result.Errors[0].Message);
    }

    [Fact]
    public async Task Handle_ShouldReturnNotFound_WhenEventNewsIsDeletedConcurrently()
    {
        var eventNews = EventNews(10);
        SetupEventNews(eventNews);
        _repositoryWrapper
            .Setup(wrapper => wrapper.SaveChangesAsync())
            .ThrowsAsync(new DbUpdateConcurrencyException());
        _eventNewsRepository
            .Setup(repository => repository.ExistsAsync(It.IsAny<EventNewsPredicate>()))
            .ReturnsAsync(false);
        var handler = new DeleteEventNewsHandler(_repositoryWrapper.Object);

        var result = await handler.Handle(new DeleteEventNewsCommand(eventNews.Id), CancellationToken.None);

        Assert.True(result.IsFailed);
        Assert.Equal(
            ErrorMessagesConstants.NotFound(eventNews.Id, typeof(EventNewsEntity)),
            result.Errors[0].Message);
    }

    [Fact]
    public async Task Handle_ShouldPropagateConcurrencyException_WhenEventNewsStillExists()
    {
        var eventNews = EventNews(10);
        var exception = new DbUpdateConcurrencyException();
        SetupEventNews(eventNews);
        _repositoryWrapper
            .Setup(wrapper => wrapper.SaveChangesAsync())
            .ThrowsAsync(exception);
        _eventNewsRepository
            .Setup(repository => repository.ExistsAsync(It.IsAny<EventNewsPredicate>()))
            .ReturnsAsync(true);
        var handler = new DeleteEventNewsHandler(_repositoryWrapper.Object);

        var actualException = await Assert.ThrowsAsync<DbUpdateConcurrencyException>(() =>
            handler.Handle(new DeleteEventNewsCommand(eventNews.Id), CancellationToken.None));

        Assert.Same(exception, actualException);
    }

    [Fact]
    public async Task Handle_ShouldPropagateNonConcurrencyDatabaseException()
    {
        var eventNews = EventNews(10);
        SetupEventNews(eventNews);
        _repositoryWrapper
            .Setup(wrapper => wrapper.SaveChangesAsync())
            .ThrowsAsync(new DbUpdateException("Unexpected database failure"));
        var handler = new DeleteEventNewsHandler(_repositoryWrapper.Object);

        await Assert.ThrowsAsync<DbUpdateException>(() =>
            handler.Handle(new DeleteEventNewsCommand(eventNews.Id), CancellationToken.None));
    }

    private void SetupEventNews(EventNewsEntity? eventNews)
    {
        _eventNewsRepository
            .Setup(repository => repository.GetFirstOrDefaultAsync(
                It.IsAny<QueryOptions<EventNewsEntity>>()))
            .ReturnsAsync(eventNews);
    }

    private static EventNewsEntity EventNews(long id)
    {
        return new EventNewsEntity
        {
            Id = id,
            CreatedAt = DateTimeOffset.UtcNow
        };
    }
}
