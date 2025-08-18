using System.Net;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Moq.Protected;
using VictoryCenter.BLL.DTOs.Public.Payment;
using VictoryCenter.IntegrationTests.Utils;
using VictoryCenter.IntegrationTests.Utils.DbFixture;

namespace VictoryCenter.IntegrationTests.ControllerTests.Payments;

public class PaymentsControllerTests : BaseTestClass
{
    public PaymentsControllerTests(IntegrationTestDbFixture fixture)
        : base(fixture)
    {
    }

    [Fact]
    public async Task Donate_ShouldRedirect_WhenDonationIsSuccessful_WithMockedWay4Pay()
    {
        var fakeExternalResponse = new HttpResponseMessage(HttpStatusCode.Found)
        {
            Headers = { Location = new Uri("https://pay.test/redirect") }
        };

        var handlerMock = new Mock<HttpMessageHandler>();
        handlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(fakeExternalResponse);

        var client = Fixture.Factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                var descriptor = services
                    .Single(d => d.ServiceType == typeof(IHttpClientFactory));
                services.Remove(descriptor);

                var mockFactory = new Mock<IHttpClientFactory>();
                var fakeClient = new HttpClient(handlerMock.Object)
                {
                    BaseAddress = new Uri("https://fake.external.api")
                };
                mockFactory.Setup(f => f.CreateClient("Way4PayClient"))
                    .Returns(fakeClient);

                services.AddSingleton(mockFactory.Object);
            });
        }).CreateClient(new WebApplicationFactoryClientOptions()
        {
            AllowAutoRedirect = false
        });

        var request = new Dictionary<string, string>
        {
            ["Amount"] = "100",
            ["Currency"] = "USD",
            ["IsSubscription"] = "false",
            ["PaymentSystem"] = ((int)PaymentSystem.WayForPay).ToString()
        };
        var content = new FormUrlEncodedContent(request);

        var response = await client.PostAsync("api/payments/donate", content);

        Assert.True(response.StatusCode is HttpStatusCode.Redirect);
        Assert.Equal("https://pay.test/redirect", response.Headers.Location?.ToString());
    }

    [Fact]
    public async Task Donate_ShouldReturnBadRequest_WhenDonationFails()
    {
        var request = new Dictionary<string, string>
        {
            ["Amount"] = "0",
            ["Currency"] = "USD",
            ["IsSubscription"] = "false",
            ["PaymentSystem"] = ((int)PaymentSystem.WayForPay).ToString()
        };
        var content = new FormUrlEncodedContent(request);

        var response = await Fixture.HttpClient.PostAsync("api/payments/donate", content);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}
