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
            .MinimumLength(MainPageConstants.Title.MinLength)
            .WithMessage(ErrorMessagesConstants.PropertyMustHaveAMinimumLengthOfNCharacters(
                nameof(BaseMainDonationsDto.Title), MainPageConstants.Title.MinLength))
            .MaximumLength(MainPageConstants.Title.MaxLength)
            .WithMessage(ErrorMessagesConstants.PropertyMustHaveAMaximumLengthOfNCharacters(
                nameof(BaseMainDonationsDto.Title), MainPageConstants.Title.MaxLength));

        RuleFor(x => x.Description)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(BaseMainDonationsDto.Description)))
            .MinimumLength(MainPageConstants.Description.MinLength)
            .WithMessage(ErrorMessagesConstants.PropertyMustHaveAMinimumLengthOfNCharacters(
                nameof(BaseMainDonationsDto.Description), MainPageConstants.Description.MinLength))
            .MaximumLength(MainPageConstants.Description.MaxLength)
            .WithMessage(ErrorMessagesConstants.PropertyMustHaveAMaximumLengthOfNCharacters(
                nameof(BaseMainDonationsDto.Description), MainPageConstants.Description.MaxLength));

        RuleFor(x => x.ImageId)
            .GreaterThan(0)
            .WithMessage(ErrorMessagesConstants.PropertyMustBePositive(nameof(BaseMainDonationsDto.ImageId)))
            .When(x => x.ImageId.HasValue);
    }
}
