using System.Linq.Expressions;
using System.Transactions;
using AutoMapper;
using FluentResults;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;
using Moq;
using VictoryCenter.BLL.Commands.Admin.FaqQuestions.Create;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.FaqQuestions;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Enums;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.UnitTests.MediatRHandlersTests.Faq;

public class CreateFaqQuestionTests
{
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<IRepositoryWrapper> _repositoryWrapperMock;
    private readonly Mock<IValidator<CreateFaqQuestionCommand>> _validator;

    private readonly CreateFaqQuestionDto _createFaqQuestionDto = new()
    {
        QuestionText = "Some very smart question to ask.",
        AnswerText = "Some very smart answer to give, also I need to write more text so here it is.",
        PageIds = [1, 2],
        Status = Status.Draft,
    };

    private readonly FaqQuestion _faqQuestion = new()
    {
        Id = 1,
        QuestionText = "Some very smart question to ask.",
        AnswerText = "Some very smart answer to give, also I need to write more text so here it is.",
        Status = Status.Draft,
        CreatedAt = DateTime.UtcNow.AddMinutes(-10),
    };

    private readonly FaqQuestionDto _faqQuestionDto = new()
    {
        Id = 1,
        QuestionText = "Some very smart question to ask.",
        AnswerText = "Some very smart answer to give, also I need to write more text so here it is.",
        Status = Status.Draft,
        PageIds = [1, 2],
    };

    private readonly List<VisitorPage> _visitorPages = new()
    {
        new VisitorPage { Id = 1, Title = "Page 1" },
        new VisitorPage { Id = 2, Title = "Page 2" }
    };

    public CreateFaqQuestionTests()
    {
        _validator = new Mock<IValidator<CreateFaqQuestionCommand>>();
        _mapperMock = new Mock<IMapper>();
        _repositoryWrapperMock = new Mock<IRepositoryWrapper>();
    }

    [Fact]
    public async Task Handle_WhenCreationIsValid_ShouldReturnFaqQuestionDto()
    {
        SetupDependencies(_faqQuestionDto, _faqQuestion, 1);
        var handler = new CreateFaqQuestionHandler(_repositoryWrapperMock.Object, _mapperMock.Object, _validator.Object);

        Result<FaqQuestionDto> result =
            await handler.Handle(new CreateFaqQuestionCommand(_createFaqQuestionDto), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(_faqQuestionDto, result.Value);
    }

    [Fact]
    public async Task Handle_WhenSaveChangeFails_ShouldReturnFailure()
    {
        var failMessage = ErrorMessagesConstants.FailedToCreateEntity(typeof(FaqQuestion));
        SetupDependencies(_faqQuestionDto, _faqQuestion, -1);

        var handler = new CreateFaqQuestionHandler(_repositoryWrapperMock.Object, _mapperMock.Object, _validator.Object);

        Result<FaqQuestionDto> result =
            await handler.Handle(new CreateFaqQuestionCommand(_createFaqQuestionDto), CancellationToken.None);

        Assert.True(result.IsFailed);
        Assert.Null(result.ValueOrDefault);
        Assert.Equal(failMessage, result.Errors[0].Message);
    }

    [Fact]
    public async Task Handle_WhenPageIdIsInvalid_ShouldReturnFailure()
    {
        SetupDependencies(_faqQuestionDto, _faqQuestion, -1);
        _repositoryWrapperMock
            .Setup(repositoryWrapper =>
                repositoryWrapper.VisitorPagesRepository.GetAllAsync(
                    It.IsAny<QueryOptions<VisitorPage>>()))
            .ReturnsAsync([]);

        var handler = new CreateFaqQuestionHandler(_repositoryWrapperMock.Object, _mapperMock.Object, _validator.Object);

        Result<FaqQuestionDto> result =
            await handler.Handle(new CreateFaqQuestionCommand(_createFaqQuestionDto), CancellationToken.None);

        Assert.True(result.IsFailed);
        Assert.Null(result.ValueOrDefault);
        Assert.Equal(ErrorMessagesConstants.NotFound(1, typeof(VisitorPage)), result.Errors[0].Message);
    }

    [Fact]
    public async Task Handle_WhenDbExceptionThrown_ShouldReturnFailure()
    {
        var testMessage = "test message";
        SetupDependencies(_faqQuestionDto, _faqQuestion, -1);
        _repositoryWrapperMock
            .Setup(repositoryWrapperMock =>
                repositoryWrapperMock.FaqQuestionsRepository.CreateAsync(It.IsAny<FaqQuestion>()))
            .ThrowsAsync(new DbUpdateException(testMessage));

        var handler = new CreateFaqQuestionHandler(_repositoryWrapperMock.Object, _mapperMock.Object, _validator.Object);

        Result<FaqQuestionDto> result =
            await handler.Handle(new CreateFaqQuestionCommand(_createFaqQuestionDto), CancellationToken.None);

        Assert.True(result.IsFailed);
        Assert.Null(result.ValueOrDefault);
        Assert.Equal(ErrorMessagesConstants.FailedToCreateEntityInDatabase(typeof(FaqQuestion)) + testMessage, result.Errors[0].Message);
    }

    private void SetupDependencies(FaqQuestionDto faqQuestionDto, FaqQuestion faqQuestion, int isSuccess)
    {
        SetupMapper(faqQuestionDto, faqQuestion);
        SetupRepositoryWrapper(faqQuestion, isSuccess);
        SetupValidator();
    }

    private void SetupMapper(FaqQuestionDto faqQuestionDto, FaqQuestion faqQuestion)
    {
        _mapperMock.Setup(mapper => mapper.Map<FaqQuestion>(It.IsAny<CreateFaqQuestionDto>())).Returns(faqQuestion);
        _mapperMock.Setup(mapper => mapper.Map<FaqQuestionDto>(It.IsAny<FaqQuestion>())).Returns(faqQuestionDto);
    }

    private void SetupValidator()
    {
        _validator.Setup(v => v.ValidateAsync(It.IsAny<CreateFaqQuestionCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());
    }

    private void SetupRepositoryWrapper(FaqQuestion faqQuestion, int isSuccess)
    {
        _repositoryWrapperMock.Setup(repositoryWrapper => repositoryWrapper.FaqQuestionsRepository
                .CreateAsync(It.IsAny<FaqQuestion>()))
            .ReturnsAsync(faqQuestion);

        _repositoryWrapperMock.Setup(r => r.FaqPlacementsRepository.MaxAsync(It.IsAny<Expression<Func<FaqPlacement, long>>>(), It.IsAny<Expression<Func<FaqPlacement, bool>>?>()))
            .ReturnsAsync(1L);

        _repositoryWrapperMock.Setup(repositoryWrapper => repositoryWrapper.SaveChangesAsync())
            .ReturnsAsync(isSuccess);

        _repositoryWrapperMock.Setup(repositoryWrapper => repositoryWrapper.BeginTransaction())
            .Returns(new TransactionScope(TransactionScopeAsyncFlowOption.Enabled));

        _repositoryWrapperMock
            .Setup(repositoryWrapper =>
                repositoryWrapper.VisitorPagesRepository.GetAllAsync(
                    It.IsAny<QueryOptions<VisitorPage>>()))
            .ReturnsAsync(_visitorPages);
    }
}
