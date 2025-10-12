using FluentValidation;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.Localization.Languages;

namespace VictoryCenter.BLL.Validators.Localization.Languages;

public class BaseLocalizationLanguageValidator : AbstractValidator<CreateLocalizationLanguageDto>
{
    public BaseLocalizationLanguageValidator()
    {
        RuleFor(x => x.Code)
            .NotEmpty().WithMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(CreateLocalizationLanguageDto.Code)))
            .Length(CodeLength).WithMessage($"Code length has to be {CodeLength} long");
    }

    public static int CodeLength { get; } = 2;
}
