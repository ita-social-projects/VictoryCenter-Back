using AutoMapper;
using Moq;
using VictoryCenter.BLL.DTOs.Public.FaqQuestions;
using VictoryCenter.BLL.Queries.Public.FaqQuestions.GetPublished;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Enums;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.UnitTests.MediatRHandlersTests.FaqQuestions;

public class GetPublishedFaqQuestionsBySlugTests
{
    private readonly List<FaqPlacement> _testPlacementEntities =
    [
        new()
        {
            Question = new()
            {
                Id = 1,
                QuestionText = "Some very smart question to ask.",
                AnswerText = "Some very smart answer to give, also I need to write more text so here it is.",
                Status = Status.Published,
                CreatedAt = DateTimeOffset.UtcNow.AddMinutes(-10),
            },
            Priority = 1
        },
        new()
        {
            Question = new()
            {
                Id = 2,
                QuestionText = "Another very smart question to ask.",
                AnswerText = "Another very smart answer to give, also I need to write more text so here it is.",
                Status = Status.Published,
                CreatedAt = DateTimeOffset.UtcNow.AddMinutes(-5),
            },
            Priority = 2
        }

    ];
    private readonly List<PublishedFaqQuestionDto> _testPlacementDtos =
    [
        new()
        {
            Id = 1,
            QuestionText = "Some very smart question to ask.",
            AnswerText = "Some very smart answer to give, also I need to write more text so here it is."
        },
        new()
        {
            Id = 2,
            QuestionText = "Another very smart question to ask.",
            AnswerText = "Another very smart answer to give, also I need to write more text so here it is."
        }

    ];

    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<IRepositoryWrapper> _mockRepoWrapper;

    public GetPublishedFaqQuestionsBySlugTests()
    {
        _mockRepoWrapper = new Mock<IRepositoryWrapper>();
        _mockMapper = new Mock<IMapper>();
    }

    [Fact]
    public async Task Handle_PageExistsAndIsNotEmpty_ShouldReturnOk()
    {
        // Arrange
        SetupRepository(_testPlacementEntities);
        SetupMapper();
        var handler = new GetPublishedFaqQuestionsBySlugHandler(_mockMapper.Object, _mockRepoWrapper.Object);
        var query = new GetPublishedFaqQuestionsBySlugQuery("some-page-slug");

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.IsSuccess);
        Assert.Equal(_testPlacementDtos, result.Value);
    }

    [Fact]
    public async Task Handle_PageDoesNotExistOrIsEmpty_ShouldReturnOkWithEmptyList()
    {
        SetupRepository([]);
        SetupMapper();
        var handler = new GetPublishedFaqQuestionsBySlugHandler(_mockMapper.Object, _mockRepoWrapper.Object);
        var query = new GetPublishedFaqQuestionsBySlugQuery("another-test-slug");

        var result = await handler.Handle(query, CancellationToken.None);

        Assert.NotNull(result);
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Empty(result.Value);
    }

    private void SetupRepository(List<FaqPlacement> entities)
    {
        _mockRepoWrapper.Setup(
            repositoryWrapper => repositoryWrapper.FaqPlacementsRepository.GetAllAsync(
                It.IsAny<QueryOptions<FaqPlacement>>())).ReturnsAsync(entities);
    }

    private void SetupMapper()
    {
        _mockMapper
            .Setup(x => x.Map<List<PublishedFaqQuestionDto>>(It.IsAny<IEnumerable<FaqQuestion>>()))
            .Returns((IEnumerable<FaqQuestion> qs) =>
                qs.Select(q => new PublishedFaqQuestionDto
                {
                    Id = q.Id,
                    QuestionText = q.QuestionText,
                    AnswerText = q.AnswerText
                }).ToList());
    }
}
