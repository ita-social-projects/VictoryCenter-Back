using System.Net;
using FluentResults;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
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
        var paymentServiceMock = new Mock<IPaymentService>();
        paymentServiceMock
            .Setup(service => service.CreatePayment(
                It.IsAny<PaymentRequestDto>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Ok(new PaymentResponseDto
            {
                PaymentUrl = "https://pay.test/redirect"
            }));

        WebApplicationBuilder builder = WebApplication.CreateBuilder(new WebApplicationOptions
        {
            EnvironmentName = Environments.Development
        });
        builder.WebHost.UseKestrel().UseUrls("http://127.0.0.1:0");
        builder.Logging.ClearProviders();
        builder.Services.AddControllers().AddApplicationPart(typeof(PaymentsController).Assembly);
        builder.Services.AddSingleton(paymentServiceMock.Object);
        builder.Services.AddRateLimiterConfiguration();

        await using WebApplication app = builder.Build();
        app.UseRateLimiter();
        app.MapControllers();
        await app.StartAsync();

        using var handler = new HttpClientHandler { AllowAutoRedirect = false };
        using var client = new HttpClient(handler)
        {
            BaseAddress = new Uri(GetApplicationAddress(app))
        };

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

    private static string GetApplicationAddress(WebApplication app)
    {
        return app.Services
            .GetRequiredService<IServer>()
            .Features
            .Get<IServerAddressesFeature>()!
            .Addresses
            .Single();
    }

    private static async Task<HttpResponseMessage> SendDonationRequestAsync(HttpClient client)
    {
        using var content = new FormUrlEncodedContent(new Dictionary<string, string>
        {
            ["Amount"] = "100",
            ["Currency"] = "UAH",
            ["IsSubscription"] = "true",
            ["PaymentSystem"] = ((int)PaymentSystem.WayForPay).ToString()
        });

        return await client.PostAsync("api/payments/donate", content);
    }
}
