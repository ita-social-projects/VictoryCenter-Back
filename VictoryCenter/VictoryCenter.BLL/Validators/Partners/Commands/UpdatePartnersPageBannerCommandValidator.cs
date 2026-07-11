using System.Text.RegularExpressions;
using FluentValidation;
using VictoryCenter.BLL.Commands.Admin.Partners.UpdateBanner;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.Partners;

namespace VictoryCenter.BLL.Validators.Partners.Commands;

public class UpdatePartnersPageBannerCommandValidator : AbstractValidator<UpdatePartnersPageBannerCommand>
{
    public UpdatePartnersPageBannerCommandValidator()
    {
        RuleFor(x => x.Dto.Title)
            .NotEmpty()
            .WithMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(UpdatePartnersPageBannerDto.Title)))
            .Must(title => StripHtmlTags(title).Length >= PartnerConstants.PartnersPageBannerTitleMinLength)
            .WithMessage(ErrorMessagesConstants.PropertyMustHaveAMinimumLengthOfNCharacters(
                nameof(UpdatePartnersPageBannerDto.Title), PartnerConstants.PartnersPageBannerTitleMinLength))
            .Must(title => StripHtmlTags(title).Length <= PartnerConstants.PartnersPageBannerTitleMaxLength)
            .WithMessage(ErrorMessagesConstants.PropertyMustHaveAMaximumLengthOfNCharacters(
                nameof(UpdatePartnersPageBannerDto.Title), PartnerConstants.PartnersPageBannerTitleMaxLength));

        RuleFor(x => x.Dto.Description)
            .NotEmpty()
            .WithMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(UpdatePartnersPageBannerDto.Description)))
            .MinimumLength(PartnerConstants.PartnersPageBannerDescriptionMinLength)
            .WithMessage(ErrorMessagesConstants.PropertyMustHaveAMinimumLengthOfNCharacters(
                nameof(UpdatePartnersPageBannerDto.Description), PartnerConstants.PartnersPageBannerDescriptionMinLength))
            .MaximumLength(PartnerConstants.PartnersPageBannerDescriptionMaxLength)
            .WithMessage(ErrorMessagesConstants.PropertyMustHaveAMaximumLengthOfNCharacters(
                nameof(UpdatePartnersPageBannerDto.Description), PartnerConstants.PartnersPageBannerDescriptionMaxLength));

        RuleFor(x => x.Dto.ImageId)
            .GreaterThan(0).WithMessage(ErrorMessagesConstants.PropertyMustBePositive(nameof(UpdatePartnersPageBannerDto.ImageId)));
    }

    private static string StripHtmlTags(string input)
    {
        return string.IsNullOrEmpty(input)
            ? input
            : Regex.Replace(input, "<.*?>", string.Empty);
    }
}
