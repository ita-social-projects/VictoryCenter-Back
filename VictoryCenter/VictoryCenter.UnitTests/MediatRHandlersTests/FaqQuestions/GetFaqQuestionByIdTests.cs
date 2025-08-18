using AutoMapper;
using Moq;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.FaqQuestions;
using VictoryCenter.BLL.Queries.Admin.FaqQuestions.GetById;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Enums;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.UnitTests.MediatRHandlersTests.FaqQuestions;

public class GetFaqQuestionByIdTests
{
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<IRepositoryWrapper> _mockRepoWrapper;
    private readonly FaqQuestion _faqQuestionEntity = new()
    {
        Id = 1,
        QuestionText = new('Q', 15),
        AnswerText = new('A', 60),
        Status = Status.Draft,
        Placements = [
                    new FaqPlacement { PageId = 1, QuestionId = 1, Priority = 1 },
                    new FaqPlacement { PageId = 2, QuestionId = 1, Priority = 2 },
                    ],
        CreatedAt = DateTime.UtcNow.AddMinutes(-20)
    };
    private readonly FaqQuestionDto _faqQuestionDto = new()
    {
        Id = 1,
        QuestionText = new('Q', 15),
        AnswerText = new('A', 60),
        Status = Status.Draft,
        PageIds = [1, 2],
    };

    public GetFaqQuestionByIdTests()
    {
        _mockMapper = new Mock<IMapper>();
        _mockRepoWrapper = new Mock<IRepositoryWrapper>();
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(0)]
    [InlineData(1000)]
    public async Task Handle_EntityNotExists_ShouldReturnFail(long questionId)
    {
        SetupMapper(null!);
        SetupRepositoryWrapper();
        var query = new GetFaqQuestionByIdQuery(questionId);
        var handler = new GetFaqQuestionByIdHandler(_mockMapper.Object, _mockRepoWrapper.Object);

        var result = await handler.Handle(query, CancellationToken.None);

        Assert.NotNull(result);
        Assert.True(result.IsFailed);
        Assert.Equal(ErrorMessagesConstants.NotFound(questionId, typeof(FaqQuestion)), result.Errors[0].Message);
    }

    [Fact]
    public async Task Handle_EntityExists_ShouldReturnOk()
    {
        SetupMapper(_faqQuestionDto);
        SetupRepositoryWrapper(_faqQuestionEntity);
        var query = new GetFaqQuestionByIdQuery(_faqQuestionEntity.Id);
        var handler = new GetFaqQuestionByIdHandler(_mockMapper.Object, _mockRepoWrapper.Object);

        var result = await handler.Handle(query, CancellationToken.None);

        Assert.NotNull(result);
        Assert.True(result.IsSuccess);
        Assert.Equal(result.Value, _faqQuestionDto);
    }

    private void SetupMapper(FaqQuestionDto dtoToReturn)
    {
        _mockMapper.Setup(mapper => mapper.Map<FaqQuestionDto>(It.IsAny<FaqQuestion>())).Returns(dtoToReturn);
    }

    private void SetupRepositoryWrapper(FaqQuestion? entityToReturn = null)
    {
        _mockRepoWrapper.Setup(
            repoWrapper => repoWrapper.FaqQuestionsRepository.GetFirstOrDefaultAsync(
                It.IsAny<QueryOptions<FaqQuestion>>())).ReturnsAsync(entityToReturn);
    }
}
