using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.PdfReports;
using VictoryCenter.IntegrationTests.Utils;
using VictoryCenter.IntegrationTests.Utils.DbFixture;

namespace VictoryCenter.IntegrationTests.ControllerTests.PdfReports.Update;

public class UpdatePdfReportTests : BaseTestClass
{
    public UpdatePdfReportTests(IntegrationTestDbFixture fixture)
        : base(fixture)
    {
    }

    [Fact]
    public async Task UpdatePdfReport_ValidRequest_ShouldUpdateNameAndReturnDto()
    {
        // Arrange
        var created = await CreatePdfReportAsync("original-name.pdf");
        var newName = "Updated Report Name";

        // Act
        var response = await Fixture.HttpClient.PutAsJsonAsync(
            $"api/PdfReports/{created.Id}",
            new UpdatePdfReportRequestDto { Name = newName });

        var responseString = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<PdfReportDto>(responseString, JsonOptions);

        // Assert
        Assert.True(response.IsSuccessStatusCode);
        Assert.NotNull(result);
        Assert.Equal(newName, result.Name);
        Assert.Equal(created.Id, result.Id);

        var dbRecord = await Fixture.DbContext.PdfReports.FirstOrDefaultAsync(p => p.Id == created.Id);
        Assert.NotNull(dbRecord);
        Assert.Equal(newName, dbRecord.Name);
    }

    [Fact]
    public async Task UpdatePdfReport_SameName_ShouldReturnDtoWithoutChanges()
    {
        // Arrange
        var created = await CreatePdfReportAsync("same-name.pdf");

        // Act
        var response = await Fixture.HttpClient.PutAsJsonAsync(
            $"api/PdfReports/{created.Id}",
            new UpdatePdfReportRequestDto { Name = created.Name });

        var responseString = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<PdfReportDto>(responseString, JsonOptions);

        // Assert
        Assert.True(response.IsSuccessStatusCode);
        Assert.NotNull(result);
        Assert.Equal(created.Name, result.Name);
    }

    [Fact]
    public async Task UpdatePdfReport_NameWithLeadingAndTrailingSpaces_ShouldTrimAndSave()
    {
        // Arrange
        var created = await CreatePdfReportAsync("trim-test.pdf");
        var nameWithSpaces = "  Trimmed Name  ";
        var expectedName = "Trimmed Name";

        // Act
        var response = await Fixture.HttpClient.PutAsJsonAsync(
            $"api/PdfReports/{created.Id}",
            new UpdatePdfReportRequestDto { Name = nameWithSpaces });

        var responseString = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<PdfReportDto>(responseString, JsonOptions);

        // Assert
        Assert.True(response.IsSuccessStatusCode);
        Assert.NotNull(result);
        Assert.Equal(expectedName, result.Name);

        var dbRecord = await Fixture.DbContext.PdfReports.FirstOrDefaultAsync(p => p.Id == created.Id);
        Assert.NotNull(dbRecord);
        Assert.Equal(expectedName, dbRecord.Name);
    }

    [Fact]
    public async Task UpdatePdfReport_NameWithMultipleSpaces_ShouldNormalizeAndSave()
    {
        // Arrange
        var created = await CreatePdfReportAsync("spaces-test.pdf");
        var nameWithMultipleSpaces = "Report   With    Multiple    Spaces";
        var expectedName = "Report With Multiple Spaces";

        // Act
        var response = await Fixture.HttpClient.PutAsJsonAsync(
            $"api/PdfReports/{created.Id}",
            new UpdatePdfReportRequestDto { Name = nameWithMultipleSpaces });

        var responseString = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<PdfReportDto>(responseString, JsonOptions);

        // Assert
        Assert.True(response.IsSuccessStatusCode);
        Assert.NotNull(result);
        Assert.Equal(expectedName, result.Name);

        var dbRecord = await Fixture.DbContext.PdfReports.FirstOrDefaultAsync(p => p.Id == created.Id);
        Assert.NotNull(dbRecord);
        Assert.Equal(expectedName, dbRecord.Name);
    }

    [Fact]
    public async Task UpdatePdfReport_NotExistingId_ShouldReturnNotFound()
    {
        // Act
        var response = await Fixture.HttpClient.PutAsJsonAsync(
            "api/PdfReports/999999",
            new UpdatePdfReportRequestDto { Name = "Some Name" });

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task UpdatePdfReport_EmptyName_ShouldReturnBadRequest()
    {
        // Arrange
        var created = await CreatePdfReportAsync("empty-name-test.pdf");

        // Act
        var response = await Fixture.HttpClient.PutAsJsonAsync(
            $"api/PdfReports/{created.Id}",
            new UpdatePdfReportRequestDto { Name = "" });

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task UpdatePdfReport_WhitespaceOnlyName_ShouldReturnBadRequest()
    {
        // Arrange
        var created = await CreatePdfReportAsync("whitespace-test.pdf");

        // Act
        var response = await Fixture.HttpClient.PutAsJsonAsync(
            $"api/PdfReports/{created.Id}",
            new UpdatePdfReportRequestDto { Name = "   " });

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var dbRecord = await Fixture.DbContext.PdfReports.FirstOrDefaultAsync(p => p.Id == created.Id);
        Assert.NotNull(dbRecord);
        Assert.NotEqual(string.Empty, dbRecord.Name);
    }

    [Fact]
    public async Task UpdatePdfReport_NameTooShort_ShouldReturnBadRequest()
    {
        // Arrange
        var created = await CreatePdfReportAsync("short-name-test.pdf");

        // Act
        var response = await Fixture.HttpClient.PutAsJsonAsync(
            $"api/PdfReports/{created.Id}",
            new UpdatePdfReportRequestDto { Name = "A" });

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task UpdatePdfReport_NameTooShortAfterNormalization_ShouldReturnBadRequest()
    {
        // Arrange
        var created = await CreatePdfReportAsync("short-after-norm-test.pdf");

        // Act
        var response = await Fixture.HttpClient.PutAsJsonAsync(
            $"api/PdfReports/{created.Id}",
            new UpdatePdfReportRequestDto { Name = "A  " });

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var dbRecord = await Fixture.DbContext.PdfReports.FirstOrDefaultAsync(p => p.Id == created.Id);
        Assert.NotNull(dbRecord);
        Assert.NotEqual("A", dbRecord.Name);
    }

    [Fact]
    public async Task UpdatePdfReport_NameTooLong_ShouldReturnBadRequest()
    {
        // Arrange
        var created = await CreatePdfReportAsync("long-name-test.pdf");
        var tooLongName = new string('a', PdfReportConstants.NameMaxLength + 1);

        // Act
        var response = await Fixture.HttpClient.PutAsJsonAsync(
            $"api/PdfReports/{created.Id}",
            new UpdatePdfReportRequestDto { Name = tooLongName });

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task UpdatePdfReport_InvalidId_ShouldReturnBadRequest()
    {
        // Act
        var response = await Fixture.HttpClient.PutAsJsonAsync(
            "api/PdfReports/-1",
            new UpdatePdfReportRequestDto { Name = "Valid Name" });

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    private async Task<PdfReportDto> CreatePdfReportAsync(string fileName = "test-report.pdf")
    {
        using var form = CreatePdfFormData(fileName: fileName);
        var response = await Fixture.HttpClient.PostAsync("api/PdfReports", form);
        response.EnsureSuccessStatusCode();
        var responseString = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<PdfReportDto>(responseString, JsonOptions)!;
    }

    private static MultipartFormDataContent CreatePdfFormData(
        byte[] content = null!,
        string fileName = "test-report.pdf",
        string contentType = "application/pdf")
    {
        var pdfContent = content ?? new byte[] { 0x25, 0x50, 0x44, 0x46, 0x2D, 0x31, 0x2E, 0x34 };
        var form = new MultipartFormDataContent();
        var fileContent = new ByteArrayContent(pdfContent);
        fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(contentType);
        form.Add(fileContent, "File", fileName);
        return form;
    }
}
