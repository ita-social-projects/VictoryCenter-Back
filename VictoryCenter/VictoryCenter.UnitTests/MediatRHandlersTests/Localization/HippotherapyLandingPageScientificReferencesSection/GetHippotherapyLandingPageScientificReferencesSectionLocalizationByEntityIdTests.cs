using AutoMapper;
using Moq;
using VictoryCenter.BLL.DTOs.Admin.Localization.HippotherapyLandingPageScientificReferencesSection;
using VictoryCenter.BLL.DTOs.Common;
using VictoryCenter.BLL.Queries.Admin.Localization.HippotherapyLandingPageScientificReferencesSection.GetByEntityId;
using VictoryCenter.DAL.Entities.HippotherapyLandingPageContents;
using VictoryCenter.DAL.Entities.Localization;
using VictoryCenter.DAL.Enums;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.UnitTests.MediatRHandlersTests.Localization.HippotherapyLandingPageScientificReferencesSection;

public class GetHippotherapyLandingPageScientificReferencesSectionLocalizationByEntityIdTests
{
    private const long SectionId = 1;
    private const long EnglishId = 2;
    private const long GermanId = 3;

    private readonly Mock<IMapper> _mockMapper = new();
    private readonly Mock<IRepositoryWrapper> _mockRepositoryWrapper = new();
    private readonly GetHippotherapyLandingPageScientificReferencesSectionLocalizationByEntityIdHandler _handler;

    public GetHippotherapyLandingPageScientificReferencesSectionLocalizationByEntityIdTests()
    {
        _handler = new GetHippotherapyLandingPageScientificReferencesSectionLocalizationByEntityIdHandler(
            _mockMapper.Object, _mockRepositoryWrapper.Object);

        _mockMapper
            .Setup(m => m.Map<HippotherapyLandingPageScientificReferenceLocalizationItemDto>(It.IsAny<HippotherapyLandingPageScientificReferenceLocalization>()))
            .Returns((object src) =>
            {
                var entity = (HippotherapyLandingPageScientificReferenceLocalization)src;
                return new HippotherapyLandingPageScientificReferenceLocalizationItemDto
                {
                    ReferenceId = entity.EntityId,
                    Name = entity.Name,
                    TranslationStatus = entity.TranslationStatus,
                };
            });
    }

    [Fact]
    public async Task Handle_ShouldReturnReferencesPerLanguage_WithEmptyItemsForMissingTranslations()
    {
        var sectionLocalizations = new List<HippotherapyLandingPageScientificReferencesSectionLocalization>
        {
            new() { EntityId = SectionId, LanguageId = EnglishId, Title = "Who the programs are for" },
            new() { EntityId = SectionId, LanguageId = GermanId, Title = "Warum dieser Ansatz" },
        };
        SetupSectionLocalizations(sectionLocalizations);
        SetupReferences(
        [
            new HippotherapyLandingPageScientificReference { Id = 10, ScientificReferencesSectionId = SectionId, Priority = 1 },
            new HippotherapyLandingPageScientificReference { Id = 11, ScientificReferencesSectionId = SectionId, Priority = 2 },
        ]);
        SetupReferenceLocalizations(
        [
            new HippotherapyLandingPageScientificReferenceLocalization
            {
                EntityId = 10, LanguageId = EnglishId, Name = "English reference 1", TranslationStatus = TranslationStatus.Relevant,
            },
            new HippotherapyLandingPageScientificReferenceLocalization
            {
                EntityId = 11, LanguageId = EnglishId, Name = "English reference 2", TranslationStatus = TranslationStatus.Outdated,
            },
            new HippotherapyLandingPageScientificReferenceLocalization
            {
                EntityId = 10, LanguageId = GermanId, Name = "German reference 1", TranslationStatus = TranslationStatus.Relevant,
            },
        ]);

        var result = await _handler.Handle(
            new GetHippotherapyLandingPageScientificReferencesSectionLocalizationByEntityIdQuery(SectionId),
            CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(2, result.Value.Count);

        var english = result.Value[0];
        Assert.Equal(new[] { 10L, 11L }, english.References.Select(c => c.ReferenceId));
        Assert.Equal("English reference 1", english.References[0].Name);
        Assert.Equal(TranslationStatus.Outdated, english.References[1].TranslationStatus);

        var german = result.Value[1];
        Assert.Equal("German reference 1", german.References[0].Name);
        Assert.Equal(11, german.References[1].ReferenceId);
        Assert.Equal(string.Empty, german.References[1].Name);
        Assert.Null(german.References[1].TranslationStatus);
    }

    [Fact]
    public async Task Handle_ShouldReturnEmptyList_WhenSectionHasNoLocalizations()
    {
        SetupSectionLocalizations([]);

        var result = await _handler.Handle(
            new GetHippotherapyLandingPageScientificReferencesSectionLocalizationByEntityIdQuery(SectionId),
            CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Empty(result.Value);
        _mockRepositoryWrapper.Verify(
            r => r.HippotherapyLandingPageScientificReferencesRepository.GetAllAsync(It.IsAny<QueryOptions<HippotherapyLandingPageScientificReference>>()),
            Times.Never);
    }

    private void SetupSectionLocalizations(List<HippotherapyLandingPageScientificReferencesSectionLocalization> localizations)
    {
        _mockRepositoryWrapper
            .Setup(r => r.HippotherapyLandingPageScientificReferencesSectionLocalizationsRepository.GetAllAsync(
                It.IsAny<QueryOptions<HippotherapyLandingPageScientificReferencesSectionLocalization>>()))
            .ReturnsAsync(localizations);
        _mockMapper
            .Setup(m => m.Map<List<HippotherapyLandingPageScientificReferencesSectionLocalizationDto>>(It.IsAny<object>()))
            .Returns(localizations
                .Select(l => new HippotherapyLandingPageScientificReferencesSectionLocalizationDto
                {
                    EntityId = l.EntityId,
                    LocalizationInfoDto = new LocalizationInfoDto { Id = l.LanguageId },
                    Title = l.Title,
                })
                .ToList());
    }

    private void SetupReferences(List<HippotherapyLandingPageScientificReference> references)
    {
        _mockRepositoryWrapper
            .Setup(r => r.HippotherapyLandingPageScientificReferencesRepository.GetAllAsync(
                It.IsAny<QueryOptions<HippotherapyLandingPageScientificReference>>()))
            .ReturnsAsync(references);
    }

    private void SetupReferenceLocalizations(List<HippotherapyLandingPageScientificReferenceLocalization> localizations)
    {
        _mockRepositoryWrapper
            .Setup(r => r.HippotherapyLandingPageScientificReferenceLocalizationsRepository.GetAllAsync(
                It.IsAny<QueryOptions<HippotherapyLandingPageScientificReferenceLocalization>>()))
            .ReturnsAsync(localizations);
    }
}
