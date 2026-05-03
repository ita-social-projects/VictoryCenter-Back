using System.Net;
using System.Net.Http.Json;
using Microsoft.Extensions.Configuration;
using VictoryCenter.BLL.DTOs.Public.ContactUs;
using VictoryCenter.DAL.Entities;
using VictoryCenter.IntegrationTests.Utils;
using VictoryCenter.IntegrationTests.Utils.DbFixture;

namespace VictoryCenter.IntegrationTests.ControllerTests.ContactUs;

public class SubmitContactFormCommandHandlerTests : BaseTestClass
{
    private const string ApiPath = "/api/ContactUs";

    public SubmitContactFormCommandHandlerTests(IntegrationTestDbFixture fixture)
        : base(fixture)
    {
    }

    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();

        if (!Fixture.DbContext.CompanyProfileContacts.Any())
        {
            var profile = new CompanyProfile();
            Fixture.DbContext.CompanyProfiles.Add(profile);

            Fixture.DbContext.CompanyProfileContacts.Add(new CompanyProfileContact
            {
                Profile = profile,
                CorrespondenceEmail = "company@victorycenter.com",
                Address = "Fake Address",
                Email = "fake@test.com",
                Motto = "Fake Motto",
                Phone = "123456789"
            });
            await Fixture.DbContext.SaveChangesAsync();
        }
    }

    [Fact]
    public async Task SubmitContactForm_WithValidDataAndPassingCaptcha_ReturnsOk()
    {
        var client = Fixture.HttpClient;

        var dto = new SubmitContactUsFormDto
        {
            FromEmail = "user@test.com",
            FromName = "John Doe",
            Subject = "Help needed",
            Message = "This is a test message. It must be long enough.",
            CaptchaResponseToken = "test_token"
        };

        var response = await client.PostAsJsonAsync(ApiPath, dto);

        var content = await response.Content.ReadAsStringAsync();
        Assert.True(response.IsSuccessStatusCode, content);
        var resultDto = await response.Content.ReadFromJsonAsync<ContactUsFormDto>(JsonOptions);
        Assert.NotNull(resultDto);
        Assert.Equal("user@test.com", resultDto.FromEmail);
    }

    [Fact]
    public async Task SubmitContactForm_WithFailingCaptcha_ReturnsBadRequest()
    {
        var client = Fixture.Factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureAppConfiguration((_, config) =>
            {
                config.AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["CloudflareTurnstileCaptchaOptions:SiteVerifyUrl"] = "https://challenges.cloudflare.com/turnstile/v0/siteverify",
                    ["CloudflareTurnstileCaptchaOptions:SecretKey"] = IntegrationTestConstants.CloudflareTurnstile.AlwaysFailsSecretKey
                });
            });
        }).CreateClient();

        var dto = new SubmitContactUsFormDto
        {
            FromEmail = "user@test.com",
            FromName = "John Doe",
            Subject = "Help needed",
            Message = "This is a test message. It must be long enough.",
            CaptchaResponseToken = "test_token"
        };

        var response = await client.PostAsJsonAsync(ApiPath, dto);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}
