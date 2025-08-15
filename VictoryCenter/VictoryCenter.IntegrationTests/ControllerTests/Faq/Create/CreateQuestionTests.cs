using System.Net;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.DTOs.Admin.FaqQuestions;
using VictoryCenter.DAL.Enums;
using VictoryCenter.IntegrationTests.ControllerTests.DbFixture;
using VictoryCenter.IntegrationTests.Utils;

namespace VictoryCenter.IntegrationTests.ControllerTests.Faq.Create;

public class CreateQuestionTests : BaseTestClass
{
    private readonly Uri _endpointUri = new("/api/faq", UriKind.Relative);
    public CreateQuestionTests(IntegrationTestDbFixture Fixture)
        : base(Fixture)
    {
    }

    [Fact]
    public async Task CreateQuestion_ShouldReturnOk_DtoIsValid()
    {
        var page = await Fixture.DbContext.VisitorPages.FirstOrDefaultAsync() ?? throw new InvalidOperationException("Couldn't setup existing entity");
        var createFaqQuestionDto = new CreateFaqQuestionDto
        {
            QuestionText = "Some pretty smart question?",
            AnswerText = "Some pretty smart answer about the point in life and why VictoryCenter exists",
            Status = Status.Draft,
            PageIds = [page.Id],
        };
        var serializedDto = JsonSerializer.Serialize(createFaqQuestionDto);

        var response = await Fixture.HttpClient.PostAsync(_endpointUri, new StringContent(serializedDto, Encoding.UTF8, "application/json"));

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task CreateQuestion_ShouldReturnBadRequest_DtoIsInvalid()
    {
        var page = await Fixture.DbContext.VisitorPages.FirstOrDefaultAsync() ?? throw new InvalidOperationException("Couldn't setup existing entity");
        var createFaqQuestionDto = new CreateFaqQuestionDto
        {
            QuestionText = "Some normal question?",
            AnswerText = "Too short answer",
            Status = Status.Published,
            PageIds = [page.Id],
        };
        var serializedDto = JsonSerializer.Serialize(createFaqQuestionDto);

        var response = await Fixture.HttpClient.PostAsync(_endpointUri, new StringContent(serializedDto, Encoding.UTF8, "application/json"));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task CreateQuestion_ShouldReturnNotFound_DtoContainsInvalidPageId()
    {
        var page = await Fixture.DbContext.VisitorPages.FirstOrDefaultAsync() ?? throw new InvalidOperationException("Couldn't setup existing entity");
        var createFaqQuestionDto = new CreateFaqQuestionDto
        {
            QuestionText = "Some pretty smart question?",
            AnswerText = "Some pretty smart answer about the point in life and why VictoryCenter exists",
            Status = Status.Published,
            PageIds = [long.MaxValue],
        };
        var serializedDto = JsonSerializer.Serialize(createFaqQuestionDto);

        var response = await Fixture.HttpClient.PostAsync(
            _endpointUri,
            new StringContent(serializedDto, Encoding.UTF8, "application/json"));

        var responseContent = JsonSerializer.Deserialize<ProblemDetails>(
            await response.Content.ReadAsStringAsync(),
            JsonOptions);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.NotNull(responseContent);
        Assert.IsType<ProblemDetails>(responseContent);
    }
}
