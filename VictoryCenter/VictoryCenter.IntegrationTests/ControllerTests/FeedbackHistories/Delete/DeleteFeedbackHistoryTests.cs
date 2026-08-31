using System.Net;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Enums;
using VictoryCenter.IntegrationTests.Utils;
using VictoryCenter.IntegrationTests.Utils.DbFixture;

namespace VictoryCenter.IntegrationTests.ControllerTests.FeedbackHistories.Delete;

public class DeleteFeedbackHistoryTests : BaseTestClass
{
    private const string BaseUrl = "/api/FeedbackHistories";

    public DeleteFeedbackHistoryTests(IntegrationTestDbFixture fixture)
        : base(fixture)
    {
    }

    [Fact]
    public async Task DeleteFeedbackHistory_ValidRequest_ShouldDeleteEntity()
    {
        var existingEntity = await CreateTestFeedbackHistoryAsync();

        var response = await Fixture.HttpClient.DeleteAsync($"{BaseUrl}/{existingEntity.Id}");

        Assert.True(response.IsSuccessStatusCode);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Null(await Fixture.DbContext.FeedbackHistories.FirstOrDefaultAsync(e => e.Id == existingEntity.Id));
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(0)]
    public async Task DeleteFeedbackHistory_InvalidId_ShouldReturnNotFound(long testId)
    {
        var response = await Fixture.HttpClient.DeleteAsync($"{BaseUrl}/{testId}");

        Assert.False(response.IsSuccessStatusCode);
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task DeleteFeedbackHistory_WithLinkedImage_ShouldDeleteFeedbackHistoryAndKeepImage()
    {
        var image = await CreateTestImageAsync();
        var existingEntity = await CreateTestFeedbackHistoryAsync(image.Id);

        var response = await Fixture.HttpClient.DeleteAsync($"{BaseUrl}/{existingEntity.Id}");

        Assert.True(response.IsSuccessStatusCode);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Null(await Fixture.DbContext.FeedbackHistories.FirstOrDefaultAsync(e => e.Id == existingEntity.Id));
        Assert.NotNull(await Fixture.DbContext.Images.FirstOrDefaultAsync(i => i.Id == image.Id));
    }

    [Fact]
    public void FeedbackHistory_ImageForeignKey_ShouldBeConfiguredWithRestrictDeleteBehavior()
    {
        var entityType = Fixture.DbContext.Model.FindEntityType(typeof(FeedbackHistory));
        var foreignKey = entityType?.GetForeignKeys()
            .FirstOrDefault(fk => fk.PrincipalEntityType.ClrType == typeof(Image));

        Assert.NotNull(foreignKey);
        Assert.Equal(DeleteBehavior.Restrict, foreignKey.DeleteBehavior);
    }

    private async Task<Image> CreateTestImageAsync()
    {
        var image = new Image
        {
            BlobName = "test-image-feedback-history",
            MimeType = "image/png",
            CreatedAt = DateTimeOffset.UtcNow
        };

        await Fixture.DbContext.Images.AddAsync(image);
        await Fixture.DbContext.SaveChangesAsync();
        return image;
    }

    private async Task<FeedbackHistory> CreateTestFeedbackHistoryAsync(long? imageId = null)
    {
        var entity = new FeedbackHistory
        {
            Title = "Title For Delete Test",
            Story = "Story content for delete test that meets all length requirements.",
            ImageId = imageId,
            CreatedAt = DateTimeOffset.UtcNow,
            Status = Status.Draft
        };

        await Fixture.DbContext.FeedbackHistories.AddAsync(entity);
        await Fixture.DbContext.SaveChangesAsync();
        return entity;
    }
}