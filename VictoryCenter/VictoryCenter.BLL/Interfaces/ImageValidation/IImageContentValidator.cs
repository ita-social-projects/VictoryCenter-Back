namespace VictoryCenter.BLL.Interfaces.ImageValidation;

public interface IImageContentValidator
{
    ImageContentValidationResult Validate(string base64, string mimeType);
}

public sealed class ImageContentValidationResult
{
    private ImageContentValidationResult(IReadOnlyCollection<ImageContentValidationFailure> failures)
    {
        Failures = failures;
    }

    public IReadOnlyCollection<ImageContentValidationFailure> Failures { get; }

    public bool IsValid => Failures.Count == 0;

    public static ImageContentValidationResult Success { get; } = new ImageContentValidationResult([]);

    public static ImageContentValidationResult Failure(string propertyName, string errorMessage)
    {
        return new ImageContentValidationResult([new ImageContentValidationFailure(propertyName, errorMessage)]);
    }
}

public sealed class ImageContentValidationFailure
{
    public ImageContentValidationFailure(string propertyName, string errorMessage)
    {
        PropertyName = propertyName;
        ErrorMessage = errorMessage;
    }

    public string PropertyName { get; }

    public string ErrorMessage { get; }
}
