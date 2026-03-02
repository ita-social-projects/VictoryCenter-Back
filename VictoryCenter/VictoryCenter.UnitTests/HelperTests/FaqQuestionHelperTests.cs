using Moq;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.Helpers;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Entities.HippotherapyProgramContents;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Interfaces.FaqQuestions;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.UnitTests.HelperTests;

public class FaqQuestionHelperTests
{
    private readonly Mock<IRepositoryWrapper> _wrapperMock = new();
    private readonly Mock<IFaqQuestionsRepository> _faqRepoMock = new();

    public FaqQuestionHelperTests()
    {
        _wrapperMock
            .SetupGet(w => w.FaqQuestionsRepository)
            .Returns(_faqRepoMock.Object);
    }

    [Fact]
    public async Task AssignSectionContentFaqQuestionsAsync_NoFaqContents_ReturnsOk()
    {
        var sections = new[] { SectionWithContents() };

        var result = await FaqQuestionHelper.AssignSectionContentFaqQuestionsAsync(
            _wrapperMock.Object, sections);

        Assert.True(result.IsSuccess);
        _faqRepoMock.Verify(r => r.GetAllAsync(It.IsAny<QueryOptions<FaqQuestion>>()), Times.Never);
    }

    [Fact]
    public async Task AssignSectionContentFaqQuestionsAsync_NullSections_ReturnsOk()
    {
        var result = await FaqQuestionHelper.AssignSectionContentFaqQuestionsAsync(
            _wrapperMock.Object, null!);

        Assert.True(result.IsSuccess);
        _faqRepoMock.Verify(r => r.GetAllAsync(It.IsAny<QueryOptions<FaqQuestion>>()), Times.Never);
    }

    [Fact]
    public async Task AssignSectionContentFaqQuestionsAsync_AllFound_ReturnsOk()
    {
        var faq1 = MakeFaqQuestion(1);
        var faq2 = MakeFaqQuestion(2);
        var content1 = MakeFaqContent(faqQuestionId: 1);
        var content2 = MakeFaqContent(faqQuestionId: 2);
        var sections = new[] { SectionWithContents(content1, content2) };

        _faqRepoMock
            .Setup(r => r.GetAllAsync(It.IsAny<QueryOptions<FaqQuestion>>()))
            .ReturnsAsync([faq1, faq2]);

        var result = await FaqQuestionHelper.AssignSectionContentFaqQuestionsAsync(
            _wrapperMock.Object, sections);

        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async Task AssignSectionContentFaqQuestionsAsync_AllFound_AssignsNavigationProperties()
    {
        var faq1 = MakeFaqQuestion(1);
        var faq2 = MakeFaqQuestion(2);
        var content1 = MakeFaqContent(faqQuestionId: 1);
        var content2 = MakeFaqContent(faqQuestionId: 2);
        var sections = new[] { SectionWithContents(content1, content2) };

        _faqRepoMock
            .Setup(r => r.GetAllAsync(It.IsAny<QueryOptions<FaqQuestion>>()))
            .ReturnsAsync([faq1, faq2]);

        await FaqQuestionHelper.AssignSectionContentFaqQuestionsAsync(
            _wrapperMock.Object, sections);

        Assert.Same(faq1, content1.FaqQuestion);
        Assert.Same(faq2, content2.FaqQuestion);
    }

    [Fact]
    public async Task AssignSectionContentFaqQuestionsAsync_MissingId_ReturnsFailed()
    {
        var content = MakeFaqContent(faqQuestionId: 99);
        var sections = new[] { SectionWithContents(content) };

        _faqRepoMock
            .Setup(r => r.GetAllAsync(It.IsAny<QueryOptions<FaqQuestion>>()))
            .ReturnsAsync([]);

        var result = await FaqQuestionHelper.AssignSectionContentFaqQuestionsAsync(
            _wrapperMock.Object, sections);

        Assert.True(result.IsFailed);
    }

    [Fact]
    public async Task AssignSectionContentFaqQuestionsAsync_MissingId_ReturnsExpectedErrorMessage()
    {
        var content = MakeFaqContent(faqQuestionId: 99);
        var sections = new[] { SectionWithContents(content) };

        _faqRepoMock
            .Setup(r => r.GetAllAsync(It.IsAny<QueryOptions<FaqQuestion>>()))
            .ReturnsAsync([]);

        var result = await FaqQuestionHelper.AssignSectionContentFaqQuestionsAsync(
            _wrapperMock.Object, sections);

        Assert.Equal(
            ErrorMessagesConstants.NotFound([99L], typeof(FaqQuestion)),
            result.Errors[0].Message);
    }

    private static HippotherapyProgramSection SectionWithContents(
        params FaqQuestionProgramContent[] contents)
    {
        return new HippotherapyProgramSection
        {
            Contents = [.. contents.Cast<ProgramSectionContent>()]
        };
    }

    private static FaqQuestionProgramContent MakeFaqContent(long faqQuestionId)
    {
        return new FaqQuestionProgramContent { FaqQuestionId = faqQuestionId };
    }

    private static FaqQuestion MakeFaqQuestion(long id)
    {
        return new FaqQuestion { Id = id, QuestionText = "Q", AnswerText = "A" };
    }
}
