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
            .MaximumLength(PartnerConstants.PartnersPageBannerTitleMaxLength)
            .WithMessage(ErrorMessagesConstants.PropertyMustHaveAMaximumLengthOfNCharacters(
                nameof(UpdatePartnersPageBannerDto.Title), PartnerConstants.PartnersPageBannerTitleMaxLength));
        RuleFor(x => x.Dto.Description)
            .NotEmpty()
            .WithMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(UpdatePartnersPageBannerDto.Title)))
            .MaximumLength(PartnerConstants.PartnersPageBannerDescriptionMaxLength)
            .WithMessage(ErrorMessagesConstants.PropertyMustHaveAMaximumLengthOfNCharacters(
                nameof(UpdatePartnersPageBannerDto.Title), PartnerConstants.PartnersPageBannerDescriptionMaxLength));
        RuleFor(x => x.Dto.ImageId)
            .NotNull().WithMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(UpdatePartnersPageBannerDto.ImageId)))
            .GreaterThan(0).WithMessage(ErrorMessagesConstants.PropertyMustBePositive(nameof(UpdatePartnersPageBannerDto.ImageId)));
    }
}
