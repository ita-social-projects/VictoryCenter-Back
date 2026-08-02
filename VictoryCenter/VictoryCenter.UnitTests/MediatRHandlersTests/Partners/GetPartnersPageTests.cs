using AutoMapper;
using Moq;
using VictoryCenter.BLL.DTOs.Common;
using VictoryCenter.BLL.DTOs.Public.Partners;
using VictoryCenter.BLL.Queries.Public.Partners.GetPage;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.UnitTests.MediatRHandlersTests.Partners;

public class GetPartnersPageTests
{
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<IRepositoryWrapper> _mockRepoWrapper;

    private readonly PartnersPageBanner _bannerEntity = new()
    {
        Id = 1,
        Title = "Test banner",
        Description = "Banner test description",
        Image = new Image { Id = 100 }
    };

    private readonly PublicPartnersPageBannerDto _bannerDto = new()
    {
        Title = "Test banner",
        Description = "Banner test description",
        Image = new ImageDto { Id = 100 }
    };

    private readonly List<PartnerSection> _partnerSectionEntities =
    [
        new() { Id = 10, Title = "Section 1" }
    ];

    private readonly List<PublicPartnersSectionDto> _partnerSectionDtos =
    [
        new() { Id = 10, Title = "Section 1" }
    ];

    public GetPartnersPageTests()
    {
        _mockMapper = new Mock<IMapper>();
        _mockRepoWrapper = new Mock<IRepositoryWrapper>();
    }

    [Fact]
    public async Task Handle_AllDataExists_ShouldReturnOkWithFullDto()
    {
        // Arrange
        SetupRepositoryWrapper(_bannerEntity, _partnerSectionEntities);
        SetupMapper(_bannerDto, _partnerSectionDtos);

        var handler = new GetPartnersPageHandler(_mockMapper.Object, _mockRepoWrapper.Object);
        var query = new GetPartnersPageQuery();

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(_bannerDto, result.Value.Banner);
        Assert.Equal(_partnerSectionDtos, result.Value.Sections);

        _mockMapper.Verify(m => m.Map<PublicPartnersPageBannerDto>(_bannerEntity), Times.Once);
        _mockMapper.Verify(m => m.Map<IEnumerable<PublicPartnersSectionDto>>(_partnerSectionEntities), Times.Once);
    }

    [Fact]
    public async Task Handle_BannerNotExists_ShouldReturnOkWithEmptyBannerDto()
    {
        // Arrange
        SetupRepositoryWrapper(null, _partnerSectionEntities);
        SetupMapper(null, _partnerSectionDtos);

        var handler = new GetPartnersPageHandler(_mockMapper.Object, _mockRepoWrapper.Object);
        var query = new GetPartnersPageQuery();

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);

        var resultBanner = result.Value.Banner;
        Assert.NotNull(resultBanner);
        Assert.Equal(string.Empty, resultBanner.Title);
        Assert.Equal(string.Empty, resultBanner.Description);
        Assert.Null(resultBanner.Image);

        Assert.Equal(_partnerSectionDtos, result.Value.Sections);

        _mockMapper.Verify(m => m.Map<PublicPartnersPageBannerDto>(It.IsAny<PartnersPageBanner>()), Times.Never);
    }

    [Fact]
    public async Task Handle_SectionsAreEmpty_ShouldReturnOkWithEmptySectionsList()
    {
        // Arrange
        var emptySectionsList = new List<PartnerSection>();
        var emptySectionsDtoList = new List<PublicPartnersSectionDto>();

        SetupRepositoryWrapper(_bannerEntity, emptySectionsList);
        SetupMapper(_bannerDto, emptySectionsDtoList);

        var handler = new GetPartnersPageHandler(_mockMapper.Object, _mockRepoWrapper.Object);
        var query = new GetPartnersPageQuery();

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(_bannerDto, result.Value.Banner);
        Assert.NotNull(result.Value.Sections);
        Assert.Empty(result.Value.Sections);
    }

    private void SetupMapper(PublicPartnersPageBannerDto? bannerDtoToReturn, IEnumerable<PublicPartnersSectionDto> sectionsDtoToReturn)
    {
        if (bannerDtoToReturn != null)
        {
            _mockMapper
                .Setup(mapper => mapper.Map<PublicPartnersPageBannerDto>(It.IsAny<PartnersPageBanner>()))
                .Returns(bannerDtoToReturn);
        }

        _mockMapper
            .Setup(mapper => mapper.Map<IEnumerable<PublicPartnersSectionDto>>(It.IsAny<IEnumerable<PartnerSection>>()))
            .Returns(sectionsDtoToReturn);
    }

    private void SetupRepositoryWrapper(PartnersPageBanner? bannerToReturn, IEnumerable<PartnerSection> sectionsToReturn)
    {
        _mockRepoWrapper.Setup(
            repo => repo.PartnersPageBannersRepository.GetFirstOrDefaultAsync(
                It.IsAny<QueryOptions<PartnersPageBanner>>()))
            .ReturnsAsync(bannerToReturn);

        _mockRepoWrapper.Setup(
            repo => repo.PartnerSectionsRepository.GetAllAsync(
                It.IsAny<QueryOptions<PartnerSection>>()))
            .ReturnsAsync(sectionsToReturn);
    }
}
