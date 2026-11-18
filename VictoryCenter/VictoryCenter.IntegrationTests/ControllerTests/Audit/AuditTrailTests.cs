using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.FaqQuestions;
using VictoryCenter.BLL.DTOs.Admin.VideoReviews;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Enums;
using VictoryCenter.IntegrationTests.Utils;
using VictoryCenter.IntegrationTests.Utils.DbFixture;

namespace VictoryCenter.IntegrationTests.ControllerTests.Audit;

public class AuditTrailTests : BaseTestClass
{
    private const string TestAdminEmail = "testadmin@victorycenter.com";

    public AuditTrailTests(IntegrationTestDbFixture fixture)
        : base(fixture)
    {
    }

    [Fact]
    public async Task UpdateFaqQuestion_ValidRequest_ShouldWriteUpdateAuditLogWithActorAndChanges()
    {
        FaqQuestion existingEntity = await Fixture.DbContext.FaqQuestions.AsNoTracking().FirstOrDefaultAsync()
            ?? throw new InvalidOperationException("No FaqQuestion entity exists in the database.");
        var entityId = existingEntity.Id.ToString();
        var newQuestionText = new string('Q', FaqConstants.QuestionTextMinLength + 1);

        HttpResponseMessage response = await PutFaqQuestionAsync(existingEntity.Id, newQuestionText);

        response.EnsureSuccessStatusCode();
        var admin = await Fixture.DbContext.Users.AsNoTracking().SingleAsync(user => user.Email == TestAdminEmail);
        var log = await Fixture.DbContext.AuditLogs.AsNoTracking().SingleAsync(item =>
            item.Operation == AuditOperation.Update
            && item.EntityType == nameof(FaqQuestion)
            && item.EntityId == entityId);
        Assert.Equal(admin.Id.ToString(), log.ActorId);
        Assert.NotNull(log.CorrelationId);
        Assert.Contains($"\"New\":\"{newQuestionText}\"", log.Changes);
    }

    [Fact]
    public async Task UpdateFaqQuestion_InvalidRequest_ShouldNotWriteAuditLog()
    {
        FaqQuestion existingEntity = await Fixture.DbContext.FaqQuestions.AsNoTracking().FirstOrDefaultAsync()
            ?? throw new InvalidOperationException("No FaqQuestion entity exists in the database.");
        var entityId = existingEntity.Id.ToString();

        HttpResponseMessage response = await PutFaqQuestionAsync(existingEntity.Id, string.Empty);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.False(await Fixture.DbContext.AuditLogs.AsNoTracking().AnyAsync(item =>
            item.Operation == AuditOperation.Update
            && item.EntityType == nameof(FaqQuestion)
            && item.EntityId == entityId));
    }

    [Fact]
    public async Task DeleteVideoReview_ExistingReview_ShouldWriteDeleteAuditLog()
    {
        var createResponse = await Fixture.HttpClient.PostAsJsonAsync(
            "/api/VideoReviews",
            new CreateVideoReviewDto
            {
                Title = $"Audit{Guid.NewGuid().ToString("N")[..8]}",
                Link = "https://example.com/video",
                Status = Status.Draft,
            });
        createResponse.EnsureSuccessStatusCode();
        var created = await createResponse.Content.ReadFromJsonAsync<VideoReviewDto>();
        Assert.NotNull(created);
        var entityId = created.Id.ToString();

        var deleteResponse = await Fixture.HttpClient.DeleteAsync($"/api/VideoReviews/{created.Id}");

        deleteResponse.EnsureSuccessStatusCode();
        var log = await Fixture.DbContext.AuditLogs.AsNoTracking().SingleAsync(item =>
            item.Operation == AuditOperation.Delete
            && item.EntityType == nameof(VideoReview)
            && item.EntityId == entityId);
        Assert.NotNull(log.ActorId);
        Assert.Contains("\"IsArchived\":{\"Old\":false,\"New\":true}", log.Changes);
    }

    private Task<HttpResponseMessage> PutFaqQuestionAsync(long id, string questionText)
    {
        var dto = new UpdateFaqQuestionDto
        {
            QuestionText = questionText,
            AnswerText = new string('A', FaqConstants.AnswerTextMinLength + 1),
            PageIds = [3],
            Status = Status.Published,
        };

        return Fixture.HttpClient.PutAsync(
            new Uri($"/api/faq/{id}", UriKind.Relative),
            new StringContent(JsonSerializer.Serialize(dto), Encoding.UTF8, "application/json"));
    }
}
