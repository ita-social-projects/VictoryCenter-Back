using FluentValidation;

namespace VictoryCenter.BLL.Validators.Partners;

public abstract class BaseImageValidator<TImageDto> : AbstractValidator<TImageDto>
{
    protected bool IsValidBase64(string? base64)
    {
        if (string.IsNullOrWhiteSpace(base64))
        {
            return false;
        }

        Span<byte> buffer = new(new byte[base64.Length]);
        return Convert.TryFromBase64String(base64, buffer, out _);
    }
}
