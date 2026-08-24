using System.Text;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.Interfaces.ImageValidation;
using VictoryCenter.BLL.Services.ImageValidation;
using VictoryCenter.UnitTests.Utils.Images;

namespace VictoryCenter.UnitTests.ServiceTests;

public class ImageContentValidatorTests
{
    private readonly IImageContentValidator _validator = new ImageContentValidator();

    [Theory]
    [InlineData("image/jpeg")]
    [InlineData("image/jpg")]
    [InlineData("image/png")]
    [InlineData("image/webp")]
    [InlineData("IMAGE/PNG")]
    public void Validate_ValidSupportedImage_ReturnsSuccess(string mimeType)
    {
        string base64 = ImageTestData.CreateBase64(mimeType);

        ImageContentValidationResult result = _validator.Validate(base64, mimeType);

        Assert.True(result.IsValid);
        Assert.Empty(result.Failures);
    }

    [Fact]
    public void Validate_Base64EncodedText_ReturnsInvalidImageContent()
    {
        string base64 = Convert.ToBase64String(Encoding.UTF8.GetBytes("This is not an image"));

        ImageContentValidationResult result = _validator.Validate(base64, ImageMimeTypes.Png);

        AssertFailure(result, ImageConstants.InvalidImageContent);
    }

    [Fact]
    public void Validate_InvalidBase64_ReturnsBase64Error()
    {
        ImageContentValidationResult result = _validator.Validate("not-base64!", ImageMimeTypes.Png);

        AssertFailure(result, ImageConstants.Base64ValidationError);
    }

    [Fact]
    public void Validate_MimeTypeDoesNotMatchImage_ReturnsMismatchError()
    {
        string png = ImageTestData.CreateBase64("image/png");

        ImageContentValidationResult result = _validator.Validate(png, ImageMimeTypes.Jpeg);

        AssertFailure(result, ImageConstants.ImageMimeTypeMismatch);
    }

    [Fact]
    public void Validate_UnsupportedActualFormat_ReturnsContentError()
    {
        string gif = ImageTestData.CreateBase64("image/gif");

        ImageContentValidationResult result = _validator.Validate(gif, ImageMimeTypes.Png);

        AssertFailure(result, ImageConstants.InvalidImageContent);
    }

    [Fact]
    public void Validate_UnsupportedDeclaredMimeType_ReturnsMimeTypeError()
    {
        string png = ImageTestData.CreateBase64("image/png");

        ImageContentValidationResult result = _validator.Validate(png, "image/gif");

        AssertFailure(result, ImageConstants.MimeTypeValidationError(ImageConstants.AllowedMimeTypes));
    }

    [Fact]
    public void Validate_TruncatedImage_ReturnsInvalidImageContent()
    {
        byte[] png = Convert.FromBase64String(ImageTestData.CreateBase64("image/png"));
        string truncated = Convert.ToBase64String(png[.. (png.Length / 2)]);

        ImageContentValidationResult result = _validator.Validate(truncated, ImageMimeTypes.Png);

        AssertFailure(result, ImageConstants.InvalidImageContent);
    }

    [Fact]
    public void Validate_EncodedPayloadExceedsLimit_ReturnsSizeError()
    {
        string oversized = new('A', ImageConstants.MaxBase64Length + 4);

        ImageContentValidationResult result = _validator.Validate(oversized, ImageMimeTypes.Png);

        AssertFailure(result, ImageConstants.InvalidImageSize);
    }

    [Fact]
    public void Validate_DecodedPayloadExceedsLimit_ReturnsSizeError()
    {
        string oversized = new('A', ImageConstants.MaxBase64Length);

        ImageContentValidationResult result = _validator.Validate(oversized, ImageMimeTypes.Png);

        AssertFailure(result, ImageConstants.InvalidImageSize);
    }

    [Fact]
    public void Validate_ImageAtDimensionLimit_ReturnsSuccess()
    {
        string png = ImageTestData.CreateBase64("image/png", ImageConstants.MaxImageWidth, 1);

        ImageContentValidationResult result = _validator.Validate(png, ImageMimeTypes.Png);

        Assert.True(result.IsValid);
    }

    [Fact]
    public void Validate_ImageWidthExceedsLimit_ReturnsDimensionsError()
    {
        string png = ImageTestData.CreateBase64(
            "image/png",
            ImageConstants.MaxImageWidth + 1,
            1);

        ImageContentValidationResult result = _validator.Validate(png, ImageMimeTypes.Png);

        AssertFailure(result, ImageConstants.ImageDimensionsExceeded);
    }

    [Fact]
    public void Validate_PixelCountExceedsLimit_ReturnsPixelCountError()
    {
        string pngHeader = ImageTestData.CreatePngHeader(4001, 3000);

        ImageContentValidationResult result = _validator.Validate(pngHeader, ImageMimeTypes.Png);

        AssertFailure(result, ImageConstants.ImagePixelCountExceeded);
    }

    [Fact]
    public void Validate_DecodedImageSizeExceedsLimit_ReturnsMemoryError()
    {
        string pngHeader = ImageTestData.CreatePngHeader(3000, 3000, bitDepth: 16);

        ImageContentValidationResult result = _validator.Validate(pngHeader, ImageMimeTypes.Png);

        AssertFailure(result, ImageConstants.DecodedImageSizeExceeded);
    }

    [Fact]
    public void Validate_AnimatedWebp_ReturnsAnimationError()
    {
        string animatedWebp = ImageTestData.CreateBase64("image/webp", animated: true);

        ImageContentValidationResult result = _validator.Validate(animatedWebp, ImageMimeTypes.Webp);

        AssertFailure(result, ImageConstants.AnimatedImageNotSupported);
    }

    private static void AssertFailure(ImageContentValidationResult result, string expectedMessage)
    {
        Assert.False(result.IsValid);
        Assert.Contains(result.Failures, failure => failure.ErrorMessage == expectedMessage);
    }
}
