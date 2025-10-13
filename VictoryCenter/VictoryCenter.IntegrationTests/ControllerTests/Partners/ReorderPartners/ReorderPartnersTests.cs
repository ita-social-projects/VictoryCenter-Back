using System.Net;
using System.Text;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.DTOs.Admin.Partners;
using VictoryCenter.IntegrationTests.Utils;
using VictoryCenter.IntegrationTests.Utils.DbFixture;

namespace VictoryCenter.IntegrationTests.ControllerTests.Partners.ReorderPartners;

public class ReorderPartnersTests : BaseTestClass
{
    private readonly Uri _endpointUri = new("/api/Partners/reorder", UriKind.Relative);

    public ReorderPartnersTests(IntegrationTestDbFixture fixture)
        : base(fixture)
    {
    }

    [Fact]
    public async Task ReorderPartners_WithValidOrder_ShouldReturnOkAndChangePriority()
    {
        // Arrange
        var targetSection = await Fixture.DbContext.PartnersSections
            .Include(s => s.Partners)
            .Where(s => s.Partners.Count > 1)
            .AsNoTracking()
            .FirstOrDefaultAsync()
            ?? throw new InvalidOperationException("Seeder must provide a section with at least 2 partners.");

        var orderedIds = targetSection.Partners.OrderBy(p => p.Priority).Select(p => p.Id).ToList();

        orderedIds.Reverse();

        var reorderDto = new ReorderPartnersDto
        {
            PartnersSectionId = targetSection.Id,
            OrderedIds = orderedIds
        };
        var serializedDto = JsonSerializer.Serialize(reorderDto);
        var content = new StringContent(serializedDto, Encoding.UTF8, "application/json");

        // Act
        var response = await Fixture.HttpClient.PutAsync(_endpointUri, content);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        Fixture.DbContext.ChangeTracker.Clear();
        var partnersAfterReorder = await Fixture.DbContext.Partners
            .Where(p => p.PartnersSectionId == targetSection.Id)
            .OrderBy(p => p.Priority)
            .ToListAsync();

        Assert.Equal(orderedIds, partnersAfterReorder.Select(p => p.Id));
    }

    [Fact]
    public async Task ReorderPartners_WithNonExistentSectionId_ShouldReturnBadRequest()
    {
        // Arrange
        var partnerIds = await Fixture.DbContext.Partners.Select(p => p.Id).Take(2).ToListAsync();
        var reorderDto = new ReorderPartnersDto
        {
            PartnersSectionId = long.MaxValue,
            OrderedIds = partnerIds
        };
        var serializedDto = JsonSerializer.Serialize(reorderDto);
        var content = new StringContent(serializedDto, Encoding.UTF8, "application/json");

        // Act
        var response = await Fixture.HttpClient.PutAsync(_endpointUri, content);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
