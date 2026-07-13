using FluentValidation;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.MainDonations;

namespace VictoryCenter.BLL.Validators.MainPage.Dto;

public abstract class BaseMainDonationsDtoValidator<TDto> : AbstractValidator<TDto>
    where TDto : BaseMainDonationsDto
{
    protected BaseMainDonationsDtoValidator()
    {
        RuleFor(x => x.Title)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(BaseMainDonationsDto.Title)))
            .MinimumLength(MainPageConstants.ValidationTitleRules.MinLen)
            .WithMessage(ErrorMessagesConstants.PropertyMustHaveAMinimumLengthOfNCharacters(
                nameof(BaseMainDonationsDto.Title), MainPageConstants.ValidationTitleRules.MinLen))
            .MaximumLength(MainPageConstants.ValidationTitleRules.MaxLen)
            .WithMessage(ErrorMessagesConstants.PropertyMustHaveAMaximumLengthOfNCharacters(
                nameof(BaseMainDonationsDto.Title), MainPageConstants.ValidationTitleRules.MaxLen));

        RuleFor(x => x.Description)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(BaseMainDonationsDto.Description)))
            .MinimumLength(MainPageConstants.ValidationDescriptionRules.MinLen)
            .WithMessage(ErrorMessagesConstants.PropertyMustHaveAMinimumLengthOfNCharacters(
                nameof(BaseMainDonationsDto.Description), MainPageConstants.ValidationDescriptionRules.MinLen))
            .MaximumLength(MainPageConstants.ValidationDescriptionRules.MaxLen)
            .WithMessage(ErrorMessagesConstants.PropertyMustHaveAMaximumLengthOfNCharacters(
                nameof(BaseMainDonationsDto.Description), MainPageConstants.ValidationDescriptionRules.MaxLen));

        RuleFor(x => x.ImageId)
            .GreaterThan(0)
            .WithMessage(ErrorMessagesConstants.PropertyMustBePositive(nameof(BaseMainDonationsDto.ImageId)))
            .When(x => x.ImageId is not null);
    }
}
