using FluentValidation.TestHelper;
using VictoryCenter.BLL.Commands.Admin.Localization.PartnerSections.Update;
using VictoryCenter.BLL.DTOs.Admin.Localization.PartnerSections;
using VictoryCenter.BLL.Validators.Localization.PartnerSections;

namespace VictoryCenter.UnitTests.ValidatorsTests.Localization.PartnerSections;

public class UpdatePartnerSectionLocalizationValidatorTests
{
    private readonly UpdatePartnerSectionLocalizationValidator _validator;

    public UpdatePartnerSectionLocalizationValidatorTests()
    {
        _validator = new UpdatePartnerSectionLocalizationValidator(
            new BasePartnerSectionLocalizationValidator(new PartnerLocalizationItemValidator()));
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
            .ShouldHaveValidationErrorFor(x => x.UpdatePartnerSectionLocalizationDto.Title);
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenDescription_IsTooLong()
    {
        var dto = GetValidDto() with { Description = new string('a', 71) };

        _validator.TestValidate(BuildCommand(dto))
            .ShouldHaveValidationErrorFor(x => x.UpdatePartnerSectionLocalizationDto.Description);
    }

    private static UpdatePartnerSectionLocalizationDto GetValidDto() => new()
    {
        Title = "Valid section title",
        Description = "Valid section description here",
        Partners = [new UpdatePartnerLocalizationItemDto { PartnerId = 1, Description = "Valid description" }]
    };

    private static UpdatePartnerSectionLocalizationCommand BuildCommand(UpdatePartnerSectionLocalizationDto dto) => new(dto, 1, 2);
}
