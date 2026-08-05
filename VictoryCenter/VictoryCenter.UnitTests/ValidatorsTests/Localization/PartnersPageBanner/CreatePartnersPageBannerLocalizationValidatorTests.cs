using FluentValidation.TestHelper;
using VictoryCenter.BLL.Commands.Admin.Localization.PartnersPageBanner.Create;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.Localization.Base;
using VictoryCenter.BLL.DTOs.Admin.Localization.PartnersPageBanner;
using VictoryCenter.BLL.Validators.Localization.PartnersPageBanner;

namespace VictoryCenter.UnitTests.ValidatorsTests.Localization.PartnersPageBanner;

public class CreatePartnersPageBannerLocalizationValidatorTests
{
    private readonly CreatePartnersPageBannerLocalizationValidator _validator;

    public CreatePartnersPageBannerLocalizationValidatorTests()
    {
        _validator = new CreatePartnersPageBannerLocalizationValidator(new BasePartnersPageBannerLocalizationValidator());
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
            .ShouldHaveValidationErrorFor(x => x.CreatePartnersPageBannerLocalizationDto.EntityId)
            .WithErrorMessage(ErrorMessagesConstants.PropertyMustBePositive(nameof(ILocalizationIdentity.EntityId)));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Validate_ShouldHaveError_WhenLanguageId_IsNotPositive(long languageId)
    {
        var dto = GetValidDto() with { LanguageId = languageId };

        _validator.TestValidate(BuildCommand(dto))
            .ShouldHaveValidationErrorFor(x => x.CreatePartnersPageBannerLocalizationDto.LanguageId)
            .WithErrorMessage(ErrorMessagesConstants.PropertyMustBePositive(nameof(ILocalizationIdentity.LanguageId)));
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenTitle_IsEmpty()
    {
        var dto = GetValidDto() with { Title = " " };

        _validator.TestValidate(BuildCommand(dto))
            .ShouldHaveValidationErrorFor(x => x.CreatePartnersPageBannerLocalizationDto.Title);
    }

    private static CreatePartnersPageBannerLocalizationDto GetValidDto() => new()
    {
        EntityId = 1,
        LanguageId = 2,
        Title = "Valid banner title",
        Description = "Valid banner description"
    };

    private static CreatePartnersPageBannerLocalizationCommand BuildCommand(CreatePartnersPageBannerLocalizationDto dto) => new(dto);
}
