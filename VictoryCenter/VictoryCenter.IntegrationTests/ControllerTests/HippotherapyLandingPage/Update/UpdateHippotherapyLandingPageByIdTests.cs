using System.Net;
using System.Text;
using System.Text.Json;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.HippotherapyLandingPage;
using VictoryCenter.BLL.DTOs.Admin.HippotherapyLandingPage.Sections;
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

    [Fact]
    public Task Put_WhenCardsAreReordered_ShouldKeepIdsAndReturnNewOrder() =>
        WithRestoredPageAsync(async page =>
        {
            var current = await EnsureAdvantagesCardsCountAsync(page, 2);
            var dto = ToValidUpdateDto(current);
            dto.AdvantagesSection.Cards.Reverse();
            var expectedIds = dto.AdvantagesSection.Cards.Select(c => c.Id!.Value).ToArray();

            var response = await PutAsync(dto);
            await AssertStatusCodeAsync(HttpStatusCode.OK, response);

            var updated = Deserialize(await response.Content.ReadAsStringAsync());
            Assert.Equal(expectedIds, updated.AdvantagesSection.Cards.Select(c => c.Id).ToArray());
        });

    [Fact]
    public Task Put_WhenPrinciplesAreReordered_ShouldKeepIdsAndReturnNewOrder() =>
        WithRestoredPageAsync(async page =>
        {
            var current = await EnsurePrinciplesCountAsync(page, 2);
            var dto = ToValidUpdateDto(current);
            dto.EthicsSection.Principles.Reverse();
            var expectedIds = dto.EthicsSection.Principles.Select(p => p.Id!.Value).ToArray();

            var response = await PutAsync(dto);
            await AssertStatusCodeAsync(HttpStatusCode.OK, response);

            var updated = Deserialize(await response.Content.ReadAsStringAsync());
            Assert.Equal(expectedIds, updated.EthicsSection.Principles.Select(p => p.Id).ToArray());
        });

    [Fact]
    public Task Put_WhenPrincipleTextChanges_ShouldKeepIdsAndRoundTripText() =>
        WithRestoredPageAsync(async page =>
        {
            var current = await EnsurePrinciplesCountAsync(page, 1);
            var dto = ToValidUpdateDto(current);
            var targetId = dto.EthicsSection.Principles[0].Id;
            dto.EthicsSection.Principles[0] = new UpdateEthicsPrincipleDto
            {
                Id = targetId,
                Text = "Updated principle text",
            };

            var response = await PutAsync(dto);
            await AssertStatusCodeAsync(HttpStatusCode.OK, response);

            var updated = Deserialize(await response.Content.ReadAsStringAsync());
            var principle = updated.EthicsSection.Principles.Single(p => p.Id == targetId);
            Assert.Equal("Updated principle text", principle.Text);
        });

    [Fact]
    public Task Put_WhenCardIsAddedWithoutId_ShouldReturnItWithGeneratedId() =>
        WithRestoredPageAsync(async page =>
        {
            var dto = ToValidUpdateDto(page);
            var newDescription = ValidDescription("_NEW");

            var newCard = new UpdateGalleryCardDto
            {
                Id = null,
                Description = newDescription,
                ImageId = null,
            };

            if (dto.ParticipantsSection.Cards.Count > 0)
            {
                dto.ParticipantsSection.Cards[0] = newCard;
            }
            else
            {
                dto.ParticipantsSection.Cards.Add(newCard);
            }

            var response = await PutAsync(dto);
            await AssertStatusCodeAsync(HttpStatusCode.OK, response);

            var updated = Deserialize(await response.Content.ReadAsStringAsync());
            var added = updated.ParticipantsSection.Cards.Single(c => c.Description == newDescription);
            Assert.True(added.Id > 0);
        });

    [Fact]
    public async Task Put_WhenCardIdIsUnknown_ShouldReturnNotFound()
    {
        var dto = ToValidUpdateDto(await GetPageAsync());
        var unknownCard = new UpdateGalleryCardDto
        {
            Id = 999999,
            Description = ValidDescription(),
            ImageId = null,
        };

        if (dto.AdvantagesSection.Cards.Count > 0)
        {
            dto.AdvantagesSection.Cards[0] = unknownCard;
        }
        else
        {
            dto.AdvantagesSection.Cards.Add(unknownCard);
        }

        var response = await PutAsync(dto);

        await AssertStatusCodeAsync(HttpStatusCode.NotFound, response);
    }

    [Fact]
    public Task Put_WhenCardIdsAreDuplicated_ShouldReturnBadRequest() =>
        WithRestoredPageAsync(async page =>
        {
            var current = await EnsureAdvantagesCardsCountAsync(page, 2);
            var dto = ToValidUpdateDto(current);
            dto.AdvantagesSection.Cards[1] = dto.AdvantagesSection.Cards[1] with
            {
                Id = dto.AdvantagesSection.Cards[0].Id,
            };

            var response = await PutAsync(dto);

            await AssertStatusCodeAsync(HttpStatusCode.BadRequest, response);
        });

    private async Task WithRestoredPageAsync(Func<HippotherapyLandingPageDto, Task> testBody)
    {
        var original = await GetPageAsync();
        try
        {
            await testBody(original);
        }
        finally
        {
            await PutAsync(ToValidUpdateDto(original));
        }
    }

    private async Task<HippotherapyLandingPageDto> EnsureAdvantagesCardsCountAsync(
        HippotherapyLandingPageDto page,
        int minCount)
    {
        if (page.AdvantagesSection.Cards.Count >= minCount)
        {
            return page;
        }

        var dto = ToValidUpdateDto(page);
        while (dto.AdvantagesSection.Cards.Count < minCount)
        {
            dto.AdvantagesSection.Cards.Add(new UpdateGalleryCardDto
            {
                Id = null,
                Description = ValidDescription($"_{dto.AdvantagesSection.Cards.Count}"),
                ImageId = null,
            });
        }

        var response = await PutAsync(dto);
        await AssertStatusCodeAsync(HttpStatusCode.OK, response);
        return Deserialize(await response.Content.ReadAsStringAsync());
    }

    private async Task<HippotherapyLandingPageDto> EnsurePrinciplesCountAsync(
        HippotherapyLandingPageDto page,
        int minCount)
    {
        if (page.EthicsSection.Principles.Count >= minCount)
        {
            return page;
        }

        var dto = ToValidUpdateDto(page);
        while (dto.EthicsSection.Principles.Count < minCount)
        {
            dto.EthicsSection.Principles.Add(new UpdateEthicsPrincipleDto
            {
                Id = null,
                Text = $"Valid principle {dto.EthicsSection.Principles.Count + 1}",
            });
        }

        var response = await PutAsync(dto);
        await AssertStatusCodeAsync(HttpStatusCode.OK, response);
        return Deserialize(await response.Content.ReadAsStringAsync());
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

    private static async Task AssertStatusCodeAsync(HttpStatusCode expected, HttpResponseMessage response)
    {
        if (response.StatusCode != expected)
        {
            var body = await response.Content.ReadAsStringAsync();
            Assert.Fail($"Expected {(int)expected} ({expected}), got {(int)response.StatusCode} ({response.StatusCode}). Body: {body}");
        }
    }

    private static string ValidDescription(string suffix = "") =>
        new string('A', HippotherapyLandingPageConstants.TextMinLength) + suffix;

    private static string EnsureText(string? value, string fallback) =>
        string.IsNullOrWhiteSpace(value) ? fallback : value;

    private static UpdateHippotherapyLandingPageDto ToValidUpdateDto(HippotherapyLandingPageDto page) => new()
    {
        IntroSection = new UpdateIntroSectionDto
        {
            Title = EnsureText(page.IntroSection.Title, "Valid intro title"),
            Description = EnsureText(page.IntroSection.Description, ValidDescription()),
            ImageId = page.IntroSection.Image?.Id,
        },
        DescriptionSection = new UpdateTextSectionDto
        {
            Title = EnsureText(page.DescriptionSection.Title, "Valid description title"),
            Description = EnsureText(page.DescriptionSection.Description, ValidDescription()),
        },
        QuoteSection = new UpdateQuoteSectionDto
        {
            QuoteText = EnsureText(page.QuoteSection.QuoteText, ValidDescription()),
            AuthorName = EnsureText(page.QuoteSection.AuthorName, "Valid author"),
            ImageId = page.QuoteSection.Image?.Id,
        },
        HippoventionSection = new UpdateTextSectionDto
        {
            Title = EnsureText(page.HippoventionSection.Title, "Valid hippovention title"),
            Description = EnsureText(page.HippoventionSection.Description, ValidDescription()),
        },
        HippoventionCenterSection = new UpdateHippoventionCenterSectionDto
        {
            Title = EnsureText(page.HippoventionCenterSection.Title, "Valid center title"),
            Description = EnsureText(page.HippoventionCenterSection.Description, ValidDescription()),
            Pros = EnsureText(page.HippoventionCenterSection.Pros, "Valid pros"),
            ImageId = page.HippoventionCenterSection.Image?.Id,
        },
        AdvantagesSection = new UpdateGallerySectionDto
        {
            Title = EnsureText(page.AdvantagesSection.Title, "Valid advantages title"),
            Cards = page.AdvantagesSection.Cards
                .Select(c => new UpdateGalleryCardDto
                {
                    Id = c.Id,
                    Description = EnsureText(c.Description, ValidDescription()),
                    ImageId = c.Image?.Id,
                })
                .ToList(),
        },
        AnalysisSection = new UpdateTextSectionDto
        {
            Title = EnsureText(page.AnalysisSection.Title, "Valid analysis title"),
            Description = EnsureText(page.AnalysisSection.Description, ValidDescription()),
        },
        ScientificReferencesSection = new UpdateScientificReferencesSectionDto
        {
            Title = EnsureText(page.ScientificReferencesSection.Title, "Valid references title"),
            Description = EnsureText(page.ScientificReferencesSection.Description, ValidDescription()),
            ScientificReferences = page.ScientificReferencesSection.ScientificReferences
                .Select(r => new UpdateScientificReferenceDto
                {
                    Id = r.Id,
                    Name = EnsureText(r.Name, "Valid reference"),
                    Url = EnsureText(r.Url, "https://example.com/reference"),
                })
                .ToList(),
        },
        AnotherQuoteSection = new UpdateQuoteSectionDto
        {
            QuoteText = EnsureText(page.AnotherQuoteSection.QuoteText, ValidDescription()),
            AuthorName = EnsureText(page.AnotherQuoteSection.AuthorName, "Valid author"),
            ImageId = page.AnotherQuoteSection.Image?.Id,
        },
        ParticipantsSection = new UpdateGallerySectionDto
        {
            Title = EnsureText(page.ParticipantsSection.Title, "Valid participants title"),
            Cards = page.ParticipantsSection.Cards
                .Select(c => new UpdateGalleryCardDto
                {
                    Id = c.Id,
                    Description = EnsureText(c.Description, ValidDescription()),
                    ImageId = c.Image?.Id,
                })
                .ToList(),
        },
        EthicsSection = new UpdateEthicsSectionDto
        {
            Title = EnsureText(page.EthicsSection.Title, "Valid ethics title"),
            Description = EnsureText(page.EthicsSection.Description, ValidDescription()),
            ImageId = page.EthicsSection.Image?.Id,
            Principles = page.EthicsSection.Principles
                .Select(p => new UpdateEthicsPrincipleDto
                {
                    Id = p.Id,
                    Text = EnsureText(p.Text, "Valid principle text"),
                })
                .ToList(),
        },
    };
}
