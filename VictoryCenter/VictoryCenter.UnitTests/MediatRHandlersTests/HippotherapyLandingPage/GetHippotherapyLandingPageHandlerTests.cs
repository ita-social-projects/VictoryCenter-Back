using AutoMapper;
using Moq;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.HippotherapyLandingPage;
using VictoryCenter.BLL.Queries.Admin.HippotherapyLandingPage.Get;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.UnitTests.MediatRHandlersTests.HippotherapyLandingPage;

public class GetHippotherapyLandingPageHandlerTests
{
    private readonly Mock<IRepositoryWrapper> _repositoryWrapperMock = new();
    private readonly Mock<IMapper> _mapperMock = new();

    [Fact]
    public async Task Handle_WhenPageExists_ShouldReturnMappedDto()
    {
        // Arrange
        var entity = new DAL.Entities.HippotherapyLandingPage { Id = 1 };
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
        };

        _repositoryWrapperMock
            .Setup(x => x.HippotherapyLandingPagesRepository.GetFirstOrDefaultAsync(
                It.IsAny<QueryOptions<DAL.Entities.HippotherapyLandingPage>>()))
            .ReturnsAsync(entity);

        _mapperMock
            .Setup(x => x.Map<HippotherapyLandingPageDto>(entity))
            .Returns(expectedDto);

        var handler = new GetHippotherapyLandingPageHandler(_repositoryWrapperMock.Object, _mapperMock.Object);

        // Act
        var result = await handler.Handle(new GetHippotherapyLandingPageQuery(), CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Same(expectedDto, result.Value);
    }

    [Fact]
    public async Task Handle_WhenPageDoesNotExist_ShouldReturnNotFoundError()
    {
        // Arrange
        _repositoryWrapperMock
            .Setup(x => x.HippotherapyLandingPagesRepository.GetFirstOrDefaultAsync(
                It.IsAny<QueryOptions<DAL.Entities.HippotherapyLandingPage>>()))
            .ReturnsAsync((DAL.Entities.HippotherapyLandingPage?)null);

        var handler = new GetHippotherapyLandingPageHandler(_repositoryWrapperMock.Object, _mapperMock.Object);

        // Act
        var result = await handler.Handle(new GetHippotherapyLandingPageQuery(), CancellationToken.None);

        // Assert
        Assert.True(result.IsFailed);
        Assert.Equal(ErrorMessagesConstants.NotFound(), result.Errors[0].Message);
        _mapperMock.Verify(x => x.Map<HippotherapyLandingPageDto>(It.IsAny<object>()), Times.Never);
    }
}
