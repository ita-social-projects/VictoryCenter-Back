using System.Linq.Expressions;
using AutoMapper;
using Moq;
using VictoryCenter.BLL.DTOs.Admin.FaqQuestions;
using VictoryCenter.BLL.Queries.Admin.FaqQuestions.GetByFilters;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Enums;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.UnitTests.MediatRHandlersTests.Faq;

public class GetFaqQuestionsByFiltersTests
{
    private readonly Mock<IRepositoryWrapper> _mockRepository;
    private readonly Mock<IMapper> _mockMapper;

    public GetFaqQuestionsByFiltersTests()
    {
        _mockRepository = new Mock<IRepositoryWrapper>();
        _mockMapper = new Mock<IMapper>();
    }

    [Theory]
    [InlineData(0, 0)]
    [InlineData(0, 4)]
    [InlineData(0, 3)]
    [InlineData(1, 2)]
    public async Task Handle_NoFilters_ShouldReturnSuccessfully(int pageNumber, int pageSize)
    {
        // Arrange
        var faqQuestionList = GetFaqQuestionList();
        var faqQuestionDtoList = GetFaqQuestionDtoList()
            .Skip(pageNumber)
            .Take(pageSize)
            .ToArray();

        SetupRepository(faqQuestionList, faqQuestionList.Count);
        SetupMapper(faqQuestionDtoList);

        var filtersDto = new FaqQuestionsFilterDto
        {
            Offset = pageNumber,
            Limit = pageSize,
            Status = null,
            PageId = null
        };

        var handler = new GetFaqQuestionsByFiltersHandler(_mockMapper.Object, _mockRepository.Object);

        // Act
        var result = await handler.Handle(new GetFaqQuestionsByFiltersQuery(filtersDto), CancellationToken.None);

        // Assert
        var faqQuestionDtoListOld = GetFaqQuestionDtoList().ToArray();
        Assert.Multiple(
            () => Assert.NotNull(result),
            () => Assert.NotNull(result.Value),
            () => Assert.NotEqual(faqQuestionDtoListOld.Length, result.Value.Items.Length),
            () => Assert.NotEqual(faqQuestionDtoListOld, result.Value.Items),
            () => Assert.Equal(faqQuestionDtoList.Length, result.Value.Items.Length),
            () => Assert.Equal(faqQuestionList.Count, result.Value.TotalItemsCount),
            () => Assert.Equal(faqQuestionDtoList, result.Value.Items));
    }

    [Fact]
    public async Task Handle_FilterByStatus_ShouldReturnSuccessfully()
    {
        // Arrange
        var status = Status.Draft;
        var faqQuestionList = GetFaqQuestionList();
        var faqQuestionDtoList = GetFaqQuestionDtoList()
            .Where(q => q.Status == status)
            .ToArray();

        SetupRepository(faqQuestionList, faqQuestionList.Count);
        SetupMapper(faqQuestionDtoList);

        var filtersDto = new FaqQuestionsFilterDto
        {
            Offset = 0,
            Limit = 0,
            Status = status,
            PageId = null
        };

        var handler = new GetFaqQuestionsByFiltersHandler(_mockMapper.Object, _mockRepository.Object);

        // Act
        var result = await handler.Handle(new GetFaqQuestionsByFiltersQuery(filtersDto), CancellationToken.None);

        // Assert
        Assert.Multiple(
            () => Assert.NotNull(result),
            () => Assert.NotNull(result.Value),
            () => Assert.NotEmpty(result.Value.Items),
            () => Assert.Equal(faqQuestionList.Count, result.Value.TotalItemsCount),
            () => Assert.Equal(faqQuestionDtoList, result.Value.Items));
    }

    [Fact]
    public async Task Handle_FilterByPageId_ShouldReturnSuccessfully()
    {
        // Arrange
        var page = new VisitorPage { Id = 2, Title = "Page 2" };
        var faqQuestionList = GetFaqQuestionList();
        var faqPlacementList = GetPlacementList();
        var faqQuestionDtoList = GetFaqQuestionDtoList()
            .Where(q => q.PageIds.Contains(page.Id))
            .OrderBy(q => faqPlacementList.Single(fp => fp.QuestionId == q.Id && fp.PageId == page.Id).Priority)
            .ToArray();

        SetupRepository(faqQuestionList, faqQuestionList.Count);
        SetupMapper(faqQuestionDtoList);

        var filtersDto = new FaqQuestionsFilterDto
        {
            Offset = 0,
            Limit = 0,
            Status = null,
            PageId = page.Id
        };

        var handler = new GetFaqQuestionsByFiltersHandler(_mockMapper.Object, _mockRepository.Object);

        // Act
        var result = await handler.Handle(new GetFaqQuestionsByFiltersQuery(filtersDto), CancellationToken.None);

        // Assert
        Assert.Multiple(
            () => Assert.NotNull(result),
            () => Assert.NotNull(result.Value),
            () => Assert.NotEmpty(result.Value.Items),
            () => Assert.Equal(faqQuestionList.Count, result.Value.TotalItemsCount),
            () => Assert.Equal(faqQuestionDtoList, result.Value.Items));
    }

    [Fact]
    public async Task Handle_FilterByStatusAndPageId_ShouldReturnSuccessfully()
    {
        // Arrange
        var status = Status.Draft;
        var page = new VisitorPage { Id = 1, Title = "Page 1" };
        var faqQuestionList = GetFaqQuestionList();
        var faqPlacementList = GetPlacementList();
        var faqQuestionDtoList = GetFaqQuestionDtoList()
            .Where(q => q.Status == status && q.PageIds.Contains(page.Id))
            .OrderBy(q => faqPlacementList.Single(fp => fp.QuestionId == q.Id && fp.PageId == page.Id).Priority)
            .ToArray();

        SetupRepository(faqQuestionList, faqQuestionList.Count);
        SetupMapper(faqQuestionDtoList);

        var filtersDto = new FaqQuestionsFilterDto
        {
            Offset = 0,
            Limit = 0,
            Status = status,
            PageId = page.Id
        };

        var handler = new GetFaqQuestionsByFiltersHandler(_mockMapper.Object, _mockRepository.Object);

        // Act
        var result = await handler.Handle(new GetFaqQuestionsByFiltersQuery(filtersDto), CancellationToken.None);

        // Assert
        Assert.Multiple(
            () => Assert.NotNull(result),
            () => Assert.NotNull(result.Value),
            () => Assert.NotEmpty(result.Value.Items),
            () => Assert.Equal(faqQuestionList.Count, result.Value.TotalItemsCount),
            () => Assert.Equal(faqQuestionDtoList, result.Value.Items));
    }

    private static List<FaqQuestion> GetFaqQuestionList()
    {
        var faqQuestionList = new List<FaqQuestion>
        {
            new()
            {
                Id = 4,
                QuestionText = new('4', 15),
                AnswerText = new('4', 60),
                Status = Status.Draft,
                Placements = [
                    new FaqPlacement { PageId = 1, QuestionId = 4, Priority = 3 },
                    new FaqPlacement { PageId = 2, QuestionId = 4, Priority = 3 },
                    new FaqPlacement { PageId = 3, QuestionId = 4, Priority = 1 },
                    ],
                CreatedAt = DateTime.UtcNow.AddMinutes(-40)
            },
            new()
            {
                Id = 2,
                QuestionText = new('2', 15),
                AnswerText = new('2', 60),
                Status = Status.Draft,
                Placements = [
                    new FaqPlacement { PageId = 1, QuestionId = 2, Priority = 2 },
                    new FaqPlacement { PageId = 2, QuestionId = 2, Priority = 1 },
                    ],
                CreatedAt = DateTime.UtcNow.AddMinutes(-20)
            },
            new()
            {
                Id = 3,
                QuestionText = new('3', 15),
                AnswerText = new('3', 60),
                Status = Status.Draft,
                Placements = [
                    new FaqPlacement { PageId = 2, QuestionId = 3, Priority = 2 },
                    ],
                CreatedAt = DateTime.UtcNow.AddMinutes(-30)
            },
            new()
            {
                Id = 5,
                QuestionText = new('5', 15),
                AnswerText = new('5', 60),
                Status = Status.Draft,
                Placements = [
                    new FaqPlacement { PageId = 2, QuestionId = 5, Priority = 4 },
                    new FaqPlacement { PageId = 3, QuestionId = 5, Priority = 2 },
                    ],
                CreatedAt = DateTime.UtcNow.AddMinutes(-50)
            },
            new()
            {
                Id = 1,
                QuestionText = new('1', 15),
                AnswerText = new('1', 60),
                Status = Status.Draft,
                Placements = [
                    new FaqPlacement { PageId = 1, QuestionId = 1, Priority = 1 },
                    ],
                CreatedAt = DateTime.UtcNow.AddMinutes(-10)
            },
        };

        return faqQuestionList;
    }

    private static List<FaqQuestionDto> GetFaqQuestionDtoList()
    {
        var faqQuestionDtoList = new List<FaqQuestionDto>
        {
            new()
            {
                Id = 1,
                QuestionText = new('1', 15),
                AnswerText = new('1', 60),
                Status = Status.Draft,
                PageIds = [1],
            },
            new()
            {
                Id = 3,
                QuestionText = new('3', 15),
                AnswerText = new('3', 60),
                Status = Status.Draft,
                PageIds = [2],
            },
            new()
            {
                Id = 2,
                QuestionText = new('2', 15),
                AnswerText = new('2', 60),
                Status = Status.Draft,
                PageIds = [1, 2],
            },
            new()
            {
                Id = 5,
                QuestionText = new('5', 15),
                AnswerText = new('5', 60),
                Status = Status.Draft,
                PageIds = [2, 3],
            },
            new()
            {
                Id = 4,
                QuestionText = new('4', 15),
                AnswerText = new('4', 60),
                Status = Status.Draft,
                PageIds = [1, 2, 3],
            },
        };

        return faqQuestionDtoList;
    }

    private static List<FaqPlacement> GetPlacementList()
    {
        var faqPlacementList = new List<FaqPlacement>
        {
            new() { PageId = 1, QuestionId = 4, Priority = 3 },
            new() { PageId = 2, QuestionId = 4, Priority = 3 },
            new() { PageId = 3, QuestionId = 4, Priority = 1 },

            new() { PageId = 1, QuestionId = 2, Priority = 2 },
            new() { PageId = 2, QuestionId = 2, Priority = 1 },

            new() { PageId = 2, QuestionId = 3, Priority = 2 },

            new() { PageId = 2, QuestionId = 5, Priority = 4 },
            new() { PageId = 3, QuestionId = 5, Priority = 2 },

            new() { PageId = 1, QuestionId = 1, Priority = 1 },
        };

        return faqPlacementList;
    }

    private void SetupRepository(List<FaqQuestion> faqQuestions, int count)
    {
        _mockRepository.Setup(repositoryWrapper => repositoryWrapper.FaqQuestionsRepository.GetAllAsync(
             It.IsAny<QueryOptions<FaqQuestion>>()))
            .ReturnsAsync(faqQuestions);

        _mockRepository.Setup(repositoryWrapper => repositoryWrapper.FaqQuestionsRepository.CountAsync(
             It.IsAny<Expression<Func<FaqQuestion, bool>>>()))
            .ReturnsAsync(count);
    }

    private void SetupMapper(FaqQuestionDto[] faqQuestionDtos)
    {
        _mockMapper
            .Setup(x => x.Map<FaqQuestionDto[]>(It.IsAny<List<FaqQuestion>>()))
            .Returns(faqQuestionDtos);
    }
}
