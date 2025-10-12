using System.Net;
using System.Text;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.Partners;
using VictoryCenter.IntegrationTests.Utils;
using VictoryCenter.IntegrationTests.Utils.DbFixture;

namespace VictoryCenter.IntegrationTests.ControllerTests.Partners.UpdateSection;

public class UpdatePartnersSectionTests : BaseTestClass
{
    private readonly string _endpointBasePath = "/api/Partners/";

    public UpdatePartnersSectionTests(IntegrationTestDbFixture fixture)
        : base(fixture)
    {
    }

    [Fact]
    public async Task UpdateSection_WithValidFullDto_ShouldSucceed()
    {
        // Arrange
        var sectionToUpdate = await Fixture.DbContext.PartnersSections
            .Include(s => s.Partners)
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.Partners.Count >= 2)
            ?? throw new InvalidOperationException("Seeder must provide a section with at least 2 partners.");

        var partnerToDelete = sectionToUpdate.Partners.First();
        var partnerToUpdate = sectionToUpdate.Partners.Last();

        var imageForNewPartner = await Fixture.DbContext.Images
            .AsNoTracking()
            .FirstOrDefaultAsync(i => sectionToUpdate.Partners.All(p => p.ImageId != i.Id))
            ?? throw new InvalidOperationException("Seeder must provide at least one unused image.");

        var updateDto = new UpdatePartnersSectionDto
        {
            Title = "Updated Section Title",
            Description = "This description has been updated.",
            PartnerIdsToDelete = [partnerToDelete.Id],
            PartnersToUpdate =
            [
                new() { Id = partnerToUpdate.Id, Description = "Updated Partner Description", ImageId = partnerToUpdate.ImageId }
            ],
            PartnersToCreate =
            [
                new() { Description = "A Brand New Partner", ImageId = imageForNewPartner.Id }
            ]
        };

        var requestUri = new Uri($"{_endpointBasePath}{sectionToUpdate.Id}", UriKind.Relative);
        var serializedDto = JsonSerializer.Serialize(updateDto, JsonOptions);
        var content = new StringContent(serializedDto, Encoding.UTF8, "application/json");

        // Act
        var response = await Fixture.HttpClient.PutAsync(requestUri, content);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        Fixture.DbContext.ChangeTracker.Clear();
        var updatedSectionInDb = await Fixture.DbContext.PartnersSections
            .Include(s => s.Partners)
            .FirstAsync(s => s.Id == sectionToUpdate.Id);

        Assert.Equal(updateDto.Title, updatedSectionInDb.Title);
        Assert.Equal(sectionToUpdate.Partners.Count, updatedSectionInDb.Partners.Count); // -1 deleted, +1 created
        Assert.Null(updatedSectionInDb.Partners.FirstOrDefault(p => p.Id == partnerToDelete.Id));
        Assert.Equal("Updated Partner Description", updatedSectionInDb.Partners.First(p => p.Id == partnerToUpdate.Id).Description);
    }

    [Fact]
    public async Task UpdateSection_WithInvalidDto_ShouldReturnBadRequest()
    {
        // Arrange
        var sectionToUpdate = await Fixture.DbContext.PartnersSections.FirstAsync();
        var updateDto = new UpdatePartnersSectionDto
        {
            Title = new string('T', PartnerConstants.PartnersSectionTitleMaxLength + 1), // Невалідний заголовок
            Description = "Valid Description"
        };
        var requestUri = new Uri($"{_endpointBasePath}{sectionToUpdate.Id}", UriKind.Relative);
        var serializedDto = JsonSerializer.Serialize(updateDto, JsonOptions);
        var content = new StringContent(serializedDto, Encoding.UTF8, "application/json");

        // Act
        var response = await Fixture.HttpClient.PutAsync(requestUri, content);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task UpdateSection_WithNonExistentSectionId_ShouldReturnNotFound()
    {
        // Arrange
        var updateDto = new UpdatePartnersSectionDto { Title = "Title", Description = "Desc" };
        var requestUri = new Uri($"{_endpointBasePath}{long.MaxValue}", UriKind.Relative);
        var serializedDto = JsonSerializer.Serialize(updateDto, JsonOptions);
        var content = new StringContent(serializedDto, Encoding.UTF8, "application/json");

        // Act
        var response = await Fixture.HttpClient.PutAsync(requestUri, content);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task UpdateSection_WithNonExistentImageId_ShouldReturnNotFound()
    {
        // Arrange
        var sectionToUpdate = await Fixture.DbContext.PartnersSections.FirstAsync();
        var updateDto = new UpdatePartnersSectionDto
        {
            Title = "Valid Title",
            Description = "Valid Description",
            PartnersToCreate = [new() { Description = "Partner with fake image", ImageId = long.MaxValue }]
        };
        var requestUri = new Uri($"{_endpointBasePath}{sectionToUpdate.Id}", UriKind.Relative);
        var serializedDto = JsonSerializer.Serialize(updateDto, JsonOptions);
        var content = new StringContent(serializedDto, Encoding.UTF8, "application/json");

        // Act
        var response = await Fixture.HttpClient.PutAsync(requestUri, content);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task UpdateSection_WithNonExistentPartnerIdToDelete_ShouldReturnNotFound()
    {
        // Arrange
        var sectionToUpdate = await Fixture.DbContext.PartnersSections.FirstAsync();
        var updateDto = new UpdatePartnersSectionDto
        {
            Title = "Valid Title",
            Description = "Valid Description",
            PartnerIdsToDelete = [long.MaxValue]
        };
        var requestUri = new Uri($"{_endpointBasePath}{sectionToUpdate.Id}", UriKind.Relative);
        var serializedDto = JsonSerializer.Serialize(updateDto, JsonOptions);
        var content = new StringContent(serializedDto, Encoding.UTF8, "application/json");

        // Act
        var response = await Fixture.HttpClient.PutAsync(requestUri, content);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
