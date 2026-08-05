using System.Reflection;
using AutoMapper;
using Moq;
using VictoryCenter.BLL;
using VictoryCenter.BLL.DTOs.Public.Partners;
using VictoryCenter.BLL.Interfaces.BlobStorage;
using VictoryCenter.BLL.Mapping.Images;
using VictoryCenter.BLL.Queries.Public.Partners.GetPage;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Entities.Localization;
using VictoryCenter.DAL.Enums;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.UnitTests.MediatRHandlersTests.Partners;

public class GetPartnersPageMappingTests
{
    private static readonly Type[] PublicLocalizationDtoTypes =
    [
        typeof(PublicPartnerSectionLocalizationDto),
        typeof(PublicPartnerLocalizationDto),
        typeof(PublicPartnersPageBannerLocalizationDto)
    ];

    [Theory]
    [MemberData(nameof(PublicLocalizationDtoTypeCases))]
    public void PublicLocalizationDtos_ShouldNotExposeAdminOnlyFields(Type dtoType)
    {
        var propertyNames = dtoType.GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Select(p => p.Name)
            .ToList();

        Assert.DoesNotContain("TranslationStatus", propertyNames);
        Assert.DoesNotContain("EntityId", propertyNames);
    }

    public static IEnumerable<object[]> PublicLocalizationDtoTypeCases()
    {
        return PublicLocalizationDtoTypes.Select(t => new object[] { t });
    }

    [Fact]
    public async Task Handle_ShouldMapLocalizationsThroughPublicDtos_WithRealMapper()
    {
        var language = new LocalizationLanguage { Id = 2, Code = "uk", Name = "Ukrainian" };

        var sectionLocalization = new PartnerSectionLocalization
        {
            EntityId = 1,
            LanguageId = 2,
            Language = language,
            Title = "Стратегічні партнери",
            Description = "Локалізований опис",
            TranslationStatus = TranslationStatus.Relevant
        };

        var partnerLocalization = new PartnerLocalization
        {
            EntityId = 10,
            LanguageId = 2,
            Language = language,
            Description = "Локалізований опис партнера",
            TranslationStatus = TranslationStatus.Outdated
        };

        var partner = new Partner
        {
            Id = 10,
            PartnersSectionId = 1,
            Description = "Base partner description",
            Image = new Image { Id = 100 },
            Localizations = [partnerLocalization]
        };

        var section = new PartnerSection
        {
            Id = 1,
            Title = "Strategic Partners",
            Description = "Base description",
            Localizations = [sectionLocalization],
            Partners = [partner]
        };

        var bannerLocalization = new PartnersPageBannerLocalization
        {
            EntityId = 5,
            LanguageId = 2,
            Language = language,
            Title = "Наші партнери",
            Description = "Локалізований опис банера",
            TranslationStatus = TranslationStatus.Relevant
        };

        var banner = new PartnersPageBanner
        {
            Id = 5,
            Title = "Our Trusted Partners",
            Description = "Base banner description",
            Localizations = [bannerLocalization]
        };

        var mockBlobService = new Mock<IBlobService>();
        mockBlobService.Setup(s => s.GetFileUrl(It.IsAny<string>(), It.IsAny<string>())).Returns("https://blob.test/image.png");

        var mapper = new MapperConfiguration(cfg =>
        {
            cfg.AddMaps(typeof(BllAssemblyMarker).Assembly);
            cfg.ConstructServicesUsing(type => type == typeof(BlobToUrlResolver)
                ? new BlobToUrlResolver(mockBlobService.Object)
                : Activator.CreateInstance(type)!);
        }).CreateMapper();

        var mockRepoWrapper = new Mock<IRepositoryWrapper>();
        mockRepoWrapper.Setup(r => r.PartnersPageBannersRepository.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<PartnersPageBanner>>()))
            .ReturnsAsync(banner);
        mockRepoWrapper.Setup(r => r.PartnerSectionsRepository.GetAllAsync(It.IsAny<QueryOptions<PartnerSection>>()))
            .ReturnsAsync([section]);

        var handler = new GetPartnersPageHandler(mapper, mockRepoWrapper.Object);

        var result = await handler.Handle(new GetPartnersPageQuery(), CancellationToken.None);

        Assert.True(result.IsSuccess);

        var sectionDto = Assert.Single(result.Value.Sections);
        var sectionLocalizationDto = Assert.Single(sectionDto.Localizations);
        Assert.Equal("uk", sectionLocalizationDto.Language.Code);
        Assert.Equal("Стратегічні партнери", sectionLocalizationDto.Title);
        Assert.Equal("Локалізований опис", sectionLocalizationDto.Description);

        var partnerDto = Assert.Single(sectionDto.Partners);
        var partnerLocalizationDto = Assert.Single(partnerDto.Localizations);
        Assert.Equal("uk", partnerLocalizationDto.Language.Code);
        Assert.Equal("Локалізований опис партнера", partnerLocalizationDto.Description);

        var bannerLocalizationDto = Assert.Single(result.Value.Banner.Localizations);
        Assert.Equal("uk", bannerLocalizationDto.Language.Code);
        Assert.Equal("Наші партнери", bannerLocalizationDto.Title);
    }
}
