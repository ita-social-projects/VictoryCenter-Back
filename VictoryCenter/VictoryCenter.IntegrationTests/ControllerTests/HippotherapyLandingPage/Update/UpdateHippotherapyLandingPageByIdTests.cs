using System.Net;
using System.Text;
using System.Text.Json;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.HippotherapyLandingPage;
using VictoryCenter.BLL.DTOs.Admin.HippotherapyLandingPage.Sections;
using VictoryCenter.IntegrationTests.StorageLifecycle;
using VictoryCenter.IntegrationTests.Utils;
using VictoryCenter.IntegrationTests.Utils.DbFixture;

namespace VictoryCenter.IntegrationTests.ControllerTests.HippotherapyLandingPage.Update;

public class UpdateHippotherapyLandingPageByIdTests : BaseTestClass
{
    private readonly Uri _endpointUri = new("/api/HippotherapyPage", UriKind.Relative);

    public UpdateHippotherapyLandingPageByIdTests(IntegrationTestDbFixture fixture)
        : base(fixture)
    {
    }

    [StorageSqlServerFact]
    public async Task Put_WhenCardsAreReordered_ShouldKeepIdsAndReturnNewOrder_OnSqlServer()
    {
        var current = await GetPageAsync();
        var dto = ToUpdateDto(current);
        dto.AdvantagesSection.Cards.Reverse();
        var expectedIds = dto.AdvantagesSection.Cards.Select(c => c.Id!.Value).ToArray();

        var response = await PutAsync(dto);
        var updated = Deserialize(await response.Content.ReadAsStringAsync());

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(expectedIds, updated.AdvantagesSection.Cards.Select(c => c.Id).ToArray());
    }

    [StorageSqlServerFact]
    public async Task Put_WhenPrinciplesAreReordered_ShouldKeepIdsAndReturnNewOrder_OnSqlServer()
    {
        var current = await GetPageAsync();
        var dto = ToUpdateDto(current);
        dto.EthicsSection.Principles.Reverse();
        var expectedIds = dto.EthicsSection.Principles.Select(p => p.Id!.Value).ToArray();

        var response = await PutAsync(dto);
        var updated = Deserialize(await response.Content.ReadAsStringAsync());

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(expectedIds, updated.EthicsSection.Principles.Select(p => p.Id).ToArray());
    }

    [Fact]
    public async Task Put_WhenPrincipleTextChanges_ShouldKeepIdsAndRoundTripText()
    {
        var current = await GetPageAsync();
        var dto = ToUpdateDto(current);
        var targetId = dto.EthicsSection.Principles[0].Id;
        dto.EthicsSection.Principles[0] = new UpdateEthicsPrincipleDto { Id = targetId, Text = "Updated principle text" };

        var response = await PutAsync(dto);
        var updated = Deserialize(await response.Content.ReadAsStringAsync());

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var principle = updated.EthicsSection.Principles.Single(p => p.Id == targetId);
        Assert.Equal("Updated principle text", principle.Text);
    }

    [Fact]
    public async Task Put_WhenCardIsAddedWithoutId_ShouldReturnItWithGeneratedId()
    {
        var current = await GetPageAsync();
        var dto = ToUpdateDto(current);

        var newDescription = new string('A', HippotherapyLandingPageConstants.TextMinLength) + "_NEW";

        dto.ParticipantsSection.Cards[0] = new UpdateGalleryCardDto
        {
            Id = null,
            Description = newDescription,
            ImageId = null
        };

        var response = await PutAsync(dto);
        var updated = Deserialize(await response.Content.ReadAsStringAsync());

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var added = updated.ParticipantsSection.Cards.Single(c => c.Description == newDescription);
        Assert.True(added.Id > 0);
    }

    [Fact]
    public async Task Put_WhenCardIdIsUnknown_ShouldReturnNotFound()
    {
        var dto = ToUpdateDto(await GetPageAsync());
        dto.AdvantagesSection.Cards[0] = new UpdateGalleryCardDto { Id = 999999, Description = new string('A', HippotherapyLandingPageConstants.TextMinLength), ImageId = null };

        var response = await PutAsync(dto);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Put_WhenCardIdsAreDuplicated_ShouldReturnBadRequest()
    {
        var dto = ToUpdateDto(await GetPageAsync());
        dto.AdvantagesSection.Cards[1] = dto.AdvantagesSection.Cards[1] with { Id = dto.AdvantagesSection.Cards[0].Id };

        var response = await PutAsync(dto);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    private async Task<HippotherapyLandingPageDto> GetPageAsync()
    {
        var response = await Fixture.HttpClient.GetAsync(_endpointUri);
        response.EnsureSuccessStatusCode();
        return Deserialize(await response.Content.ReadAsStringAsync());
    }

    private Task<HttpResponseMessage> PutAsync(UpdateHippotherapyLandingPageDto dto) =>
        Fixture.HttpClient.PutAsync(
            _endpointUri,
            new StringContent(JsonSerializer.Serialize(dto, JsonOptions), Encoding.UTF8, "application/json"));

    private HippotherapyLandingPageDto Deserialize(string json) =>
        JsonSerializer.Deserialize<HippotherapyLandingPageDto>(json, JsonOptions)!;

    private static UpdateHippotherapyLandingPageDto ToUpdateDto(HippotherapyLandingPageDto page) => new()
    {
        IntroSection = new UpdateIntroSectionDto { Title = page.IntroSection.Title, Description = page.IntroSection.Description, ImageId = page.IntroSection.Image?.Id },
        DescriptionSection = new UpdateTextSectionDto { Title = page.DescriptionSection.Title, Description = page.DescriptionSection.Description },
        QuoteSection = new UpdateQuoteSectionDto { QuoteText = page.QuoteSection.QuoteText, AuthorName = page.QuoteSection.AuthorName, ImageId = page.QuoteSection.Image?.Id },
        HippoventionSection = new UpdateTextSectionDto { Title = page.HippoventionSection.Title, Description = page.HippoventionSection.Description },
        HippoventionCenterSection = new UpdateHippoventionCenterSectionDto
        {
            Title = page.HippoventionCenterSection.Title,
            Description = page.HippoventionCenterSection.Description,
            Pros = page.HippoventionCenterSection.Pros,
            ImageId = page.HippoventionCenterSection.Image?.Id,
        },
        AdvantagesSection = new UpdateGallerySectionDto
        {
            Title = page.AdvantagesSection.Title,
            Cards = page.AdvantagesSection.Cards
                .Select(c => new UpdateGalleryCardDto { Id = c.Id, Description = c.Description, ImageId = c.Image?.Id })
                .ToList(),
        },
        AnalysisSection = new UpdateTextSectionDto { Title = page.AnalysisSection.Title, Description = page.AnalysisSection.Description },
        ScientificReferencesSection = new UpdateScientificReferencesSectionDto
        {
            Title = page.ScientificReferencesSection.Title,
            Description = page.ScientificReferencesSection.Description,
            ScientificReferences = page.ScientificReferencesSection.ScientificReferences
                .Select(r => new UpdateScientificReferenceDto { Id = r.Id, Name = r.Name, Url = r.Url })
                .ToList(),
        },
        AnotherQuoteSection = new UpdateQuoteSectionDto { QuoteText = page.AnotherQuoteSection.QuoteText, AuthorName = page.AnotherQuoteSection.AuthorName, ImageId = page.AnotherQuoteSection.Image?.Id },
        ParticipantsSection = new UpdateGallerySectionDto
        {
            Title = page.ParticipantsSection.Title,
            Cards = page.ParticipantsSection.Cards
                .Select(c => new UpdateGalleryCardDto { Id = c.Id, Description = c.Description, ImageId = c.Image?.Id })
                .ToList(),
        },
        EthicsSection = new UpdateEthicsSectionDto
        {
            Title = page.EthicsSection.Title,
            Description = page.EthicsSection.Description,
            ImageId = page.EthicsSection.Image?.Id,
            Principles = page.EthicsSection.Principles
                .Select(p => new UpdateEthicsPrincipleDto { Id = p.Id, Text = p.Text })
                .ToList(),
        },
    };
}
