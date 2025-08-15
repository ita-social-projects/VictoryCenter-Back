using System.Net;
using System.Text;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.DTOs.Admin.FaqQuestions;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Enums;
using VictoryCenter.IntegrationTests.Utils.DbFixture;
using VictoryCenter.IntegrationTests.Utils;

namespace VictoryCenter.IntegrationTests.ControllerTests.Faq.Update;

public class UpdateFaqQuestionTests : BaseTestClass
{
    public UpdateFaqQuestionTests(IntegrationTestDbFixture fixture)
        : base(fixture)
    {
    }

    [Fact]
    public async Task UpdateFaqQuestion_ValidRequest_ReturnsSuccess()
    {
        FaqQuestion existingEntity = await Fixture.DbContext.FaqQuestions.FirstOrDefaultAsync()
            ?? throw new InvalidOperationException("No FaqQuestion entity exists in the database.");

        var updateFaqQuestionDto = new UpdateFaqQuestionDto
        {
            QuestionText = "QQQQQQQQQQQQQQQQQQQQ",
            AnswerText = "AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA",
            PageIds = [3],
            Status = Status.Published,
        };
        var serializedDto = JsonSerializer.Serialize(updateFaqQuestionDto);

        HttpResponseMessage response = await Fixture.HttpClient.PutAsync(new Uri($"/api/faq/{existingEntity.Id}", UriKind.Relative), new StringContent(
                serializedDto, Encoding.UTF8, "application/json"));
        var responseString = await response.Content.ReadAsStringAsync();
        FaqQuestionDto? responseContent = JsonSerializer.Deserialize<FaqQuestionDto>(responseString, JsonOptions);

        response.EnsureSuccessStatusCode();
        Assert.NotNull(responseContent);
    }

    [Fact]
    public async Task UpdateFaqQuestion_InvalidQuestionText_ReturnsBadRequest()
    {
        FaqQuestion existingEntity = await Fixture.DbContext.FaqQuestions.FirstOrDefaultAsync()
            ?? throw new InvalidOperationException("No FaqQuestion entity exists in the database.");

        var updateFaqQuestionDto = new UpdateFaqQuestionDto
        {
            QuestionText = "",
            AnswerText = "AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA",
            PageIds = [3],
            Status = Status.Published,
        };
        var serializedDto = JsonSerializer.Serialize(updateFaqQuestionDto);

        HttpResponseMessage response = await Fixture.HttpClient.PutAsync(new Uri($"/api/faq/{existingEntity.Id}", UriKind.Relative), new StringContent(
                serializedDto, Encoding.UTF8, "application/json"));

        Assert.False(response.IsSuccessStatusCode);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task UpdateFaqQuestion_InvalidId_ReturnsNotFound()
    {
        var updateFaqQuestionDto = new UpdateFaqQuestionDto
        {
            QuestionText = "QQQQQQQQQQQQQQQQQQQQ",
            AnswerText = "AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA",
            PageIds = [3],
            Status = Status.Published,
        };
        var serializedDto = JsonSerializer.Serialize(updateFaqQuestionDto);

        HttpResponseMessage response = await Fixture.HttpClient.PutAsync(new Uri($"/api/faq/{-1}", UriKind.Relative), new StringContent(
                serializedDto, Encoding.UTF8, "application/json"));

        Assert.False(response.IsSuccessStatusCode);
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
