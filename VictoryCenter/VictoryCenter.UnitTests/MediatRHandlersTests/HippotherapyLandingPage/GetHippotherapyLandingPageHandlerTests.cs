using AutoMapper;
using FluentResults;
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
    private readonly Mock<IHippotherapyLandingPageLocalizationBuilder> _localizationBuilderMock = new();

    [Fact]
    public async Task Handle_WhenPageExists_ShouldReturnMappedDtoWithAggregatedLocalizationsAndStatuses()
    {
        // Arrange
        var entity = HippotherapyLandingPageTestDataHelper.CreateLandingPageEntity();
        var initialDto = new HippotherapyLandingPageDto
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

        var localizationSource = HippotherapyLandingPageTestDataHelper.CreateFullLocalizationSource();

        _repositoryWrapperMock
            .Setup(x => x.HippotherapyLandingPagesRepository.GetFirstOrDefaultAsync(
                It.IsAny<QueryOptions<DAL.Entities.HippotherapyLandingPage>>()))
            .ReturnsAsync(entity);

        _mapperMock
            .Setup(x => x.Map<HippotherapyLandingPageDto>(entity))
            .Returns(initialDto);

        _localizationBuilderMock
           .Setup(x => x.Build(entity).ToResult());

        var handler = CreateHandler();

        // Act
        var result = await handler.Handle(new GetHippotherapyLandingPageQuery(), CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.NotEmpty(result.Value.Localizations);
        Assert.NotEmpty(result.Value.TranslationStatuses);
        _localizationBuilderMock.Verify(x => x.Build(entity).ToResult());
    }

    [Fact]
    public async Task Handle_WhenPageDoesNotExist_ShouldReturnNotFoundErrorAndNotCallLoader()
    {
        // Arrange
        _repositoryWrapperMock
            .Setup(x => x.HippotherapyLandingPagesRepository.GetFirstOrDefaultAsync(
                It.IsAny<QueryOptions<DAL.Entities.HippotherapyLandingPage>>()))
            .ReturnsAsync((DAL.Entities.HippotherapyLandingPage?)null);

        var handler = CreateHandler();

        // Act
        var result = await handler.Handle(new GetHippotherapyLandingPageQuery(), CancellationToken.None);

        // Assert
        Assert.True(result.IsFailed);
        Assert.Equal(ErrorMessagesConstants.NotFound(), result.Errors[0].Message);
        _mapperMock.Verify(x => x.Map<HippotherapyLandingPageDto>(It.IsAny<object>()), Times.Never);
        _localizationBuilderMock.Verify(
            x => x.Build(It.IsAny<DAL.Entities.HippotherapyLandingPage>()).ToResult());
    }

    private GetHippotherapyLandingPageHandler CreateHandler() =>
       new(_repositoryWrapperMock.Object, _mapperMock.Object, _localizationBuilderMock.Object);
}
