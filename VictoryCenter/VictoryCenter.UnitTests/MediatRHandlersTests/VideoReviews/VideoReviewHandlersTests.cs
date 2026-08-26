using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Moq;
using VictoryCenter.BLL.Commands.Admin.VideoReviews.Create;
using VictoryCenter.BLL.Commands.Admin.VideoReviews.Delete;
using VictoryCenter.BLL.Commands.Admin.VideoReviews.Update;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.VideoReviews;
using VictoryCenter.BLL.Queries.Admin.VideoReviews.GetAll;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Interfaces.VideoReviews;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.UnitTests.MediatRHandlersTests.VideoReviews;

public class VideoReviewHandlersTests
{
    private readonly Mock<IMapper> _mapper = new();
    private readonly Mock<IRepositoryWrapper> _wrapper = new();
    private readonly Mock<IVideoReviewsRepository> _repository = new();

    public VideoReviewHandlersTests()
    {
        _wrapper.SetupGet(item => item.VideoReviewsRepository).Returns(_repository.Object);
        _mapper.Setup(mapper => mapper.Map<VideoReviewDto>(It.IsAny<VideoReview>()))
            .Returns((VideoReview videoReview) => new VideoReviewDto
            {
                Id = videoReview.Id,
                Title = videoReview.Title,
                Link = videoReview.Link
            });
    }

    [Fact]
    public async Task Create_ShouldTrimTitleAndLinkBeforePersisting()
    {
        VideoReview? createdEntity = null;
        _mapper.Setup(mapper => mapper.Map<VideoReview>(It.IsAny<CreateVideoReviewDto>()))
            .Returns((CreateVideoReviewDto dto) => new VideoReview { Title = dto.Title, Link = dto.Link });
        _repository
            .Setup(repository => repository.CreateAsync(It.IsAny<VideoReview>()))
            .Callback<VideoReview>(entity => createdEntity = entity)
            .ReturnsAsync((VideoReview entity) => entity);
        _wrapper.Setup(wrapper => wrapper.SaveChangesAsync()).ReturnsAsync(1);
        var handler = new CreateVideoReviewHandler(_mapper.Object, _wrapper.Object);

        var result = await handler.Handle(
            new CreateVideoReviewCommand(new CreateVideoReviewDto
            {
                Title = "  Title  ",
                Link = "  https://example.com/video  "
            }),
            CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.NotNull(createdEntity);
        Assert.Equal("Title", createdEntity.Title);
        Assert.Equal("https://example.com/video", createdEntity.Link);
        Assert.NotEqual(default, createdEntity.CreatedAt);
    }

    [Fact]
    public async Task Create_ShouldFail_WhenNoRowsAffected()
    {
        _mapper.Setup(mapper => mapper.Map<VideoReview>(It.IsAny<CreateVideoReviewDto>()))
            .Returns(new VideoReview { Title = "Title", Link = "https://example.com/video" });
        _repository
            .Setup(repository => repository.CreateAsync(It.IsAny<VideoReview>()))
            .ReturnsAsync((VideoReview entity) => entity);
        _wrapper.Setup(wrapper => wrapper.SaveChangesAsync()).ReturnsAsync(0);
        var handler = new CreateVideoReviewHandler(_mapper.Object, _wrapper.Object);

        var result = await handler.Handle(new CreateVideoReviewCommand(CreateDto()), CancellationToken.None);

        Assert.True(result.IsFailed);
        Assert.Equal(ErrorMessagesConstants.FailedToCreateEntity(typeof(VideoReview)), result.Errors[0].Message);
    }

    [Fact]
    public async Task Create_ShouldFail_WhenDatabaseThrows()
    {
        _mapper.Setup(mapper => mapper.Map<VideoReview>(It.IsAny<CreateVideoReviewDto>()))
            .Returns(new VideoReview { Title = "Title", Link = "https://example.com/video" });
        _repository
            .Setup(repository => repository.CreateAsync(It.IsAny<VideoReview>()))
            .ReturnsAsync((VideoReview entity) => entity);
        _wrapper.Setup(wrapper => wrapper.SaveChangesAsync()).ThrowsAsync(new DbUpdateException());
        var handler = new CreateVideoReviewHandler(_mapper.Object, _wrapper.Object);

        var result = await handler.Handle(new CreateVideoReviewCommand(CreateDto()), CancellationToken.None);

        Assert.True(result.IsFailed);
        Assert.Equal(
            ErrorMessagesConstants.FailedToCreateEntityInDatabase(typeof(VideoReview)),
            result.Errors[0].Message);
    }

    [Fact]
    public async Task Update_ShouldFail_WhenEntityDoesNotExist()
    {
        _repository
            .Setup(repository => repository.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<VideoReview>>()))
            .ReturnsAsync((VideoReview?)null);
        var handler = new UpdateVideoReviewHandler(_mapper.Object, _wrapper.Object);

        var result = await handler.Handle(
            new UpdateVideoReviewCommand(10, new UpdateVideoReviewDto
            {
                Title = "Title",
                Link = "https://example.com/video"
            }),
            CancellationToken.None);

        Assert.True(result.IsFailed);
        Assert.Contains("not found", result.Errors[0].Message, StringComparison.OrdinalIgnoreCase);
        _wrapper.Verify(wrapper => wrapper.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task Update_ShouldReturnOkWithoutSaving_WhenNothingChangedAfterTrimming()
    {
        var entity = new VideoReview
        {
            Id = 10,
            Title = "Title",
            Link = "https://example.com/video"
        };
        _repository
            .Setup(repository => repository.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<VideoReview>>()))
            .ReturnsAsync(entity);
        var handler = new UpdateVideoReviewHandler(_mapper.Object, _wrapper.Object);

        var result = await handler.Handle(
            new UpdateVideoReviewCommand(10, new UpdateVideoReviewDto
            {
                Title = "  Title  ",
                Link = "  https://example.com/video  "
            }),
            CancellationToken.None);

        Assert.True(result.IsSuccess);
        _wrapper.Verify(wrapper => wrapper.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task Update_ShouldPersistTrimmedChangesAndReturnMappedDto()
    {
        var entity = new VideoReview { Id = 10, Title = "Old title", Link = "https://example.com/old" };
        _repository
            .Setup(repository => repository.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<VideoReview>>()))
            .ReturnsAsync(entity);
        _mapper
            .Setup(mapper => mapper.Map(It.IsAny<UpdateVideoReviewDto>(), It.IsAny<VideoReview>()))
            .Callback<UpdateVideoReviewDto, VideoReview>((dto, target) =>
            {
                target.Title = dto.Title;
                target.Link = dto.Link;
            });
        _wrapper.Setup(wrapper => wrapper.SaveChangesAsync()).ReturnsAsync(1);
        var handler = new UpdateVideoReviewHandler(_mapper.Object, _wrapper.Object);

        var result = await handler.Handle(
            new UpdateVideoReviewCommand(10, new UpdateVideoReviewDto
            {
                Title = "  New title  ",
                Link = "  https://example.com/new  "
            }),
            CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal("New title", entity.Title);
        Assert.Equal("https://example.com/new", entity.Link);
    }

    [Fact]
    public async Task Update_ShouldFail_WhenDatabaseThrows()
    {
        var entity = new VideoReview { Id = 10, Title = "Old", Link = "https://example.com/old" };
        _repository
            .Setup(repository => repository.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<VideoReview>>()))
            .ReturnsAsync(entity);
        _wrapper.Setup(wrapper => wrapper.SaveChangesAsync()).ThrowsAsync(new DbUpdateException());
        var handler = new UpdateVideoReviewHandler(_mapper.Object, _wrapper.Object);

        var result = await handler.Handle(
            new UpdateVideoReviewCommand(10, new UpdateVideoReviewDto
            {
                Title = "New",
                Link = "https://example.com/new"
            }),
            CancellationToken.None);

        Assert.True(result.IsFailed);
        Assert.Equal(
            ErrorMessagesConstants.FailedToUpdateEntityInDatabase(typeof(VideoReview)),
            result.Errors[0].Message);
    }

    [Fact]
    public async Task Delete_ShouldFail_WhenEntityDoesNotExist()
    {
        _repository
            .Setup(repository => repository.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<VideoReview>>()))
            .ReturnsAsync((VideoReview?)null);
        var handler = new DeleteVideoReviewHandler(_wrapper.Object);

        var result = await handler.Handle(new DeleteVideoReviewCommand(10), CancellationToken.None);

        Assert.True(result.IsFailed);
        Assert.Contains("not found", result.Errors[0].Message, StringComparison.OrdinalIgnoreCase);
        _repository.Verify(repository => repository.Delete(It.IsAny<VideoReview>()), Times.Never);
    }

    [Fact]
    public async Task Delete_ShouldRemoveEntityAndReturnItsId()
    {
        var entity = new VideoReview { Id = 10, Title = "Title", Link = "https://example.com/video" };
        _repository
            .Setup(repository => repository.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<VideoReview>>()))
            .ReturnsAsync(entity);
        _wrapper.Setup(wrapper => wrapper.SaveChangesAsync()).ReturnsAsync(1);
        var handler = new DeleteVideoReviewHandler(_wrapper.Object);

        var result = await handler.Handle(new DeleteVideoReviewCommand(10), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(10, result.Value);
        _repository.Verify(repository => repository.Delete(entity), Times.Once);
    }

    [Fact]
    public async Task Delete_ShouldFail_WhenDatabaseThrows()
    {
        var entity = new VideoReview { Id = 10, Title = "Title", Link = "https://example.com/video" };
        _repository
            .Setup(repository => repository.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<VideoReview>>()))
            .ReturnsAsync(entity);
        _wrapper.Setup(wrapper => wrapper.SaveChangesAsync()).ThrowsAsync(new DbUpdateException());
        var handler = new DeleteVideoReviewHandler(_wrapper.Object);

        var result = await handler.Handle(new DeleteVideoReviewCommand(10), CancellationToken.None);

        Assert.True(result.IsFailed);
        Assert.Equal(
            ErrorMessagesConstants.FailedToDeleteEntityInDatabase(typeof(VideoReview)),
            result.Errors[0].Message);
    }

    [Fact]
    public async Task GetAll_ShouldUseReadOnlyOrderedQuery()
    {
        QueryOptions<VideoReview>? capturedOptions = null;
        _repository
            .Setup(repository => repository.GetAllAsync(It.IsAny<QueryOptions<VideoReview>>()))
            .Callback<QueryOptions<VideoReview>?>(options => capturedOptions = options)
            .ReturnsAsync([]);
        _mapper
            .Setup(mapper => mapper.Map<List<VideoReviewDto>>(It.IsAny<IEnumerable<VideoReview>>()))
            .Returns([]);
        var handler = new GetAllVideoReviewsHandler(_mapper.Object, _wrapper.Object);

        var result = await handler.Handle(new GetAllVideoReviewsQuery(), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.NotNull(capturedOptions);
        Assert.True(capturedOptions.AsNoTracking);
        Assert.NotNull(capturedOptions.OrderByASC);
    }

    private static CreateVideoReviewDto CreateDto() => new()
    {
        Title = "Title",
        Link = "https://example.com/video"
    };
}
