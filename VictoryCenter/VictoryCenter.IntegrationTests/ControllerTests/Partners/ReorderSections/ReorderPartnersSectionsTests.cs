using System.Net;
using System.Text;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.DTOs.Admin.Partners;
using VictoryCenter.IntegrationTests.Utils;
using VictoryCenter.IntegrationTests.Utils.DbFixture;

namespace VictoryCenter.IntegrationTests.ControllerTests.Partners.ReorderSections;

public class ReorderPartnersSectionsTests : BaseTestClass
{
    private readonly Uri _endpointUri = new("/api/Partners/sections/reorder", UriKind.Relative);

    public ReorderPartnersSectionsTests(IntegrationTestDbFixture fixture)
        : base(fixture)
    {
    }

    [Fact]
    public async Task ReorderSections_WithValidOrder_ShouldReturnOkAndChangePriority()
    {
        // Arrange
        var initialSections = await Fixture.DbContext.PartnersSections
            .OrderBy(s => s.Priority)
            .AsNoTracking()
            .ToListAsync();

        var orderedIds = initialSections.Select(s => s.Id).ToList();

        orderedIds.Reverse();

        var reorderDto = new ReorderPartnersSectionsDto { OrderedIds = orderedIds };
        var serializedDto = JsonSerializer.Serialize(reorderDto);
        var content = new StringContent(serializedDto, Encoding.UTF8, "application/json");

        // Act
        var response = await Fixture.HttpClient.PutAsync(_endpointUri, content);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        Fixture.DbContext.ChangeTracker.Clear();
        var sectionsAfterReorder = await Fixture.DbContext.PartnersSections
            .OrderBy(s => s.Priority)
            .ToListAsync();

        Assert.Equal(orderedIds, sectionsAfterReorder.Select(s => s.Id));
    }

    [Fact]
    public async Task ReorderSections_WithNonExistentId_ShouldReturnBadRequest()
    {
        // Arrange
        var existingIds = await Fixture.DbContext.PartnersSections.Select(s => s.Id).ToListAsync();

        var invalidOrderedIds = existingIds.Append(long.MaxValue).ToList();

        var reorderDto = new ReorderPartnersSectionsDto { OrderedIds = invalidOrderedIds };
        var serializedDto = JsonSerializer.Serialize(reorderDto);
        var content = new StringContent(serializedDto, Encoding.UTF8, "application/json");

        // Act
        var response = await Fixture.HttpClient.PutAsync(_endpointUri, content);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task ReorderSections_WithInvalidDto_ShouldReturnBadRequest()
    {
        // Arrange
        var reorderDto = new ReorderPartnersSectionsDto { OrderedIds = [1, 1, 2] };
        var serializedDto = JsonSerializer.Serialize(reorderDto);
        var content = new StringContent(serializedDto, Encoding.UTF8, "application/json");

        // Act
        var response = await Fixture.HttpClient.PutAsync(_endpointUri, content);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}
