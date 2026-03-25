using FluentValidation.TestHelper;
using VictoryCenter.BLL.Commands.Admin.CompanyProfile.Create;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.CompanyProfileContacts;
using VictoryCenter.BLL.DTOs.Admin.CompanyProfileRequisites;
using VictoryCenter.BLL.DTOs.Admin.CompanyProfiles;
using VictoryCenter.BLL.DTOs.Admin.SocialLinks;
using VictoryCenter.BLL.Validators.CompanyProfile.Commands;
using VictoryCenter.DAL.Enums;

namespace VictoryCenter.UnitTests.ValidatorsTests.CompanyProfile;

public class CreateCompanyProfileCommandValidatorTests
{
    private readonly CreateCompanyProfileCommandValidator _validator = new();

    [Fact]
    public void Validate_ShouldNotHaveErrors_WhenDataIsValid()
    {
        var result = _validator.TestValidate(new CreateCompanyProfileCommand(GetValidDto()));
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenDtoIsNull()
    {
        var result = _validator.TestValidate(new CreateCompanyProfileCommand(null!));
        result.ShouldHaveValidationErrorFor(x => x.CreateCompanyProfileDto);
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenContactsIsNull()
    {
        var dto = GetValidDto();
        dto.Contacts = null!;
        var result = _validator.TestValidate(new CreateCompanyProfileCommand(dto));
        result.ShouldHaveValidationErrorFor(x => x.CreateCompanyProfileDto.Contacts);
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
        var result = _validator.TestValidate(new CreateCompanyProfileCommand(dto));
        result.ShouldHaveValidationErrorFor(x => x.CreateCompanyProfileDto.Contacts.Phone)
            .WithErrorMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(BaseCompanyProfileContactsDto.Phone)));
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenPhoneExceedsMaxLength()
    {
        var contacts = GetValidContacts();
        contacts.Phone = new string('1', CompanyProfileConstants.Phone.MaxLength + 1);
        var dto = GetValidDto();
        dto.Contacts = contacts;
        var result = _validator.TestValidate(new CreateCompanyProfileCommand(dto));
        result.ShouldHaveValidationErrorFor(x => x.CreateCompanyProfileDto.Contacts.Phone);
    }

    [Theory]
    [InlineData("notanemail")]
    [InlineData("missing@")]
    [InlineData("@nodomain.com")]
    public void Validate_ShouldHaveError_WhenEmailIsInvalid(string email)
    {
        var contacts = GetValidContacts();
        contacts.Email = email;
        var dto = GetValidDto();
        dto.Contacts = contacts;
        var result = _validator.TestValidate(new CreateCompanyProfileCommand(dto));
        result.ShouldHaveValidationErrorFor(x => x.CreateCompanyProfileDto.Contacts.Email);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Validate_ShouldHaveError_WhenContactAddressIsEmpty(string? address)
    {
        var contacts = GetValidContacts();
        contacts.Address = address!;
        var dto = GetValidDto();
        dto.Contacts = contacts;
        var result = _validator.TestValidate(new CreateCompanyProfileCommand(dto));
        result.ShouldHaveValidationErrorFor(x => x.CreateCompanyProfileDto.Contacts.Address)
            .WithErrorMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(BaseCompanyProfileContactsDto.Address)));
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenMottoExceedsMaxLength()
    {
        var contacts = GetValidContacts();
        contacts.Motto = new string('x', CompanyProfileConstants.Motto.MaxLength + 1);
        var dto = GetValidDto();
        dto.Contacts = contacts;
        var result = _validator.TestValidate(new CreateCompanyProfileCommand(dto));
        result.ShouldHaveValidationErrorFor(x => x.CreateCompanyProfileDto.Contacts.Motto);
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenRequisitesIsNull()
    {
        var dto = GetValidDto();
        dto.Requisites = null!;
        var result = _validator.TestValidate(new CreateCompanyProfileCommand(dto));
        result.ShouldHaveValidationErrorFor(x => x.CreateCompanyProfileDto.Requisites);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Validate_ShouldHaveError_WhenRecipientIsEmpty(string? recipient)
    {
        var requisites = GetValidRequisites();
        requisites.Recipient = recipient!;
        var dto = GetValidDto();
        dto.Requisites = requisites;
        var result = _validator.TestValidate(new CreateCompanyProfileCommand(dto));
        result.ShouldHaveValidationErrorFor(x => x.CreateCompanyProfileDto.Requisites.Recipient)
            .WithErrorMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(BaseCompanyProfileRequisiteDto.Recipient)));
    }

    [Theory]
    [InlineData("1234567")]
    [InlineData("123456789")]
    public void Validate_ShouldHaveError_WhenEdrpouHasWrongLength(string edrpou)
    {
        var requisites = GetValidRequisites();
        requisites.Edrpou = edrpou;
        var dto = GetValidDto();
        dto.Requisites = requisites;
        var result = _validator.TestValidate(new CreateCompanyProfileCommand(dto));
        result.ShouldHaveValidationErrorFor(x => x.CreateCompanyProfileDto.Requisites.Edrpou);
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenEdrpouContainsNonDigits()
    {
        var requisites = GetValidRequisites();
        requisites.Edrpou = "1234567a";
        var dto = GetValidDto();
        dto.Requisites = requisites;
        var result = _validator.TestValidate(new CreateCompanyProfileCommand(dto));
        result.ShouldHaveValidationErrorFor(x => x.CreateCompanyProfileDto.Requisites.Edrpou)
            .WithErrorMessage(ErrorMessagesConstants.PropertyMustContainOnlyDigits(nameof(BaseCompanyProfileRequisiteDto.Edrpou)));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("not-a-url")]
    public void Validate_ShouldHaveError_WhenSocialLinkUrlIsInvalid(string? url)
    {
        var dto = GetValidDto();
        dto.SocialLinks = [new CreateSocialLinkDto { SocialPlatform = SocialPlatform.Facebook, Url = url! }];
        var result = _validator.TestValidate(new CreateCompanyProfileCommand(dto));
        Assert.NotEmpty(result.Errors);
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenSocialLinkUrlExceedsMaxLength()
    {
        var longUrl = "https://" + new string('a', CompanyProfileConstants.SocialLinkUrl.MaxLength) + ".com";
        var dto = GetValidDto();
        dto.SocialLinks = [new CreateSocialLinkDto { SocialPlatform = SocialPlatform.Facebook, Url = longUrl }];
        var result = _validator.TestValidate(new CreateCompanyProfileCommand(dto));
        Assert.NotEmpty(result.Errors);
    }

    private static CreateCompanyProfileDto GetValidDto() => new()
    {
        Contacts = GetValidContacts(),
        Requisites = GetValidRequisites(),
        SocialLinks =
        [
            new CreateSocialLinkDto { SocialPlatform = SocialPlatform.Facebook, Url = "https://facebook.com/victory" }
        ]
    };

    private static CreateCompanyProfileContactsDto GetValidContacts() => new()
    {
        Phone = "+380501234567",
        Address = "Kyiv, Str 1",
        Email = "test@test.com",
        CorrespondenceEmail = "corr@test.com",
        Motto = "Our motto",
        Localization = []
    };

    private static CreateCompanyProfileRequisiteDto GetValidRequisites() => new()
    {
        Recipient = "Recipient Name",
        Edrpou = "12345678",
        Address = "Requisite Address",
        Localization = []
    };
}
