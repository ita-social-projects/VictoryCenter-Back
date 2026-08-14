using FluentValidation.TestHelper;
using VictoryCenter.BLL.Commands.Admin.Localization.PartnerSections.Create;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.Localization.Base;
using VictoryCenter.BLL.DTOs.Admin.Localization.PartnerSections;
using VictoryCenter.BLL.Validators.Localization.PartnerSections;

namespace VictoryCenter.UnitTests.ValidatorsTests.Localization.PartnerSections;

public class CreatePartnerSectionLocalizationValidatorTests
{
    private readonly CreatePartnerSectionLocalizationValidator _validator;

    public CreatePartnerSectionLocalizationValidatorTests()
    {
        _validator = new CreatePartnerSectionLocalizationValidator(
            new BasePartnerSectionLocalizationValidator(new PartnerLocalizationItemValidator()));
    }

    [Fact]
    public void Validate_ShouldNotHaveErrors_WhenModel_IsValid()
    {
        _validator.TestValidate(BuildCommand(GetValidDto())).ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Validate_ShouldHaveError_WhenEntityId_IsNotPositive(long entityId)
    {
        var dto = GetValidDto() with { EntityId = entityId };

        _validator.TestValidate(BuildCommand(dto))
            .ShouldHaveValidationErrorFor(x => x.CreatePartnerSectionLocalizationDto.EntityId)
            .WithErrorMessage(ErrorMessagesConstants.PropertyMustBePositive(nameof(ILocalizationIdentity.EntityId)));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Validate_ShouldHaveError_WhenLanguageId_IsNotPositive(long languageId)
    {
        var dto = GetValidDto() with { LanguageId = languageId };

        _validator.TestValidate(BuildCommand(dto))
            .ShouldHaveValidationErrorFor(x => x.CreatePartnerSectionLocalizationDto.LanguageId)
            .WithErrorMessage(ErrorMessagesConstants.PropertyMustBePositive(nameof(ILocalizationIdentity.LanguageId)));
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenTitle_IsEmpty()
    {
        var dto = GetValidDto() with { Title = " " };

        _validator.TestValidate(BuildCommand(dto))
            .ShouldHaveValidationErrorFor(x => x.CreatePartnerSectionLocalizationDto.Title);
    }

    private static CreatePartnerSectionLocalizationDto GetValidDto() => new()
    {
        EntityId = 1,
        LanguageId = 2,
        Title = "Valid section title",
        Description = "Valid section description here",
        Partners = [new UpdatePartnerLocalizationItemDto { PartnerId = 1, Description = "Valid description" }]
    };

    private static CreatePartnerSectionLocalizationCommand BuildCommand(CreatePartnerSectionLocalizationDto dto) => new(dto);
}
