using System.Net;
using System.Text;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.DTOs.Admin.FaqQuestions;
using VictoryCenter.IntegrationTests.Utils.DbFixture;
using VictoryCenter.IntegrationTests.Utils;

namespace VictoryCenter.IntegrationTests.ControllerTests.Faq.Reorder;

public class ReorderQuestionsTests : BaseTestClass
{
    private readonly Uri _endpointUri = new("/api/faq/reorder", UriKind.Relative);
    public ReorderQuestionsTests(IntegrationTestDbFixture fixture)
        : base(fixture)
    {
    }

    [Fact]
    public async Task ReorderQuestions_ValidDto_ShouldReturnOk()
    {
        var page = await Fixture.DbContext.VisitorPages.FirstOrDefaultAsync() ?? throw new InvalidOperationException("Couldn't setup existing entity");
        var orderedIds = await Fixture.DbContext.FaqPlacements
            .Where(x => x.PageId == page.Id)
            .OrderBy(x => x.Priority)
            .Select(x => x.QuestionId)
            .ToListAsync();

        orderedIds.Reverse();
        var reorderDto = new ReorderFaqQuestionsDto
        {
            PageId = page.Id,
            OrderedIds = orderedIds
        };
        var serializedDto = JsonSerializer.Serialize(reorderDto);

        var response = await Fixture.HttpClient.PutAsync(_endpointUri, new StringContent(serializedDto, Encoding.UTF8, "application/json"));

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task ReorderQuestions_DtoIsInvalid_ShouldReturnBadRequest()
    {
        var page = await Fixture.DbContext.VisitorPages.FirstOrDefaultAsync() ?? throw new InvalidOperationException("Couldn't setup existing entity");
        var orderedIds = await Fixture.DbContext.FaqPlacements
            .Where(x => x.PageId == page.Id)
            .OrderBy(x => x.Priority)
            .Select(x => x.QuestionId)
            .ToListAsync();

        if (orderedIds.Count < 3)
        {
            throw new InvalidOperationException("Not enough questions to test reordering");
        }

        var reorderDto = new ReorderFaqQuestionsDto
        {
            PageId = page.Id,
            OrderedIds = [orderedIds[2], orderedIds[0]]
        };
        var serializedDto = JsonSerializer.Serialize(reorderDto);

        var response = await Fixture.HttpClient.PutAsync(_endpointUri, new StringContent(serializedDto, Encoding.UTF8, "application/json"));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task ReorderQuestions_DtoContainsInvalidPageId_ShouldReturnNotFound()
    {
        var page = await Fixture.DbContext.VisitorPages.FirstOrDefaultAsync() ?? throw new InvalidOperationException("Couldn't setup existing entity");

        var reorderDto = new ReorderFaqQuestionsDto
        {
            PageId = page.Id,
            OrderedIds = [long.MaxValue]
        };
        var serializedDto = JsonSerializer.Serialize(reorderDto);

        var response = await Fixture.HttpClient.PutAsync(_endpointUri, new StringContent(serializedDto, Encoding.UTF8, "application/json"));

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
