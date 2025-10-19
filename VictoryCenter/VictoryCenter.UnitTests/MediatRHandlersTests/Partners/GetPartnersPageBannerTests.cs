using AutoMapper;
using Moq;
using VictoryCenter.BLL.DTOs.Admin.Partners;
using VictoryCenter.BLL.DTOs.Common;
using VictoryCenter.BLL.Queries.Admin.Partners.GetBanner;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.UnitTests.MediatRHandlersTests.Partners;

public class GetPartnersPageBannerTests
{
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<IRepositoryWrapper> _mockRepoWrapper;

    private readonly PartnersPageBanner _bannerEntity = new()
    {
        Id = 1,
        Title = "Our partners displayed here",
        Description = "Super-puper description about cool partners. It's a banner guys...",
        ImageId = 10,
        Image = new Image { Id = 10, BlobName = "banner.jpg", MimeType = "image/jpeg" },
        CreatedAt = DateTimeOffset.UtcNow.AddDays(-5)
    };

    private readonly PartnersPageBannerDto _bannerDto = new()
    {
        Title = "Our partners displayed here",
        Description = "Super-puper description about cool partners. It's a banner guys...",
        Image = new ImageDto { Id = 10, Url = "http://storage/banner.jpg" }
    };

    public GetPartnersPageBannerTests()
    {
        _mockMapper = new Mock<IMapper>();
        _mockRepoWrapper = new Mock<IRepositoryWrapper>();
    }

    [Fact]
    public async Task Handle_ShouldReturnOkWithMappedDto()
    {
        // Arrange
        SetupMapper(_bannerDto);
        SetupRepositoryWrapper(_bannerEntity);
        var query = new GetPartnersPageBannerQuery();
        var handler = new GetPartnersPageBannerHandler(_mockRepoWrapper.Object, _mockMapper.Object);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.IsSuccess);
        Assert.Equal(_bannerDto, result.Value);

        _mockMapper.Verify(m => m.Map<PartnersPageBannerDto>(It.IsAny<PartnersPageBanner>()), Times.Once);
    }

    private void SetupMapper(PartnersPageBannerDto dtoToReturn)
    {
        _mockMapper
            .Setup(mapper => mapper.Map<PartnersPageBannerDto>(It.IsAny<PartnersPageBanner>()))
            .Returns(dtoToReturn);
    }

    private void SetupRepositoryWrapper(PartnersPageBanner? entityToReturn)
    {
        _mockRepoWrapper.Setup(
            repoWrapper => repoWrapper.PartnersPageBannersRepository.GetFirstOrDefaultAsync(
                It.IsAny<QueryOptions<PartnersPageBanner>>()))
            .ReturnsAsync(entityToReturn);
    }
}
