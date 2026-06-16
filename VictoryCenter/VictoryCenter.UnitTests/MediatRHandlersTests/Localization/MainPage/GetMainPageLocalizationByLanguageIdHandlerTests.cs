using AutoMapper;
using Moq;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.Localization.MainPage;
using VictoryCenter.BLL.Queries.Admin.Localization.MainPage.GetByLanguageId;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Entities.Localization;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Interfaces.Localization.MainPage;
using VictoryCenter.DAL.Repositories.Interfaces.MainPage;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.UnitTests.MediatRHandlersTests.Localization.MainPage;

public class GetMainPageLocalizationByLanguageIdHandlerTests
{
    private readonly Mock<IMapper> _mapperMock = new();
    private readonly Mock<IRepositoryWrapper> _repositoryWrapperMock = new();
    private readonly Mock<IMainPageRepository> _mainPageRepositoryMock = new();
    private readonly Mock<IMainPageLocalizationsRepository> _mainPageLocalizationsRepositoryMock = new();
    private readonly Mock<IMainAboutUsLocalizationsRepository> _mainAboutUsLocalizationsRepositoryMock = new();
    private readonly Mock<IMainPartnersLocalizationsRepository> _mainPartnersLocalizationsRepositoryMock = new();
    private readonly Mock<IMainDonationsLocalizationsRepository> _mainDonationsLocalizationsRepositoryMock = new();

    public GetMainPageLocalizationByLanguageIdHandlerTests()
    {
        _repositoryWrapperMock.SetupGet(x => x.MainPageRepository).Returns(_mainPageRepositoryMock.Object);
        _repositoryWrapperMock.SetupGet(x => x.MainPageLocalizationsRepository).Returns(_mainPageLocalizationsRepositoryMock.Object);
        _repositoryWrapperMock.SetupGet(x => x.MainAboutUsLocalizationsRepository).Returns(_mainAboutUsLocalizationsRepositoryMock.Object);
        _repositoryWrapperMock.SetupGet(x => x.MainPartnersLocalizationsRepository).Returns(_mainPartnersLocalizationsRepositoryMock.Object);
        _repositoryWrapperMock.SetupGet(x => x.MainDonationsLocalizationsRepository).Returns(_mainDonationsLocalizationsRepositoryMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnAggregateLocalization_WhenLocalizationExists()
    {
        // Arrange
        const long entityId = 1;
        const long languageId = 2;

        var mainPage = new DAL.Entities.MainPage
        {
            Id = entityId,
            MainAboutUs = new MainAboutUs { Id = 10 },
            MainPartners = new MainPartners { Id = 11 },
            MainDonations = new MainDonations { Id = 12 },
        };

        var mainPageLocalization = new MainPageLocalization { EntityId = entityId, LanguageId = languageId };
        var aboutUsLocalization = new MainAboutUsLocalization { EntityId = 10, LanguageId = languageId };
        var partnersLocalization = new MainPartnersLocalization { EntityId = 11, LanguageId = languageId };
        var donationsLocalization = new MainDonationsLocalization { EntityId = 12, LanguageId = languageId };

        var dto = new MainPageLocalizationDto { EntityId = entityId };
        var aboutUsDto = new MainAboutUsLocalizationDto { EntityId = 10 };
        var partnersDto = new MainPartnersLocalizationDto { EntityId = 11 };
        var donationsDto = new MainDonationsLocalizationDto { EntityId = 12 };

        _mainPageRepositoryMock
            .Setup(x => x.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<DAL.Entities.MainPage>?>()))
            .ReturnsAsync(mainPage);
        _mainPageLocalizationsRepositoryMock
            .Setup(x => x.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<MainPageLocalization>?>()))
            .ReturnsAsync(mainPageLocalization);
        _mainAboutUsLocalizationsRepositoryMock
            .Setup(x => x.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<MainAboutUsLocalization>?>()))
            .ReturnsAsync(aboutUsLocalization);
        _mainPartnersLocalizationsRepositoryMock
            .Setup(x => x.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<MainPartnersLocalization>?>()))
            .ReturnsAsync(partnersLocalization);
        _mainDonationsLocalizationsRepositoryMock
            .Setup(x => x.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<MainDonationsLocalization>?>()))
            .ReturnsAsync(donationsLocalization);

        _mapperMock.Setup(x => x.Map<MainPageLocalizationDto>(mainPageLocalization)).Returns(dto);
        _mapperMock.Setup(x => x.Map<MainAboutUsLocalizationDto?>(aboutUsLocalization)).Returns(aboutUsDto);
        _mapperMock.Setup(x => x.Map<MainPartnersLocalizationDto?>(partnersLocalization)).Returns(partnersDto);
        _mapperMock.Setup(x => x.Map<MainDonationsLocalizationDto?>(donationsLocalization)).Returns(donationsDto);

        var handler = new GetMainPageLocalizationByLanguageIdHandler(_mapperMock.Object, _repositoryWrapperMock.Object);

        // Act
        var result = await handler.Handle(new GetMainPageLocalizationByLanguageIdQuery(entityId, languageId), CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(entityId, result.Value.EntityId);
        Assert.Equal(10, result.Value.MainAboutUs!.EntityId);
        Assert.Equal(11, result.Value.MainPartners!.EntityId);
        Assert.Equal(12, result.Value.MainDonations!.EntityId);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenMainPageLocalizationDoesNotExist()
    {
        // Arrange
        _mainPageRepositoryMock
            .Setup(x => x.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<DAL.Entities.MainPage>?>()))
            .ReturnsAsync(new DAL.Entities.MainPage { Id = 1 });
        _mainPageLocalizationsRepositoryMock
            .Setup(x => x.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<MainPageLocalization>?>()))
            .ReturnsAsync((MainPageLocalization?)null);

        var handler = new GetMainPageLocalizationByLanguageIdHandler(_mapperMock.Object, _repositoryWrapperMock.Object);

        // Act
        var result = await handler.Handle(new GetMainPageLocalizationByLanguageIdQuery(1, 2), CancellationToken.None);

        // Assert
        Assert.True(result.IsFailed);
        Assert.Equal(ErrorMessagesConstants.NotFound((1L, 2L), typeof(MainPageLocalization)), result.Errors[0].Message);
    }
}
