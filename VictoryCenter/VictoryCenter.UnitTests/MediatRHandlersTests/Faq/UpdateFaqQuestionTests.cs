using System.Transactions;
using AutoMapper;
using FluentResults;
using FluentValidation;
using Moq;
using VictoryCenter.BLL.Commands.Admin.FaqQuestions.Update;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.FaqQuestions;
using VictoryCenter.BLL.Validators.FaqQuestions;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Enums;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.UnitTests.MediatRHandlersTests.Faq;

public class UpdateFaqQuestionTests
{
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<IRepositoryWrapper> _mockRepositoryWrapper;
    private readonly IValidator<UpdateFaqQuestionCommand> _validator;

    private readonly FaqQuestion _testExistingFaqQuestion = new()
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

    private readonly FaqQuestion _updatedFaqQuestion = new()
    {
        Id = 1,
        QuestionText = new('G', 20),
        AnswerText = new('R', 80),
        Status = Status.Published,
        Placements = [
                    new FaqPlacement { PageId = 1, QuestionId = 1, Priority = 2 },
                    new FaqPlacement { PageId = 3, QuestionId = 1, Priority = 2 },
                    ],
        CreatedAt = DateTime.UtcNow.AddMinutes(-20)
    };

    private readonly FaqQuestionDto _updatedFaqQuestionDto = new()
    {
        Id = 1,
        QuestionText = new('G', 20),
        AnswerText = new('R', 80),
        Status = Status.Published,
        PageIds = [1, 3],
    };

    private readonly UpdateFaqQuestionDto _updateFaqQuestionDto = new()
    {
        QuestionText = new('G', 20),
        AnswerText = new('R', 80),
        Status = Status.Published,
        PageIds = [1, 3],
    };

    private readonly List<FaqPlacement> _faqPlacements = [
        new FaqPlacement { PageId = 1, QuestionId = 1, Priority = 1 },
        new FaqPlacement { PageId = 2, QuestionId = 1, Priority = 1 },
    ];

    private readonly List<VisitorPage> _visitorPages = [
        new VisitorPage { Id = 1, Title = "Page 1" },
        new VisitorPage { Id = 2, Title = "Page 2" },
        new VisitorPage { Id = 3, Title = "Page 3" },
    ];

    public UpdateFaqQuestionTests()
    {
        var baseFaqQuestionsValidator = new BaseFaqQuestionValidator();
        _mockMapper = new Mock<IMapper>();
        _mockRepositoryWrapper = new Mock<IRepositoryWrapper>();
        _validator = new UpdateFaqQuestionValidator(baseFaqQuestionsValidator);
    }

    [Theory]
    [InlineData(1L)]
    [InlineData(3L)]
    [InlineData(2L, 3L)]
    [InlineData(1L, 2L, 3L)]
    public async Task Handle_ValidRequestWithDifferentPageIds_ShouldUpdateEntity(params long[] pageIds)
    {
        var existingFaqQuestion = _testExistingFaqQuestion;
        var updatedFaqQuestion = _updatedFaqQuestion;
        updatedFaqQuestion.Placements =
            pageIds.Select(id => new FaqPlacement { PageId = id, QuestionId = updatedFaqQuestion.Id, Priority = 1 }).ToList();
        var validUpdatedFaqQuestionDto = _updatedFaqQuestionDto with
        { PageIds = pageIds.ToList() };
        var validUpdateFaqQuestionDto = _updateFaqQuestionDto with
        { PageIds = pageIds.ToList() };
        SetupDependencies(existingFaqQuestion, updatedFaqQuestion);
        var handler = new UpdateFaqQuestionHandler(_mockMapper.Object, _mockRepositoryWrapper.Object, _validator);

        Result<FaqQuestionDto> result = await handler.Handle(
            new UpdateFaqQuestionCommand(validUpdateFaqQuestionDto, _testExistingFaqQuestion.Id), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(validUpdatedFaqQuestionDto, result.Value);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public async Task Handle_InvalidData_ShouldReturnValidationError(string? questionText)
    {
        var invalidUpdatedFaqQuestionDto = _updatedFaqQuestionDto with
        { QuestionText = questionText! };
        var invalidUpdateFaqQuestionDto = _updateFaqQuestionDto with
        { QuestionText = questionText! };
        SetupDependencies(_testExistingFaqQuestion);
        var handler = new UpdateFaqQuestionHandler(_mockMapper.Object, _mockRepositoryWrapper.Object, _validator);

        Result<FaqQuestionDto> result = await handler.Handle(
            new UpdateFaqQuestionCommand(invalidUpdateFaqQuestionDto, _testExistingFaqQuestion.Id), CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Contains(ErrorMessagesConstants.PropertyIsRequired(nameof(UpdateFaqQuestionDto.QuestionText)), result.Errors[0].Message);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(0)]
    public async Task Handle_FaqQuestionNotFound_ShouldReturnNotFoundError(long testId)
    {
        SetupDependencies();
        var handler = new UpdateFaqQuestionHandler(_mockMapper.Object, _mockRepositoryWrapper.Object, _validator);

        Result<FaqQuestionDto> result = await handler.Handle(
            new UpdateFaqQuestionCommand(_updateFaqQuestionDto, testId), CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessagesConstants.NotFound(testId, typeof(FaqQuestion)), result.Errors[0].Message);
    }

    [Fact]
    public async Task Handle_SaveChangesFails_ShouldReturnFailureError()
    {
        SetupDependencies(_testExistingFaqQuestion, _testExistingFaqQuestion, -1);
        var handler = new UpdateFaqQuestionHandler(_mockMapper.Object, _mockRepositoryWrapper.Object, _validator);

        Result<FaqQuestionDto> result = await handler.Handle(
            new UpdateFaqQuestionCommand(_updateFaqQuestionDto, _testExistingFaqQuestion.Id), CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessagesConstants.FailedToUpdateEntity(typeof(FaqQuestion)), result.Errors[0].Message);
    }

    private void SetupDependencies(FaqQuestion? faqQuestionToFind = null, FaqQuestion? faqQuestionToReturn = null, int saveResult = 1)
    {
        SetupMapper();
        SetupRepositoryWrapper(faqQuestionToFind, faqQuestionToReturn, saveResult);
    }

    private void SetupMapper()
    {
        _mockMapper.Setup(x => x.Map<UpdateFaqQuestionDto, FaqQuestion>(It.IsAny<UpdateFaqQuestionDto>()))
            .Returns(_updatedFaqQuestion);

        _mockMapper.Setup(x => x.Map<FaqQuestion, FaqQuestionDto>(It.IsAny<FaqQuestion>()))
            .Returns(_updatedFaqQuestionDto);
    }

    private void SetupRepositoryWrapper(FaqQuestion? faqQuestionToFind = null, FaqQuestion? faqQuestionToReturn = null, int saveResult = 1)
    {
        _mockRepositoryWrapper.SetupSequence(x => x.FaqQuestionsRepository.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<FaqQuestion>>()))
            .ReturnsAsync(faqQuestionToFind)
            .ReturnsAsync(faqQuestionToReturn);

        _mockRepositoryWrapper.Setup(x =>
                x.VisitorPagesRepository.GetAllAsync(It.IsAny<QueryOptions<VisitorPage>>()))
            .ReturnsAsync(_visitorPages);

        _mockRepositoryWrapper
        .Setup(x => x.FaqPlacementsRepository.GetAllAsync(It.IsAny<QueryOptions<FaqPlacement>>()))
        .ReturnsAsync((QueryOptions<FaqPlacement> options) =>
        {
            IQueryable<FaqPlacement> query = _faqPlacements.AsQueryable();

            if (options.Filter != null)
            {
                query = query.Where(options.Filter);
            }

            if (options.OrderByASC != null)
            {
                query = query.OrderBy(options.OrderByASC);
            }
            else if (options.OrderByDESC != null)
            {
                query = query.OrderByDescending(options.OrderByDESC);
            }

            if (options.Offset > 0)
            {
                query = query.Skip(options.Offset);
            }

            if (options.Limit > 0)
            {
                query = query.Take(options.Limit);
            }

            return query.ToList();
        });

        _mockRepositoryWrapper.Setup(x => x.SaveChangesAsync())
            .ReturnsAsync(saveResult);

        _mockRepositoryWrapper.Setup(x => x.BeginTransaction())
            .Returns(new TransactionScope(TransactionScopeAsyncFlowOption.Enabled));
    }
}
