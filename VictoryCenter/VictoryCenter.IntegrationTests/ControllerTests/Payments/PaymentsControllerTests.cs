using System.Globalization;
using System.Net;
using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Moq.Protected;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Public.Payment;
using VictoryCenter.BLL.DTOs.Public.Payment.Common;
using VictoryCenter.BLL.Interfaces.PaymentService;
using VictoryCenter.BLL.Options.Payment;
using VictoryCenter.BLL.Services.PaymentService;
using VictoryCenter.BLL.Validators.Payment;
using VictoryCenter.WebAPI.Controllers.Public;
using VictoryCenter.WebAPI.Extensions;

namespace VictoryCenter.IntegrationTests.ControllerTests.Payments;

public class PaymentsControllerTests
{
    [Fact]
    public async Task Donate_ShouldRedirect_WhenDonationIsSuccessful_WithMockedWay4Pay()
    {
        var fakeExternalResponse = CreateRedirectResponse("https://pay.test/redirect");
        var handlerMock = CreateHandlerMock(fakeExternalResponse);
        await using WebApplication app = await CreateApplicationAsync(handlerMock.Object);
        using var client = CreateClient(app);
        using var content = CreateDonationContent(isSubscription: false);

        using var response = await client.PostAsync("api/payments/donate", content);

        Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
        Assert.Equal("https://pay.test/redirect", response.Headers.Location?.ToString());
    }

    [Fact]
    public async Task Donate_SubscriptionIsRequested_ShouldSendRecurringFieldsAndRedirect()
    {
        string? capturedContent = null;
        var fakeExternalResponse = CreateRedirectResponse("https://pay.test/subscription");
        var handlerMock = new Mock<HttpMessageHandler>();
        handlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .Callback<HttpRequestMessage, CancellationToken>((request, _) =>
            {
                capturedContent = request.Content!.ReadAsStringAsync().GetAwaiter().GetResult();
            })
            .ReturnsAsync(fakeExternalResponse);
        await using WebApplication app = await CreateApplicationAsync(handlerMock.Object);
        using var client = CreateClient(app);
        const string returnUrl = "https://donate.example.com/payment/result";
        using var content = CreateDonationContent(isSubscription: true, returnUrl: returnUrl);

        using var response = await client.PostAsync("api/payments/donate", content);

        Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
        Assert.Equal("https://pay.test/subscription", response.Headers.Location?.ToString());
        Assert.NotNull(capturedContent);
        var parsed = System.Web.HttpUtility.ParseQueryString(capturedContent);
        Assert.Equal("1", parsed["regularOn"]);
        Assert.Equal("100", parsed["regularAmount"]);
        Assert.Equal(PaymentConstants.ClientSelectedRegularPaymentMode, parsed["regularMode"]);
        Assert.Equal(returnUrl, parsed["returnUrl"]);
        Assert.Null(parsed["regularBehavior"]);
        Assert.Null(parsed["regularCount"]);
    }

    [Fact]
    public async Task Donate_ReturnUrlUsesUntrustedHost_ShouldReturnBadRequest()
    {
        var handlerMock = new Mock<HttpMessageHandler>();
        await using WebApplication app = await CreateApplicationAsync(handlerMock.Object);
        using var client = CreateClient(app);
        using var content = CreateDonationContent(
            isSubscription: true,
            returnUrl: "https://untrusted.example/payment/result");

        using var response = await client.PostAsync("api/payments/donate", content);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        handlerMock.Protected().Verify(
            "SendAsync",
            Times.Never(),
            ItExpr.IsAny<HttpRequestMessage>(),
            ItExpr.IsAny<CancellationToken>());
    }

    [Fact]
    public async Task Donate_ShouldReturnBadRequest_WhenDonationFails()
    {
        var handlerMock = new Mock<HttpMessageHandler>();
        await using WebApplication app = await CreateApplicationAsync(handlerMock.Object);
        using var client = CreateClient(app);
        using var content = CreateDonationContent(isSubscription: false, amount: 0);

        using var response = await client.PostAsync("api/payments/donate", content);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        handlerMock.Protected().Verify(
            "SendAsync",
            Times.Never(),
            ItExpr.IsAny<HttpRequestMessage>(),
            ItExpr.IsAny<CancellationToken>());
    }

    [Fact]
    public async Task Donate_WayForPayRejectsRequest_ShouldNotExposeProviderDetails()
    {
        const string providerDetails = "Sensitive provider rejection details";
        var fakeExternalResponse = new HttpResponseMessage(HttpStatusCode.InternalServerError)
        {
            ReasonPhrase = providerDetails
        };
        var handlerMock = CreateHandlerMock(fakeExternalResponse);
        await using WebApplication app = await CreateApplicationAsync(handlerMock.Object);
        using var client = CreateClient(app);
        using var content = CreateDonationContent(isSubscription: true);

        using var response = await client.PostAsync("api/payments/donate", content);
        var responseBody = await response.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Contains(PaymentConstants.UnableToConductDonation, responseBody);
        Assert.DoesNotContain(providerDetails, responseBody);
    }

    private static async Task<WebApplication> CreateApplicationAsync(HttpMessageHandler wayForPayHandler)
    {
        WebApplicationBuilder builder = WebApplication.CreateBuilder(new WebApplicationOptions
        {
            EnvironmentName = Environments.Development
        });
        builder.WebHost.UseKestrel().UseUrls("http://127.0.0.1:0");
        builder.Logging.ClearProviders();
        builder.Services.AddControllers().AddApplicationPart(typeof(PaymentsController).Assembly);
        builder.Services.AddRateLimiterConfiguration();
        builder.Services.AddSingleton<IOptions<WayForPayOptions>>(Options.Create(new WayForPayOptions
        {
            MerchantLogin = "test-login",
            MerchantSecretKey = "test-secret",
            MerchantDomainName = "donate.example.com",
            ApiUrl = "https://secure.wayforpay.com/pay",
            AllowedReturnUrlHosts = ["donate.example.com"]
        }));
        builder.Services.AddScoped<IValidator<PaymentRequestDto>, PaymentRequestValidator>();
        builder.Services.AddScoped<IPaymentService, PaymentService>();
        builder.Services.AddScoped<IPaymentFactory, WayForPayPaymentFactory>();

        var httpClientFactoryMock = new Mock<IHttpClientFactory>();
        httpClientFactoryMock
            .Setup(factory => factory.CreateClient("Way4PayClient"))
            .Returns(new HttpClient(wayForPayHandler));
        builder.Services.AddSingleton(httpClientFactoryMock.Object);

        WebApplication app = builder.Build();
        app.UseRateLimiter();
        app.MapControllers();
        await app.StartAsync();
        return app;
    }

    private static HttpClient CreateClient(WebApplication app)
    {
        var handler = new HttpClientHandler { AllowAutoRedirect = false };
        return new HttpClient(handler)
        {
            BaseAddress = new Uri(GetApplicationAddress(app))
        };
    }

    private static string GetApplicationAddress(WebApplication app)
    {
        return app.Services
            .GetRequiredService<IServer>()
            .Features
            .Get<IServerAddressesFeature>()!
            .Addresses
            .Single();
    }

    private static FormUrlEncodedContent CreateDonationContent(
        bool isSubscription,
        decimal amount = 100,
        string? returnUrl = null)
    {
        var values = new Dictionary<string, string>
        {
            ["Amount"] = amount.ToString(CultureInfo.InvariantCulture),
            ["Currency"] = "UAH",
            ["IsSubscription"] = isSubscription.ToString(),
            ["PaymentSystem"] = ((int)PaymentSystem.WayForPay).ToString()
        };
        if (returnUrl is not null)
        {
            values["ReturnUrl"] = returnUrl;
        }

        return new FormUrlEncodedContent(values);
    }

    private static Mock<HttpMessageHandler> CreateHandlerMock(HttpResponseMessage response)
    {
        var handlerMock = new Mock<HttpMessageHandler>();
        handlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(response);
        return handlerMock;
    }

    private static HttpResponseMessage CreateRedirectResponse(string location)
    {
        return new HttpResponseMessage(HttpStatusCode.Found)
        {
            Headers = { Location = new Uri(location) }
        };
    }
}
