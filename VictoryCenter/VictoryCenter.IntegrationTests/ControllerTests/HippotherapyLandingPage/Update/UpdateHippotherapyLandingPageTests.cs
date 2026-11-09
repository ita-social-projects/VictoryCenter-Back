using System.Net;
using System.Text;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.DTOs.Admin.HippotherapyLandingPage;
using VictoryCenter.BLL.DTOs.Admin.HippotherapyLandingPage.Sections;
using VictoryCenter.DAL.Entities;
using VictoryCenter.IntegrationTests.Utils;
using VictoryCenter.IntegrationTests.Utils.DbFixture;

using EntityHippotherapyLandingPage = VictoryCenter.DAL.Entities.HippotherapyLandingPage;

namespace VictoryCenter.IntegrationTests.ControllerTests.HippotherapyLandingPage.Update;

public class UpdateHippotherapyLandingPageTests : BaseTestClass
{
    private readonly Uri _endpointUri = new("/api/HippotherapyPage", UriKind.Relative);

    public UpdateHippotherapyLandingPageTests(IntegrationTestDbFixture fixture)
        : base(fixture)
    {
    }

    [Fact]
    public async Task Update_ValidData_ShouldReturnOkAndUpdateEntity()
    {
        var existing = await GetSeededPageAsync();
        var existingReferenceCount = existing.ScientificReferencesSection!.ScientificReferences.Count;
        var image = await EnsureImageExistsAsync();

        var dto = CreateValidUpdateDto(existing, image.Id);

        var response = await PutRaw(dto);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        Fixture.DbContext.ChangeTracker.Clear();
        var updated = await Fixture.DbContext.HippotherapyLandingPages
            .Include(p => p.IntroSection)
            .Include(p => p.HippoventionCenterSection)
            .Include(p => p.ScientificReferencesSection).ThenInclude(s => s!.ScientificReferences)
            .Include(p => p.AdvantagesSection).ThenInclude(s => s!.AdvantageCards)
            .SingleAsync(p => p.Id == existing.Id);

        Assert.Equal("Updated intro title", updated.IntroSection!.Title);
        Assert.Equal("Updated pros text as a single string", updated.HippoventionCenterSection!.Pros);
        Assert.Equal(existingReferenceCount, updated.ScientificReferencesSection!.ScientificReferences.Count);
        Assert.Equal(4, updated.AdvantagesSection!.AdvantageCards.Count);
    }

    [Fact]
    public async Task Update_EmptyTitle_ShouldReturnBadRequest()
    {
        var existing = await GetSeededPageAsync();
        var image = await EnsureImageExistsAsync();

        var dto = CreateValidUpdateDto(existing, image.Id) with
        {
            IntroSection = CreateValidUpdateDto(existing, image.Id).IntroSection with { Title = string.Empty },
        };

        var response = await PutRaw(dto);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Update_WrongAdvantageCardsCount_ShouldReturnBadRequest()
    {
        var existing = await GetSeededPageAsync();
        var image = await EnsureImageExistsAsync();

        var baseDto = CreateValidUpdateDto(existing, image.Id);
        var dto = baseDto with
        {
            AdvantagesSection = baseDto.AdvantagesSection with
            {
                Cards = baseDto.AdvantagesSection.Cards.Take(2).ToList(),
            },
        };

        var response = await PutRaw(dto);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Update_NonExistentImageId_ShouldReturnNotFound()
    {
        var existing = await GetSeededPageAsync();
        var nonExistentImageId = (await Fixture.DbContext.Images.MaxAsync(i => (long?)i.Id) ?? 0) + 1000;

        var dto = CreateValidUpdateDto(existing, nonExistentImageId);

        var response = await PutRaw(dto);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Update_WhenPageDoesNotExist_ShouldCreatePage()
    {
        var seededRows = await Fixture.DbContext.HippotherapyLandingPages.ToListAsync();
        Fixture.DbContext.HippotherapyLandingPages.RemoveRange(seededRows);
        await Fixture.DbContext.SaveChangesAsync();
        Fixture.DbContext.ChangeTracker.Clear();

        var image = await EnsureImageExistsAsync();

        var dto = CreateValidUpdateDto(new EntityHippotherapyLandingPage(), image.Id);

        var response = await PutRaw(dto);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        Fixture.DbContext.ChangeTracker.Clear();
        var created = await Fixture.DbContext.HippotherapyLandingPages
            .Include(p => p.IntroSection)
            .SingleOrDefaultAsync();

        Assert.NotNull(created);
        Assert.Equal("Updated intro title", created.IntroSection!.Title);
    }

    [Fact]
    public async Task Update_ScientificReferences_ShouldAddNewAndDeleteOmitted()
    {
        var existing = await GetSeededPageAsync();
        var image = await EnsureImageExistsAsync();
        var keptReference = existing.ScientificReferencesSection!.ScientificReferences.First();

        var baseDto = CreateValidUpdateDto(existing, image.Id);
        var dto = baseDto with
        {
            ScientificReferencesSection = new UpdateScientificReferencesSectionDto
            {
                Title = "Updated references title",
                Description = "Updated references description",
                ScientificReferences =
                [
                    new UpdateScientificReferenceDto { Id = keptReference.Id, Name = keptReference.Name, Url = keptReference.Url },
                    new UpdateScientificReferenceDto { Id = null, Name = "Newly added reference", Url = "https://example.com/newly-added" },
                ],
            },
        };

        var response = await PutRaw(dto);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        Fixture.DbContext.ChangeTracker.Clear();
        var updated = await Fixture.DbContext.HippotherapyLandingPages
            .Include(p => p.ScientificReferencesSection).ThenInclude(s => s!.ScientificReferences)
            .SingleAsync(p => p.Id == existing.Id);

        Assert.Equal(2, updated.ScientificReferencesSection!.ScientificReferences.Count);
        Assert.Contains(updated.ScientificReferencesSection.ScientificReferences, r => r.Id == keptReference.Id);
        Assert.Contains(updated.ScientificReferencesSection.ScientificReferences, r => r.Name == "Newly added reference");
    }

    [Fact]
    public async Task Update_ReplacingAnImage_ShouldReturnOkAndDeleteTheOldImage()
    {
        var existing = await GetSeededPageAsync();
        var oldImage = await EnsureImageExistsAsync();

        var firstResponse = await PutRaw(CreateValidUpdateDto(existing, oldImage.Id));
        Assert.Equal(HttpStatusCode.OK, firstResponse.StatusCode);

        var newImage = await CreateImageAsync("https://example.com/replacement-image.jpg");

        Fixture.DbContext.ChangeTracker.Clear();
        existing = await GetSeededPageAsync();

        var response = await PutRaw(CreateValidUpdateDto(existing, newImage.Id));

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        Fixture.DbContext.ChangeTracker.Clear();
        var updated = await Fixture.DbContext.HippotherapyLandingPages
            .Include(p => p.IntroSection)
            .SingleAsync(p => p.Id == existing.Id);

        Assert.Equal(newImage.Id, updated.IntroSection!.ImageId);
        Assert.False(await Fixture.DbContext.Images.AnyAsync(i => i.Id == oldImage.Id));
    }

    private async Task<HttpResponseMessage> PutRaw(UpdateHippotherapyLandingPageDto payload)
    {
        var serialized = JsonSerializer.Serialize(payload);
        return await Fixture.HttpClient.PutAsync(
            _endpointUri,
            new StringContent(serialized, Encoding.UTF8, "application/json"));
    }

    private static UpdateHippotherapyLandingPageDto CreateValidUpdateDto(EntityHippotherapyLandingPage existing, long imageId)
    {
        var referenceIds = existing.ScientificReferencesSection?.ScientificReferences
            .OrderBy(r => r.Priority)
            .Select(r => (long?)r.Id)
            .ToList() ?? [null, null];

        var advantageCards = existing.AdvantagesSection?.AdvantageCards
            .OrderBy(c => c.Priority)
            .Select((c, i) => new UpdateGalleryCardDto { Id = c.Id, Description = $"Updated advantage {i + 1}", ImageId = c.ImageId })
            .ToList() ?? [];
        while (advantageCards.Count < 4)
        {
            advantageCards.Add(new UpdateGalleryCardDto { Description = $"Updated advantage {advantageCards.Count + 1}", ImageId = null });
        }

        advantageCards = advantageCards.Take(4).ToList();

        var participantCards = existing.ParticipantsSection?.ParticipantCards
            .OrderBy(c => c.Priority)
            .Select((c, i) => new UpdateGalleryCardDto { Id = c.Id, Description = $"Updated participant {i + 1}", ImageId = c.ImageId })
            .ToList() ?? [];
        while (participantCards.Count < 4)
        {
            participantCards.Add(new UpdateGalleryCardDto { Description = $"Updated participant {participantCards.Count + 1}", ImageId = null });
        }

        participantCards = participantCards.Take(4).ToList();

        var ethicsPrinciples = existing.EthicsSection?.EthicsPrinciples
            .OrderBy(p => p.Priority)
            .Select((p, i) => new UpdateEthicsPrincipleDto { Id = p.Id, Text = $"New principle {i + 1}" })
            .ToList() ?? [];
        while (ethicsPrinciples.Count < 4)
        {
            ethicsPrinciples.Add(new UpdateEthicsPrincipleDto { Text = $"New principle {ethicsPrinciples.Count + 1}" });
        }

        ethicsPrinciples = ethicsPrinciples.Take(4).ToList();

        return new UpdateHippotherapyLandingPageDto
        {
            IntroSection = new UpdateIntroSectionDto { Title = "Updated intro title", Description = "Updated intro description", ImageId = imageId },
            DescriptionSection = new UpdateTextSectionDto { Title = "Updated description title", Description = "Updated description text" },
            QuoteSection = new UpdateQuoteSectionDto { QuoteText = "Updated quote text", AuthorName = "Updated author", ImageId = imageId },
            HippoventionSection = new UpdateTextSectionDto { Title = "Updated hippovention title", Description = "Updated hippovention description" },
            HippoventionCenterSection = new UpdateHippoventionCenterSectionDto
            {
                Title = "Updated center title",
                Description = "Updated center description",
                Pros = "Updated pros text as a single string",
                ImageId = imageId,
            },
            AdvantagesSection = new UpdateGallerySectionDto
            {
                Title = "Updated advantages title",
                Cards = advantageCards,
            },
            AnalysisSection = new UpdateTextSectionDto { Title = "Updated analysis title", Description = "Updated analysis description" },
            ScientificReferencesSection = new UpdateScientificReferencesSectionDto
            {
                Title = "Updated references title",
                Description = "Updated references description",
                ScientificReferences = referenceIds
                    .Select((id, i) => new UpdateScientificReferenceDto { Id = id, Name = $"Updated reference {i + 1}", Url = $"https://example.com/updated-{i + 1}" })
                    .ToList(),
            },
            AnotherQuoteSection = new UpdateQuoteSectionDto { QuoteText = "Updated another quote", AuthorName = "Updated author two", ImageId = null },
            ParticipantsSection = new UpdateGallerySectionDto
            {
                Title = "Updated participants title",
                Cards = participantCards,
            },
            EthicsSection = new UpdateEthicsSectionDto
            {
                Title = "Updated ethics title",
                Description = "Updated ethics description",
                Principles = ethicsPrinciples,
                ImageId = existing.EthicsSection?.ImageId,
            },
        };
    }

    private async Task<EntityHippotherapyLandingPage> GetSeededPageAsync()
    {
        return await Fixture.DbContext.HippotherapyLandingPages
            .Include(p => p.IntroSection)
            .Include(p => p.DescriptionSection)
            .Include(p => p.QuoteSection)
            .Include(p => p.HippoventionSection)
            .Include(p => p.HippoventionCenterSection)
            .Include(p => p.AdvantagesSection).ThenInclude(s => s!.AdvantageCards)
            .Include(p => p.AnalysisSection)
            .Include(p => p.ScientificReferencesSection).ThenInclude(s => s!.ScientificReferences)
            .Include(p => p.AnotherQuoteSection)
            .Include(p => p.ParticipantsSection).ThenInclude(s => s!.ParticipantCards)
            .Include(p => p.EthicsSection).ThenInclude(s => s!.EthicsPrinciples)
            .SingleAsync();
    }

    private async Task<Image> CreateImageAsync(string url)
    {
        var image = new Image
        {
            BlobName = "replacement-image.jpg",
            MimeType = "image/jpeg",
            Url = url,
            CreatedAt = DateTimeOffset.UtcNow,
        };

        await Fixture.DbContext.Images.AddAsync(image);
        await Fixture.DbContext.SaveChangesAsync();

        return image;
    }

    private async Task<Image> EnsureImageExistsAsync()
    {
        var image = await Fixture.DbContext.Images.OrderBy(i => i.Id).FirstOrDefaultAsync();
        if (image is not null)
        {
            return image;
        }

        image = new Image
        {
            Url = "https://example.com/seed-image.jpg",
            CreatedAt = DateTimeOffset.UtcNow,
        };

        await Fixture.DbContext.Images.AddAsync(image);
        await Fixture.DbContext.SaveChangesAsync();

        return image;
    }
}
