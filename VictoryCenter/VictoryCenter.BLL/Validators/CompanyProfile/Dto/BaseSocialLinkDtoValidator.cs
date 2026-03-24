using FluentValidation;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.SocialLinks;

namespace VictoryCenter.BLL.Validators.CompanyProfile.Dto;

public class BaseSocialLinkDtoValidator : AbstractValidator<BaseSocialLinkDto>
{
    public BaseSocialLinkDtoValidator()
    {
        RuleFor(x => x.SocialPlatform)
            .IsInEnum()
            .WithMessage(ErrorMessagesConstants.PropertyMustBeValidEnum(nameof(BaseSocialLinkDto.SocialPlatform)));

        RuleFor(x => x.Url)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(BaseSocialLinkDto.Url)))
            .MaximumLength(CompanyProfileConstants.SocialLinkUrl.MaxLength)
            .WithMessage(ErrorMessagesConstants.PropertyMustHaveAMaximumLengthOfNCharacters(
                nameof(BaseSocialLinkDto.Url), CompanyProfileConstants.SocialLinkUrl.MaxLength))
            .Must(url => Uri.TryCreate(url, UriKind.Absolute, out _))
            .WithMessage(ErrorMessagesConstants.PropertyMustBeInAValidFormat(nameof(BaseSocialLinkDto.Url)));
    }
}
