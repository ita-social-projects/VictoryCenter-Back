using FluentValidation.TestHelper;
using VictoryCenter.BLL.Commands.Admin.Localization.MainPage.Create;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.Localization.Base;
using VictoryCenter.BLL.DTOs.Admin.Localization.MainPage;
using VictoryCenter.BLL.Validators.Localization.MainPage;

namespace VictoryCenter.UnitTests.ValidatorsTests.Localization.MainPage;

public class CreateMainPageLocalizationValidatorTests
{
    private readonly CreateMainPageLocalizationCommandValidator _validator;

    public CreateMainPageLocalizationValidatorTests()
    {
        var baseValidator = new BaseMainPageLocalizationDtoValidator();
        var mainAboutUsValidator = new CreateMainAboutUsLocalizationDtoValidator(baseValidator);
        var mainPartnersValidator = new CreateMainPartnersLocalizationDtoValidator(baseValidator);
        var mainDonationsValidator = new CreateMainDonationsLocalizationDtoValidator(baseValidator);
        var dtoValidator = new CreateMainPageLocalizationDtoValidator(
            baseValidator,
            mainAboutUsValidator,
            mainPartnersValidator,
            mainDonationsValidator);
        _validator = new CreateMainPageLocalizationCommandValidator(dtoValidator);
    }

    // ── Happy path ──────────────────────────────────────────────────────────

    [Fact]
    public void Validate_ShouldNotHaveErrors_WhenAllFieldsAreValid()
    {
        var command = BuildCommand(GetValidDto());

        _validator.TestValidate(command).ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_ShouldNotHaveErrors_WhenOptionalFieldsAreNull()
    {
        var command = BuildCommand(new CreateMainPageLocalizationDto
        {
            EntityId = 1,
            LanguageId = 1,
            Title = null,
            Description = null,
            MainAboutUs = null,
            MainPartners = null,
            MainDonations = null
        });

        _validator.TestValidate(command).ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_ShouldNotHaveErrors_WhenOnlyMainPageFieldsAreProvided()
    {
        var command = BuildCommand(new CreateMainPageLocalizationDto
        {
            EntityId = 1,
            LanguageId = 1,
            Title = new string('a', MainPageConstants.Title.MinLength),
            Description = new string('a', MainPageConstants.Description.MinLength),
            MainAboutUs = null,
            MainPartners = null,
            MainDonations = null
        });

        _validator.TestValidate(command).ShouldNotHaveAnyValidationErrors();
    }

    // ── ILocalizationIdentity ────────────────────────────────────────────────

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Validate_ShouldHaveError_WhenEntityIdIsNotPositive(long entityId)
    {
        var dto = GetValidDto() with { EntityId = entityId };

        _validator.TestValidate(BuildCommand(dto))
            .ShouldHaveValidationErrorFor(x => x.Dto.EntityId)
            .WithErrorMessage(ErrorMessagesConstants.PropertyMustBePositive(
                nameof(ILocalizationIdentity.EntityId)));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Validate_ShouldHaveError_WhenLanguageIdIsNotPositive(long languageId)
    {
        var dto = GetValidDto() with { LanguageId = languageId };

        _validator.TestValidate(BuildCommand(dto))
            .ShouldHaveValidationErrorFor(x => x.Dto.LanguageId)
            .WithErrorMessage(ErrorMessagesConstants.PropertyMustBePositive(
                nameof(ILocalizationIdentity.LanguageId)));
    }

    // ── MainPage Title ───────────────────────────────────────────────────────

    [Fact]
    public void Validate_ShouldHaveError_WhenTitleIsTooShort()
    {
        var dto = GetValidDto() with
        {
            Title = new string('a', MainPageConstants.Title.MinLength - 1)
        };

        _validator.TestValidate(BuildCommand(dto))
            .ShouldHaveValidationErrorFor(x => x.Dto.Title)
            .WithErrorMessage(ErrorMessagesConstants.PropertyMustHaveAMinimumLengthOfNCharacters(
                nameof(CreateMainPageLocalizationDto.Title), MainPageConstants.Title.MinLength));
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenTitleIsTooLong()
    {
        var dto = GetValidDto() with
        {
            Title = new string('a', MainPageConstants.Title.MaxLength + 1)
        };

        _validator.TestValidate(BuildCommand(dto))
            .ShouldHaveValidationErrorFor(x => x.Dto.Title)
            .WithErrorMessage(ErrorMessagesConstants.PropertyMustHaveAMaximumLengthOfNCharacters(
                nameof(CreateMainPageLocalizationDto.Title), MainPageConstants.Title.MaxLength));
    }

    [Fact]
    public void Validate_ShouldNotHaveError_WhenTitleIsNull()
    {
        var dto = GetValidDto() with { Title = null };

        _validator.TestValidate(BuildCommand(dto))
            .ShouldNotHaveValidationErrorFor(x => x.Dto.Title);
    }

    // ── MainPage Description ─────────────────────────────────────────────────

    [Fact]
    public void Validate_ShouldHaveError_WhenDescriptionIsTooShort()
    {
        var dto = GetValidDto() with
        {
            Description = new string('a', MainPageConstants.Description.MinLength - 1)
        };

        _validator.TestValidate(BuildCommand(dto))
            .ShouldHaveValidationErrorFor(x => x.Dto.Description)
            .WithErrorMessage(ErrorMessagesConstants.PropertyMustHaveAMinimumLengthOfNCharacters(
                nameof(CreateMainPageLocalizationDto.Description), MainPageConstants.Description.MinLength));
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenDescriptionIsTooLong()
    {
        var dto = GetValidDto() with
        {
            Description = new string('a', MainPageConstants.Description.MaxLength + 1)
        };

        _validator.TestValidate(BuildCommand(dto))
            .ShouldHaveValidationErrorFor(x => x.Dto.Description)
            .WithErrorMessage(ErrorMessagesConstants.PropertyMustHaveAMaximumLengthOfNCharacters(
                nameof(CreateMainPageLocalizationDto.Description), MainPageConstants.Description.MaxLength));
    }

    [Fact]
    public void Validate_ShouldNotHaveError_WhenDescriptionIsNull()
    {
        var dto = GetValidDto() with { Description = null };

        _validator.TestValidate(BuildCommand(dto))
            .ShouldNotHaveValidationErrorFor(x => x.Dto.Description);
    }

    // ── MainAboutUs ──────────────────────────────────────────────────────────

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Validate_ShouldHaveError_WhenMainAboutUsEntityIdIsNotPositive(long entityId)
    {
        var dto = GetValidDto() with
        {
            MainAboutUs = new CreateMainAboutUsLocalizationDto { EntityId = entityId }
        };

        _validator.TestValidate(BuildCommand(dto))
            .ShouldHaveValidationErrorFor("Dto.MainAboutUs.EntityId");
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenMainAboutUsTitleIsTooShort()
    {
        var dto = GetValidDto() with
        {
            MainAboutUs = new CreateMainAboutUsLocalizationDto
            {
                EntityId = 1,
                Title = new string('a', MainPageConstants.Title.MinLength - 1)
            }
        };

        _validator.TestValidate(BuildCommand(dto))
            .ShouldHaveValidationErrorFor("Dto.MainAboutUs.Title")
            .WithErrorMessage(ErrorMessagesConstants.PropertyMustHaveAMinimumLengthOfNCharacters(
                nameof(CreateMainAboutUsLocalizationDto.Title), MainPageConstants.Title.MinLength));
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenMainAboutUsTitleIsTooLong()
    {
        var dto = GetValidDto() with
        {
            MainAboutUs = new CreateMainAboutUsLocalizationDto
            {
                EntityId = 1,
                Title = new string('a', MainPageConstants.Title.MaxLength + 1)
            }
        };

        _validator.TestValidate(BuildCommand(dto))
            .ShouldHaveValidationErrorFor("Dto.MainAboutUs.Title")
            .WithErrorMessage(ErrorMessagesConstants.PropertyMustHaveAMaximumLengthOfNCharacters(
                nameof(CreateMainAboutUsLocalizationDto.Title), MainPageConstants.Title.MaxLength));
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenMainAboutUsDescriptionIsTooShort()
    {
        var dto = GetValidDto() with
        {
            MainAboutUs = new CreateMainAboutUsLocalizationDto
            {
                EntityId = 1,
                Description = new string('a', MainPageConstants.Description.MinLength - 1)
            }
        };

        _validator.TestValidate(BuildCommand(dto))
            .ShouldHaveValidationErrorFor("Dto.MainAboutUs.Description")
            .WithErrorMessage(ErrorMessagesConstants.PropertyMustHaveAMinimumLengthOfNCharacters(
                nameof(CreateMainAboutUsLocalizationDto.Description), MainPageConstants.Description.MinLength));
    }

    [Fact]
    public void Validate_ShouldNotHaveErrors_WhenMainAboutUsHasOnlyEntityId()
    {
        var dto = GetValidDto() with
        {
            MainAboutUs = new CreateMainAboutUsLocalizationDto { EntityId = 1 }
        };

        _validator.TestValidate(BuildCommand(dto)).ShouldNotHaveAnyValidationErrors();
    }

    // ── MainPartners ─────────────────────────────────────────────────────────

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Validate_ShouldHaveError_WhenMainPartnersEntityIdIsNotPositive(long entityId)
    {
        var dto = GetValidDto() with
        {
            MainPartners = new CreateMainPartnersLocalizationDto { EntityId = entityId }
        };

        _validator.TestValidate(BuildCommand(dto))
            .ShouldHaveValidationErrorFor("Dto.MainPartners.EntityId");
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenMainPartnersTitleIsTooShort()
    {
        var dto = GetValidDto() with
        {
            MainPartners = new CreateMainPartnersLocalizationDto
            {
                EntityId = 1,
                Title = new string('a', MainPageConstants.Title.MinLength - 1)
            }
        };

        _validator.TestValidate(BuildCommand(dto))
            .ShouldHaveValidationErrorFor("Dto.MainPartners.Title")
            .WithErrorMessage(ErrorMessagesConstants.PropertyMustHaveAMinimumLengthOfNCharacters(
                nameof(CreateMainPartnersLocalizationDto.Title), MainPageConstants.Title.MinLength));
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenMainPartnersTitleIsTooLong()
    {
        var dto = GetValidDto() with
        {
            MainPartners = new CreateMainPartnersLocalizationDto
            {
                EntityId = 1,
                Title = new string('a', MainPageConstants.Title.MaxLength + 1)
            }
        };

        _validator.TestValidate(BuildCommand(dto))
            .ShouldHaveValidationErrorFor("Dto.MainPartners.Title")
            .WithErrorMessage(ErrorMessagesConstants.PropertyMustHaveAMaximumLengthOfNCharacters(
                nameof(CreateMainPartnersLocalizationDto.Title), MainPageConstants.Title.MaxLength));
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenMainPartnersDescriptionIsTooShort()
    {
        var dto = GetValidDto() with
        {
            MainPartners = new CreateMainPartnersLocalizationDto
            {
                EntityId = 1,
                Description = new string('a', MainPageConstants.Description.MinLength - 1)
            }
        };

        _validator.TestValidate(BuildCommand(dto))
            .ShouldHaveValidationErrorFor("Dto.MainPartners.Description")
            .WithErrorMessage(ErrorMessagesConstants.PropertyMustHaveAMinimumLengthOfNCharacters(
                nameof(CreateMainPartnersLocalizationDto.Description), MainPageConstants.Description.MinLength));
    }

    [Fact]
    public void Validate_ShouldNotHaveErrors_WhenMainPartnersHasOnlyEntityId()
    {
        var dto = GetValidDto() with
        {
            MainPartners = new CreateMainPartnersLocalizationDto { EntityId = 1 }
        };

        _validator.TestValidate(BuildCommand(dto)).ShouldNotHaveAnyValidationErrors();
    }

    // ── MainDonations ────────────────────────────────────────────────────────

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Validate_ShouldHaveError_WhenMainDonationsEntityIdIsNotPositive(long entityId)
    {
        var dto = GetValidDto() with
        {
            MainDonations = new CreateMainDonationsLocalizationDto { EntityId = entityId }
        };

        _validator.TestValidate(BuildCommand(dto))
            .ShouldHaveValidationErrorFor("Dto.MainDonations.EntityId");
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenMainDonationsTitleIsTooShort()
    {
        var dto = GetValidDto() with
        {
            MainDonations = new CreateMainDonationsLocalizationDto
            {
                EntityId = 1,
                Title = new string('a', MainPageConstants.Title.MinLength - 1)
            }
        };

        _validator.TestValidate(BuildCommand(dto))
            .ShouldHaveValidationErrorFor("Dto.MainDonations.Title")
            .WithErrorMessage(ErrorMessagesConstants.PropertyMustHaveAMinimumLengthOfNCharacters(
                nameof(CreateMainDonationsLocalizationDto.Title), MainPageConstants.Title.MinLength));
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenMainDonationsTitleIsTooLong()
    {
        var dto = GetValidDto() with
        {
            MainDonations = new CreateMainDonationsLocalizationDto
            {
                EntityId = 1,
                Title = new string('a', MainPageConstants.Title.MaxLength + 1)
            }
        };

        _validator.TestValidate(BuildCommand(dto))
            .ShouldHaveValidationErrorFor("Dto.MainDonations.Title")
            .WithErrorMessage(ErrorMessagesConstants.PropertyMustHaveAMaximumLengthOfNCharacters(
                nameof(CreateMainDonationsLocalizationDto.Title), MainPageConstants.Title.MaxLength));
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenMainDonationsDescriptionIsTooShort()
    {
        var dto = GetValidDto() with
        {
            MainDonations = new CreateMainDonationsLocalizationDto
            {
                EntityId = 1,
                Description = new string('a', MainPageConstants.Description.MinLength - 1)
            }
        };

        _validator.TestValidate(BuildCommand(dto))
            .ShouldHaveValidationErrorFor("Dto.MainDonations.Description")
            .WithErrorMessage(ErrorMessagesConstants.PropertyMustHaveAMinimumLengthOfNCharacters(
                nameof(CreateMainDonationsLocalizationDto.Description), MainPageConstants.Description.MinLength));
    }

    [Fact]
    public void Validate_ShouldNotHaveErrors_WhenMainDonationsHasOnlyEntityId()
    {
        var dto = GetValidDto() with
        {
            MainDonations = new CreateMainDonationsLocalizationDto { EntityId = 1 }
        };

        _validator.TestValidate(BuildCommand(dto)).ShouldNotHaveAnyValidationErrors();
    }

    // ── Helpers ──────────────────────────────────────────────────────────────

    private static CreateMainPageLocalizationDto GetValidDto() => new()
    {
        EntityId = 1,
        LanguageId = 1,
        Title = new string('a', MainPageConstants.Title.MinLength),
        Description = new string('a', MainPageConstants.Description.MinLength),
        MainAboutUs = new CreateMainAboutUsLocalizationDto
        {
            EntityId = 1,
            Title = new string('a', MainPageConstants.Title.MinLength),
            Description = new string('a', MainPageConstants.Description.MinLength)
        },
        MainPartners = new CreateMainPartnersLocalizationDto
        {
            EntityId = 1,
            Title = new string('a', MainPageConstants.Title.MinLength),
            Description = new string('a', MainPageConstants.Description.MinLength)
        },
        MainDonations = new CreateMainDonationsLocalizationDto
        {
            EntityId = 1,
            Title = new string('a', MainPageConstants.Title.MinLength),
            Description = new string('a', MainPageConstants.Description.MinLength)
        }
    };

    private static CreateMainPageLocalizationCommand BuildCommand(CreateMainPageLocalizationDto dto)
        => new(dto);
}
