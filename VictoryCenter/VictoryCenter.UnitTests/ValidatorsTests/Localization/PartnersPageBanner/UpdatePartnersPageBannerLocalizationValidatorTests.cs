using FluentValidation.TestHelper;
using VictoryCenter.BLL.Commands.Admin.Localization.PartnersPageBanner.Update;
using VictoryCenter.BLL.DTOs.Admin.Localization.PartnersPageBanner;
using VictoryCenter.BLL.Validators.Localization.PartnersPageBanner;

namespace VictoryCenter.UnitTests.ValidatorsTests.Localization.PartnersPageBanner;

public class UpdatePartnersPageBannerLocalizationValidatorTests
{
    private readonly UpdatePartnersPageBannerLocalizationValidator _validator;

    public UpdatePartnersPageBannerLocalizationValidatorTests()
    {
        _validator = new UpdatePartnersPageBannerLocalizationValidator(new BasePartnersPageBannerLocalizationValidator());
    }

    [Fact]
    public void Validate_ShouldNotHaveErrors_WhenModel_IsValid()
    {
        _validator.TestValidate(BuildCommand(GetValidDto())).ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenTitle_IsEmpty()
    {
        var dto = GetValidDto() with { Title = " " };

        _validator.TestValidate(BuildCommand(dto))
            .ShouldHaveValidationErrorFor(x => x.UpdatePartnersPageBannerLocalizationDto.Title);
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenDescription_IsTooLong()
    {
        var dto = GetValidDto() with { Description = new string('a', 31) };

        _validator.TestValidate(BuildCommand(dto))
            .ShouldHaveValidationErrorFor(x => x.UpdatePartnersPageBannerLocalizationDto.Description);
    }

    private static UpdatePartnersPageBannerLocalizationDto GetValidDto() => new()
    {
        Title = "Valid banner title",
        Description = "Valid banner description"
    };

    private static UpdatePartnersPageBannerLocalizationCommand BuildCommand(UpdatePartnersPageBannerLocalizationDto dto) => new(dto, 1, 2);
}
