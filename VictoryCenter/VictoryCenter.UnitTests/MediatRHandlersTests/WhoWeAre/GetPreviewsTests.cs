using Moq;
using VictoryCenter.BLL.Queries.Admin.WhoWeAreSections.GetPreviews;
using VictoryCenter.DAL.Enums;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Interfaces.WhoWeAreSections;
using VictoryCenter.DAL.Repositories.Models;

namespace VictoryCenter.UnitTests.MediatRHandlersTests.WhoWeAre;

public class GetPreviewsTests
{
    [Fact]
    public async Task Handle_EmptyRepositoryReturnsEmptyPreviewsAndPassesCancellation()
    {
        var repository = new Mock<IWhoWeAreSectionsRepository>(MockBehavior.Strict);
        using var cancellation = new CancellationTokenSource();
        repository.Setup(value => value.GetPreviewRowsAsync(cancellation.Token)).ReturnsAsync([]);
        var wrapper = new Mock<IRepositoryWrapper>();
        wrapper.SetupGet(value => value.WhoWeAreSectionsRepository).Returns(repository.Object);

        var result = await new GetWhoWeAreSectionPreviewsHandler(wrapper.Object)
            .Handle(new GetWhoWeAreSectionPreviewsQuery(), cancellation.Token);

        Assert.True(result.IsSuccess);
        Assert.Empty(result.Value);
        repository.Verify(value => value.GetPreviewRowsAsync(cancellation.Token), Times.Once);
        repository.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task Handle_ShapesAggregatedRowsWithoutDroppingSectionsOrMixingLanguages()
    {
        List<WhoWeAreSectionPreviewRow> rows =
        [
            new() { Id = 1, SectionType = SectionType.Main, Title = "Main", LanguageId = 2, TranslationStatus = TranslationStatus.Outdated },
            new() { Id = 1, SectionType = SectionType.Main, Title = "Main", LanguageId = 3, TranslationStatus = TranslationStatus.Relevant },
            new() { Id = 2, SectionType = SectionType.Team, Title = "Empty team" },
            new() { Id = 3, SectionType = SectionType.People, Title = "People", LanguageId = 2, TranslationStatus = TranslationStatus.Relevant }
        ];
        var repository = new Mock<IWhoWeAreSectionsRepository>(MockBehavior.Strict);
        repository.Setup(value => value.GetPreviewRowsAsync(CancellationToken.None)).ReturnsAsync(rows);
        var wrapper = new Mock<IRepositoryWrapper>();
        wrapper.SetupGet(value => value.WhoWeAreSectionsRepository).Returns(repository.Object);

        var result = await new GetWhoWeAreSectionPreviewsHandler(wrapper.Object)
            .Handle(new GetWhoWeAreSectionPreviewsQuery(), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Collection(
            result.Value,
            section =>
            {
                Assert.Equal(1, section.Id);
                Assert.Equal("Main", section.Title);
                Assert.Equal(nameof(SectionType.Main), section.SectionType);
                Assert.Collection(
                    section.TranslationStatuses,
                    status =>
                    {
                        Assert.Equal(2, status.LanguageId);
                        Assert.Equal(TranslationStatus.Outdated, status.TranslationStatus);
                    },
                    status =>
                    {
                        Assert.Equal(3, status.LanguageId);
                        Assert.Equal(TranslationStatus.Relevant, status.TranslationStatus);
                    });
            },
            section =>
            {
                Assert.Equal(2, section.Id);
                Assert.Equal("Empty team", section.Title);
                Assert.Equal(nameof(SectionType.Team), section.SectionType);
                Assert.Empty(section.TranslationStatuses);
            },
            section =>
            {
                Assert.Equal(3, section.Id);
                Assert.Equal(nameof(SectionType.People), section.SectionType);
                var status = Assert.Single(section.TranslationStatuses);
                Assert.Equal(2, status.LanguageId);
                Assert.Equal(TranslationStatus.Relevant, status.TranslationStatus);
            });
        repository.Verify(value => value.GetPreviewRowsAsync(CancellationToken.None), Times.Once);
        repository.VerifyNoOtherCalls();
    }
}
