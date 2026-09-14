using System.Net;
using FluentResults;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Moq;
using VictoryCenter.BLL.DTOs.Public.Payment;
using VictoryCenter.BLL.DTOs.Public.Payment.Common;
using VictoryCenter.BLL.Interfaces.PaymentService;
using VictoryCenter.WebAPI.Controllers.Public;
using VictoryCenter.WebAPI.Extensions;

namespace VictoryCenter.IntegrationTests.ControllerTests.Payments;

public class PaymentRateLimitTests
{
    private const int PermittedRequestsPerWindow = 10;

    [Fact]
    public async Task Donate_RequestLimitExceeded_ShouldReturnTooManyRequests()
    {
        var paymentServiceMock = CreatePaymentServiceMock();
        await using WebApplication app = await CreateApplicationAsync(paymentServiceMock);
        using var client = CreateClient(app);

        for (var requestNumber = 0; requestNumber < PermittedRequestsPerWindow; requestNumber++)
        {
            using var permittedResponse = await SendDonationRequestAsync(client);
            Assert.Equal(HttpStatusCode.Redirect, permittedResponse.StatusCode);
        }

        using var rejectedResponse = await SendDonationRequestAsync(client);

        Assert.Equal(HttpStatusCode.TooManyRequests, rejectedResponse.StatusCode);
        paymentServiceMock.Verify(
            service => service.CreatePayment(
                It.IsAny<PaymentRequestDto>(),
                It.IsAny<CancellationToken>()),
            Times.Exactly(PermittedRequestsPerWindow));
    }

    [Fact]
    public async Task Donate_RequestsFromDifferentForwardedAddresses_ShouldUseSeparateRateLimits()
    {
        const string firstDonorAddress = "198.51.100.10";
        const string secondDonorAddress = "203.0.113.10";
        var paymentServiceMock = CreatePaymentServiceMock();
        await using WebApplication app = await CreateApplicationAsync(
            paymentServiceMock,
            proxyAddress: "172.20.0.2");
        using var client = CreateClient(app);

        for (var requestNumber = 0; requestNumber < PermittedRequestsPerWindow; requestNumber++)
        {
            using var permittedResponse = await SendDonationRequestAsync(client, firstDonorAddress);
            Assert.Equal(HttpStatusCode.Redirect, permittedResponse.StatusCode);
        }

        using var firstDonorRejectedResponse = await SendDonationRequestAsync(client, firstDonorAddress);
        using var secondDonorResponse = await SendDonationRequestAsync(client, secondDonorAddress);

        Assert.Equal(HttpStatusCode.TooManyRequests, firstDonorRejectedResponse.StatusCode);
        Assert.Equal(HttpStatusCode.Redirect, secondDonorResponse.StatusCode);
        paymentServiceMock.Verify(
            service => service.CreatePayment(
                It.IsAny<PaymentRequestDto>(),
                It.IsAny<CancellationToken>()),
            Times.Exactly(PermittedRequestsPerWindow + 1));
    }

    private static async Task<WebApplication> CreateApplicationAsync(
        Mock<IPaymentService> paymentServiceMock,
        string? proxyAddress = null)
    {
        WebApplicationBuilder builder = WebApplication.CreateBuilder(new WebApplicationOptions
        {
            EnvironmentName = Environments.Development
        });
        builder.WebHost.UseKestrel().UseUrls("http://127.0.0.1:0");
        builder.Logging.ClearProviders();
        builder.Services.AddControllers().AddApplicationPart(typeof(PaymentsController).Assembly);
        builder.Services.AddSingleton(paymentServiceMock.Object);
        builder.Services.AddRateLimiterConfiguration();

        if (proxyAddress is not null)
        {
            builder.Configuration.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ReverseProxy:KnownNetworks:0"] = "172.16.0.0/12"
            });
            builder.Services.AddForwardedHeadersConfiguration(builder.Configuration);
        }

        WebApplication app = builder.Build();

        if (proxyAddress is not null)
        {
            app.Use((context, next) =>
            {
                context.Connection.RemoteIpAddress = IPAddress.Parse(proxyAddress);
                return next(context);
            });
            app.UseForwardedHeaders();
        }

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

    private static Mock<IPaymentService> CreatePaymentServiceMock()
    {
        var paymentServiceMock = new Mock<IPaymentService>();
        paymentServiceMock
            .Setup(service => service.CreatePayment(
                It.IsAny<PaymentRequestDto>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Ok(new PaymentResponseDto
            {
                PaymentUrl = "https://pay.test/redirect"
            }));

        return paymentServiceMock;
    }

    private static async Task<HttpResponseMessage> SendDonationRequestAsync(
        HttpClient client,
        string? forwardedFor = null)
    {
        using var content = new FormUrlEncodedContent(new Dictionary<string, string>
        {
            ["Amount"] = "100",
            ["Currency"] = "UAH",
            ["IsSubscription"] = "true",
            ["PaymentSystem"] = ((int)PaymentSystem.WayForPay).ToString()
        });

        using var request = new HttpRequestMessage(HttpMethod.Post, "api/payments/donate")
        {
            Content = content
        };

        if (forwardedFor is not null)
        {
            request.Headers.Add("X-Forwarded-For", forwardedFor);
            request.Headers.Add("X-Forwarded-Proto", "https");
        }

        return await client.SendAsync(request);
    }
}
