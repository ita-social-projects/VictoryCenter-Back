using System.Transactions;
using FluentResults;
using Moq;
using VictoryCenter.BLL.Commands.Admin.HippotherapyPrograms.Delete;
using VictoryCenter.BLL.Constants;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Entities.HippotherapyProgramContents;
using VictoryCenter.DAL.Enums;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.UnitTests.MediatRHandlersTests.HippotherapyPrograms;

public class DeleteHippotherapyProgramTests
{
    private readonly Mock<IRepositoryWrapper> _repositoryWrapperMock;

    private readonly HippotherapyProgram _programEntity = new()
    {
        Id = 1,
        Name = "TestName",
        Description = "TestDescription",
        Status = Status.Draft,
        Categories = [],
        Sections = []
    };

    public DeleteHippotherapyProgramTests()
    {
        _repositoryWrapperMock = new Mock<IRepositoryWrapper>();
    }

    [Fact]
    public async Task Handle_ShouldDeleteProgram()
    {
        SetUpDependencies(_programEntity);
        var handler = new DeleteHippotherapyProgramHandler(_repositoryWrapperMock.Object);
        Result<long> result = await handler.Handle(new DeleteHippotherapyProgramCommand(1), CancellationToken.None);
        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async Task Handle_ShouldFailDelete_ProgramNotFound()
    {
        SetUpDependencies();
        var handler = new DeleteHippotherapyProgramHandler(_repositoryWrapperMock.Object);
        Result<long> result = await handler.Handle(new DeleteHippotherapyProgramCommand(1), CancellationToken.None);
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessagesConstants.NotFound(1, typeof(HippotherapyProgram)), result.Errors[0].Message);
    }

    [Fact]
    public async Task Handle_ShouldFailDelete_SaveFail()
    {
        SetUpDependencies(_programEntity, -1);
        var handler = new DeleteHippotherapyProgramHandler(_repositoryWrapperMock.Object);
        Result<long> result = await handler.Handle(new DeleteHippotherapyProgramCommand(1), CancellationToken.None);
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessagesConstants.FailedToDeleteEntity(typeof(HippotherapyProgram)), result.Errors[0].Message);
    }

    [Fact]
    public async Task Handle_ReturnsDeletedEntityId()
    {
        SetUpDependencies(_programEntity);
        var handler = new DeleteHippotherapyProgramHandler(_repositoryWrapperMock.Object);
        Result<long> result = await handler.Handle(new DeleteHippotherapyProgramCommand(1), CancellationToken.None);
        Assert.Equal(_programEntity.Id, result.Value);
    }

    [Fact]
    public async Task Handle_CallsDeleteOnRepository()
    {
        SetUpDependencies(_programEntity);
        var handler = new DeleteHippotherapyProgramHandler(_repositoryWrapperMock.Object);
        await handler.Handle(new DeleteHippotherapyProgramCommand(1), CancellationToken.None);
        _repositoryWrapperMock.Verify(r => r.HippotherapyProgramsRepository.Delete(_programEntity), Times.Once);
    }

    [Fact]
    public async Task Handle_BeginTransaction_CalledOnce()
    {
        SetUpDependencies(_programEntity);
        var handler = new DeleteHippotherapyProgramHandler(_repositoryWrapperMock.Object);
        await handler.Handle(new DeleteHippotherapyProgramCommand(1), CancellationToken.None);
        _repositoryWrapperMock.Verify(r => r.BeginTransaction(), Times.Once);
    }

    [Fact]
    public async Task Handle_WithNoFaqQuestions_CallsSaveChangesOnce()
    {
        SetUpDependencies(_programEntity);
        var handler = new DeleteHippotherapyProgramHandler(_repositoryWrapperMock.Object);
        await handler.Handle(new DeleteHippotherapyProgramCommand(1), CancellationToken.None);
        _repositoryWrapperMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        _repositoryWrapperMock.Verify(r => r.FaqQuestionsRepository.DeleteRange(It.IsAny<IEnumerable<FaqQuestion>>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WithFaqQuestions_CallsSaveChangesTwice()
    {
        var program = ProgramWithFaqContent(faqQuestionId: 10);
        SetUpDependencies(program, faqQuestions: [new FaqQuestion { Id = 10, QuestionText = "Q", AnswerText = "A" }]);
        var handler = new DeleteHippotherapyProgramHandler(_repositoryWrapperMock.Object);
        await handler.Handle(new DeleteHippotherapyProgramCommand(1), CancellationToken.None);
        _repositoryWrapperMock.Verify(r => r.SaveChangesAsync(), Times.Exactly(2));
    }

    [Fact]
    public async Task Handle_WithFaqQuestions_CallsDeleteRangeWithCorrectIds()
    {
        var faqQuestion = new FaqQuestion { Id = 10, QuestionText = "Q", AnswerText = "A" };
        var program = ProgramWithFaqContent(faqQuestionId: 10);
        SetUpDependencies(program, faqQuestions: [faqQuestion]);
        var handler = new DeleteHippotherapyProgramHandler(_repositoryWrapperMock.Object);
        await handler.Handle(new DeleteHippotherapyProgramCommand(1), CancellationToken.None);
        _repositoryWrapperMock.Verify(r => r.FaqQuestionsRepository.DeleteRange(It.Is<IEnumerable<FaqQuestion>>(list => list.Any(q => q.Id == 10))), Times.Once);
    }

    [Fact]
    public async Task Handle_FirstSaveChangesReturnsZero_DoesNotCallDeleteRangeOnFaqQuestions()
    {
        var program = ProgramWithFaqContent(faqQuestionId: 10);
        SetUpDependencies(program, saveResult: -1);
        var handler = new DeleteHippotherapyProgramHandler(_repositoryWrapperMock.Object);
        await handler.Handle(new DeleteHippotherapyProgramCommand(1), CancellationToken.None);
        _repositoryWrapperMock.Verify(r => r.FaqQuestionsRepository.DeleteRange(It.IsAny<IEnumerable<FaqQuestion>>()), Times.Never);
    }

    [Fact]
    public async Task Handle_FirstSaveChangesReturnsZero_DoesNotCallSecondSaveChanges()
    {
        var program = ProgramWithFaqContent(faqQuestionId: 10);
        SetUpDependencies(program, saveResult: -1);
        var handler = new DeleteHippotherapyProgramHandler(_repositoryWrapperMock.Object);
        await handler.Handle(new DeleteHippotherapyProgramCommand(1), CancellationToken.None);
        _repositoryWrapperMock.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    private static HippotherapyProgram ProgramWithFaqContent(long faqQuestionId)
    {
        var faqContent = new FaqQuestionProgramContent
        {
            FaqQuestionId = faqQuestionId,
            ContentType = ContentType.FaqQuestion,
            Order = 0
        };

        return new HippotherapyProgram
        {
            Id = 1,
            Name = "TestName",
            Description = "TestDescription",
            Status = Status.Draft,
            Categories = [],
            Sections =
            [
                new HippotherapyProgramSection
                {
                    Template = default,
                    Order = 0,
                    CreatedAt = DateTimeOffset.UtcNow,
                    Contents = [faqContent]
                },
            ]
        };
    }

    private void SetUpDependencies(
        HippotherapyProgram? program = null,
        int saveResult = 1,
        List<FaqQuestion>? faqQuestions = null)
    {
        _repositoryWrapperMock.Setup(r => r.HippotherapyProgramsRepository
            .GetFirstOrDefaultAsync(It.IsAny<QueryOptions<HippotherapyProgram>>())).ReturnsAsync(program);
        _repositoryWrapperMock.Setup(r => r.HippotherapyProgramsRepository.Delete(It.IsAny<HippotherapyProgram>()));
        _repositoryWrapperMock.Setup(r => r.FaqQuestionsRepository.GetAllAsync(It.IsAny<QueryOptions<FaqQuestion>>()))
            .ReturnsAsync(faqQuestions ?? []);
        _repositoryWrapperMock.Setup(r => r.FaqQuestionsRepository.DeleteRange(It.IsAny<IEnumerable<FaqQuestion>>()));
        _repositoryWrapperMock.Setup(r => r.SaveChangesAsync()).ReturnsAsync(saveResult);
        _repositoryWrapperMock.Setup(r => r.BeginTransaction())
            .Returns(new TransactionScope(TransactionScopeAsyncFlowOption.Enabled));
    }
}
