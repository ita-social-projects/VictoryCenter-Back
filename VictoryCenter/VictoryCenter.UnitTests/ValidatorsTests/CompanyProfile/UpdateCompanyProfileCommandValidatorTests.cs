using FluentValidation.TestHelper;
using VictoryCenter.BLL.Commands.Admin.CompanyProfile.Update;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.CompanyProfileContacts;
using VictoryCenter.BLL.DTOs.Admin.CompanyProfileRequisites;
using VictoryCenter.BLL.DTOs.Admin.CompanyProfiles;
using VictoryCenter.BLL.DTOs.Admin.Localization.CompanyProfile;
using VictoryCenter.BLL.DTOs.Admin.SocialLinks;
using VictoryCenter.BLL.Validators.CompanyProfile.Commands;
using VictoryCenter.DAL.Enums;

namespace VictoryCenter.UnitTests.ValidatorsTests.CompanyProfile;

public class UpdateCompanyProfileCommandValidatorTests
{
    private readonly UpdateCompanyProfileCommandValidator _validator = new();

    [Fact]
    public void Validate_ShouldNotHaveErrors_WhenDataIsValid()
    {
        var result = _validator.TestValidate(new UpdateCompanyProfileCommand(GetValidDto()));
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_ShouldNotHaveErrors_WhenLocalizationsAreValid()
    {
        var dto = GetValidDto();
        dto.Contacts = GetValidContacts();
        dto.Contacts.Localizations =
        [
            new UpdateCompanyProfileContactLocalizationDto { LanguageId = 1, Address = "Addr UA" },
            new UpdateCompanyProfileContactLocalizationDto { LanguageId = 2, Address = "Addr EN" }
        ];
        dto.Requisites = GetValidRequisites();
        dto.Requisites.Localizations =
        [
            new UpdateCompanyProfileRequisiteLocalizationDto { LanguageId = 1, Recipient = "Recipient UA" }
        ];

        var result = _validator.TestValidate(new UpdateCompanyProfileCommand(dto));
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenDtoIsNull()
    {
        var result = _validator.TestValidate(new UpdateCompanyProfileCommand(null!));
        result.ShouldHaveValidationErrorFor(x => x.UpdateCompanyProfileDto);
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenContactsIsNull()
    {
        var dto = GetValidDto();
        dto.Contacts = null!;
        var result = _validator.TestValidate(new UpdateCompanyProfileCommand(dto));
        result.ShouldHaveValidationErrorFor(x => x.UpdateCompanyProfileDto.Contacts);
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenRequisitesIsNull()
    {
        var dto = GetValidDto();
        dto.Requisites = null!;
        var result = _validator.TestValidate(new UpdateCompanyProfileCommand(dto));
        result.ShouldHaveValidationErrorFor(x => x.UpdateCompanyProfileDto.Requisites);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Validate_ShouldHaveError_WhenPhoneIsEmpty(string? phone)
    {
        var contacts = GetValidContacts();
        contacts.Phone = phone!;
        var dto = GetValidDto();
        dto.Contacts = contacts;
        var result = _validator.TestValidate(new UpdateCompanyProfileCommand(dto));
        result.ShouldHaveValidationErrorFor(x => x.UpdateCompanyProfileDto.Contacts.Phone)
            .WithErrorMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(BaseCompanyProfileContactsDto.Phone)));
    }

    [Theory]
    [InlineData("notanemail")]
    [InlineData("missing@")]
    public void Validate_ShouldHaveError_WhenEmailIsInvalid(string email)
    {
        var contacts = GetValidContacts();
        contacts.Email = email;
        var dto = GetValidDto();
        dto.Contacts = contacts;
        var result = _validator.TestValidate(new UpdateCompanyProfileCommand(dto));
        result.ShouldHaveValidationErrorFor(x => x.UpdateCompanyProfileDto.Contacts.Email);
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenContactLocalizationLanguageIdIsZero()
    {
        var contacts = GetValidContacts();
        contacts.Localizations =
        [
            new UpdateCompanyProfileContactLocalizationDto { LanguageId = 0 }
        ];
        var dto = GetValidDto();
        dto.Contacts = contacts;
        var result = _validator.TestValidate(new UpdateCompanyProfileCommand(dto));
        Assert.NotEmpty(result.Errors);
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenRequisiteLocalizationLanguageIdIsZero()
    {
        var requisites = GetValidRequisites();
        requisites.Localizations =
        [
            new UpdateCompanyProfileRequisiteLocalizationDto { LanguageId = 0 }
        ];
        var dto = GetValidDto();
        dto.Requisites = requisites;
        var result = _validator.TestValidate(new UpdateCompanyProfileCommand(dto));
        Assert.NotEmpty(result.Errors);
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenEdrpouContainsNonDigits()
    {
        var requisites = GetValidRequisites();
        requisites.Edrpou = "1234567a";
        var dto = GetValidDto();
        dto.Requisites = requisites;
        var result = _validator.TestValidate(new UpdateCompanyProfileCommand(dto));
        result.ShouldHaveValidationErrorFor(x => x.UpdateCompanyProfileDto.Requisites.Edrpou);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("not-a-url")]
    public void Validate_ShouldHaveError_WhenSocialLinkUrlIsInvalid(string? url)
    {
        var dto = GetValidDto();
        dto.SocialLinks = [new UpdateSocialLinkDto { SocialPlatform = SocialPlatform.Facebook, Url = url! }];
        var result = _validator.TestValidate(new UpdateCompanyProfileCommand(dto));
        Assert.NotEmpty(result.Errors);
    }

    private static UpdateCompanyProfileDto GetValidDto() => new()
    {
        Contacts = GetValidContacts(),
        Requisites = GetValidRequisites(),
        SocialLinks = []
    };

    private static UpdateCompanyProfileContactDto GetValidContacts() => new()
    {
        Phone = "+380501234567",
        Address = "Kyiv, Str 1",
        Email = "test@test.com",
        CorrespondenceEmail = "corr@test.com",
        Motto = "Our motto",
        Localizations = []
    };

    private static UpdateCompanyProfileRequisiteDto GetValidRequisites() => new()
    {
        Recipient = "Recipient Name",
        Edrpou = "12345678",
        Address = "Requisite Address",
        Localizations = []
    };
}
