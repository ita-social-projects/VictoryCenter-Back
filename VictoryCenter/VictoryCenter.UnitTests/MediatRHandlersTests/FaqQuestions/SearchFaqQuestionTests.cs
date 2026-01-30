using AutoMapper;
using FluentValidation;
using Moq;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.FaqQuestions;
using VictoryCenter.BLL.Interfaces.Search;
using VictoryCenter.BLL.Queries.Admin.FaqQuestions.Search;
using VictoryCenter.BLL.Validators.FaqQuestions;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.UnitTests.MediatRHandlersTests.FaqQuestions;

public class SearchFaqQuestionsTests
{
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<IRepositoryWrapper> _repositoryWrapperMock;
    private readonly Mock<ISearchService<FaqQuestion>> _searchServiceMock;
    private readonly IValidator<SearchFaqQuestionQuery> _validator;

    private readonly List<FaqQuestion> _faqQuestions =
    [
        CreateFaqQuestion(1, "Test question?")
    ];

    private readonly List<FaqQuestionDto> _faqQuestionDtos =
    [
        CreateFaqQuestionDto(1, "Test question?")
    ];

    public SearchFaqQuestionsTests()
    {
        _mapperMock = new Mock<IMapper>();
        _repositoryWrapperMock = new Mock<IRepositoryWrapper>();
        _searchServiceMock = new Mock<ISearchService<FaqQuestion>>();
        _validator = new SearchFaqQuestionValidator();
    }

    [Fact]
    public async Task Handle_ExistingSearchQuery_ShouldReturnNotEmpty()
    {
        // Arrange
        SetupMapper(_faqQuestionDtos);
        SetupRepositoryWrapper(_faqQuestions);

        var dto = new SearchFaqQuestionDto { SearchQuery = "Test" };
        var query = new SearchFaqQuestionQuery(dto);
        var handler = new SearchFaqQuestionHandler(
            _mapperMock.Object,
            _repositoryWrapperMock.Object,
            _validator,
            _searchServiceMock.Object);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(_faqQuestionDtos, result.Value.Items);
    }

    [Fact]
    public async Task Handle_NonexistentSearchQuery_ShouldReturnEmpty()
    {
        // Arrange
        SetupMapper([]);
        SetupRepositoryWrapper([]);

        var dto = new SearchFaqQuestionDto { SearchQuery = "Nonexistent" };
        var query = new SearchFaqQuestionQuery(dto);
        var handler = new SearchFaqQuestionHandler(
            _mapperMock.Object,
            _repositoryWrapperMock.Object,
            _validator,
            _searchServiceMock.Object);

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

        var dto = new SearchFaqQuestionDto { SearchQuery = "" };
        var query = new SearchFaqQuestionQuery(dto);
        var handler = new SearchFaqQuestionHandler(
            _mapperMock.Object,
            _repositoryWrapperMock.Object,
            _validator,
            _searchServiceMock.Object);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Contains(
            ErrorMessagesConstants.PropertyIsRequired(nameof(SearchFaqQuestionDto.SearchQuery)),
            result.Errors[0].Message);
    }

    private static FaqQuestion CreateFaqQuestion(int id, string questionText)
    {
        return new FaqQuestion
        {
            Id = id,
            QuestionText = questionText,
            AnswerText = "Answer",
            CreatedAt = DateTimeOffset.UtcNow
        };
    }

    private static FaqQuestionDto CreateFaqQuestionDto(int id, string questionText)
    {
        return new FaqQuestionDto
        {
            Id = id,
            QuestionText = questionText,
            AnswerText = "Answer"
        };
    }

    private void SetupMapper(List<FaqQuestionDto> faqQuestionDtos)
    {
        _mapperMock
            .Setup(mapper =>
                mapper.Map<List<FaqQuestionDto>>(It.IsAny<List<FaqQuestion>>()))
            .Returns(faqQuestionDtos);
    }

    private void SetupRepositoryWrapper(List<FaqQuestion> faqQuestionsToReturn)
    {
        _repositoryWrapperMock
            .Setup(x => x.FaqQuestionsRepository.GetAllAsync(
                It.IsAny<QueryOptions<FaqQuestion>>()))
            .ReturnsAsync(faqQuestionsToReturn);

        _repositoryWrapperMock
            .Setup(x => x.FaqQuestionsRepository.CountAsync(
                It.IsAny<QueryOptions<FaqQuestion>>()))
            .ReturnsAsync(faqQuestionsToReturn.Count);
    }
}
