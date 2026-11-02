using AutoMapper;
using Moq;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.HippotherapyLandingPage;
using VictoryCenter.BLL.Queries.Admin.HippotherapyLandingPage.Get;
using VictoryCenter.BLL.Services.HippotherapyLandingPages;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;
using VictoryCenter.UnitTests.Utils.HippotherapyLandingPageTestDataHelper;

namespace VictoryCenter.UnitTests.MediatRHandlersTests.HippotherapyLandingPage;

public class GetHippotherapyLandingPageHandlerTests
{
    private readonly Mock<IRepositoryWrapper> _repositoryWrapperMock = new();
    private readonly Mock<IMapper> _mapperMock = new();
    private readonly Mock<IHippotherapyLandingPageResponseFactory> _responseFactoryMock = new();
    private readonly Mock<IHippotherapyLandingPageLocalizationBuilder> _builderMock = new();

    [Fact]
    public async Task Handle_WhenPageExists_ShouldReturnDtoCreatedByResponseFactory()
    {
        var entity = HippotherapyLandingPageTestDataHelper.CreateLandingPageEntity();
        var expected = CreateEmptyDto();
        SetupRepository(entity);
        _responseFactoryMock.Setup(x => x.Create(entity)).Returns(expected);

        var result = await CreateHandler().Handle(new GetHippotherapyLandingPageQuery(), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Same(expected, result.Value);
        _responseFactoryMock.Verify(x => x.Create(entity), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenPageDoesNotExist_ShouldReturnNotFoundAndNotCallFactory()
    {
        SetupRepository(null);

        var result = await CreateHandler().Handle(new GetHippotherapyLandingPageQuery(), CancellationToken.None);

        Assert.True(result.IsFailed);
        Assert.Equal(ErrorMessagesConstants.NotFound(), result.Errors[0].Message);
        _responseFactoryMock.Verify(x => x.Create(It.IsAny<DAL.Entities.HippotherapyLandingPage>()), Times.Never);
    }

    [Theory]
    [MemberData(nameof(SectionRemovers))]
    public async Task Handle_WhenAnySectionIsMissing_ShouldReturnNotFoundAndNotCallFactory(
        Action<DAL.Entities.HippotherapyLandingPage> removeSection)
    {
        var entity = HippotherapyLandingPageTestDataHelper.CreateLandingPageEntity();
        removeSection(entity);
        SetupRepository(entity);

        var result = await CreateHandler().Handle(new GetHippotherapyLandingPageQuery(), CancellationToken.None);

        Assert.True(result.IsFailed);
        Assert.Equal(ErrorMessagesConstants.NotFound(), result.Errors[0].Message);
        _responseFactoryMock.Verify(x => x.Create(It.IsAny<DAL.Entities.HippotherapyLandingPage>()), Times.Never);
    }

    public static IEnumerable<object[]> SectionRemovers() =>
        SectionRemoverActions().Select(action => new object[] { action });

    internal static IEnumerable<Action<DAL.Entities.HippotherapyLandingPage>> SectionRemoverActions()
    {
        yield return p => p.IntroSection = null;
        yield return p => p.DescriptionSection = null;
        yield return p => p.QuoteSection = null;
        yield return p => p.HippoventionSection = null;
        yield return p => p.HippoventionCenterSection = null;
        yield return p => p.AdvantagesSection = null;
        yield return p => p.AnalysisSection = null;
        yield return p => p.ScientificReferencesSection = null;
        yield return p => p.AnotherQuoteSection = null;
        yield return p => p.ParticipantsSection = null;
        yield return p => p.EthicsSection = null;
    }

    private void SetupRepository(DAL.Entities.HippotherapyLandingPage? entity) =>
        _repositoryWrapperMock
            .Setup(x => x.HippotherapyLandingPagesRepository.GetFirstOrDefaultAsync(
                It.IsAny<QueryOptions<DAL.Entities.HippotherapyLandingPage>>()))
            .ReturnsAsync(entity);

    private GetHippotherapyLandingPageHandler CreateHandler() =>
        new(_repositoryWrapperMock.Object,
            _responseFactoryMock.Object);

    private static HippotherapyLandingPageDto CreateEmptyDto() => new()
    {
        IntroSection = null!,
        DescriptionSection = null!,
        QuoteSection = null!,
        HippoventionSection = null!,
        HippoventionCenterSection = null!,
        AdvantagesSection = null!,
        AnalysisSection = null!,
        ScientificReferencesSection = null!,
        AnotherQuoteSection = null!,
        ParticipantsSection = null!,
        EthicsSection = null!,
    };
}
