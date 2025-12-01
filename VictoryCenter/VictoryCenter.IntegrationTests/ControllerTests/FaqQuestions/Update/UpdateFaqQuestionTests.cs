using System.Net;
using System.Text;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.FaqQuestions;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Enums;
using VictoryCenter.IntegrationTests.Utils;
using VictoryCenter.IntegrationTests.Utils.DbFixture;

namespace VictoryCenter.IntegrationTests.ControllerTests.FaqQuestions.Update;

public class UpdateFaqQuestionTests : BaseTestClass
{
    public UpdateFaqQuestionTests(IntegrationTestDbFixture fixture)
        : base(fixture)
    {
    }

    [Fact]
    public async Task UpdateFaqQuestion_ValidRequest_ShouldReturnSuccess()
    {
        FaqQuestion existingEntity = await Fixture.DbContext.FaqQuestions.FirstOrDefaultAsync()
            ?? throw new InvalidOperationException("No FaqQuestion entity exists in the database.");

        var updateFaqQuestionDto = new UpdateFaqQuestionDto
        {
            QuestionText = new('Q', FaqConstants.QuestionTextMinLength + 1),
            AnswerText = new('A', FaqConstants.AnswerTextMinLength + 1),
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
    public async Task UpdateFaqQuestion_InvalidQuestionText_ShouldReturnBadRequest()
    {
        FaqQuestion existingEntity = await Fixture.DbContext.FaqQuestions.FirstOrDefaultAsync()
            ?? throw new InvalidOperationException("No FaqQuestion entity exists in the database.");

        var updateFaqQuestionDto = new UpdateFaqQuestionDto
        {
            QuestionText = "",
            AnswerText = new('A', FaqConstants.AnswerTextMinLength + 1),
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
    public async Task UpdateFaqQuestion_InvalidId_ShouldReturnNotFound()
    {
        var updateFaqQuestionDto = new UpdateFaqQuestionDto
        {
            QuestionText = new('Q', FaqConstants.QuestionTextMinLength + 1),
            AnswerText = new('A', FaqConstants.AnswerTextMinLength + 1),
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
