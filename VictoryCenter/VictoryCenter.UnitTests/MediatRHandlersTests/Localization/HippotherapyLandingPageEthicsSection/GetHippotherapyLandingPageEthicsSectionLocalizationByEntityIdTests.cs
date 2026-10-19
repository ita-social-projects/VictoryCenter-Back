using AutoMapper;
using Moq;
using VictoryCenter.BLL.DTOs.Admin.Localization.HippotherapyLandingPageEthicsSection;
using VictoryCenter.BLL.DTOs.Common;
using VictoryCenter.BLL.Queries.Admin.Localization.HippotherapyLandingPageEthicsSection.GetByEntityId;
using VictoryCenter.DAL.Entities.HippotherapyLandingPageContents;
using VictoryCenter.DAL.Entities.Localization;
using VictoryCenter.DAL.Enums;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.UnitTests.MediatRHandlersTests.Localization.HippotherapyLandingPageEthicsSection;

public class GetHippotherapyLandingPageEthicsSectionLocalizationByEntityIdTests
{
    private const long SectionId = 1;
    private const long EnglishId = 2;
    private const long GermanId = 3;

    private readonly Mock<IMapper> _mockMapper = new();
    private readonly Mock<IRepositoryWrapper> _mockRepositoryWrapper = new();
    private readonly GetHippotherapyLandingPageEthicsSectionLocalizationByEntityIdHandler _handler;

    public GetHippotherapyLandingPageEthicsSectionLocalizationByEntityIdTests()
    {
        _handler = new GetHippotherapyLandingPageEthicsSectionLocalizationByEntityIdHandler(
            _mockMapper.Object, _mockRepositoryWrapper.Object);

        _mockMapper
            .Setup(m => m.Map<HippotherapyLandingPageEthicsPrincipleLocalizationItemDto>(It.IsAny<HippotherapyLandingPageEthicsPrincipleLocalization>()))
            .Returns((object src) =>
            {
                var entity = (HippotherapyLandingPageEthicsPrincipleLocalization)src;
                return new HippotherapyLandingPageEthicsPrincipleLocalizationItemDto
                {
                    PrincipleId = entity.EntityId,
                    Text = entity.Text,
                    TranslationStatus = entity.TranslationStatus,
                };
            });
    }

    [Fact]
    public async Task Handle_ShouldReturnPrinciplesPerLanguage_WithEmptyItemsForMissingTranslations()
    {
        var sectionLocalizations = new List<HippotherapyLandingPageEthicsSectionLocalization>
        {
            new() { EntityId = SectionId, LanguageId = EnglishId, Title = "Who the programs are for" },
            new() { EntityId = SectionId, LanguageId = GermanId, Title = "Warum dieser Ansatz" },
        };
        SetupSectionLocalizations(sectionLocalizations);
        SetupPrinciples(
        [
            new HippotherapyLandingPageEthicsPrinciple { Id = 10, EthicsSectionId = SectionId, Priority = 1 },
            new HippotherapyLandingPageEthicsPrinciple { Id = 11, EthicsSectionId = SectionId, Priority = 2 },
        ]);
        SetupPrincipleLocalizations(
        [
            new HippotherapyLandingPageEthicsPrincipleLocalization
            {
                EntityId = 10, LanguageId = EnglishId, Text = "English principle 1", TranslationStatus = TranslationStatus.Relevant,
            },
            new HippotherapyLandingPageEthicsPrincipleLocalization
            {
                EntityId = 11, LanguageId = EnglishId, Text = "English principle 2", TranslationStatus = TranslationStatus.Outdated,
            },
            new HippotherapyLandingPageEthicsPrincipleLocalization
            {
                EntityId = 10, LanguageId = GermanId, Text = "German principle 1", TranslationStatus = TranslationStatus.Relevant,
            },
        ]);

        var result = await _handler.Handle(
            new GetHippotherapyLandingPageEthicsSectionLocalizationByEntityIdQuery(SectionId),
            CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(2, result.Value.Count);

        var english = result.Value[0];
        Assert.Equal(new[] { 10L, 11L }, english.Principles.Select(c => c.PrincipleId));
        Assert.Equal("English principle 1", english.Principles[0].Text);
        Assert.Equal(TranslationStatus.Outdated, english.Principles[1].TranslationStatus);

        var german = result.Value[1];
        Assert.Equal("German principle 1", german.Principles[0].Text);
        Assert.Equal(11, german.Principles[1].PrincipleId);
        Assert.Equal(string.Empty, german.Principles[1].Text);
        Assert.Null(german.Principles[1].TranslationStatus);
    }

    [Fact]
    public async Task Handle_ShouldReturnEmptyList_WhenSectionHasNoLocalizations()
    {
        SetupSectionLocalizations([]);

        var result = await _handler.Handle(
            new GetHippotherapyLandingPageEthicsSectionLocalizationByEntityIdQuery(SectionId),
            CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Empty(result.Value);
        _mockRepositoryWrapper.Verify(
            r => r.HippotherapyLandingPageEthicsPrinciplesRepository.GetAllAsync(It.IsAny<QueryOptions<HippotherapyLandingPageEthicsPrinciple>>()),
            Times.Never);
    }

    private void SetupSectionLocalizations(List<HippotherapyLandingPageEthicsSectionLocalization> localizations)
    {
        _mockRepositoryWrapper
            .Setup(r => r.HippotherapyLandingPageEthicsSectionLocalizationsRepository.GetAllAsync(
                It.IsAny<QueryOptions<HippotherapyLandingPageEthicsSectionLocalization>>()))
            .ReturnsAsync(localizations);
        _mockMapper
            .Setup(m => m.Map<List<HippotherapyLandingPageEthicsSectionLocalizationDto>>(It.IsAny<object>()))
            .Returns(localizations
                .Select(l => new HippotherapyLandingPageEthicsSectionLocalizationDto
                {
                    EntityId = l.EntityId,
                    LocalizationInfoDto = new LocalizationInfoDto { Id = l.LanguageId },
                    Title = l.Title,
                })
                .ToList());
    }

    private void SetupPrinciples(List<HippotherapyLandingPageEthicsPrinciple> principles)
    {
        _mockRepositoryWrapper
            .Setup(r => r.HippotherapyLandingPageEthicsPrinciplesRepository.GetAllAsync(
                It.IsAny<QueryOptions<HippotherapyLandingPageEthicsPrinciple>>()))
            .ReturnsAsync(principles);
    }

    private void SetupPrincipleLocalizations(List<HippotherapyLandingPageEthicsPrincipleLocalization> localizations)
    {
        _mockRepositoryWrapper
            .Setup(r => r.HippotherapyLandingPageEthicsPrincipleLocalizationsRepository.GetAllAsync(
                It.IsAny<QueryOptions<HippotherapyLandingPageEthicsPrincipleLocalization>>()))
            .ReturnsAsync(localizations);
    }
}
