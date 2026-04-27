using System.Net;
using System.Text;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.DTOs.Admin.PdfSection;
using VictoryCenter.DAL.Entities;
using VictoryCenter.IntegrationTests.Utils;
using VictoryCenter.IntegrationTests.Utils.DbFixture;

namespace VictoryCenter.IntegrationTests.ControllerTests.PdfSections.Update;

public class UpdatePdfSectionTests : BaseTestClass
{
    public UpdatePdfSectionTests(IntegrationTestDbFixture fixture)
        : base(fixture)
    {
    }

    [Fact]
    public async Task UpdatePdfSection_ValidRequest_ShouldUpdateAndReturnSection()
    {
        // Arrange
        var sections = await Fixture.DbContext.PdfSections.ToListAsync();
        if (!sections.Any())
        {
            Fixture.DbContext.PdfSections.Add(new PdfSection
            {
                Title = "Тестова секція",
                Description = "Тестовий опис",
                CreatedAt = DateTimeOffset.UtcNow
            });
            await Fixture.DbContext.SaveChangesAsync();
        }

        var updateDto = new PdfSectionDto
        {
            Title = "Оновлена назва",
            Description = "Оновлений опис"
        };
        var content = new StringContent(
            JsonSerializer.Serialize(updateDto, JsonOptions),
            Encoding.UTF8,
            "application/json");

        // Act
        var response = await Fixture.HttpClient.PutAsync("/api/PdfSection", content);
        var responseString = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<PdfSectionDto>(responseString, JsonOptions);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(result);
        Assert.Equal("Оновлена назва", result!.Title);
        Assert.Equal("Оновлений опис", result.Description);

        Fixture.DbContext.ChangeTracker.Clear();
        var updatedSection = await Fixture.DbContext.PdfSections.AsNoTracking().FirstAsync();
        Assert.Equal("Оновлена назва", updatedSection.Title);
        Assert.Equal("Оновлений опис", updatedSection.Description);
    }

    [Fact]
    public async Task UpdatePdfSection_NoSection_ShouldReturnNotFound()
    {
        // Arrange
        var sections = await Fixture.DbContext.PdfSections.ToListAsync();
        Fixture.DbContext.PdfSections.RemoveRange(sections);
        await Fixture.DbContext.SaveChangesAsync();

        var updateDto = new PdfSectionDto
        {
            Title = "Оновлена назва",
            Description = "Оновлений опис"
        };
        var content = new StringContent(
            JsonSerializer.Serialize(updateDto, JsonOptions),
            Encoding.UTF8,
            "application/json");

        // Act
        var response = await Fixture.HttpClient.PutAsync("/api/PdfSection", content);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task UpdatePdfSection_EmptyTitle_ShouldReturnBadRequest()
    {
        // Arrange
        var sections = await Fixture.DbContext.PdfSections.ToListAsync();
        if (!sections.Any())
        {
            Fixture.DbContext.PdfSections.Add(new PdfSection
            {
                Title = "Тестова секція",
                Description = "Тестовий опис",
                CreatedAt = DateTimeOffset.UtcNow
            });
            await Fixture.DbContext.SaveChangesAsync();
        }

        var updateDto = new PdfSectionDto
        {
            Title = "",
            Description = "Оновлений опис"
        };
        var content = new StringContent(
            JsonSerializer.Serialize(updateDto, JsonOptions),
            Encoding.UTF8,
            "application/json");

        // Act
        var response = await Fixture.HttpClient.PutAsync("/api/PdfSection", content);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task UpdatePdfSection_EmptyDescription_ShouldReturnBadRequest()
    {
        // Arrange
        var sections = await Fixture.DbContext.PdfSections.ToListAsync();
        if (!sections.Any())
        {
            Fixture.DbContext.PdfSections.Add(new PdfSection
            {
                Title = "Тестова секція",
                Description = "Тестовий опис",
                CreatedAt = DateTimeOffset.UtcNow
            });
            await Fixture.DbContext.SaveChangesAsync();
        }

        var updateDto = new PdfSectionDto
        {
            Title = "Оновлена назва",
            Description = ""
        };
        var content = new StringContent(
            JsonSerializer.Serialize(updateDto, JsonOptions),
            Encoding.UTF8,
            "application/json");

        // Act
        var response = await Fixture.HttpClient.PutAsync("/api/PdfSection", content);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task UpdatePdfSection_NullDto_ShouldReturnBadRequest()
    {
        // Arrange
        var sections = await Fixture.DbContext.PdfSections.ToListAsync();
        if (!sections.Any())
        {
            Fixture.DbContext.PdfSections.Add(new PdfSection
            {
                Title = "Тестова секція",
                Description = "Тестовий опис",
                CreatedAt = DateTimeOffset.UtcNow
            });
            await Fixture.DbContext.SaveChangesAsync();
        }

        var content = new StringContent(
            JsonSerializer.Serialize((object?)null, JsonOptions),
            Encoding.UTF8,
            "application/json");

        // Act
        var response = await Fixture.HttpClient.PutAsync("/api/PdfSection", content);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task UpdatePdfSection_NormalizesExtraSpaces_ShouldReturnCleanedText()
    {
        // Arrange
        var sections = await Fixture.DbContext.PdfSections.ToListAsync();
        if (!sections.Any())
        {
            Fixture.DbContext.PdfSections.Add(new PdfSection
            {
                Title = "Тестова секція",
                Description = "Тестовий опис",
                CreatedAt = DateTimeOffset.UtcNow
            });
            await Fixture.DbContext.SaveChangesAsync();
        }

        var updateDto = new PdfSectionDto
        {
            Title = "  Оновлена   назва  ",
            Description = "  Опис   з   пробілами  "
        };
        var content = new StringContent(
            JsonSerializer.Serialize(updateDto, JsonOptions),
            Encoding.UTF8,
            "application/json");

        // Act
        var response = await Fixture.HttpClient.PutAsync("/api/PdfSection", content);
        var responseString = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<PdfSectionDto>(responseString, JsonOptions);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(result);
        Assert.Equal("Оновлена назва", result!.Title);
        Assert.Equal("Опис з пробілами", result.Description);
    }
}
