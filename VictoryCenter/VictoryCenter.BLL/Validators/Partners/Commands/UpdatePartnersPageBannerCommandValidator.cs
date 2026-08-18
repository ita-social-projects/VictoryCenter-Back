using FluentValidation;
using VictoryCenter.BLL.Commands.Admin.Partners.UpdateBanner;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.Partners;
using VictoryCenter.BLL.Helpers;

namespace VictoryCenter.BLL.Validators.Partners.Commands;

public class UpdatePartnersPageBannerCommandValidator : AbstractValidator<UpdatePartnersPageBannerCommand>
{
    public UpdatePartnersPageBannerCommandValidator()
    {
        RuleFor(x => x.Dto.Title)
            .NotEmpty()
            .WithMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(UpdatePartnersPageBannerDto.Title)))
            .Must(title => HtmlContentHelper.StripHtmlTags(title).Length >= PartnerConstants.PartnersPageBannerTitleMinLength)
            .WithMessage(ErrorMessagesConstants.PropertyMustHaveAMinimumVisibleLengthOfNCharacters(
                nameof(UpdatePartnersPageBannerDto.Title), PartnerConstants.PartnersPageBannerTitleMinLength))
            .Must(title => HtmlContentHelper.StripHtmlTags(title).Length <= PartnerConstants.PartnersPageBannerTitleMaxLength)
            .WithMessage(ErrorMessagesConstants.PropertyMustHaveAMaximumVisibleLengthOfNCharacters(
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
}
