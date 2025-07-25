using System.Transactions;
using Moq;
using VictoryCenter.BLL.Commands.Admin.FaqQuestions.Delete;
using VictoryCenter.BLL.Constants;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Enums;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.UnitTests.MediatRHandlersTests.Faq;

public class DeleteFaqQuestionTests
{
    private readonly Mock<IRepositoryWrapper> _mockRepoWrapper;
    private readonly FaqQuestion _existingFaqQuestion = new()
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

    public DeleteFaqQuestionTests()
    {
        _mockRepoWrapper = new Mock<IRepositoryWrapper>();
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(0)]
    [InlineData(1000)]
    public async Task Handle_DtoIsInvalid_ShouldReturnFail(long questionId)
    {
        SetupRepositoryWrapper();
        var command = new DeleteFaqQuestionCommand(questionId);
        var handler = new DeleteFaqQuestionHandler(_mockRepoWrapper.Object);

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.NotNull(result);
        Assert.True(result.IsFailed);
        Assert.Equal(ErrorMessagesConstants.NotFound(questionId, typeof(FaqQuestion)), result.Errors[0].Message);
    }

    [Fact]
    public async Task Handle_EntityExists_ShouldReturnOk()
    {
        SetupRepositoryWrapper(_existingFaqQuestion);
        var command = new DeleteFaqQuestionCommand(_existingFaqQuestion.Id);
        var handler = new DeleteFaqQuestionHandler(_mockRepoWrapper.Object);

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.NotNull(result);
        Assert.True(result.IsSuccess);
        Assert.Equal(result.Value, _existingFaqQuestion.Id);
    }

    [Fact]
    public async Task Handle_SaveChangesFails_ShouldReturnFail()
    {
        SetupRepositoryWrapper(_existingFaqQuestion, 0);
        var command = new DeleteFaqQuestionCommand(_existingFaqQuestion.Id);
        var handler = new DeleteFaqQuestionHandler(_mockRepoWrapper.Object);

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.NotNull(result);
        Assert.True(result.IsFailed);
        Assert.Equal(ErrorMessagesConstants.FailedToDeleteEntity(typeof(FaqQuestion)), result.Errors[0].Message);
    }

    private void SetupRepositoryWrapper(FaqQuestion? entityToDelete = null, int saveResult = 1)
    {
        _mockRepoWrapper.Setup(
            repoWrapper => repoWrapper.FaqQuestionsRepository.GetFirstOrDefaultAsync(
                It.IsAny<QueryOptions<FaqQuestion>>())).ReturnsAsync(entityToDelete);

        _mockRepoWrapper.Setup(
            repoWrapper => repoWrapper.FaqPlacementsRepository.GetAllAsync(
                It.IsAny<QueryOptions<FaqPlacement>>())).ReturnsAsync(entityToDelete?.Placements ?? []);

        _mockRepoWrapper.Setup(repoWrapper => repoWrapper.SaveChangesAsync()).ReturnsAsync(saveResult);

        _mockRepoWrapper.Setup(repoWrapper => repoWrapper.BeginTransaction())
            .Returns(new TransactionScope(TransactionScopeAsyncFlowOption.Enabled));
    }
}
