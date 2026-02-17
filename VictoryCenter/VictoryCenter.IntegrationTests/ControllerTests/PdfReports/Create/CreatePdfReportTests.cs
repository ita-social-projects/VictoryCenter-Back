using System.Net;
using System.Text.Json;
using VictoryCenter.BLL.DTOs.Admin.PdfReports;
using VictoryCenter.IntegrationTests.Utils;
using VictoryCenter.IntegrationTests.Utils.DbFixture;
using Microsoft.EntityFrameworkCore;

namespace VictoryCenter.IntegrationTests.ControllerTests.PdfReports.Create;

public class CreatePdfReportTests : BaseTestClass
{
    public CreatePdfReportTests(IntegrationTestDbFixture fixture)
        : base(fixture)
    {
    }

    [Fact]
    public async Task CreatePdfReport_ValidData_ShouldCreatePdfReport()
    {
        // Arrange
        using var form = CreatePdfFormData();

        // Act
        var response = await Fixture.HttpClient.PostAsync("api/PdfReports", form);
        var responseString = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<PdfReportDto>(responseString, JsonOptions);

        // Assert
        Assert.True(response.IsSuccessStatusCode);
        Assert.NotNull(result);
        Assert.Equal("test-report", result.Name);
        Assert.NotNull(result.BlobName);
        Assert.True(result.FileSizeBytes > 0);

        var filePath = Path.Combine(Fixture.PdfEnvironmentVariables.FullPath, result.BlobName);
        Assert.True(File.Exists(filePath));

        var dbRecord = await Fixture.DbContext.PdfReports.FirstOrDefaultAsync(p => p.BlobName == result.BlobName);
        Assert.NotNull(dbRecord);
        Assert.Equal(result.Name, dbRecord.Name);
    }

    [Fact]
    public async Task CreatePdfReport_InvalidMimeType_ShouldReturnBadRequest()
    {
        // Arrange
        using var form = CreatePdfFormData(contentType: "image/jpeg");

        // Act
        var response = await Fixture.HttpClient.PostAsync("api/PdfReports", form);

        // Assert
        Assert.False(response.IsSuccessStatusCode);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task CreatePdfReport_EmptyFile_ShouldReturnBadRequest()
    {
        // Arrange
        using var form = CreatePdfFormData(content: Array.Empty<byte>());

        // Act
        var response = await Fixture.HttpClient.PostAsync("api/PdfReports", form);

        // Assert
        Assert.False(response.IsSuccessStatusCode);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task CreatePdfReport_InvalidPdfSignature_ShouldReturnBadRequest()
    {
        // Arrange
        using var form = CreatePdfFormData(content: new byte[] { 0x00, 0x01, 0x02, 0x03 });

        // Act
        var response = await Fixture.HttpClient.PostAsync("api/PdfReports", form);

        // Assert
        Assert.False(response.IsSuccessStatusCode);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task CreatePdfReport_NoFile_ShouldReturnBadRequest()
    {
        // Arrange
        using var form = new MultipartFormDataContent();

        // Act
        var response = await Fixture.HttpClient.PostAsync("api/PdfReports", form);

        // Assert
        Assert.False(response.IsSuccessStatusCode);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task CreatePdfReport_ShouldSetPriorityCorrectly()
    {
        // Arrange
        using var form1 = CreatePdfFormData(fileName: "first-report.pdf");
        using var form2 = CreatePdfFormData(fileName: "second-report.pdf");

        // Act
        var response1 = await Fixture.HttpClient.PostAsync("api/PdfReports", form1);
        var response2 = await Fixture.HttpClient.PostAsync("api/PdfReports", form2);

        var result1 = JsonSerializer.Deserialize<PdfReportDto>(
            await response1.Content.ReadAsStringAsync(), JsonOptions);
        var result2 = JsonSerializer.Deserialize<PdfReportDto>(
            await response2.Content.ReadAsStringAsync(), JsonOptions);

        // Assert
        Assert.True(response1.IsSuccessStatusCode);
        Assert.True(response2.IsSuccessStatusCode);
        Assert.True(result2!.Priority > result1!.Priority);
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
