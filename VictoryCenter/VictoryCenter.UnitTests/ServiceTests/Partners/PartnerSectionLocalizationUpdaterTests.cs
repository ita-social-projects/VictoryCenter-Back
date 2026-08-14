using AutoMapper;
using Moq;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.Localization.PartnerSections;
using VictoryCenter.BLL.Interfaces.Localization;
using VictoryCenter.BLL.Services.Partners;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Entities.Localization;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Interfaces.Localization.Partners;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.UnitTests.ServiceTests.Partners;

public class PartnerSectionLocalizationUpdaterTests
{
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<IRepositoryWrapper> _mockRepositoryWrapper;
    private readonly Mock<ILocalizationService<Partner, PartnerLocalization>> _mockPartnerLocalizationService;
    private readonly Mock<IPartnerLocalizationsRepository> _mockPartnerLocalizationRepo;
    private readonly PartnerSectionLocalizationUpdater _updater;

    private readonly PartnerSection _section = new()
    {
        Id = 1,
        Partners =
        [
            new Partner { Id = 10, PartnersSectionId = 1, Description = "Original description 1" },
            new Partner { Id = 11, PartnersSectionId = 1, Description = "Original description 2" }
        ]
    };

    public PartnerSectionLocalizationUpdaterTests()
    {
        _mockMapper = new Mock<IMapper>();
        _mockRepositoryWrapper = new Mock<IRepositoryWrapper>();
        _mockPartnerLocalizationService = new Mock<ILocalizationService<Partner, PartnerLocalization>>();
        _mockPartnerLocalizationRepo = new Mock<IPartnerLocalizationsRepository>();

        _mockRepositoryWrapper.Setup(r => r.PartnerLocalizationsRepository)
            .Returns(_mockPartnerLocalizationRepo.Object);
        _mockRepositoryWrapper.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);
        _mockMapper.Setup(m => m.Map<PartnerLocalization>(It.IsAny<UpdatePartnerLocalizationItemDto>()))
            .Returns((UpdatePartnerLocalizationItemDto item) => new PartnerLocalization { EntityId = item.PartnerId, Description = item.Description });
        _mockMapper.Setup(m => m.Map<PartnerLocalizationItemDto>(It.IsAny<PartnerLocalization>()))
            .Returns((PartnerLocalization entity) => new PartnerLocalizationItemDto { PartnerId = entity.EntityId, Description = entity.Description });

        _updater = new PartnerSectionLocalizationUpdater(
            _mockMapper.Object, _mockRepositoryWrapper.Object, _mockPartnerLocalizationService.Object);
    }

    [Fact]
    public async Task UpsertPartnersAsync_ShouldFail_WhenPartnerIdDoesNotBelongToSection()
    {
        var partners = new List<UpdatePartnerLocalizationItemDto>
        {
            new() { PartnerId = 999, Description = "Description for unrelated partner" }
        };

        var result = await _updater.UpsertPartnersAsync(_section, partners, languageId: 2);

        Assert.True(result.IsFailed);
        Assert.Equal(ErrorMessagesConstants.NotFound([999L], typeof(Partner)), result.Errors[0].Message);
        _mockPartnerLocalizationRepo.Verify(r => r.GetAllAsync(It.IsAny<QueryOptions<PartnerLocalization>>()), Times.Never);
        _mockPartnerLocalizationService.Verify(s => s.TrackEntityLocalizationAsync(It.IsAny<IEnumerable<PartnerLocalization>>(), It.IsAny<bool>()), Times.Never);
    }

    [Fact]
    public async Task UpsertPartnersAsync_ShouldReportAllInvalidIds_WhenMultiplePartnersDoNotBelongToSection()
    {
        var partners = new List<UpdatePartnerLocalizationItemDto>
        {
            new() { PartnerId = 998, Description = "Unrelated 1" },
            new() { PartnerId = 999, Description = "Unrelated 2" }
        };

        var result = await _updater.UpsertPartnersAsync(_section, partners, languageId: 2);

        Assert.True(result.IsFailed);
        Assert.Equal(ErrorMessagesConstants.NotFound([998L, 999L], typeof(Partner)), result.Errors[0].Message);
    }

    [Fact]
    public async Task UpsertPartnersAsync_ShouldReturnEmptyResult_WithoutTouchingTheDatabase_WhenPartnersListIsEmpty()
    {
        var result = await _updater.UpsertPartnersAsync(_section, [], languageId: 2);

        Assert.True(result.IsSuccess);
        Assert.Empty(result.Value);
        _mockPartnerLocalizationRepo.Verify(r => r.GetAllAsync(It.IsAny<QueryOptions<PartnerLocalization>>()), Times.Never);
        _mockRepositoryWrapper.Verify(r => r.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task UpsertPartnersAsync_ShouldBatchCreate_WhenNoExistingLocalizations()
    {
        _mockPartnerLocalizationRepo.Setup(r => r.GetAllAsync(It.IsAny<QueryOptions<PartnerLocalization>>()))
            .ReturnsAsync([]);

        var partners = new List<UpdatePartnerLocalizationItemDto>
        {
            new() { PartnerId = 10, Description = "New localized description 1" },
            new() { PartnerId = 11, Description = "New localized description 2" }
        };

        var result = await _updater.UpsertPartnersAsync(_section, partners, languageId: 2);

        Assert.True(result.IsSuccess);
        Assert.Equal(2, result.Value.Count);

        _mockPartnerLocalizationRepo.Verify(r => r.GetAllAsync(It.IsAny<QueryOptions<PartnerLocalization>>()), Times.Once);
        _mockPartnerLocalizationService.Verify(
            s => s.TrackEntityLocalizationAsync(It.Is<IEnumerable<PartnerLocalization>>(l => l.Count() == 2), false),
            Times.Once);
        _mockPartnerLocalizationService.Verify(
            s => s.TrackEntityLocalizationAsync(It.IsAny<IEnumerable<PartnerLocalization>>(), true),
            Times.Never);
        _mockRepositoryWrapper.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task UpsertPartnersAsync_ShouldBatchUpdate_WhenLocalizationsAlreadyExist()
    {
        _mockPartnerLocalizationRepo.Setup(r => r.GetAllAsync(It.IsAny<QueryOptions<PartnerLocalization>>()))
            .ReturnsAsync(
            [
                new PartnerLocalization { EntityId = 10, LanguageId = 2 },
                new PartnerLocalization { EntityId = 11, LanguageId = 2 }
            ]);

        var partners = new List<UpdatePartnerLocalizationItemDto>
        {
            new() { PartnerId = 10, Description = "Updated localized description 1" },
            new() { PartnerId = 11, Description = "Updated localized description 2" }
        };

        var result = await _updater.UpsertPartnersAsync(_section, partners, languageId: 2);

        Assert.True(result.IsSuccess);
        _mockPartnerLocalizationService.Verify(
            s => s.TrackEntityLocalizationAsync(It.Is<IEnumerable<PartnerLocalization>>(l => l.Count() == 2), true),
            Times.Once);
        _mockPartnerLocalizationService.Verify(
            s => s.TrackEntityLocalizationAsync(It.IsAny<IEnumerable<PartnerLocalization>>(), false),
            Times.Never);
        _mockRepositoryWrapper.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task UpsertPartnersAsync_ShouldSplitIntoOneCreateBatchAndOneUpdateBatch_ForAMixOfNewAndExistingPartners()
    {
        _mockPartnerLocalizationRepo.Setup(r => r.GetAllAsync(It.IsAny<QueryOptions<PartnerLocalization>>()))
            .ReturnsAsync([new PartnerLocalization { EntityId = 10, LanguageId = 2 }]);

        var partners = new List<UpdatePartnerLocalizationItemDto>
        {
            new() { PartnerId = 10, Description = "Existing partner - will update" },
            new() { PartnerId = 11, Description = "New partner - will create" }
        };

        var result = await _updater.UpsertPartnersAsync(_section, partners, languageId: 2);

        Assert.True(result.IsSuccess);
        Assert.Equal(2, result.Value.Count);

        _mockPartnerLocalizationRepo.Verify(r => r.GetAllAsync(It.IsAny<QueryOptions<PartnerLocalization>>()), Times.Once);
        _mockPartnerLocalizationService.Verify(
            s => s.TrackEntityLocalizationAsync(It.Is<IEnumerable<PartnerLocalization>>(l => l.Count() == 1), true),
            Times.Once);
        _mockPartnerLocalizationService.Verify(
            s => s.TrackEntityLocalizationAsync(It.Is<IEnumerable<PartnerLocalization>>(l => l.Count() == 1), false),
            Times.Once);
        _mockRepositoryWrapper.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task UpsertPartnersAsync_ShouldThrow_WhenSaveChangesPersistsNothing()
    {
        _mockPartnerLocalizationRepo.Setup(r => r.GetAllAsync(It.IsAny<QueryOptions<PartnerLocalization>>()))
            .ReturnsAsync([]);
        _mockRepositoryWrapper.Setup(r => r.SaveChangesAsync()).ReturnsAsync(0);

        var partners = new List<UpdatePartnerLocalizationItemDto>
        {
            new() { PartnerId = 10, Description = "New localized description" }
        };

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => _updater.UpsertPartnersAsync(_section, partners, languageId: 2));
    }
}
