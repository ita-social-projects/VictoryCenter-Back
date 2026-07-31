using AutoMapper;
using Moq;
using VictoryCenter.BLL.DTOs.Admin.Localization.PartnersPageBanner;
using VictoryCenter.BLL.DTOs.Common;
using VictoryCenter.BLL.Queries.Admin.Localization.PartnersPageBanner.GetByLanguageId;
using VictoryCenter.DAL.Entities.Localization;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.UnitTests.MediatRHandlersTests.Localization.PartnersPageBanner;

public class GetPartnersPageBannerLocalizationByLanguageIdHandlerTests
{
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<IRepositoryWrapper> _mockRepositoryWrapper;
    private readonly GetPartnersPageBannerLocalizationByLanguageIdHandler _handler;

    private readonly long _entityId = 1;
    private readonly long _languageId = 2;

    private readonly PartnersPageBannerLocalization _localizationEntity = new()
    {
        EntityId = 1,
        LanguageId = 2,
        Title = "Localized banner title",
        Description = "Localized banner description"
    };

    private readonly PartnersPageBannerLocalizationDto _localizationDto = new()
    {
        EntityId = 1,
        LocalizationInfoDto = new LocalizationInfoDto { Id = 2, Code = "en" },
        Title = "Localized banner title",
        Description = "Localized banner description"
    };

    public GetPartnersPageBannerLocalizationByLanguageIdHandlerTests()
    {
        _mockMapper = new Mock<IMapper>();
        _mockRepositoryWrapper = new Mock<IRepositoryWrapper>();
        _handler = new GetPartnersPageBannerLocalizationByLanguageIdHandler(_mockMapper.Object, _mockRepositoryWrapper.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnLocalization_WhenItExists()
    {
        _mockRepositoryWrapper
            .Setup(r => r.PartnersPageBannerLocalizationsRepository.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<PartnersPageBannerLocalization>>()))
            .ReturnsAsync(_localizationEntity);
        _mockMapper.Setup(m => m.Map<PartnersPageBannerLocalizationDto>(_localizationEntity)).Returns(_localizationDto);

        var query = new GetPartnersPageBannerLocalizationByLanguageIdQuery(_entityId, _languageId);
        var result = await _handler.Handle(query, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(_localizationDto.Title, result.Value.Title);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenLocalizationDoesNotExist()
    {
        _mockRepositoryWrapper
            .Setup(r => r.PartnersPageBannerLocalizationsRepository.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<PartnersPageBannerLocalization>>()))
            .ReturnsAsync((PartnersPageBannerLocalization?)null);

        var query = new GetPartnersPageBannerLocalizationByLanguageIdQuery(_entityId, _languageId);
        var result = await _handler.Handle(query, CancellationToken.None);

        Assert.False(result.IsSuccess);
    }
}
