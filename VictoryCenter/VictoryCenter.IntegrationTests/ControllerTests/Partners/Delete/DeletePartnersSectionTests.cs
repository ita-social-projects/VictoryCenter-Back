using System.Net;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.IntegrationTests.Utils;
using VictoryCenter.IntegrationTests.Utils.DbFixture;

namespace VictoryCenter.IntegrationTests.ControllerTests.Partners.Delete;

public class DeletePartnersSectionTests : BaseTestClass
{
    private readonly Uri _endpointUri = new("/api/Partners/", UriKind.Relative);

    public DeletePartnersSectionTests(IntegrationTestDbFixture fixture)
        : base(fixture)
    {
    }

    [Fact]
    public async Task DeletePartnersSection_WithExistingId_ShouldDeleteSectionAndRenumberOthers()
    {
        // Arrange
        var sectionsBeforeDelete = await Fixture.DbContext.PartnersSections
            .OrderBy(s => s.Priority)
            .AsNoTracking()
            .ToListAsync();

        if (sectionsBeforeDelete.Count < 2)
        {
            throw new InvalidOperationException("Seeder must provide at least 2 partner sections for this test.");
        }

        var sectionToDelete = sectionsBeforeDelete.First();
        var initialCount = sectionsBeforeDelete.Count;

        var requestUri = new Uri($"{_endpointUri}{sectionToDelete.Id}", UriKind.Relative);

        // Act
        var response = await Fixture.HttpClient.DeleteAsync(requestUri);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        Fixture.DbContext.ChangeTracker.Clear();

        var deletedSection = await Fixture.DbContext.PartnersSections.FindAsync(sectionToDelete.Id);
        Assert.Null(deletedSection);

        var sectionsAfterDelete = await Fixture.DbContext.PartnersSections
            .OrderBy(s => s.Priority)
            .ToListAsync();
        Assert.Equal(initialCount - 1, sectionsAfterDelete.Count);

        for (int i = 0; i < sectionsAfterDelete.Count; i++)
        {
            Assert.Equal(i + 1, sectionsAfterDelete[i].Priority);
        }
    }

    [Fact]
    public async Task DeletePartnersSection_WithNonExistentId_ShouldReturnNotFound()
    {
        var nonExistingEntityId = long.MaxValue;

        // Arrange
        var requestUri = new Uri($"{_endpointUri}{nonExistingEntityId}", UriKind.Relative);

        // Act
        var response = await Fixture.HttpClient.DeleteAsync(requestUri);

        // Assert
        Assert.False(response.IsSuccessStatusCode);
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
