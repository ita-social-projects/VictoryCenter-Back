using Microsoft.AspNetCore.Http;
using Moq;
using VictoryCenter.BLL.Commands.Admin.PdfReports.Create;
using VictoryCenter.BLL.DTOs.Admin.PdfReports;
using VictoryCenter.BLL.Validators.PdfReports;
using FluentValidation.TestHelper;

namespace VictoryCenter.UnitTests.ValidatorsTests.PdfReports;

public class CreatePdfReportValidatorTests
{
    private const long MaxPdfSizeInBytes = 10 * 1024 * 1024;
    private const string PdfMimeType = "application/pdf";
    private readonly CreatePdfReportValidator _validator;

    public CreatePdfReportValidatorTests()
    {
        _validator = new CreatePdfReportValidator();
    }

    [Fact]
    public void Validate_FileIsNull_ShouldHaveError()
    {
        // Arrange
        var command = CreateCommand(null);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.CreatePdfReportDto.File);
    }

    [Fact]
    public void Validate_FileIsEmpty_ShouldHaveError()
    {
        // Arrange
        var command = CreateCommand(CreateMockFile(length: 0));

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.CreatePdfReportDto.File)
            .WithErrorMessage("File cannot be empty");
    }

    [Fact]
    public void Validate_FileSizeExceedsLimit_ShouldHaveError()
    {
        // Arrange
        var command = CreateCommand(CreateMockFile(length: MaxPdfSizeInBytes + 1));

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.CreatePdfReportDto.File)
            .WithErrorMessage($"File size cannot exceed {MaxPdfSizeInBytes / 1024 / 1024} MB");
    }

    [Theory]
    [InlineData("image/jpeg")]
    [InlineData("image/png")]
    [InlineData("application/msword")]
    [InlineData("text/plain")]
    public void Validate_FileIsNotPdf_ShouldHaveError(string contentType)
    {
        // Arrange
        var command = CreateCommand(CreateMockFile(contentType: contentType));

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.CreatePdfReportDto.File)
            .WithErrorMessage("File must be a PDF");
    }

    [Fact]
    public void Validate_ValidPdfFile_ShouldNotHaveErrors()
    {
        // Arrange
        var command = CreateCommand(CreateMockFile());

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_FileSizeAtLimit_ShouldNotHaveError()
    {
        // Arrange
        var command = CreateCommand(CreateMockFile(length: MaxPdfSizeInBytes));

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.CreatePdfReportDto.File);
    }

    private static IFormFile CreateMockFile(
        string contentType = PdfMimeType,
        long length = 1024,
        string fileName = "test.pdf")
    {
        var mockFile = new Mock<IFormFile>();
        mockFile.Setup(f => f.ContentType).Returns(contentType);
        mockFile.Setup(f => f.Length).Returns(length);
        mockFile.Setup(f => f.FileName).Returns(fileName);
        return mockFile.Object;
    }

    private static CreatePdfReportCommand CreateCommand(IFormFile? file)
    {
        return new CreatePdfReportCommand(new CreatePdfReportDto
        {
            File = file!
        });
    }
}
