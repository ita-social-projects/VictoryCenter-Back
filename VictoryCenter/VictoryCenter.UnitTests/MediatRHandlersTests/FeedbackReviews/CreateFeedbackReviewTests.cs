using AutoMapper;
using Moq;
using VictoryCenter.BLL.Commands.Admin.FeedbackReviews.Create;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.FeedbackReviews;
using VictoryCenter.BLL.Interfaces.ReorderService;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Enums;
using VictoryCenter.DAL.Repositories.Interfaces.Base;

namespace VictoryCenter.UnitTests.MediatRHandlersTests.FeedbackReviews;

public class CreateFeedbackReviewTests
{
    private readonly Mock<IMapper> _mapper = new();
    private readonly Mock<IRepositoryWrapper> _repositoryWrapper = new();
    private readonly Mock<IReorderService> _reorderService = new();
    private static readonly DateTimeOffset TestNow = new(2026, 1, 1, 0, 0, 0, TimeSpan.Zero);
    private readonly Mock<TimeProvider> _timeProvider = new();

    [Fact]
    public async Task Handle_ValidRequest_ReturnsSuccess()
    {
        var entity = SetUp(saveChanges: 1, nextPriority: 0);
        var handler = CreateHandler();

        var result = await handler.Handle(Command(), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal("Anastasiia", entity.AuthorName);
        Assert.Equal(Status.Draft, entity.Status);
    }

    [Fact]
    public async Task Handle_ValidRequest_SetsNextPriority()
    {
        var entity = SetUp(saveChanges: 1, nextPriority: 5);
        var handler = CreateHandler();

        await handler.Handle(Command(), CancellationToken.None);

        Assert.Equal(5, entity.Priority);
        _reorderService.Verify(
            service => service.GetNextDisplayOrderAsync<FeedbackReview>(null),
            Times.Once);
    }

    [Fact]
    public async Task Handle_ValidRequest_TrimsTextFields()
    {
        var entity = SetUp(saveChanges: 1, nextPriority: 0);
        var handler = CreateHandler();

        await handler.Handle(
            Command(new CreateFeedbackReviewDto
            {
                AuthorName = "  Anastasiia  ",
                Text = "  Very happy  ",
                Status = Status.Draft
            }),
            CancellationToken.None);

        Assert.Equal("Anastasiia", entity.AuthorName);
        Assert.Equal("Very happy", entity.Text);
    }

    [Fact]
    public async Task Handle_ValidRequest_SetsCreatedAt()
    {
        var entity = SetUp(saveChanges: 1, nextPriority: 0);
        var handler = CreateHandler();

        await handler.Handle(Command(), CancellationToken.None);

        Assert.NotEqual(default, entity.CreatedAt);
    }

    [Fact]
    public async Task Handle_SaveChangesReturnsZero_ReturnsFailure()
    {
        SetUp(saveChanges: 0, nextPriority: 0);
        var handler = CreateHandler();

        var result = await handler.Handle(Command(), CancellationToken.None);

        Assert.True(result.IsFailed);
        Assert.Equal(
            ErrorMessagesConstants.FailedToCreateEntity(typeof(FeedbackReview)),
            result.Errors[0].Message);
    }

    [Fact]
    public async Task Handle_ValidRequest_SetsCreatedAtFromTimeProvider()
    {
        var entity = SetUp(saveChanges: 1, nextPriority: 0);
        var handler = CreateHandler();

        await handler.Handle(Command(), CancellationToken.None);

        Assert.Equal(TestNow, entity.CreatedAt);
    }

    private FeedbackReview SetUp(int saveChanges, long nextPriority)
    {
        var entity = new FeedbackReview();

        _timeProvider.Setup(provider => provider.GetUtcNow()).Returns(TestNow);

        _mapper
            .Setup(mapper => mapper.Map<FeedbackReview>(It.IsAny<CreateFeedbackReviewDto>()))
            .Returns((CreateFeedbackReviewDto dto) =>
            {
                entity.AuthorName = dto.AuthorName;
                entity.Text = dto.Text;
                entity.Status = dto.Status;
                return entity;
            });

        _mapper
            .Setup(mapper => mapper.Map<FeedbackReviewDto>(It.IsAny<FeedbackReview>()))
            .Returns((FeedbackReview review) => new FeedbackReviewDto
            {
                Id = review.Id,
                AuthorName = review.AuthorName,
                Text = review.Text,
                Status = review.Status,
                Priority = review.Priority,
                CreatedAt = review.CreatedAt
            });

        _reorderService
            .Setup(service => service.GetNextDisplayOrderAsync<FeedbackReview>(null))
            .ReturnsAsync(nextPriority);

        _repositoryWrapper
            .Setup(wrapper => wrapper.FeedbackReviewsRepository.CreateAsync(It.IsAny<FeedbackReview>()))
            .ReturnsAsync(entity);

        _repositoryWrapper
            .Setup(wrapper => wrapper.SaveChangesAsync())
            .ReturnsAsync(saveChanges);

        return entity;
    }

    private CreateFeedbackReviewHandler CreateHandler() =>
        new(_mapper.Object, _repositoryWrapper.Object, _reorderService.Object, _timeProvider.Object);

    private static CreateFeedbackReviewCommand Command(CreateFeedbackReviewDto? dto = null) =>
        new(dto ?? new CreateFeedbackReviewDto
        {
            AuthorName = "Anastasiia",
            Text = "Very happy with the therapy",
            Status = Status.Draft
        });
}
