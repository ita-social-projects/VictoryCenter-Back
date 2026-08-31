using FluentValidation;
using VictoryCenter.BLL.Interfaces.ImageValidation;

namespace VictoryCenter.BLL.Validators.Images;

public abstract class BaseImageValidator<TImageCommand> : AbstractValidator<TImageCommand>
{
    protected void AddImageContentRule(
        Func<TImageCommand, string?> base64Selector,
        Func<TImageCommand, string?> mimeTypeSelector,
        IImageContentValidator imageContentValidator)
    {
        RuleFor(command => command).Custom((command, context) =>
        {
            string? base64 = base64Selector(command);
            string? mimeType = mimeTypeSelector(command);
            if (string.IsNullOrWhiteSpace(base64) || string.IsNullOrWhiteSpace(mimeType))
            {
                return;
            }

            ImageContentValidationResult result = imageContentValidator.Validate(base64, mimeType);
            foreach (ImageContentValidationFailure failure in result.Failures)
            {
                context.AddFailure(failure.PropertyName, failure.ErrorMessage);
            }
        });
    }
}
