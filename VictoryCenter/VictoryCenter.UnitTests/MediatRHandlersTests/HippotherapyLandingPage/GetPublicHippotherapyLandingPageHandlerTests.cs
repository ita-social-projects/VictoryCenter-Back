using Moq;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.HippotherapyLandingPage;
using VictoryCenter.BLL.Queries.Public.HippotherapyLandingPage.Get;
using VictoryCenter.BLL.Services.HippotherapyLandingPages;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;
using VictoryCenter.UnitTests.Utils.HippotherapyLandingPageTestDataHelper;

namespace VictoryCenter.UnitTests.MediatRHandlersTests.HippotherapyLandingPage;

public class GetPublicHippotherapyLandingPageHandlerTests
{
    private readonly Mock<IRepositoryWrapper> _repositoryWrapperMock = new();
    private readonly Mock<IHippotherapyLandingPageResponseFactory> _responseFactoryMock = new();

    [Fact]
    public async Task Handle_WhenPageExists_ShouldReturnDtoCreatedByResponseFactory()
    {
        // Arrange
        var entity = HippotherapyLandingPageTestDataHelper.CreateLandingPageEntity();
        var expectedDto = new HippotherapyLandingPageDto
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
            Localizations = [],
            TranslationStatuses = [],
        };

        SetupRepository(entity);
        _responseFactoryMock.Setup(x => x.Create(entity)).Returns(expectedDto);

        var handler = CreateHandler();

        // Act
        var result = await handler.Handle(new GetPublicHippotherapyLandingPageQuery(), CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Empty(result.Value.TranslationStatuses);
        Assert.Equal(expectedDto.Localizations, result.Value.Localizations);
        _responseFactoryMock.Verify(x => x.Create(entity), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenPageDoesNotExist_ShouldReturnNotFoundError()
    {
        // Arrange
        SetupRepository(null);

        var handler = CreateHandler();

        // Act
        var result = await handler.Handle(new GetPublicHippotherapyLandingPageQuery(), CancellationToken.None);

        // Assert
        Assert.True(result.IsFailed);
        Assert.Equal(ErrorMessagesConstants.NotFound(), result.Errors[0].Message);
        _responseFactoryMock.Verify(x => x.Create(It.IsAny<DAL.Entities.HippotherapyLandingPage>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WhenSectionIsMissing_ShouldReturnNotFoundError()
    {
        // Arrange
        var entity = HippotherapyLandingPageTestDataHelper.CreateLandingPageEntity();
        entity.IntroSection = null;
        SetupRepository(entity);

        var handler = CreateHandler();

        // Act
        var result = await handler.Handle(new GetPublicHippotherapyLandingPageQuery(), CancellationToken.None);

        // Assert
        Assert.True(result.IsFailed);
        Assert.Equal(ErrorMessagesConstants.NotFound(), result.Errors[0].Message);
        _responseFactoryMock.Verify(x => x.Create(It.IsAny<DAL.Entities.HippotherapyLandingPage>()), Times.Never);
    }

    private void SetupRepository(DAL.Entities.HippotherapyLandingPage? entity) =>
        _repositoryWrapperMock
            .Setup(x => x.HippotherapyLandingPagesRepository.GetFirstOrDefaultAsync(
                It.IsAny<QueryOptions<DAL.Entities.HippotherapyLandingPage>>()))
            .ReturnsAsync(entity);

    private GetPublicHippotherapyLandingPageHandler CreateHandler() =>
        new(_repositoryWrapperMock.Object, _responseFactoryMock.Object);
}
