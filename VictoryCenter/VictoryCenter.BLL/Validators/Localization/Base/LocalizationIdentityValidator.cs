using FluentValidation;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.Localization.Base;

namespace VictoryCenter.BLL.Validators.Localization.Base;
public class LocalizationIdentityValidator<T> : AbstractValidator<T>
    where T : ILocalizationIdentity
{
    public LocalizationIdentityValidator()
    {
        RuleFor(x => x.EntityId)
            .GreaterThan(0).WithMessage(ErrorMessagesConstants.PropertyMustBePositive(nameof(ILocalizationIdentity.EntityId)));
        RuleFor(x => x.LanguageId)
            .GreaterThan(0).WithMessage(ErrorMessagesConstants.PropertyMustBePositive(nameof(ILocalizationIdentity.LanguageId)));
    }
}
