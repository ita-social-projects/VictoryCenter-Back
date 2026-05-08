namespace VictoryCenter.BLL.Constants;

public static class PdfReportConstants
{
    public static readonly int MaxPdfSizeInMb = 10;
    public static readonly int BytesPerMb = 1024 * 1024;
    public static readonly int MaxPdfSizeInBytes = MaxPdfSizeInMb * BytesPerMb;

    public static readonly string PdfMimeType = "application/pdf";
    public static readonly string PdfExtension = "pdf";

    public static readonly byte[] PdfSignature = [0x25, 0x50, 0x44, 0x46]; // %PDF

    public static readonly string FileCannotBeEmpty = "PDF file cannot be empty";
    public static readonly string InvalidPdfFormat = "Invalid file format. Expected PDF";
    public static readonly string InvalidPdfSignature = "File does not have a valid PDF signature";
    public static readonly string FileTooLarge = "File size cannot exceed {0} MB";
    public static readonly string FileMustBePdf = "File must be a PDF";

    public static readonly string FileNameCannotBeEmpty = "File name cannot be empty";
    public static readonly string InvalidFileName = "Invalid file name";
    public static readonly string PdfNotFound = "PDF file not found";
    public static readonly string FailedToCreateDirectory = "Failed to create PDF directory";
    public static readonly string FailedToSavePdf = "Failed to save PDF file";
    public static readonly string FailedToReadPdf = "Failed to read PDF file";
    public static readonly string FailedToDeletePdf = "Failed to delete PDF file";

    public static readonly int NameMinLength = 2;
    public static readonly int NameMaxLength = 50;
    public static readonly string NameMinLengthErrorMessage = "Не менше 2 символів";
    public static readonly string NameMaxLengthErrorMessage = "Не більше 50 символів";
    public static readonly string NameRequiredErrorMessage = "Поле обов'язкове";
}
